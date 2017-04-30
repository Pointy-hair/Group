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
