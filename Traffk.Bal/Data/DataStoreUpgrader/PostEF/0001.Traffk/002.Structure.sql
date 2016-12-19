﻿/*
drop index UX_UserName on dbo.AspNetUsers
*/

create type RowStatus from char(1) not null
create type JsonObject from nvarchar(max) null

GO

create table Tenants
(
	TenantId int not null identity primary key,
	ParentTenantId int null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()), --given CDC and sys.fn_cdc_map_lsn_to_time, do we need created at?
	TenantRowStatus dbo.RowStatus not null default '1',
	TenantName dbo.Title not null,
	LoginDomain dbo.DeveloperName null,
	TenantSettings dbo.JsonObject
);

GO

exec db.TablePropertySet  'Tenants', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Tenants', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'Tenants', 'TenantSettings', 'Bal.Settings.TenantSettings', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'Tenants', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'Tenants', 'TenantRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Tenants', 'TenantRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

GO

ALTER TABLE [db].[SchemaUpgraderLog] ENABLE TRIGGER SchemaUpgraderNonDeletable
GO

create table Contacts
(
	ContactId bigint not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	ContactRowStatus dbo.RowStatus not null default '1',
	ContactType dbo.developerName not null,
	CreatedAtUtc datetime not null default (getutcdate()), 
	FullName dbo.Title null,
	MemberId varchar(50) null,
	DeerwalkMemberId varchar(50) null,
	PrimaryEmail dbo.EmailAddress null,

	DateOfBirth date null,
	Gender varchar(100) null,
	Prefix dbo.Title null,
	FirstName dbo.Title null,
	MiddleName dbo.Title null,
	LastName dbo.Title null,
	Suffix dbo.Title null,

	ContactDetails nvarchar(max)
)

GO

create unique index UX_MemberId on Contacts(TenantId, MemberId) where MemberId is not null and ContactRowStatus <> 'p' and ContactRowStatus <> 'd'
create unique index UX_DeerwalkMemberId on Contacts(TenantId, DeerwalkMemberId) where DeerwalkMemberId is not null and ContactRowStatus <> 'p' and ContactRowStatus <> 'd'
exec db.TablePropertySet  'Contacts', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Contacts', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'Contacts', 'ContactDetails', 'Traffk.Bal.Data.Rdb.ContactDetails', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'Contacts', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'Contacts', 'ContactRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Contacts', 'ContactRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

exec db.ColumnPropertySet 'Contacts', 'PrimaryEmail', 'EmailAddress', @propertyName='DataAnnotation'

exec db.TablePropertySet  'Contacts', 'Organization', @propertyName='InheritanceClass:Organization,Organizations'
exec db.TablePropertySet  'Contacts', 'Person', @propertyName='InheritanceClass:Person,People'
exec db.ColumnPropertySet 'Contacts', 'ContactType', '1', @propertyName='IsInheritanceDiscriminatorField'
exec db.ColumnPropertySet 'Contacts', 'DateOfBirth', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'Gender', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'Prefix', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'FirstName', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'MiddleName', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'LastName', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'Suffix', '1', @propertyName='InheritanceField:Person'


GO


alter table aspnetusers add TenantId int not null references Tenants(TenantId);
alter table aspnetusers add UserRowStatus dbo.RowStatus not null default '1'
exec db.ColumnPropertySet 'aspnetusers', 'UserRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'aspnetusers', 'UserRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
create unique index UX_UserName on dbo.AspNetUsers (TenantId, NormalizedUserName) where UserRowStatus <> 'p' and UserRowStatus <> 'd'
alter table aspnetusers add UserSettings dbo.JsonObject null;
alter table aspnetusers add CreatedAtUtc datetime not null default(getutcdate());
exec db.TablePropertySet  'aspnetusers', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'aspnetusers', 'CreatedAtUtc', 'Datetime when this entity was created.'


GO

alter table AspNetRoles add TenantId int not null references Tenants(TenantId);
exec db.TablePropertySet  'AspNetRoles', 'ITraffkTenanted', @propertyName='Implements'

GO

create table Jobs
(
	JobId int not null identity primary key,
	JobType dbo.developername not null,
	TenantId int null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()),
	DontRunBeforeUtc datetime null,
	JobRowStatus dbo.RowStatus not null default '1',
	JobStatus dbo.developername not null,	
	DequeuedAtUtc datetime null,
	CompletedAtUtc datetime null,
	[Priority] int not null,
	ServiceRoleMachineName dbo.developername null,
	JobData nvarchar(max) null,
	JobResult nvarchar(max) null
)

GO

