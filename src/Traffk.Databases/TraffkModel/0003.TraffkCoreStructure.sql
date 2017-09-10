create table Tenants
(
	TenantId int not null primary key,
	ParentTenantId int null references Tenants(TenantId),
	CreatedAtUtc datetime not null default (getutcdate()),
	RowStatus dbo.RowStatus not null default '1',
	TenantName dbo.Title not null,
	HostDatabaseName as db_name(),
	LoginDomain dbo.DeveloperName null,
	TenantType dbo.developerName not null,
	TenantSettings dbo.JsonObject null
)

GO

exec db.TablePropertySet  'Tenants', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Tenants', '1', @propertyName='GeneratePoco'
exec db.ColumnPropertySet 'Tenants', 'TenantSettings', 'Bal.Settings.TenantSettings', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'Tenants', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'Tenants', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Tenants', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.ColumnPropertySet 'Tenants', 'TenantType', 'Tenant.TenantTypes', @propertyName='EnumType'

GO

alter table AspNetRoles add TenantId int not null references Tenants(TenantId);
alter table aspnetusers add TenantId int not null references Tenants(TenantId);
alter table aspnetusers add RowStatus dbo.RowStatus not null default '1'
alter table aspnetusers add UserSettings dbo.JsonObject null;
alter table aspnetusers add CreatedAtUtc datetime not null default(getutcdate());
create unique index UX_UserName on dbo.AspNetUsers (TenantId, NormalizedUserName) where RowStatus = '1'
exec db.ColumnPropertySet 'aspnetusers', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'aspnetusers', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.TablePropertySet  'aspnetusers', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'aspnetusers', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.TablePropertySet  'AspNetRoles', 'ITraffkTenanted', @propertyName='Implements'

GO

create table dbo.Lookups
(
	LookupId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	RowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default (getutcdate()), 
	LookupType dbo.developerName not null,
	LookupKey nvarchar(255) null,
	LookupValue nvarchar(max) null
)

GO

create unique index UX_Lookup_Key on dbo.Lookups(TenantId, LookupType, LookupKey) where RowStatus='1'

GO

exec db.ColumnPropertySet 'Lookups', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Lookups', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.TablePropertySet  'Lookups', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Lookups', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Lookups', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Lookups', 'TenantId', 'Foreign key to the tenant that owns this account'

GO

create table Addresses
(
	AddressId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	AddressLine1 nvarchar(255) null,
	AddressLine2 nvarchar(255) null,
	City nvarchar(100) null,
	[State] nvarchar(100) null,
	PostalCode nvarchar(50) null,
	CountryId int null,
	AddressHash as 
		cast(hashbytes('SHA1', 
			lower(ltrim(rtrim(coalesce(AddressLine1, ''))))+'|'+
			lower(ltrim(rtrim(coalesce(AddressLine2, ''))))+'|'+
			lower(ltrim(rtrim(coalesce(City, ''))))+'|'+
			lower(ltrim(rtrim(coalesce([State], ''))))+'|'+
			lower(ltrim(rtrim(coalesce(PostalCode, ''))))+'|'+
			cast(CountryId as nvarchar(10))
		) as binary(20)) persisted,
	CleanAddressId int null references Addresses(AddressId),
	IsClean bit not null default(0)
)

GO

create unique index UX_AddressHash on Addresses(TenantId, AddressHash)
exec db.TablePropertySet  'Addresses', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Addresses', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Addresses', 'Address', @propertyName='ClassName'
exec db.TablePropertySet  'Addresses', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Addresses', 'TenantId', 'Foreign key to the tenant that owns this account'
exec db.ColumnPropertySet 'Addresses', 'AddressHash', 'missing', @propertyName='AccessModifier'

GO

create table Contacts
(
	ContactId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	RowStatus dbo.RowStatus not null default '1',
	ContactType dbo.developerName not null,
	CreatedAtUtc datetime not null default (getutcdate()), 
	ForeignId ForeignIdType,

	FullName dbo.Title null,

	PrimaryEmail dbo.EmailAddress null,

	SocialSecurityNumber varchar(9) null,
	DateOfBirth date null,
	Gender varchar(100) null,
	Prefix dbo.Title null,
	FirstName dbo.Title null,
	MiddleName dbo.Title null,
	LastName dbo.Title null,
	Suffix dbo.Title null,

	CarrierNumber ForeignIdType null,
	ContactDetails dbo.JsonObject
)

