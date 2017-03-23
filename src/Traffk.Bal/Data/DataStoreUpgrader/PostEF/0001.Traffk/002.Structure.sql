/*
drop index UX_UserName on dbo.AspNetUsers
*/

create type RowStatus from char(1) not null
create type JsonObject from nvarchar(max) null
create type ForeignIdType from nvarchar(50) null

GO

create table Tenants
(
	TenantId int not null identity primary key,
	ParentTenantId int null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()),
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


alter table AspNetRoles add TenantId int not null references Tenants(TenantId);
alter table aspnetusers add UserRowStatus dbo.RowStatus not null default '1'
alter table aspnetusers add UserSettings dbo.JsonObject null;
alter table aspnetusers add CreatedAtUtc datetime not null default(getutcdate());
create unique index UX_UserName on dbo.AspNetUsers (TenantId, NormalizedUserName) where UserRowStatus <> 'p' and UserRowStatus <> 'd'
exec db.ColumnPropertySet 'aspnetusers', 'UserRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'aspnetusers', 'UserRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.TablePropertySet  'aspnetusers', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'aspnetusers', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.TablePropertySet  'AspNetRoles', 'ITraffkTenanted', @propertyName='Implements'

GO

ALTER TABLE [db].[SchemaUpgraderLog] ENABLE TRIGGER SchemaUpgraderNonDeletable

GO

create table Countries
(
	CountryId int not null primary key,--intentionally not identity
	CountryName nvarchar(100) not null unique,
	Alpha2 char(2) not null unique,
	Alpha3 char(3) not null unique,
	NumericCode char(3) not null unique
)

GO

exec db.TablePropertySet  'Countries', 'Country', @propertyName='ClassName'
exec db.TablePropertySet  'Countries', 'IDontCreate', @propertyName='Implements', @tableSchema='dbo'
exec db.TablePropertySet  'Countries', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Countries', '1', @propertyName='GeneratePoco'

GO

create table dbo.Lookups
(
	LookupId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	LookupRowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default (getutcdate()), 
	LookupType dbo.developerName not null,
	LookupKey nvarchar(255) null,
	LookupValue nvarchar(max) null
)

GO

create unique index UX_Lookup_Key on dbo.Lookups(TenantId, LookupType, LookupKey) where LookupRowStatus <> 'p' and LookupRowStatus <> 'd'

GO

exec db.ColumnPropertySet 'Lookups', 'LookupRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.TablePropertySet  'Lookups', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Lookups', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Lookups', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Lookups', 'TenantId', 'Foreign key to the tenant that owns this account'

GO

create table Addresses
(
	AddressId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	Address1 nvarchar(255) null,
	Address2 nvarchar(255) null,
	City nvarchar(100) null,
	[State] nvarchar(100) null,
	PostalCode nvarchar(50) null,
	CountryId int null references Countries(CountryId)
)

GO

exec db.TablePropertySet  'Addresses', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Addresses', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Addresses', 'Address', @propertyName='ClassName'
exec db.TablePropertySet  'Addresses', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Addresses', 'TenantId', 'Foreign key to the tenant that owns this account'

GO

create table Contacts
(
	ContactId bigint not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	ContactRowStatus dbo.RowStatus not null default '1',
	ContactType dbo.developerName not null,
	CreatedAtUtc datetime not null default (getutcdate()), 
	ForeignId ForeignIdType,
	FullName dbo.Title null,
	MemberId varchar(50) null,
	ProviderId varchar(50) null,
	CarrierId varchar(50) null,
	DeerwalkMemberId varchar(50) null,
	PrimaryEmail dbo.EmailAddress null,

	SocialSecurityNumber varchar(9) null,
	DateOfBirth date null,
	Gender varchar(100) null,
	Prefix dbo.Title null,
	FirstName dbo.Title null,
	MiddleName dbo.Title null,
	LastName dbo.Title null,
	Suffix dbo.Title null,

	ContactDetails dbo.JsonObject
)

GO