exec db.TablePropertySet  'Jobs', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Jobs', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'Jobs', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'Jobs', 'JobResult', 'Traffk.Bal.JobResult', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'Jobs', 'JobRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Jobs', 'JobRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

GO

create table Applications
(
	ApplicationId int not null identity primary key,
	TenantId int not null references Tenants(TenantId) on delete cascade,
	ApplicationType dbo.DeveloperName not null,
	ApplicationName dbo.Title not null,
	ApplicationSettings dbo.JsonObject
)

GO

exec db.TablePropertySet  'Applications', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Applications', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Applications', 'Type of application that runs this system'
exec db.TablePropertySet  'Applications', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Applications', 'ApplicationId', 'Primary key'
exec db.ColumnPropertySet 'Applications', 'TenantId', 'Foreign key to the tenant that owns this account'
exec db.ColumnPropertySet 'Applications', 'ApplicationType', 'Type of application'
exec db.ColumnPropertySet 'Applications', 'ApplicationName', 'Human readible name of the application'
exec db.ColumnPropertySet 'Applications', 'ApplicationSettings', 'Settings particular to this type of application'
exec db.ColumnPropertySet 'Applications', 'ApplicationSettings', 'Bal.Settings.ApplicationSettings', @propertyName='JsonSettingsClass'

GO

create table Templates
(
	TemplateId int not null identity primary key,
	TenantId int not null references Tenants(TenantId) on delete cascade,
	CreatedAtUtc datetime not null default (getutcdate()),
	TemplateName dbo.developername null,
	ModelType dbo.DeveloperName NULL,
	TemplateEngineType dbo.developername not null,
	IsLayout bit not null default(0),
	Code nvarchar(max)
)

GO

create unique index UX_Templates_Name on Templates (TenantId, TemplateName) where TemplateName is not null

GO

exec db.TablePropertySet  'Templates', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Templates', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Templates', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Templates', 'CreatedAtUtc', 'Datetime when this entity was created.'

GO

create table MessageTemplates
(
	MessageTemplateId int not null identity primary key,
	TenantId int not null references Tenants(TenantId) on delete cascade,
	CreatedAtUtc datetime not null default (getutcdate()),
	MessageTemplateTitle dbo.Title not null,
	SubjectTemplateId int null references Templates(TemplateId),
	HtmlBodyTemplateId int null references Templates(TemplateId),
	TextBodyTemplateId int null references Templates(TemplateId)
)

GO

exec db.TablePropertySet  'MessageTemplates', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'MessageTemplates', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'MessageTemplates', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'MessageTemplates', 'CreatedAtUtc', 'Datetime when this entity was created.'

GO

create table SystemCommunications
(
	SystemCommunicationId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	ApplicationId int null references Applications(ApplicationId),
	CommunicationPurpose dbo.developername not null,
	CommunicationMedium dbo.developername not null,
	MessageTemplateId int not null references MessageTemplates(MessageTemplateId)
)

GO

create unique index UX_SystemCommunications on SystemCommunications(TenantId, ApplicationId, CommunicationPurpose, CommunicationMedium)

GO

exec db.TablePropertySet  'SystemCommunications', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'SystemCommunications', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'SystemCommunications', 'ITraffkTenanted', @propertyName='Implements'

GO

exec db.TablePropertySet  'AspNetRoles', 'ApplicationRole', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetRoleClaims', 'RoleClaim', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserClaims', 'UserClaim', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserLogins', 'UserLogin', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserRoles', 'ApplicationUserRole', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUsers', 'ApplicationUser', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserTokens', 'UserToken', @propertyName='ClassName'

GO

create table CommunicationBlasts
(
	CommunicationBlastId int not null identity primary key,
	ParentCommunicationBlastId int references CommunicationBlasts(CommunicationBlastId),
	TenantId int not null references Tenants(TenantId),
	CommunicationBlastRowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default (getutcdate()),
	JobId int references Jobs(JobId),
	CommunicationBlastTitle dbo.Title not null,
	CommunicationMedium dbo.DeveloperName not null,
	TopicName dbo.Title,
	CampaignName dbo.Title,
	MessageTemplateId int not null references MessageTemplates(MessageTemplateId),
	CommunicationBlastSettings dbo.JsonObject
--	Audience...
)

GO

exec db.TablePropertySet  'CommunicationBlasts', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CommunicationBlasts', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'CommunicationBlasts', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'CommunicationBlasts', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'CommunicationBlasts', 'CommunicationBlastSettings', 'Bal.Settings.CommunicationSettings', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'CommunicationBlasts', 'CommunicationBlastRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'CommunicationBlasts', 'CommunicationBlastRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

