CREATE proc [dbo].[DateDimensionsForTenantCreate]
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
						t.RowStatus in ('1') and 
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
	(TenantId, calendarDate, FiscalYear, FiscalPeriod, FiscalMonth, FiscalDay, FiscalYearName, FiscalPeriodName)
	select 
		TenantId, 
		d, 
		0,0,0,0,'FY??','FQ??'
	from
		#i

	exec db.PrintNow 'Created {n0} dates', @@rowcount 

end

GO

create proc [dbo].[FiscalYearsConfigure]
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

FindMinCalYear:
	select top(1) @minCalYear=calendaryear
	from datedimensions
	where tenantid=@tenantId
	order by calendardate 

	if (@minCalYear is null)
	begin
		exec [dbo].DateDimensionsForTenantCreate @tenantId
		goto FindMinCalYear;
	end

	set @baselineFiscalYear = @baselineFiscalYear - (@baselineCalendarYear-@minCalYear)
	set @baselineCalendarYear = @baselineCalendarYear - (@baselineCalendarYear-@minCalYear)

	create table #z
	(
		CalendarDate date not null,
		FiscalYear smallint not null,
		FiscalYearName nvarchar(50) not null,
		FiscalPeriod tinyint not null,
		FiscalPeriodName nvarchar(50) not null,
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
		fiscalperiod=coalesce(z.fiscalperiod,0),
		fiscalperiodname=coalesce(z.fiscalperiodname, 'FQ??'),
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

exec db.SprocPropertySet  'FiscalYearsConfigure', '1', @propertyName='AddToDbContext'
exec db.SprocPropertySet  'FiscalYearsConfigure', 'NonQuery', @propertyName='SprocType'

GO

create proc GetFieldCounts
	@schemaName sysname,
	@tableName sysname,
	@tenantId int,
	@fieldNamesCsv nvarchar(max)
AS
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	create table #cnts
	(
		fieldName nvarchar(128),
		fieldVal nvarchar(max),
		fieldCnt int
	)


	declare @fieldName sysname
	declare @sql nvarchar(max)

	DECLARE x CURSOR FOR 
	SELECT val
	FROM dbo.SplitString(@fieldNamesCsv, ',')

	OPEN x

	NEXT_TASK:

	FETCH NEXT FROM x INTO @fieldName

	IF @@FETCH_STATUS = 0
	BEGIN

		set @sql = 'select '''+@fieldName+''', z.['+@fieldName+'], count(*) from '+@schemaName+'.'+@tableName+' z where tenantid='+cast(@tenantId as varchar(10))+' group by z.['+@fieldName+']'

		exec db.PrintNow @sql

		insert into #cnts
		exec(@sql)

		Goto NEXT_TASK;
	END

	CLOSE x
	DEALLOCATE x

	set @sql = 'select null, null, count(*) from '+@schemaName+'.'+@tableName+' z where tenantid='+cast(@tenantId as varchar(10))
	exec db.PrintNow @sql
	insert into #cnts
	exec(@sql)

	select * from #cnts

end

GO

exec db.SprocPropertySet  'GetFieldCounts', '1', @propertyName='AddToDbContext'
exec db.SprocPropertySet  'GetFieldCounts', 'internal', @propertyName='AccessModifier'
exec db.SprocPropertySet  'GetFieldCounts', 'Collection:Traffk.Bal.Data.GetCountsResult.Item', @propertyName='SprocType'

GO
