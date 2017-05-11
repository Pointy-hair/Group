create table HangfireTenantMap
(
	HangfireTenantMapId int not null identity primary key,
	JobId int not null references Hangfire.Job(Id) unique,
	TenantId int not null
)

alter table hangfire.Job Add TenantId int null
alter table hangfire.Job Add ResultData nvarchar(max) null
exec db.TablePropertySet  'Job', '1', @propertyName='AddToDbContext', @tableSchema='hangfire'
exec db.TablePropertySet  'Job', '1', @propertyName='GeneratePoco', @tableSchema='hangfire'

GO

create table DataSources
(
	DataSourceId int not null identity primary key,
	RowStatus dbo.RowStatus not null default '1',
	TenantId int null,
	CreatedAtUtc datetime not null default (getutcdate()),
	DataSourceSettings dbo.JsonObject -- 
)

GO

exec db.TablePropertySet  'DataSources', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'DataSources', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'DataSources', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'DataSources', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.ColumnPropertySet 'DataSources', 'DataSourceSettings', 'Bal.Settings.DataSourceSettings', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'DataSources', 'CreatedAtUtc', 'Datetime when this entity was created.'

GO

create table DataSourceFetches
(
	DataSourceFetchId int not null identity primary key,
	DataSourceId int not null references DataSources(DataSourceId),
	CreatedAtUtc datetime not null default getutcdate(),
	Size bigint null,
	LastModifiedAt datetime null,
	ContentMD5 binary(16) null,
	ETag nvarchar(max),
)

GO

exec db.TablePropertySet  'DataSourceFetches', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'DataSourceFetches', '1', @propertyName='GeneratePoco'

GO