GO

create unique index UX_ForeignId on Contacts(TenantId, ForeignId) where ForeignId is not null and RowStatus = '1'
exec db.TablePropertySet  'Contacts', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Contacts', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Contacts', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Contacts', 'ContactDetails', 'Contact.ContactDetails_', @propertyName='JsonSettingsClass'


exec db.ColumnPropertySet 'Contacts', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'Contacts', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Contacts', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

exec db.ColumnPropertySet 'Contacts', 'ContactType', 'ContactTypes', @propertyName='EnumType'

exec db.ColumnPropertySet 'Contacts', 'PrimaryEmail', 'EmailAddress', @propertyName='DataAnnotation'

exec db.TablePropertySet  'Contacts', 'User', @propertyName='InheritanceClass:UserContact,UserContacts'
exec db.TablePropertySet  'Contacts', 'Carrier', @propertyName='InheritanceClass:CarrierContact,CarrierContacts'
exec db.TablePropertySet  'Contacts', 'Organization', @propertyName='InheritanceClass:OrganizationContact,OrganizationContacts'
exec db.TablePropertySet  'Contacts', 'Person', @propertyName='InheritanceClass:PersonContact,PeopleContacts'
exec db.TablePropertySet  'Contacts', 'Provider', @propertyName='InheritanceClass:ProviderContact,ProviderContacts'

exec db.ColumnPropertySet 'Contacts', 'ContactType', '1', @propertyName='IsInheritanceDiscriminatorField'
exec db.ColumnPropertySet 'Contacts', 'DateOfBirth', 'PersonContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'Gender', 'PersonContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'Prefix', 'PersonContact,ProviderContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'FirstName', 'PersonContact,ProviderContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'MiddleName', 'PersonContact,ProviderContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'LastName', 'PersonContact,ProviderContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'Suffix', 'PersonContact,ProviderContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'SocialSecurityNumber', 'PersonContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'CarrierNumber', 'CarrierContact', @propertyName='InheritanceField'
exec db.ColumnPropertySet 'Contacts', 'ForeignId', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

GO

alter table AspNetUsers add ContactId int not null references Contacts(ContactId);

GO

create table Apps
(
	AppId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	RowStatus dbo.RowStatus not null default '1',
	AppType dbo.DeveloperName not null,
	AppName dbo.Title not null,
	AppSettings dbo.JsonObject
)

GO

exec db.TablePropertySet  'Apps', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Apps', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Apps', 'Type of application that runs this system'
exec db.TablePropertySet  'Apps', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Apps', 'AppId', 'Primary key'
exec db.ColumnPropertySet 'Apps', 'TenantId', 'Foreign key to the tenant that owns this account'
exec db.ColumnPropertySet 'Apps', 'AppType', 'Type of application'
exec db.ColumnPropertySet 'Apps', 'AppName', 'Human readible name of the application'
exec db.ColumnPropertySet 'Apps', 'AppSettings', 'Settings particular to this type of application'
exec db.ColumnPropertySet 'Apps', 'AppSettings', 'Bal.Settings.ApplicationSettings', @propertyName='JsonSettingsClass'
exec db.ColumnPropertySet 'Apps', 'AppType', 'AppTypes', @propertyName='EnumType'
exec db.ColumnPropertySet 'Apps', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'Apps', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

GO

exec db.TablePropertySet  'AspNetRoles', 'ApplicationRole', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetRoleClaims', 'RoleClaim', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserClaims', 'UserClaim', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserLogins', 'UserLogin', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserRoles', 'ApplicationUserRole', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUsers', 'ApplicationUser', @propertyName='ClassName'
exec db.TablePropertySet  'AspNetUserTokens', 'UserToken', @propertyName='ClassName'

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
	FiscalPeriod tinyint not null,
	FiscalMonth tinyint not null,
	FiscalDay smallint not null,
	FiscalYearName nvarchar(50) not null,
	FiscalPeriodName nvarchar(50) not null
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
	JobId int null
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
	ContactId int null references Contacts(ContactId),
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

/*

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
*/

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

