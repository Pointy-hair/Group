create proc ApplicationFindByHost
	@hostName nvarchar(1024),
	@applicationType dbo.developerName=null
	as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	exec db.PrintNow 
		'ApplicationFindByHost hostName=[{s0}] applicationType={s1}', 
		@s0=@hostName,
		@s1=@applicationType

	select a.ApplicationId, a.TenantId, d.Hostname ActualHostname, JSON_VALUE(a.applicationsettings, '$.Hosts.HostInfos[0].Hostname') as PreferredHostname
	from 
		applications a
			cross apply
		openjson(a.applicationsettings, N'$.Hosts.HostInfos') with (Hostname nvarchar(1024)) as d
	where
		(a.ApplicationType=@applicationType or @applicationType is null) and
		(d.Hostname=@hostName)
		
end

exec db.SprocPropertySet  'ApplicationFindByHost', '1', @propertyName='AddToDbContext'
exec db.SprocPropertySet  'ApplicationFindByHost', 'Collection:Traffk.Bal.Data.Rdb.ApplicationHostItem', @propertyName='SprocType'

GO

create proc [dbo].[DateDimensionsForTenantCreate]
	@tenantId int=null
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	exec db.PrintNow 'DateDimensionsForTenantCreate tenantId={n0}', @tenantId 

	;WITH 
	dayAdder(d) AS
	(
		SELECT 0
		UNION ALL
		SELECT d+1 FROM dayAdder WHERE d <= 366
	),
	yearAdder(y) as
	(
		SELECT 0
		UNION ALL
		SELECT y+1 FROM yearAdder WHERE y <= 200
	),
	d(d) as
	(
		select distinct dateAdd(dd, da.d, dateAdd(yy, ya.y, datefromparts(1900,1,1))) d
		from 
			dayAdder da,
			yearAdder ya
		where
			dateAdd(dd, da.d, dateAdd(yy, ya.y, datefromparts(1900,1,1))) < dateAdd(yy, ya.y+1, datefromparts(1900,1,1))
	)
	select 
		b.TenantId, 
		b.d
	into #i
	from 
		(
			select a.TenantID, d.d
			from				
				(
					select t.tenantId
					from
						tenants t
							outer apply
						(select top 1 1 HasDates from datedimensions dd where dd.tenantid=t.tenantId) z
					where
						(t.tenantId=@tenantId or @tenantId is null) and
						t.TenantRowStatus not in ('p','d') and 
						z.HasDates is null		
				) a,
				d
		) b
			left join
		datedimensions ex 
			on b.d=ex.calendardate 
			and ex.tenantId = b.tenantId
	where
		ex.datedimensionid is null
	OPTION (MAXRECURSION 1000);

	exec db.PrintNow 'Staged {n0} dates', @@rowcount 

	insert into datedimensions
	(TenantId, calendarDate, FiscalYear, FiscalQuarter, FiscalMonth, FiscalDay, FiscalYearName, FiscalQuarterName)
	select 
		TenantId, 
		d, 
		0,0,0,0,'FY??','FQ??'
	from
		#i

	exec db.PrintNow 'Created {n0} dates', @@rowcount 

end


GO

create proc FiscalYearsConfigure
	@tenantId int,
	@baselineFiscalYear int,
	@baselineCalendarYear int,
	@baselineCalendarMonth int
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	set nocount on

	exec db.PrintNow 
		'FiscalYearsConfigure @tenantId={n0} FY{n1} starts on 1/{n2}/{n3}', 
		@tenantId, @baselineFiscalYear, @baselineCalendarMonth, @baselineCalendarYear;

	declare @minCalYear smallint

	select top(1) @minCalYear=calendaryear
	from datedimensions
	where tenantid=@tenantId
	order by calendardate 

	set @baselineFiscalYear = @baselineFiscalYear - (@baselineCalendarYear-@minCalYear)
	set @baselineCalendarYear = @baselineCalendarYear - (@baselineCalendarYear-@minCalYear)

	create table #z
	(
		CalendarDate date not null,
		FiscalYear smallint not null,
		FiscalYearName nvarchar(50) not null,
		FiscalQuarter tinyint not null,
		FiscalQuarterName nvarchar(50) not null,
		FiscalMonth tinyint not null,
		FiscalDay smallint not null
	)

	declare @baselineCalendarDate date = datefromparts(@baselineCalendarYear, @baselineCalendarMonth, 1)
	declare @baselineFiscalMonth smallint=1
	declare @fiscalDay smallint=1
	declare @cd date

	DECLARE x CURSOR FOR 
	SELECT calendardate
	FROM datedimensions
	where tenantid=1 and calendardate>=@baselineCalendarDate
	order by calendardate

	OPEN x

	NEXT_ITEM:

	FETCH NEXT FROM x INTO @cd

	IF @@FETCH_STATUS = 0
	BEGIN

		if (@cd>=dateadd(mm,@baselineFiscalMonth,@baselineCalendarDate))
		begin
			set @baselineFiscalMonth = @baselineFiscalMonth + 1
		end

		if (@cd>=dateadd(yy,1,@baselineCalendarDate))
		begin
			set @baselineCalendarDate = dateadd(yy,1,@baselineCalendarDate)
			set @baselineFiscalYear = @baselineFiscalYear + 1
			set @baselineFiscalMonth = 1
			set @fiscalDay = 1
			exec db.PrintNow 'Working on baselineFiscalYear={n0}', @baselineFiscalYear
		end

		insert into #z 
		values 
		(
			@cd,
			@baselineFiscalYear, 
			'FY'+cast(@baselineFiscalYear as varchar(10)),
			((@baselineFiscalMonth-1)/3)+1,
			'FQ'+cast(((@baselineFiscalMonth-1)/3)+1 as nvarchar(10)),
			@baselineFiscalMonth,
			@fiscalDay
		)

		set @fiscalDay = @fiscalDay + 1

		Goto NEXT_ITEM;
	END

	CLOSE x
	DEALLOCATE x

	exec db.PrintNow 'About to update the datedimensions'

	update dd
	set
		fiscalyear=coalesce(z.fiscalyear,0),
		fiscalyearname=coalesce(z.fiscalyearname, 'FY??'),
		fiscalquarter=coalesce(z.fiscalquarter,0),
		fiscalquartername=coalesce(z.fiscalquartername, 'FQ??'),
		fiscalmonth=coalesce(z.fiscalmonth,0),
		fiscalday=coalesce(z.fiscalday,0)
	from
		datedimensions dd
			left join
		#z z 
			on dd.calendardate=z.calendardate
	where
		dd.tenantid=@tenantId

	exec db.PrintNow 'updated {n0} datedimensions', @@rowcount

end

GO

Create TRIGGER [dbo].[TenantsAfterInsert]         
ON [dbo].[Tenants]  
AFTER INSERT 
AS  BEGIN     
	
	declare @tenantId int
	declare @cnt int

	select @tenantId=min(tenantId), @cnt=count(*) from inserted

	if (@cnt>1)
	begin

		RAISERROR('Can only insert a single row at a time into Tenants', 16, 1)
		ROLLBACK TRAN
		return

	end

	exec [dbo].[DateDimensionsForTenantCreate] @tenantId
	exec dbo.FiscalYearsConfigure @tenantId, 2000, 2000, 1

END


GO
