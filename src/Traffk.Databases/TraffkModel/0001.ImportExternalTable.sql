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
exec db.TraffkGlobalTableImport 'CmsGov', 'BerensonEggersTypeOfServices'
exec db.TablePropertySet  'BerensonEggersTypeOfServices', '1', @propertyName='AddToDbContext', @tableSchema='CmsGov'
exec db.TablePropertySet  'BerensonEggersTypeOfServices', '1', @propertyName='GeneratePoco', @tableSchema='CmsGov'
exec db.ColumnPropertySet 'BerensonEggersTypeOfServices', 'BetosId', 'Key', @propertyName='CustomAttribute', @tableSchema='CmsGov'

select * from [db].[ColumnProperties] where schemaname='CmsGov' and propertyname='Key' and propertyvalue='1'

exec db.TraffkGlobalTableImport 'CmsGov', 'HealthcareCommonProcedureCodingSystemCodes'
exec db.TablePropertySet  'HealthcareCommonProcedureCodingSystemCodes', '1', @propertyName='AddToDbContext', @tableSchema='CmsGov'
exec db.TablePropertySet  'HealthcareCommonProcedureCodingSystemCodes', '1', @propertyName='GeneratePoco', @tableSchema='CmsGov'
exec db.ColumnPropertySet 'HealthcareCommonProcedureCodingSystemCodes', 'HcpcsId', 'Key', @propertyName='CustomAttribute', @tableSchema='CmsGov'

exec db.TraffkGlobalTableImport 'CmsGov', 'HealthCareProviderTaxonomyCodeCrosswalk'
exec db.TraffkGlobalTableImport 'CmsGov', 'LabCertificationCodes'
exec db.TraffkGlobalTableImport 'CmsGov', 'MedicareOutpatientGroupsPaymentGroupCodes'
exec db.TraffkGlobalTableImport 'CmsGov', 'MedicareSpecialtyCodes'
exec db.TraffkGlobalTableImport 'CmsGov', 'PricingIndicatorCodes'
exec db.TraffkGlobalTableImport 'ISO3166', 'Countries'
exec db.TraffkGlobalTableImport 'NationalDrugCode', 'Labelers'
exec db.TraffkGlobalTableImport 'NationalDrugCode', 'Packages'
exec db.TablePropertySet  'Packages', '1', @propertyName='AddToDbContext', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Packages', '1', @propertyName='GeneratePoco', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Packages', 'PackageId', 'Key', @propertyName='CustomAttribute', @tableSchema='NationalDrugCode'

exec db.TraffkGlobalTableImport 'NationalDrugCode', 'Products'
exec db.TraffkGlobalTableImport 'InternationalClassificationDiseases', 'ICD10'
exec db.TablePropertySet  'ICD10', '1', @propertyName='AddToDbContext', @tableSchema='InternationalClassificationDiseases'
exec db.TablePropertySet  'ICD10', '1', @propertyName='GeneratePoco', @tableSchema='InternationalClassificationDiseases'
exec db.ColumnPropertySet 'ICD10', 'Icd10Id', 'Key', @propertyName='CustomAttribute', @tableSchema='InternationalClassificationDiseases'


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

[DataSrc_RefData]

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



