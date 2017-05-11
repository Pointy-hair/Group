CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'TenantGlobalEncryptionKey%#89edfch9ew82425rt36wg';

GO

CREATE DATABASE SCOPED CREDENTIAL _TraffkGlobalUserCred
WITH IDENTITY = '_TraffkGlobalUser',
SECRET = 'dsioajmvgr3tGVer80j5qiGDoe';

GO

CREATE EXTERNAL DATA SOURCE TraffkGlobalDataSource
with
(
	type = RDBMS,
	location = 'traffkrdb-prod.database.windows.net',
	database_name = 'TraffkGlobal',
	credential = _TraffkGlobalUserCred,
)


GO

CREATE DATABASE SCOPED CREDENTIAL _TraffkTenantShardsUserCred
WITH IDENTITY = '_TraffkTenantShardsUser',
SECRET = 'dsioajmvgr3tGVer80j5qiGDoe';

GO

CREATE EXTERNAL DATA SOURCE TraffkTenantShardsDataSource
with
(

	type = RDBMS,
	location = 'traffkrdb-prod.database.windows.net',
	database_name = 'TraffkTenantShards',
	credential = _TraffkTenantShardsUserCred,
)

GO

CREATE DATABASE SCOPED CREDENTIAL _ReferenceDataUserCred
WITH IDENTITY = '_TraffkPortalApp',
SECRET = 'dsJFDIOMFK43IUcartwheelFENGEVW3G98chapter85R43gf39QFJ3MAr112075QGJOQQ8HF4F_fuew';

GO

CREATE EXTERNAL DATA SOURCE ReferenceDataDataSource
with
(
	type = RDBMS,
	location = 'traffkrdb-prod.database.windows.net',
	database_name = 'ReferenceData',
	credential = _ReferenceDataUserCred,
)

GO

select * from sys.external_data_sources; 

GO

create EXTERNAL TABLE db.TraffkGlobalSchemaMeta
(
	HostDatabaseName sysname,
	T nvarchar(max) null
)
WITH 
(
	SCHEMA_NAME = 'db',
	OBJECT_NAME = 'schemameta',
	DATA_SOURCE = [TraffkGlobalDataSource]

)

GO

create EXTERNAL TABLE db.ReferenceDataSchemaMeta
(
	HostDatabaseName sysname,
	T nvarchar(max) null
)
WITH 
(
	SCHEMA_NAME = 'db',
	OBJECT_NAME = 'schemameta',
	DATA_SOURCE = [ReferenceDataDataSource]

)

GO

CREATE PROCEDURE [db].ReferenceDataTableImport
	@schema sysname,
	@table sysname,
	@srcSchema sysname=null,
	@srcTable sysname=NULL,
	@addToDbContext BIT =NULL,
	@generatePoco BIT =NULL
AS
begin

	declare @smt nvarchar(max)
	select @smt=T from db.ReferenceDataSchemaMeta

	exec db.ExternalTableImport 'ReferenceDataDataSource', @schema, @table, @smt, @srcSchema=@srcSchema, @srcTable=@srcTable 

	IF (@addToDbContext IS NOT NULL)
	begin
		EXEC db.TablePropertySet @table, @addToDbContext, @propertyName='AddToDbContext', @tableSchema=@schema
	end
	IF (@generatePoco IS NOT NULL)
	begin
		exec db.TablePropertySet @table, @generatePoco, @propertyName='GeneratePoco', @tableSchema=@schema
	end
end

GO

CREATE PROCEDURE [db].TraffkGlobalTableImport
	@schema sysname,
	@table sysname,
	@srcSchema sysname=null,
	@srcTable sysname=null
AS
begin

	declare @smt nvarchar(max)
	select @smt=T from db.TraffkGlobalSchemaMeta

	exec db.ExternalTableImport 'TraffkGlobalDataSource', @schema, @table, @smt, @srcSchema=@srcSchema, @srcTable=@srcTable 

end

GO

create schema Globals

GO



