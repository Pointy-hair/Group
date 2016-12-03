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

create proc DateDimensionsForTenantCreate
	@tenantId int
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	exec db.PrintNow 'DateDimensionsForTenantCreate tenantId={n0}', @tenantId 
	exec db.AssertNotNull @tenantId

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
	insert into datedimensions
	(TenantId, calendarDate, FiscalYear, FiscalQuarter, FiscalMonth, FiscalDay, FiscalYearName, FiscalQuarterName)
	select 
		@tenantId, 
		d.d, 
		0,0,0,0,'FY??','FQ??'
	from 
		d
			left join
		datedimensions ex 
			on d.d=ex.calendardate and ex.tenantId = @tenantId
	where
		ex.datedimensionid is null
	OPTION (MAXRECURSION 1000);

	exec db.PrintNow 'Created {n0} dates', @@rowcount 

end

GO
