--https://github.com/Microsoft/azure-docs/blob/master/articles/sql-database/sql-database-elastic-query-getting-started.md

CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'ShardMasterEncrptionKey%#89edfch9ew82425rt36wg';

GO

--credential is a login that can work to login to each of the sharded databases
--could be in the master db and enabled in all shard databases or a separate user created in each shard db
--we can sort this detail out later
--drop DATABASE SCOPED CREDENTIAL _TraffkTenantShardsUserCred
CREATE DATABASE SCOPED CREDENTIAL _TraffkTenantShardsUserCred
WITH IDENTITY = '_TraffkTenantShardsUser',
SECRET = 'dsioajmvgr3tGVer80j5qiGDoe';



--drop EXTERNAL DATA SOURCE TraffkTenantShardsDataSource
CREATE EXTERNAL DATA SOURCE TraffkTenantShardsDataSource
with
(
	type = shard_map_manager,
	location = 'traffkrdb-prod.database.windows.net',
	database_name = 'TraffkTenantShardManager',
	credential = _TraffkTenantShardsUserCred,
	shard_map_name = 'TraffkTenantShardMap'
)

GO

CREATE EXTERNAL DATA SOURCE TraffkDirectoryShardsDataSource
with
(
	type = shard_map_manager,
	location = 'traffkrdb-prod.database.windows.net',
	database_name = 'TraffkTenantShardManager',
	credential = _TraffkTenantShardsUserCred,
	shard_map_name = 'TraffkDirectoryShardMap'
)

GO

CREATE EXTERNAL DATA SOURCE TraffkTenantModelDataSource
with
(
	type = RDBMS,
	location = 'traffkrdb-prod.database.windows.net',
	database_name = 'TraffkTenantModel',
	credential = _TraffkTenantShardsUserCred,
)

GO

select * from sys.external_data_sources; 

GO

create EXTERNAL TABLE db.TraffkTenantModelSchemaMeta
(
	HostDatabaseName sysname,
	T nvarchar(max) null
)
WITH 
(
	SCHEMA_NAME = 'db',
	OBJECT_NAME = 'schemameta',
	DATA_SOURCE = [TraffkTenantModelDataSource]

)

GO

select * from db.TraffkTenantModelSchemaMeta

GO

create PROCEDURE [db].TraffkShardTableImport
	@schema sysname,
	@table sysname
AS
begin

	declare @smt nvarchar(max)
	select @smt=T from db.TraffkTenantModelSchemaMeta

	exec db.ExternalTableImport 'TraffkTenantShardsDataSource', @schema, @table, @smt, @distribution='ROUND_ROBIN'

end

GO

create PROCEDURE [db].TraffkDirectoryTableImport
	@schema sysname,
	@table sysname
AS
begin

	declare @smt nvarchar(max)
	select @smt=T from db.TraffkTenantModelSchemaMeta

	exec db.ExternalTableImport 'TraffkDirectoryShardsDataSource', @schema, @table, @smt, @distribution='ROUND_ROBIN'

end

GO

exec db.TraffkShardTableImport 'dbo', 'tenants'

GO

exec db.TraffkShardTableImport 'dbo', 'apps'

GO

select * from dbo.tenants

select * from dbo.apps

GO

create table TenantIds
(
	TenantId int not null identity primary key,
	HostDatabaseName nvarchar(128)
)

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

CREATE USER _TraffkTenantShardsUser
	FOR LOGIN _TraffkTenantShardsUser
	WITH DEFAULT_SCHEMA = dbo
GO

-- Add user to the database owner role
EXEC sp_addrolemember N'db_datareader', N'_TraffkTenantShardsUser'
EXEC sp_addrolemember N'db_datawriter', N'_TraffkTenantShardsUser'
grant execute on [dbo].[TenantIdReserve] to _TraffkTenantShardsUser  

GO

grant select on tenants to _TraffkPortalApp

GO