exec db.ViewPropertySet  'SchemaTables', '1', @propertyName='AddToDbContext', @viewSchema='db'
exec db.ViewPropertySet  'SchemaTables', '1', @propertyName='GeneratePoco', @viewSchema='db'
exec db.ViewColumnPropertySet 'SchemaTables', 'ObjectId', 'Key', @propertyName='CustomAttribute', @viewSchema='db'

GO

create table Notes
(
	NoteId int not null identity primary key,
	CreatedAtUtc datetime not null default (getutcdate()),
	TenantId int not null references Tenants(TenantId),
	RowStatus dbo.RowStatus not null default '1',
	ParentNoteId int null references Notes(NoteId),
	CreatedByContactId int not null references Contacts(ContactId),
	Title dbo.Title null,
	Body nvarchar(max)
)

GO

exec db.TablePropertySet  'Notes', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Notes', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'Notes', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'Notes', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics'
exec db.ColumnPropertySet 'Notes', 'RowStatus', 'missing', @propertyName='AccessModifier'

GO

create table NoteTargets
(
	NoteTargetId int not null identity primary key,
	TenantId int not null references Tenants(TenantId),
	RowStatus dbo.RowStatus not null default '1',
	NoteId int not null references Notes(NoteId),
	TableObjectId int not null,-- references sys.objects(object_id),
	TablePkIntVal int null
)

GO

exec db.TablePropertySet  'NoteTargets', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'NoteTargets', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'NoteTargets', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'NoteTargets', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics'
exec db.ColumnPropertySet 'NoteTargets', 'RowStatus', 'missing', @propertyName='AccessModifier'

GO

/*
create table Workflows
(
	WorkflowId int not null identity primary key,
	TenantId int not null identity primary key,
	RowStatus dbo.RowStatus not null default '1',
	OwnerContactId int not null references Contact(ContactId),
--	WorkflowDefinitionId
)

GO
*/

/*
create table ProviderMedicareSpecialtyCodeMap
(
	ProviderMedicareSpecialtyCodeMap int not null identity primary key,
	ProviderContactId int not null references Contacts(ContactId),
	MedicareSpecialtyCodeId int not null references CmsGov.MedicareSpecialtyCodes(MedicareSpecialtyCodeId),
	IsPrimary bit not null
)

GO

exec db.TablePropertySet  'ProviderMedicareSpecialtyCodeMap', '1', @propertyName='AddToDbContext', @tableSchema='dbo'
exec db.TablePropertySet  'ProviderMedicareSpecialtyCodeMap', '1', @propertyName='GeneratePoco', @tableSchema='dbo'
exec db.TablePropertySet  'ProviderMedicareSpecialtyCodeMap', 'IDontCreate', @propertyName='Implements', @tableSchema='dbo'

GO
*/


CREATE TABLE [dbo].[ReportMetaData](
	[ReportMetaDataId] [int] IDENTITY(1,1) NOT NULL primary key,
	[ExternalReportKey] [nvarchar](2000) NOT NULL,
	[ParentReportMetaDataId] [int] NULL,
	[RowStatus] [dbo].[RowStatus] NOT NULL default(1),
	TenantId int not null references Tenants(TenantId),
	[OwnerContactId] [bigint] NULL,
	[CreatedAtUtc] [datetime] NOT NULL default (getutcdate()),
	[ReportDetails] [dbo].[JsonObject] NOT NULL
)

GO

exec db.TablePropertySet  'ReportMetaData', '0', @propertyName='AddToDbContext'
exec db.TablePropertySet  'ReportMetaData', '0', @propertyName='GeneratePoco'
exec db.TablePropertySet  'ReportMetaData', 'Tenant', @propertyName='JointPart'
exec db.TablePropertySet  'ReportMetaData', 'ITraffkTenanted', @propertyName='Implements'
exec db.ColumnPropertySet 'ReportMetaData', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'ReportMetaData', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReportMetaData', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReportMetaData', 'TenantId', 'Foreign key to the tenant that owns this account'

GO

exec [db].[TraffkGlobalTableImport] 'globals', 'ReportMetaData', 'dbo','ReportMetaData'
exec db.JointViewCreate 'globals', 'ReportMetaData', 'dbo', 'ReportMetaData'