GO

CREATE proc [dbo].[JobDequeue]
	@jobType dbo.developername,
	@serviceRoleMachineName dbo.developername,
	@numUnits int=null,
	@jobId int=null,
	@tenantId int=null,
	@maxConcurrentPerTenant int=null
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	set @numUnits = coalesce(@numUnits,1);
	set @maxConcurrentPerTenant = coalesce(@maxConcurrentPerTenant,4);
	set @maxConcurrentPerTenant = case when @maxConcurrentPerTenant>3 then 3 else @maxConcurrentPerTenant end;
	declare @stuckMinutes int = 60*2
	declare @dt datetime = getutcdate()

	exec db.PrintNow 
		'JobDequeue jobType={s0}, @numUnits={n0}, @jobId={n1}, @tenantId={n2}, @maxConcurrentPerTenant={n3}',
		@s0=@jobType, @n0=@numUnits, @n1=@jobId, @n2=@tenantId, @n3=@maxConcurrentPerTenant;

	;with 
	inprog(JobId, TenantId, ConcurrencyToken) as --tasks that are currently in progress
	(
		select JobId, TenantId, ConcurrencyToken
		from
			Jobs
		where
			JobType=@jobType and
			DequeuedAtUtc is not null and 
			CompletedAtUtc is null and
			jobStatus in ('Dequeued', 'Running') and
			datediff(minute, DequeuedAtUtc, @dt) < @stuckMinutes -- in case a processor gets stuck and doesn't auto-reset
	),
	cc(TenantId, CurrentCnt) as  --count of inprogress tasks by client
	(
		select TenantId, count(*)
		from
			inprog
		group by TenantId	
	),
	concurrencyTokens(TenantId, ConcurrencyToken) as -- in progress concurrency tokens
	(
		select distinct TenantId, ConcurrencyToken
		from
			inprog
		where
			ConcurrencyToken is not null
	),
	a(JobId, TenantId, OverallRank, ClientRank, ClientConcurrencyRank, ConcurrencyToken, JobStatus) as --partially filtered list of candidates
	(
		select 
			JobId,
			j.TenantId,
			DENSE_RANK () over (order by [Priority] desc, coalesce(DontRunBeforeUtc, cast('2174-04-09' as datetime)), JobId),
			DENSE_RANK () over (partition by j.TenantId order by [Priority] desc, coalesce(DontRunBeforeUtc, cast('2174-04-09' as datetime)), JobId),
			DENSE_RANK () over (partition by j.TenantId, coalesce(j.ConcurrencyToken, cast(JobId as varchar(20))) order by [Priority] desc, coalesce(DontRunBeforeUtc, cast('2174-04-09' as datetime)), JobId),
			j.ConcurrencyToken,
			j.JobStatus
		from 
			Jobs j 
				left join
			Tenants t 
				on j.TenantId=t.TenantId 
				left join
			concurrencyTokens ct 
				on ct.ConcurrencyToken =j.ConcurrencyToken
				and ct.TenantId=j.TenantId
		where 
			JobType=@jobType and
			jobStatus in ('Queued') and
			(DontRunBeforeUtc is null or @dt>DontRunBeforeUtc) and
			(j.TenantId is null or @tenantId is null or j.TenantId=@tenantId) and
			DequeuedAtUtc is null and
			ct.ConcurrencyToken is null
	)
	select top(@numUnits) a.JobId
	into #z
	from 
		a left join
		cc on a.TenantId = cc.TenantId
	where
		a.ClientRank+coalesce(cc.CurrentCnt,0) <= @maxConcurrentPerTenant and
		a.ClientConcurrencyRank=1
	order by OverallRank

	declare @updatedJobIds dbo.intlisttype 

	update j
	set 
		jobstatus='Dequeued',
		dequeuedAtutc=@dt,
		ServiceRoleMachineName=@serviceRoleMachineName
	output inserted.JobId, inserted.TenantId into @updatedJobIds
	from
		jobs j
			inner join
		#z z
			on j.jobid=z.jobid
	where
		dequeuedAtutc is null or datediff(mi, dequeuedAtutc, @dt)>1

	select j.*
	from 
		jobs j
	where 
		j.JobId in (select val from @updatedJobIds)

end


GO

exec db.SprocPropertySet  'JobDequeue', '1', @propertyName='AddToDbContext'
exec db.SprocPropertySet  'JobDequeue', 'Collection:Job', @propertyName='SprocType'

GO

create proc JobReset
	@jobId int
