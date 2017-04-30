CREATE proc [dbo].[AppFindByHostname]
	@hostName nvarchar(1024),
	@appType dbo.developerName=null
	as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	exec db.PrintNow 
		'AppFindByHostname hostName=[{s0}] appType={s1}', 
		@s0=@hostName,
		@s1=@appType

	select a.AppId, a.TenantId, t.HostDatabaseName, d.Hostname ActualHostname, JSON_VALUE(a.appsettings, '$.Hosts.HostInfos[0].Hostname') as PreferredHostname
	from 
		tenants t
			inner join
		apps a
			on a.TenantId=t.TenantId
			cross apply
		openjson(a.appsettings, N'$.Hosts.HostInfos') with (Hostname nvarchar(1024)) as d
	where
		(a.AppType=@appType or @appType is null) and
		(d.Hostname=@hostName)
		
end

GO

create proc TenantFindByTenantId
	@tenantId int
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	exec db.PrintNow 
		'DatabaseFindByTenantId tenantId=[{n0}]', 
		@tenantId;

	select * from tenants where tenantId=@tenantId

end

GO