create unique index UX_CarrierId on Contacts(TenantId, CarrierId) where CarrierId is not null and ContactRowStatus <> 'p' and ContactRowStatus <> 'd'
create unique index UX_ProviderId on Contacts(TenantId, ProviderId) where ProviderId is not null and ContactRowStatus <> 'p' and ContactRowStatus <> 'd'
create unique index UX_MemberId on Contacts(TenantId, MemberId) where MemberId is not null and ContactRowStatus <> 'p' and ContactRowStatus <> 'd'
create unique index UX_DeerwalkMemberId on Contacts(TenantId, DeerwalkMemberId) where DeerwalkMemberId is not null and ContactRowStatus <> 'p' and ContactRowStatus <> 'd'
exec db.TablePropertySet  'Contacts', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Contacts', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Contacts', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Contacts', 'ContactDetails', 'Traffk.Bal.Data.Rdb.ContactDetails', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'Contacts', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'Contacts', 'ContactRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Contacts', 'ContactRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

exec db.ColumnPropertySet 'Contacts', 'ContactType', 'ContactTypes', @propertyName='EnumType'

exec db.ColumnPropertySet 'Contacts', 'PrimaryEmail', 'EmailAddress', @propertyName='DataAnnotation'

exec db.TablePropertySet  'Contacts', 'User', @propertyName='InheritanceClass:UserContact,UserContacts'
exec db.TablePropertySet  'Contacts', 'Carrier', @propertyName='InheritanceClass:Carrier,Carriers'
exec db.TablePropertySet  'Contacts', 'Organization', @propertyName='InheritanceClass:Organization,Organizations'
exec db.TablePropertySet  'Contacts', 'Person', @propertyName='InheritanceClass:Person,People'
exec db.TablePropertySet  'Contacts', 'Provider', @propertyName='InheritanceClass:Provider,Providers'
exec db.ColumnPropertySet 'Contacts', 'ContactType', '1', @propertyName='IsInheritanceDiscriminatorField'
exec db.ColumnPropertySet 'Contacts', 'DateOfBirth', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'Gender', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'Prefix', '1', @propertyName='InheritanceField:Person,Provider'
exec db.ColumnPropertySet 'Contacts', 'FirstName', '1', @propertyName='InheritanceField:Person,Provider'
exec db.ColumnPropertySet 'Contacts', 'MiddleName', '1', @propertyName='InheritanceField:Person,Provider'
exec db.ColumnPropertySet 'Contacts', 'LastName', '1', @propertyName='InheritanceField:Person,Provider'
exec db.ColumnPropertySet 'Contacts', 'Suffix', '1', @propertyName='InheritanceField:Person,Provider'
exec db.ColumnPropertySet 'Contacts', 'ProviderId', '1', @propertyName='InheritanceField:Provider'
exec db.ColumnPropertySet 'Contacts', 'CarrierId', '1', @propertyName='InheritanceField:Carrier'
exec db.ColumnPropertySet 'Contacts', 'SocialSecurityNumber', '1', @propertyName='InheritanceField:Person'
exec db.ColumnPropertySet 'Contacts', 'ForeignId', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

GO

alter table AspNetUsers add ContactId bigint null references Contacts(ContactId);

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
exec db.ColumnPropertySet 'Jobs', 'JobType', 'JobTypes', @propertyName='EnumType'
exec db.ColumnPropertySet 'Jobs', 'JobStatus', 'JobStatuses', @propertyName='EnumType'


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
exec db.ColumnPropertySet 'Applications', 'ApplicationType', 'ApplicationTypes', @propertyName='EnumType'


GO

exec db.TablePropertySet  'AspNetRoles', 'ApplicationRole', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetRoleClaims', 'RoleClaim', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserClaims', 'UserClaim', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserLogins', 'UserLogin', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserRoles', 'ApplicationUserRole', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUsers', 'ApplicationUser', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserTokens', 'UserToken', @propertyName='ClassName'

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
exec db.ColumnPropertySet 'Creatives', 'ModelType', 'Traffk.Bal.Communications.CommunicationModelTypes', @propertyName='EnumType'
exec db.ColumnPropertySet 'Creatives', 'TemplateEngineType', 'Traffk.Bal.Communications.TemplateEngineTypes', @propertyName='EnumType'

GO