exec db.ViewPropertySet  'ReportMetaData', '1', @propertyName='AddToDbContext', @viewSchema='joint'
exec db.ViewPropertySet  'ReportMetaData', '1', @propertyName='GeneratePoco', @viewSchema='joint'
exec db.ViewPropertySet  'ReportMetaData', 'ITraffkTenanted', @propertyName='Implements', @viewSchema='joint'
exec db.ViewColumnPropertySet 'ReportMetaData', 'ReportMetaDataId', 'Key', @propertyName='CustomAttribute', @viewSchema='joint'
exec db.ViewColumnPropertySet 'ReportMetaData', 'CreatedAtUtc', 'Datetime when this entity was created.', @viewSchema='joint'
exec db.ViewColumnPropertySet 'ReportMetaData', 'RowStatus', 'missing', @propertyName='AccessModifier', @viewSchema='joint'
exec db.ViewColumnPropertySet 'ReportMetaData', 'TenantId', 'Foreign key to the tenant that owns this account', @viewSchema='joint'

exec db.ViewColumnPropertySet 'ReportMetaData', 'ReportDetails', 'Traffk.Bal.ReportVisuals.ReportDetails', @propertyName='JsonSettingsClass', @viewSchema='joint'



GO




exec [db].TraffkGlobalTableImport 'global', 'hangfire_job', 'hangfire', 'job'
exec db.TablePropertySet 'hangfire_job', '0', @propertyName='AddToDbContext', @tableSchema='global'
exec db.TablePropertySet 'hangfire_job', '0', @propertyName='GeneratePoco', @tableSchema='global'

deny all on global.HangFire_Job to tenant_all_reader
deny all on global.HangFire_Job to tenant_all_writer

GO

create view HangFire.Job
AS
select 
	j.Id,	
	j.StateId,	
	j.StateName,	
	j.InvocationData,	
	j.Arguments,
	j.CreatedAt,
	j.ExpireAt,
	t.TenantId,
	j.ResultData,	
	j.RecurringJobId,	
	j.ContactId
from 
	global.hangfire_job j
		inner join
	tenants t with (nolock)
		on j.tenantid=t.tenantid

GO

exec db.ViewPropertySet 'Job', '1', @propertyName='AddToDbContext', @viewSchema='HangFire'
exec db.ViewPropertySet 'Job', '1', @propertyName='GeneratePoco', @viewSchema='HangFire'
exec db.ViewPropertySet 'Job', 'ITraffkTenanted', @propertyName='Implements', @viewSchema='HangFire'
exec db.ViewColumnPropertySet 'Job', 'Id', 'Key', @propertyName='CustomAttribute', @viewSchema='HangFire'
exec db.ViewColumnPropertySet 'Job', 'TenantId', 'dbo.Tenants(TenantId)', @propertyName='LinksTo', @viewSchema='HangFire'
exec db.ViewColumnPropertySet 'Job', 'ContactId', 'dbo.Contacts(ContactId)', @propertyName='LinksTo', @viewSchema='HangFire'


exec db.TablePropertySet  'ShardMappingsLocal', '1', @propertyName='AddToDbContext', @tableSchema='__ShardManagement'
exec db.TablePropertySet  'ShardMappingsLocal', '1', @propertyName='GeneratePoco', @tableSchema='__ShardManagement'
exec db.TablePropertySet  'ShardMapsLocal', '1', @propertyName='AddToDbContext', @tableSchema='__ShardManagement'
exec db.TablePropertySet  'ShardMapsLocal', '1', @propertyName='GeneratePoco', @tableSchema='__ShardManagement'
exec db.TablePropertySet  'ShardsLocal', '1', @propertyName='AddToDbContext', @tableSchema='__ShardManagement'
exec db.TablePropertySet  'ShardsLocal', '1', @propertyName='GeneratePoco', @tableSchema='__ShardManagement'




exec [db].TraffkGlobalTableImport 'global', 'dbo_datasources', 'dbo', 'datasources'
exec [db].TraffkGlobalTableImport 'global', 'dbo_datasourcefetches', 'dbo', 'datasourcefetches'
exec [db].TraffkGlobalTableImport 'global', 'dbo_datasourcefetchitems', 'dbo', 'datasourcefetchitems'
exec db.TablePropertySet 'dbo_datasources', '0', @propertyName='AddToDbContext', @tableSchema='global'
exec db.TablePropertySet 'dbo_datasources', '0', @propertyName='GeneratePoco', @tableSchema='global'
exec db.TablePropertySet 'dbo_datasourcefetches', '0', @propertyName='AddToDbContext', @tableSchema='global'
exec db.TablePropertySet 'dbo_datasourcefetches', '0', @propertyName='GeneratePoco', @tableSchema='global'
exec db.TablePropertySet 'dbo_datasourcefetchitems', '0', @propertyName='AddToDbContext', @tableSchema='global'
exec db.TablePropertySet 'dbo_datasourcefetchitems', '0', @propertyName='GeneratePoco', @tableSchema='global'