as
begin

	update jobs 
	set 
		jobStatus='Queued',
		DequeuedAtUtc = null,
		ServiceRoleMachineName = null
	where jobid=@jobId

end

GO

exec db.SprocPropertySet  'JobReset', '1', @propertyName='AddToDbContext'
exec db.SprocPropertySet  'JobReset', 'void', @propertyName='SprocType'

GO

CREATE FUNCTION SplitString(@csv_str NVARCHAR(4000), @delimiter nvarchar(20))
 RETURNS @splittable table (val nvarchar(max), pos int)
AS
BEGIN  
 
-- Check for NULL string or empty sting
    IF  (LEN(@csv_str) < 1 OR @csv_str IS NULL)
    BEGIN
        RETURN
    END
 
    ; WITH csvtbl(i,j, pos)
    AS
    (
        SELECT i=1, j= CHARINDEX(@delimiter,@csv_str+@delimiter), 1
 
        UNION ALL 
 
        SELECT i=j+1, j=CHARINDEX(@delimiter,@csv_str+@delimiter,j+1), pos+1
        FROM csvtbl
        WHERE CHARINDEX(@delimiter,@csv_str+@delimiter,j+1) <> 0
    )   
    INSERT  INTO @splittable(val, pos)
    SELECT  SUBSTRING(@csv_str,i,j-i), pos
    FROM    csvtbl 
 
    RETURN
END  

GO

alter proc GetFieldCounts
	@schemaName sysname,
	@tableName sysname,
	@tenantId int,
	@fieldNamesCsv nvarchar(max)