create table Communications
(
	CommunicationId int not null identity primary key,
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

create table CommunicationBlastTrackers
(
	CommunicationBlastTrackerId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CommunicationBlastTrackerRowStatus dbo.RowStatus not null default '1',
	CommunicationBlastId int not null references CommunicationBlasts(CommunicationBlastId),
	LinkType dbo.DeveloperName not null,
	CommunicationType dbo.DeveloperName not null,
	TrackerUid uniqueidentifier not null,
	RedirectUrl dbo.Url not null,
	Position int not null
)

GO

create unique index UX_CommunicationBlastTrackersUids on CommunicationBlastTrackers(TrackerUid)
exec db.TablePropertySet  'CommunicationBlastTrackers', 'Assets referenced by a creative whose hyperlinks have been munged to support tracking'
exec db.TablePropertySet  'CommunicationBlastTrackers', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CommunicationBlastTrackers', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'CommunicationBlastTrackers', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'CommunicationBlastTrackers', 'CommunicationBlastTrackerRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'CommunicationBlastTrackers', 'CommunicationBlastTrackerRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.ColumnPropertySet 'CommunicationBlastTrackers', 'TrackerUid', 'Non-guessable guid for this item'
exec db.ColumnPropertySet 'CommunicationBlastTrackers', 'LinkType', 'CommunicationBlastTrackerLinkTypes', @propertyName='EnumType'
exec db.ColumnPropertySet 'CommunicationBlastTrackers', 'CommunicationType', 'Traffk.Bal.Communications.CommunicationTypes', @propertyName='EnumType'

GO

create table CommunicationPieces
(
	CommunicationPieceId bigint not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()),
	CommunicationPieceUid uniqueidentifier not null,
	CommunicationBlastId int not null references CommunicationBlasts(CommunicationBlastId),
	ContactId bigint null references Contacts(ContactId),
	Data dbo.JsonObject
)

GO

create unique index UX_CommunicationPiecesUids on CommunicationPieces(CommunicationPieceUid)
exec db.TablePropertySet  'CommunicationPieces', 'Instance of a communication piece that has been queued for delivery'
exec db.TablePropertySet  'CommunicationPieces', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CommunicationPieces', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'CommunicationPieces', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'CommunicationPieces', 'CommunicationPieceUid', 'Non-guessable guid for this item'
exec db.ColumnPropertySet 'CommunicationPieces', 'Data', 'Bal.Settings.CommunicationPieceData', @propertyName='JsonSettingsClass'

GO

create table CommunicationPieceVisits
(
	CommunicationPieceVisitId bigint not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()),
	CommunicationPieceId bigint not null references CommunicationPieces(CommunicationPieceId),
	CommunicationBlastTrackerId int not null references CommunicationBlastTrackers(CommunicationBlastTrackerId)
)

GO

exec db.TablePropertySet  'CommunicationPieceVisits', 'Record that a creator tracker has been seen relative to communication piece'
exec db.TablePropertySet  'CommunicationPieceVisits', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CommunicationPieceVisits', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'CommunicationPieceVisits', 'ITraffkTenanted', @propertyName='Implements'

GO

create table InsurancePlans
(
	InsurancePlanId int not null identity primary key,
	InsurancePlanRowStatus dbo.RowStatus not null default '1',
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default(GetUtcDate()),
	InsuranceCarrierContactId bigint not null references Contacts(ContactId),
	InsurancePlanCode nvarchar(100),
	InsurancePlanDescription nvarchar(max)
)

GO

create unique index UX_InsuracePlanKeys on dbo.InsurancePlans(InsuranceCarrierContactId, InsurancePlanCode) where InsurancePlanRowStatus='1'
exec db.TablePropertySet  'InsurancePlans', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'InsurancePlans', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'InsurancePlans', 'InsurancePlanRowStatus', '1', @propertyName='ImplementsRowStatusSemantics'
exec db.ColumnPropertySet 'InsurancePlans', 'InsurancePlanRowStatus', 'missing', @propertyName='AccessModifier'
exec db.TablePropertySet  'InsurancePlans', 'ITraffkTenanted', @propertyName='Implements'

GO

create table CustomInsurancePlans
(
	CustomInsurancePlanId int not null identity primary key,
	CustomInsurancePlanRowStatus dbo.RowStatus not null default '1',
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default(GetUtcDate()),
	InsurancePlanId int not null references InsurancePlans(InsurancePlanId),
	CustomInsurancePlanCode nvarchar(100),
	CustomInsurancePlanDescription nvarchar(max)
)

GO

