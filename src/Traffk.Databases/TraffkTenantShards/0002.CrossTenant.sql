CREATE proc [dbo].[AppFindByHostname]
	@hostName nvarchar(1024),
	@appType dbo.developerName=null,
	@loginDomain dbo.developerName=null,
	@tenantType dbo.developerName=null
	as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	set nocount on

	exec db.PrintNow 
		'AppFindByHostname hostName=[{s0}] appType={s1} loginDomain={s2} tenantType={s3}', 
		@s0=@hostName,
		@s1=@appType,
		@s2=@loginDomain,
		@s3=@tenantType;

	set @loginDomain = coalesce(@loginDomain, ')(*&^%$#@#$%^&*&^%$#@#$%^&*');

	select a.AppId, a.TenantId, t.HostDatabaseName, d.Hostname ActualHostname, JSON_VALUE(a.appsettings, '$.Hosts.HostInfos[0].Hostname') as PreferredHostname, LoginDomain
	from 
		tenants t
			inner join
		apps a
			on a.TenantId=t.TenantId
			outer apply
		openjson(a.appsettings, N'$.Hosts.HostInfos') with (Hostname nvarchar(1024)) as d
	where
		(a.AppType=@appType or @appType is null) and
		(
			t.LoginDomain=@loginDomain or
			d.Hostname=@hostName
		) and
		(t.tenantType=@tenantType or @tenantType is null)
	order by
		case when t.LoginDomain=@loginDomain then 0 else 1 end,
		t.TenantId
		
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

