create schema health

GO

create table health.Members
(
	MemberId int not null identity primary key,
	MemberRowStatus dbo.RowStatus not null default '1',
	TenantId int not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default(GetUtcDate()),
	PersonContactId bigint not null references Contacts(ContactId),
	CarrierContactId bigint not null references Contacts(ContactId),
	MemberNumber varchar(50) not null,
	MemberRelationshipFamilyNumber varchar(50) not null,
	MemberRelationshipCode varchar(10) not null,
	MemberRelationshipCodeDescription varchar(50) not null,
	MemberStatus varchar(20) null,
	MemberDetails dbo.JsonObject
)

GO

create unique index ux_member on health.members(MemberNumber, CarrierContactId, TenantId) where MemberRowStatus='1'

GO

exec db.TablePropertySet  'Members', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'Members', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.ColumnPropertySet 'Members', 'MemberRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='health'
exec db.ColumnPropertySet 'Members', 'MemberRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='health'

GO

create table health.Eligibility
(
	EligibilityId int not null identity primary key, 
	EligibilityRowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default(GetUtcDate()), 
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint  null references Contacts(ContactId),
	MemberId int not null references health.Members(MemberId),
	MemberAddressId int references Addresses(AddressId),
	MedicalEffDdim int null references DateDimensions(DateDimensionId),
	MedicalTermDdim int null references DateDimensions(DateDimensionId),
	PrescriptionEffDdim int null references DateDimensions(DateDimensionId),
	PrescriptionTermDdim int null references DateDimensions(DateDimensionId),
	DentalEffDdim int null references DateDimensions(DateDimensionId),
	DentalTermDdim int null references DateDimensions(DateDimensionId),
	VisionEffDdim int null references DateDimensions(DateDimensionId),
	VisionTermDdim int null references DateDimensions(DateDimensionId),
	LongTermDisabilityEffDdim int null references DateDimensions(DateDimensionId),
	LongTermDisabilityTermDdim int null references DateDimensions(DateDimensionId),
	ShortTermDisabilityEffDdim int null references DateDimensions(DateDimensionId),
	ShortTermDisabilityTermDdim int null references DateDimensions(DateDimensionId),
	PlanTypeLid int null references Lookups(LookupId),
	CoverageTypeLid int null references Lookups(LookupId),
	PlanLid int null references Lookups(LookupId),
	EmployerGroupLid int null references Lookups(LookupId),
	DivisionLid int null references Lookups(LookupId),
	CobraLid int null references Lookups(LookupId),
	EligibilityDetails dbo.JsonObject
)

GO

exec db.TablePropertySet  'Eligibility', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'Eligibility', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.TablePropertySet  'Eligibility', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'MemberId', 'Member ID to display on the application, as sent by client', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'EligibilityRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'EligibilityRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='health'

exec db.ColumnPropertySet 'Eligibility', 'PlanTypeLid', 'Plan type code', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'CoverageTypeLid', 'Coverage type', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'PlanLid', 'Plan id of insurance', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'EmployerGroupLid', 'Identification of the group the subscriber is employed with', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'DivisionLid', 'Identification of the division the subscriber is employed with', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'CobraLid', 'Status Code of the Employee - Not Specified : 00, Working : 01, Terminated : 02', @tableSchema='health'


exec db.ColumnPropertySet 'Eligibility', 'PlanTypeLid', 'Commercial', @propertyName='SampleData', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'CoverageTypeLid', 'Family', @propertyName='SampleData', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'PlanLid', 'Family', @propertyName='SampleData', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'EmployerGroupLid', 'health', @propertyName='SampleData', @tableSchema='health'

exec db.ColumnPropertySet 'Eligibility', 'MedicalEffDdim', 'Effective date for medical plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'MedicalTermDdim', 'Termination date for medical plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'PrescriptionEffDdim', 'Effective date for drug plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'PrescriptionTermDdim', 'Termination date for drug plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'DentalEffDdim', 'Effective date for dental plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'DentalTermDdim', 'Termination date for dental plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'VisionEffDdim', 'Effective date for vision plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'VisionTermDdim', 'Termination date for vision plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'LongTermDisabilityEffDdim', 'Effective date for long term disability plan plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'LongTermDisabilityTermDdim', 'Termination date for long term disability plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'ShortTermDisabilityEffDdim', 'Effective date for short term disability plan', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'ShortTermDisabilityTermDdim', 'Termination date for short term disability plan', @tableSchema='health'



GO

