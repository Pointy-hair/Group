CREATE USER _TraffkBackgroundJobServer
	FOR LOGIN _TraffkBackgroundJobServer
	WITH DEFAULT_SCHEMA = dbo
GO
create role _TraffkBackgroundJobServerRole
GO
EXEC sp_addrolemember N'_TraffkBackgroundJobServerRole', N'_TraffkBackgroundJobServer'
grant all on __ShardManagement.ShardMapsGlobal to _TraffkBackgroundJobServerRole
grant all on __ShardManagement.ShardsGlobal to _TraffkBackgroundJobServerRole
GO
CREATE USER _TraffkTenantShardsUser
	FOR LOGIN _TraffkTenantShardsUser
	WITH DEFAULT_SCHEMA = dbo
GO
EXEC sp_addrolemember N'db_owner', N'_TraffkTenantShardsUser'
GO
CREATE USER _TraffkPortalApp
	FOR LOGIN _TraffkPortalApp
	WITH DEFAULT_SCHEMA = dbo
GO
grant execute on [dbo].[AppFindByHostname] to _TraffkPortalApp
grant execute on [dbo].[TenantFindByTenantId] to _TraffkPortalApp
GO
