CREATE USER _TraffkPortalApp
	FOR LOGIN _TraffkPortalApp
	WITH DEFAULT_SCHEMA = dbo
GO

create role HangfireClient

GO

exec sp_addrolemember 'HangfireClient', '_TraffkPortalApp'

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

grant insert on dbo.HangfireTenantMap to HangfireClient
grant select on dbo.HangfireTenantMap to HangfireClient

GO
