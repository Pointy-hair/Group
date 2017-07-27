CREATE USER _TraffkTenantShardsUser
	FOR LOGIN _TraffkTenantShardsUser
	WITH DEFAULT_SCHEMA = dbo
GO
EXEC sp_addrolemember N'tenant_all_reader', N'_TraffkTenantShardsUser'
GO

CREATE USER _TraffkPortalApp
	FOR LOGIN _TraffkPortalApp
	WITH DEFAULT_SCHEMA = dbo
GO

create role HangfireClient

GO

create role DataSourceClient

GO

exec sp_addrolemember 'HangfireClient', '_TraffkPortalApp'
exec sp_addrolemember 'DataSourceClient', '_TraffkPortalApp'

grant insert on dbo.DataSources to DataSourceClient
grant select on dbo.DataSources to DataSourceClient
grant update on dbo.DataSources to DataSourceClient
grant insert on dbo.DataSourceFetches to DataSourceClient
grant select on dbo.DataSourceFetches to DataSourceClient
grant update on dbo.DataSourceFetches to DataSourceClient
grant insert on dbo.DataSourceFetchItems to DataSourceClient
grant select on dbo.DataSourceFetchItems to DataSourceClient
grant update on dbo.DataSourceFetchItems to DataSourceClient

grant all on Hangfire.[AggregatedCounter] to HangfireClient
grant all on Hangfire.[Counter] to HangfireClient
grant all on Hangfire.[Hash] to HangfireClient
grant all on Hangfire.[Job] to HangfireClient
grant all on Hangfire.[JobParameter] to HangfireClient
grant all on Hangfire.[JobQueue] to HangfireClient
grant all on Hangfire.[List] to HangfireClient
grant all on Hangfire.[Schema] to HangfireClient
grant all on Hangfire.[Server] to HangfireClient
grant all on Hangfire.[Set] to HangfireClient
grant all on Hangfire.[State] to HangfireClient


GO
