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


--alter table hangfire.Job add RowStatus dbo.RowStatus not null default '1'
--exec db.ColumnPropertySet 'Job', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='hangfire'
--exec db.ColumnPropertySet 'Job', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='hangfire'
alter table hangfire.Job Add TenantId int null
alter table hangfire.Job Add ResultData nvarchar(max) null
alter table hangfire.Job Add RecurringJobId nvarchar(256) null
alter table hangfire.Job Add ParentJobId int null references hangfire.job(id)
alter table hangfire.Job Add ContactId int null



exec db.TablePropertySet  'Job', '1', @propertyName='AddToDbContext', @tableSchema='hangfire'
exec db.TablePropertySet  'Job', '1', @propertyName='GeneratePoco', @tableSchema='hangfire'
exec db.TablePropertySet  'Job', 'HangfireJob', @propertyName='ClassName', @tableSchema='hangfire'
exec db.TablePropertySet  'Job', 'HangfireJobs', @propertyName='CollectionName', @tableSchema='hangfire'
exec db.TablePropertySet  'Hash', '1', @propertyName='AddToDbContext', @tableSchema='hangfire'
exec db.TablePropertySet  'Hash', '1', @propertyName='GeneratePoco', @tableSchema='hangfire'
exec db.TablePropertySet  'Hash', 'HangfireHash', @propertyName='ClassName', @tableSchema='hangfire'
exec db.TablePropertySet  'Hash', 'HangfireHashes', @propertyName='CollectionName', @tableSchema='hangfire'


GO

CREATE TRIGGER [HangFire].[JobParameterRecurringIdLinker]
   ON  [HangFire].[JobParameter] 
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
		RecurringJobId=m.RecurringJobId,
		TenantId=t.TenantId,
		ContactId=c.ContactId
	from 
		hangfire.job j
			inner join
		m
			on j.id=m.jobid
			outer apply
		(select try_cast(value as int) TenantId from hangfire.hash where field='TenantId' and [key]=m.RecurringJobId) t
			outer apply
		(select try_cast(value as int) ContactId from hangfire.hash where field='ContactId' and [key]=m.RecurringJobId) c

END

GO

CREATE TRIGGER Hangfire.StateJobParentIdLinker
   ON  Hangfire.[State] 
   AFTER INSERT
AS 
BEGIN

	SET NOCOUNT ON;

	;with
	jp(JobId, ParentJobId) as
	(
		select s.JobId, json_value(s.[data], '$.ParentId') 
		from 
			inserted s
		where
			try_cast(json_value(s.[data], '$.ParentId') as int) is not null
	)
	update j 
	set
		ParentJobId=jp.ParentJobId
	from 
		hangfire.job j
			inner join
		jp
			on j.id=jp.jobid

END

GO

create proc HangfireJobReset
	@jobId int
as
begin

	set nocount on

	declare @stateId int
	declare @stateName nvarchar(20)

	select top(1) @stateId=Id, @stateName=name
	from hangfire.[state]
	where jobid=@jobId
	order by Id

	exec db.PrintNow 'HangfireJobReset jobId={n0} => stateId={n1}, stateName=[{s0}]', @jobId, @stateId, @s0=@stateName

	update hangfire.Job
	set
		StateId=@stateId,
		StateName=@stateName
	where
		Id=@jobId

	exec db.PrintNow 'Updated {n0} job rows', @@rowcount

	delete 
	from hangfire.[State]
	where
		jobId=@jobId and
		id<>@stateId

	exec db.PrintNow 'Deleted {n0} state rows', @@rowcount

end

GO