AS
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	create table #cnts
	(
		fieldName nvarchar(128),
		fieldVal nvarchar(max),
		fieldCnt int
	)


	declare @fieldName sysname
	declare @sql nvarchar(max)

	DECLARE x CURSOR FOR 
	SELECT val
	FROM dbo.SplitString(@fieldNamesCsv, ',')

	OPEN x

	NEXT_TASK:

	FETCH NEXT FROM x INTO @fieldName

	IF @@FETCH_STATUS = 0
	BEGIN

		set @sql = 'select '''+@fieldName+''', z.['+@fieldName+'], count(*) from '+@schemaName+'.'+@tableName+' z where tenantid='+cast(@tenantId as varchar(10))+' group by z.['+@fieldName+']'

		exec db.PrintNow @sql

		insert into #cnts
		exec(@sql)

		Goto NEXT_TASK;
	END

	CLOSE x
	DEALLOCATE x

	set @sql = 'select null, null, count(*) from '+@schemaName+'.'+@tableName+' z where tenantid='+cast(@tenantId as varchar(10))
	exec db.PrintNow @sql
	insert into #cnts
	exec(@sql)

	select * from #cnts

end

GO

exec db.SprocPropertySet  'GetFieldCounts', '1', @propertyName='AddToDbContext'
exec db.SprocPropertySet  'GetFieldCounts', 'internal', @propertyName='AccessModifier'
exec db.SprocPropertySet  'GetFieldCounts', 'Collection:Traffk.Bal.Data.GetCountsResult.Item', @propertyName='SprocType'

GO

create table UserActivities
(
	UserActivityId int not null identity primary key,
	UserId dbo.AspNetId not null references AspNetUsers(Id),
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()), 
	ActivityType dbo.DeveloperName not null,
	ActivityResult dbo.DeveloperName null
)

GO

exec db.TablePropertySet  'UserActivities', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'UserActivities', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'UserActivities', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.TablePropertySet  'UserActivities', 'ITraffkTenanted', @propertyName='Implements'

GO

create table DateDimensions
(
	DateDimensionId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CalendarDate date not null,
	CalendarYear as year(CalendarDate) persisted,
	CalendarQuarter as month(CalendarDate)/3+1 persisted,
	CalendarMonth as month(CalendarDate) persisted,
	CalendarDay as day(CalendarDate) persisted,
	FiscalYear smallint not null,
	FiscalQuarter tinyint not null,
	FiscalMonth tinyint not null,
	FiscalDay smallint not null,
	FiscalYearName nvarchar(50) not null,
	FiscalQuarterName nvarchar(50) not null
)

GO

create unique index UX_CalendarDate on DateDimensions (TenantId, CalendarDate)
exec db.TablePropertySet  'DateDimensions', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'DateDimensions', '1', @propertyName='GeneratePoco'

GO






------------------------------------------------------------


--SystemCommunications go into TenantSettings

create table Creatives
(
	CreativeId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CreativesRowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default (getutcdate()),
	CreativeTitle dbo.title null,
	ModelType dbo.DeveloperName NULL,
	TemplateEngineType dbo.developername not null,
	CreativeSettings dbo.JsonObject null
--	_IsLayout bit not null,
--	_IsFlat bit not null
)

GO

exec db.TablePropertySet  'Creatives', 'Assets that are used in a communication piece'
exec db.TablePropertySet  'Creatives', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Creatives', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Creatives', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Creatives', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'Creatives', 'CreativeTitle', 'Identifyable title for this asset.'
exec db.ColumnPropertySet 'Creatives', 'CreativesRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Creatives', 'CreativesRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.ColumnPropertySet 'Creatives', 'CreativeSettings', 'Bal.Settings.CreativeSettings', @propertyName='JsonSettingsClass'


GO

create table Communications
(
	CommunicationId int not null primary key,
	TenantId int not null references Tenants(TenantId),
	CommunicationRowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default (getutcdate()),
	CommunicationTitle dbo.title null,
	TopicName dbo.Title,
	CampaignName dbo.Title,
	CreativeId int null references Creatives(CreativeId),
	CommunicationSettings dbo.JsonObject
)

GO

exec db.TablePropertySet  'Communications', 'A communication outreach piece.'
exec db.TablePropertySet  'Communications', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Communications', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Communications', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Communications', 'CommunicationRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Communications', 'CommunicationRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.ColumnPropertySet 'Communications', 'CommunicationSettings', 'Traffk.Bal.Settings.CommunicationSettings', @propertyName='JsonSettingsClass'


GO

create table CommunicationBlasts
(
	CommunicationBlastId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CommunicationBlastRowStatus dbo.RowStatus not null default '1',	
	CommunicationId int not null references Communications(CommunicationId),
	CreativeId int null references Creatives(CreativeId),
	JobId int null references Jobs(JobId)
)

GO

exec db.TablePropertySet  'CommunicationBlasts', 'An instance of a communication.'
exec db.TablePropertySet  'CommunicationBlasts', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CommunicationBlasts', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'CommunicationBlasts', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'CommunicationBlasts', 'CommunicationBlastRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'CommunicationBlasts', 'CommunicationBlastRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

GO

create table CreativeBlastTrackers
(
	CreativeBlastTrackerId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CreativeBlastCreativeTrackerRowStatus dbo.RowStatus not null default '1',
	CommunicationBlastId int not null references CommunicationBlasts(CommunicationBlastId),
	LinkType dbo.DeveloperName not null,
	TrackerUid uniqueidentifier not null,
	RedirectUrl dbo.Url not null,
	Position int not null
)

GO

create unique index UX_CreativeBlastTrackersUids on CreativeBlastTrackers(TrackerUid)
exec db.TablePropertySet  'CreativeBlastTrackers', 'Assets referenced by a creative whose hyperlinks have been munged to support tracking'
exec db.TablePropertySet  'CreativeBlastTrackers', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CreativeBlastTrackers', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'CreativeBlastTrackers', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'CreativeBlastTrackers', 'CreativeBlastCreativeTrackerRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'CreativeBlastTrackers', 'CreativeBlastCreativeTrackerRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.ColumnPropertySet 'CreativeBlastTrackers', 'TrackerUid', 'Non-guessable guid for this item'

GO

create table CommunicationPieces
(
	CommunicationPieceId bigint not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()),
	CommunicationPieceUid uniqueidentifier not null,
	CommunicationBlastId int not null references CommunicationBlasts(CommunicationBlastId),
	ContactId bigint null references Contacts(ContactId),
	UserId dbo.AspNetId null references AspNetUsers(Id)
)

GO

create unique index UX_CommunicationPiecesUids on CommunicationPieces(CommunicationPieceUid)
exec db.TablePropertySet  'CommunicationPieces', 'Instance of a communication piece that has been queued for delivery'
exec db.TablePropertySet  'CommunicationPieces', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CommunicationPieces', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'CommunicationPieces', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'CommunicationPieces', 'CommunicationPieceUid', 'Non-guessable guid for this item'

GO

create table CommunicationPieceVisits
(
	CommunicationPieceVisitId bigint not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()),
	CommunicationPieceId bigint not null references CommunicationPieces(CommunicationPieceId),
	CreativeBlastTrackerId int not null references CreativeBlastTrackers(CreativeBlastTrackerId)
)

GO

exec db.TablePropertySet  'CommunicationPieceVisits', 'Record that a creator tracker has been seen relative to communication piece'
exec db.TablePropertySet  'CommunicationPieceVisits', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CommunicationPieceVisits', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'CommunicationPieceVisits', 'ITraffkTenanted', @propertyName='Implements'

GO

--07803