EXEC db.ReferenceDataTableImport 'CmsGov', 'PricingIndicatorCodes', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'CmsGov', 'LabCertificationCodes', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'CmsGov', 'MedicareOutpatientGroupsPaymentGroupCodes', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'CmsGov', 'BerensonEggersTypeOfServices', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'CmsGov', 'HealthcareCommonProcedureCodingSystemCodes', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'CmsGov', 'MedicareSpecialtyCodes', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'CmsGov', 'HealthCareProviderTaxonomyCodeCrosswalk', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'InternationalClassificationDiseases', 'ICD10', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'ISO3166', 'Countries', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'NationalDrugCode', 'Labelers', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'NationalDrugCode', 'Packages', @addToDbContext=1, @generatePoco=1
EXEC db.ReferenceDataTableImport 'NationalDrugCode', 'Products', @addToDbContext=1, @generatePoco=1


select * from [db].[ColumnProperties] where schemaname='dbo' and propertyname='Key' and propertyvalue='1'
drop external table dbo.ReleaseChanges
drop external table dbo.Releases
exec db.TraffkGlobalTableImport 'dbo', 'Releases'
exec db.TablePropertySet  'Releases', '1', @propertyName='AddToDbContext', @tableSchema='dbo'
exec db.TablePropertySet  'Releases', '1', @propertyName='GeneratePoco', @tableSchema='dbo'
exec db.ColumnPropertySet 'Releases', 'ReleaseId', 'Key', @propertyName='CustomAttribute', @tableSchema='dbo'
exec db.TraffkGlobalTableImport 'dbo', 'ReleaseChanges'
exec db.TablePropertySet  'ReleaseChanges', '1', @propertyName='AddToDbContext', @tableSchema='dbo'
exec db.TablePropertySet  'ReleaseChanges', '1', @propertyName='GeneratePoco', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReleaseChanges', 'ReleaseChangeId', 'Key', @propertyName='CustomAttribute', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReleaseChanges', 'ReleaseId', 'dbo.Releases(ReleaseId)', @propertyName='LinksTo', @tableSchema='dbo'

CREATE EXTERNAL TABLE [dbo].[ReleaseChanges](
[ReleaseChangeId] int not null
, [ReleaseId] int not null
, [ChangeType] nvarchar(255) null
, [Title] nvarchar(255) null
, [Order] int null
) with (
	DATA_SOURCE = [DataSrc_RefData],
	SCHEMA_NAME = 'dbo',
	OBJECT_NAME = 'ReleaseChanges'
)
GO
exec db.ColumnPropertySet 'ReleaseChanges', 'ReleaseChangeId', 'Key', @propertyName='CustomAttribute', @tableSchema='dbo'
GO
exec db.ColumnPropertySet 'ReleaseChanges', 'ReleaseId', 'dbo.Releases(ReleaseId)', @propertyName='LinksTo', @tableSchema='dbo'

select * from db.schemameta

--exec db.TraffkGlobalTableImport 'hangfire', 'Job'


GO

create schema Joint

GO

create proc db.JointViewCreate
	@globalSchema sysname,
	@globalTable sysname,
	@tenantSchema sysname,
	@tenantTable sysname,
	@jointSchema sysname=null,
	@jointTable sysname=null
as
begin

	set @jointSchema = coalesce(@jointSchema, 'Joint')
	set @jointTable = coalesce(@tenantTable, @tenantTable)

	declare @sql nvarchar(max)=''

	select @sql=string_agg(column_name, ', ')
	from information_Schema.columns
	where table_schema=@globalSchema and table_name=@globalTable

	set @sql='create view '+quotename(@jointSchema)+'.'+quotename(@jointTable)+'
as

select g.*, t.TenantId 
from 
	(select * from '+quotename(@globalSchema)+'.'+quotename(@globalTable)+') g,
	tenants t with (nolock)

union all

select '+@sql+', TenantId 
from '+quotename(@tenantSchema)+'.'+quotename(@tenantTable)+' with (nolock)

'
	
	exec db.PrintSql @sql
	exec(@sql)

end

GO