GO

create view DataSources
AS
SELECT g.[DataSourceId]
      ,g.[RowStatus]
      ,t.[TenantId]
      ,g.[CreatedAtUtc]
      ,g.[DataSourceSettings]
FROM 
	[global].[dbo_datasources] g
		inner join
	dbo.Tenants t with (nolock)
		on g.tenantid=t.tenantid
GO

exec db.ViewPropertySet 'DataSources', '1', @propertyName='AddToDbContext', @viewSchema='dbo'
exec db.ViewPropertySet 'DataSources', '1', @propertyName='GeneratePoco', @viewSchema='dbo'
exec db.ViewPropertySet 'DataSources', 'ITraffkTenanted', @propertyName='Implements', @viewSchema='dbo'
exec db.ViewColumnPropertySet 'DataSources', 'DataSourceId', 'Key', @propertyName='CustomAttribute', @viewSchema='dbo'
exec db.ViewColumnPropertySet 'DataSources', 'TenantId', 'dbo.Tenants(TenantId)', @propertyName='LinksTo', @viewSchema='dbo'

GO

create view DataSourceFetches
AS
SELECT g.[DataSourceFetchId]
      ,g.[DataSourceId]
      ,g.[CreatedAtUtc]
  FROM 
	[global].[dbo_datasourcefetches] g
		inner join
	dbo.DataSources ds
		on ds.DataSourceId=g.DataSourceId

GO

exec db.ViewPropertySet 'DataSourceFetches', '1', @propertyName='AddToDbContext', @viewSchema='dbo'
exec db.ViewPropertySet 'DataSourceFetches', '1', @propertyName='GeneratePoco', @viewSchema='dbo'
exec db.ViewColumnPropertySet 'DataSourceFetches', 'DataSourceFetchId', 'Key', @propertyName='CustomAttribute', @viewSchema='dbo'
exec db.ViewColumnPropertySet 'DataSourceFetches', 'DataSourceId', 'dbo.DataSources(DataSourceId)', @propertyName='LinksTo', @viewSchema='dbo'

GO

CREATE VIEW DataSourceFetchItems
AS
SELECT g.[DataSourceFetchItemId]
      ,f.[DataSourceFetchId]
      ,g.[DataSourceFetchItemType]
      ,g.[ParentDataSourceFetchItemId]
      ,g.[SameDataSourceReplicatedDataSourceFetchItemId]
      ,g.[Name]
      ,g.[Size]
      ,g.[Url]
      ,g.[DataSourceFetchItemProperties]
  FROM 
	[global].[dbo_datasourcefetchitems] g
		inner join
	DataSourceFetches f
		on f.DataSourceFetchId=g.DataSourceFetchId

GO

exec db.ViewPropertySet 'DataSourceFetchItems', '1', @propertyName='AddToDbContext', @viewSchema='dbo'
exec db.ViewPropertySet 'DataSourceFetchItems', '1', @propertyName='GeneratePoco', @viewSchema='dbo'
exec db.ViewColumnPropertySet 'DataSourceFetchItems', 'DataSourceFetchItemId', 'Key', @propertyName='CustomAttribute', @viewSchema='dbo'
exec db.ViewColumnPropertySet 'DataSourceFetchItems', 'DataSourceFetchId', 'dbo.DataSourceFetches(DataSourceFetchId)', @propertyName='LinksTo', @viewSchema='dbo'

GO

create schema Map

GO

exec db.ColumnPropertySet 'MedicalCodes', 'MedicalCodeId', 'Key', @propertyName='CustomAttribute', @tableSchema='Map'
exec db.TablePropertySet 'MedicalCodes', '1', @propertyName='AddToDbContext', @tableSchema='Map'
exec db.TablePropertySet 'MedicalCodes', '1', @propertyName='GeneratePoco', @tableSchema='Map'

GO

