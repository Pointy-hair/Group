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

create EXTERNAL TABLE db.ModelColumns
(
	[TABLE_CATALOG] nvarchar(128) null
	,[TABLE_SCHEMA] nvarchar(128) null
	,[TABLE_NAME] nvarchar(128) not null
	,[COLUMN_NAME] nvarchar(128) null
	,[ORDINAL_POSITION] int null
	,[COLUMN_DEFAULT] nvarchar(4000) null
	,[IS_NULLABLE] varchar(3) null
	,[DATA_TYPE] nvarchar(128) null
	,[CHARACTER_MAXIMUM_LENGTH] int null
	,[CHARACTER_OCTET_LENGTH] int null
	,[NUMERIC_PRECISION] tinyint null
	,[NUMERIC_PRECISION_RADIX] smallint null
	,[NUMERIC_SCALE] int null
	,[DATETIME_PRECISION] smallint null
	,[CHARACTER_SET_CATALOG] nvarchar(128) null
	,[CHARACTER_SET_SCHEMA] nvarchar(128) null
	,[CHARACTER_SET_NAME] nvarchar(128) null
	,[COLLATION_CATALOG] nvarchar(128) null
	,[COLLATION_SCHEMA] nvarchar(128) null
	,[COLLATION_NAME] nvarchar(128) null
	,[DOMAIN_CATALOG] nvarchar(128) null
	,[DOMAIN_SCHEMA] nvarchar(128) null
	,[DOMAIN_NAME] nvarchar(128) null
)
with (
	SCHEMA_NAME = 'information_schema',
	OBJECT_NAME = 'columns',
	DATA_SOURCE = [TraffkTenantModelDataSource]
)

GO

select * from db.ModelColumns

GO

create PROCEDURE [db].TraffkShardTableImport
	@schema sysname,
	@table sysname
AS
begin

	exec db.ExternalTableImport 'TraffkTenantShardsDataSource', @schema, @table, 'db.ModelColumns', @distribution='ROUND_ROBIN'

end


GO

exec db.TraffkShardTableImport 'dbo', 'tenants'
exec db.TraffkShardTableImport 'dbo', 'apps'

GO

select * from dbo.tenants

