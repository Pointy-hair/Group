CREATE TABLE [dbo].[ReportMetaData](
	[ReportMetaDataId] [int] IDENTITY(-1,-1) NOT NULL,
	[ExternalReportKey] [nvarchar](2000) NOT NULL,
	[ParentReportMetaDataId] [int] NULL,
	[RowStatus] [dbo].[RowStatus] NOT NULL default(1),
	[OwnerContactId] [bigint] NULL,
	[CreatedAtUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ReportDetails] [dbo].[JsonObject] NOT NULL
)

GO

exec db.TablePropertySet  'ReportMetaData', '0', @propertyName='AddToDbContext'
exec db.TablePropertySet  'ReportMetaData', '0', @propertyName='GeneratePoco'
exec db.TablePropertySet  'ReportMetaData', '"Global"', @propertyName='"JointPart"'

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
)

GO

exec db.TablePropertySet  'DataSourceFetches', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'DataSourceFetches', '1', @propertyName='GeneratePoco'

GO

create table DataSourceFetchItems
(
	DataSourceFetchItemId int not null identity primary key,
	DataSourceFetchId int not null references DataSourceFetches(DataSourceFetchId),
	DataSourceFetchItemType dbo.DeveloperName not null,
	ParentDataSourceFetchItemId int null references DataSourceFetchItems(DataSourceFetchItemId),
	SameDataSourceReplicatedDataSourceFetchItemId int null references DataSourceFetchItems(DataSourceFetchItemId),
	Name nvarchar(1024),
	Size bigint,
	Url dbo.Url,
	DataSourceFetchItemProperties dbo.[JsonObject]
)

GO

exec db.TablePropertySet  'DataSourceFetchItems', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'DataSourceFetchItems', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'DataSourceFetchItems', 'DataSourceFetchItemProperties', 'Bal.Settings.DataSourceFetchItemProperties', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'DataSourceFetchItems', 'DataSourceFetchItemType', 'DataSourceFetchItem.DataSourceFetchItemTypes', @propertyName='EnumType'

GO


alter table hangfire.Job Add TenantId int null
alter table hangfire.Job Add ResultData nvarchar(max) null
alter table hangfire.Job Add RecurringJobId nvarchar(256) null
alter table hangfire.Job Add ContactId int null

GO

CREATE TRIGGER Hangfire.JobParameterRecurringIdLinker
   ON  Hangfire.JobParameter 
   AFTER INSERT
AS 
BEGIN

	SET NOCOUNT ON;


	;with
	jp(JobId, RecurringJobId) as
	(
		select JobId, right(left(value, len(value)-1), len(value)-2)
		from inserted
		where 
			name='RecurringJobId' and
			value is not null and
			len(value)>2 and
			len(value)<258
	),
	m(JobId, RecurringJobId) as
	(
		select distinct jp.JobId, jp.RecurringJobId
		from 
			jp
				inner join
			hangfire.[hash] h
				on h.[key]='recurring-job:'+jp.RecurringJobId	
	)
	update j 
	set
		RecurringJobId=m.RecurringJobId
	from 
		hangfire.job j
			inner join
		m
			on j.id=m.jobid

END

GO