create unique index UX_CustomInsuracePlanKeys on CustomInsurancePlans(InsurancePlanId, CustomInsurancePlanCode) where CustomInsurancePlanRowStatus='1'
exec db.TablePropertySet  'CustomInsurancePlans', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CustomInsurancePlans', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'CustomInsurancePlans', 'CustomInsurancePlanRowStatus', '1', @propertyName='ImplementsRowStatusSemantics'
exec db.ColumnPropertySet 'CustomInsurancePlans', 'CustomInsurancePlanRowStatus', 'missing', @propertyName='AccessModifier'
exec db.TablePropertySet  'CustomInsurancePlans', 'ITraffkTenanted', @propertyName='Implements'

GO

create table CustomInsurancePlanEconomics
(
	CustomInsurancePlanEconomicId int not null identity primary key,
	RowStatus dbo.RowStatus not null default '1',
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default(GetUtcDate()),
	CustomInsurancePlanId int not null references CustomInsurancePlans(CustomInsurancePlanId),
	CostType dbo.DeveloperName,
	FeeType dbo.DeveloperName,
	PeriodStartDdim int not null references DateDimensions(DateDimensionId),
	PeriodEndDdim int not null references DateDimensions(DateDimensionId),
	Amount money not null,
)

GO

exec db.TablePropertySet  'CustomInsurancePlanEconomics', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'CustomInsurancePlanEconomics', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'CustomInsurancePlanEconomics', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics'
exec db.ColumnPropertySet 'CustomInsurancePlanEconomics', 'RowStatus', 'missing', @propertyName='AccessModifier'
exec db.TablePropertySet  'CustomInsurancePlanEconomics', 'ITraffkTenanted', @propertyName='Implements'

GO

create view CommunicationHistory
AS

select c.TenantId, cp.ContactId, cp.CommunicationPieceId, cp.CreatedAtUtc, v.FirstVisitAtUtc, c.CommunicationId, c.CommunicationTitle, c.TopicName, c.CampaignName, cr.CreativeId, cr.CreativeTitle
from 
	communicationpieces cp (nolock)
		inner join
	CommunicationBlasts b (nolock)
		on b.communicationblastid=cp.communicationblastid
		inner join
	Communications c (nolock)
		on c.communicationid=b.communicationid
		inner join
	creatives cr (nolock)
		on cr.creativeid=b.creativeid
		outer apply
	(select top(1) CreatedAtUtc FirstVisitAtUtc from communicationpiecevisits v  (nolock) where v.CommunicationPieceId=cp.CommunicationPieceId) v

GO

exec db.ViewPropertySet  'CommunicationHistory', '1', @propertyName='AddToDbContext'
exec db.ViewPropertySet  'CommunicationHistory', '1', @propertyName='GeneratePoco'
exec db.ViewColumnPropertySet 'CommunicationHistory', 'CommunicationPieceId', 'Key', @propertyName='CustomAttribute'

GO

create table ProviderMedicareSpecialtyCodeMap
(
	ProviderMedicareSpecialtyCodeMap int not null identity primary key,
	ProviderContactId bigint not null references Contacts(ContactId),
	MedicareSpecialtyCodeId int not null references CmsGov.MedicareSpecialtyCodes(MedicareSpecialtyCodeId),
	IsPrimary bit not null
)

GO

exec db.TablePropertySet  'ProviderMedicareSpecialtyCodeMap', '1', @propertyName='AddToDbContext', @tableSchema='dbo'
exec db.TablePropertySet  'ProviderMedicareSpecialtyCodeMap', '1', @propertyName='GeneratePoco', @tableSchema='dbo'
exec db.TablePropertySet  'ProviderMedicareSpecialtyCodeMap', 'IDontCreate', @propertyName='Implements', @tableSchema='dbo'

GO

create table ReportMetaData
(
	ReportMetaDataId int not null identity primary key,
	ExternalReportKey nvarchar(2000) not null,
	ParentReportMetaDataId int null,
	RowStatus dbo.RowStatus not null default '1',
	OwnerContactId bigint null references Contacts(ContactId),
	TenantId int null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()),
	MetaData dbo.JsonObject not null
);

GO

exec db.TablePropertySet  'ReportMetaData', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'ReportMetaData', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'ReportMetaData', 'MetaData', 'Bal.ReportVisuals.ReportDetails', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'ReportMetaData', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReportMetaData', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReportMetaData', 'CreatedAtUtc', 'Datetime when this entity was created.'

GO