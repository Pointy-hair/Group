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

CREATE proc [dbo].[TenantIdReserve]
	@hostDatabaseName nvarchar(128)=null
as
begin

	set nocount on

	declare @tenantId int
	declare @maxExistingTenantId int
	select @maxExistingTenantId=max(tenantId) from tenants
	set @maxExistingTenantId=coalesce(@maxExistingTenantId,0)
	
Again:
	insert into TenantIds
	values
	(@hostDatabaseName)

	set @tenantId=@@identity

	delete from TenantIds where tenantId=@tenantId

	if (@tenantId<=@maxExistingTenantId)
	begin
		exec db.PrintNow 'Something got hosed... Out identity value={n0}<maxExistingTenantId={n1};  Trying again', @tenantId, @maxExistingTenantId
		goto Again
	end

	exec db.PrintNow 'Reserved tenantId={n0}', @tenantId

	select @tenantId TenantId

end

GO

exec db.SprocPropertySet  'TenantIdReserve', '1', @propertyName='AddToDbContext'
exec db.SprocPropertySet  'TenantIdReserve', 'Collection:int', @propertyName='SprocType'

