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


create EXTERNAL TABLE db.TraffkGlobalCols
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
WITH 
(
	SCHEMA_NAME = 'information_schema',
	OBJECT_NAME = 'columns',
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

	exec db.ExternalTableImport 'TraffkGlobalDataSource', @schema, @table, 'db.TraffkGlobalCols', @srcSchema=@srcSchema, @srcTable=@srcTable 

end

GO

create schema Globals

GO

exec db.TraffkGlobalTableImport 'CmsGov', 'BerensonEggersTypeOfServices'
exec db.TablePropertySet  'BerensonEggersTypeOfServices', '1', @propertyName='AddToDbContext', @tableSchema='CmsGov'
exec db.TablePropertySet  'BerensonEggersTypeOfServices', '1', @propertyName='GeneratePoco', @tableSchema='CmsGov'
exec db.ColumnPropertySet 'BerensonEggersTypeOfServices', 'BetosId', 'Key', @propertyName='CustomAttribute', @tableSchema='CmsGov'

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

exec db.TraffkGlobalTableImport 'dbo', 'Releases'
exec db.TablePropertySet  'Releases', '1', @propertyName='AddToDbContext', @tableSchema='dbo'
exec db.TablePropertySet  'Releases', '1', @propertyName='GeneratePoco', @tableSchema='dbo'
exec db.ColumnPropertySet 'Releases', 'ReleaseId', 'Key', @propertyName='CustomAttribute', @tableSchema='dbo'
exec db.TraffkGlobalTableImport 'dbo', 'ReleaseChanges'
exec db.TablePropertySet  'ReleaseChanges', '1', @propertyName='AddToDbContext', @tableSchema='dbo'
exec db.TablePropertySet  'ReleaseChanges', '1', @propertyName='GeneratePoco', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReleaseChanges', 'ReleaseChangeId', 'Key', @propertyName='CustomAttribute', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReleaseChanges', 'ReleaseId', 'dbo.Releases(ReleaseId)', @propertyName='LinksTo', @tableSchema='dbo'



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



