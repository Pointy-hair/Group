﻿create schema health

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
	MemberDetails dbo.JsonObject
)

GO

create unique index ux_member on health.members(MemberNumber, CarrierContactId, TenantId) where MemberRowStatus='1'

GO

exec db.TablePropertySet  'Members', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'Members', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.ColumnPropertySet 'Members', 'MemberRowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='health'
exec db.ColumnPropertySet 'Members', 'MemberRowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='health'
exec db.TablePropertySet  'Members', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'

GO

create view health.MemberMap
as
select distinct m.TenantId, m.PersonContactId, m.MemberId, m.MemberNumber, cCarriers.CarrierId, cPeople.ForeignId
from 
	health.members m (nolock)
		inner join
	contacts cCarriers (nolock)
		on cCarriers.ContactId=m.CarrierContactId
		inner join
	contacts cPeople (nolock)
		on cPeople.ContactId=m.PersonContactId

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
	MemberRelationshipLid int null references Lookups(LookupId),
	CoverageTypeLid int null references Lookups(LookupId),
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

exec db.ColumnPropertySet 'Eligibility', 'CoverageTypeLid', 'Coverage type', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'CoverageTypeLid', 'Family', @propertyName='SampleData', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'CobraLid', 'Status Code of the Employee - Not Specified : 00, Working : 01, Terminated : 02', @tableSchema='health'

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

CREATE TABLE health.MillimanScores
(
	MillimanScoreId bigint not null identity primary key,
	RowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default(GetUtcDate()), 
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint  null references Contacts(ContactId),
	MemberId int not null references health.Members(MemberId),
	ScorePeriodStartDdim int NULL references DateDimensions(DateDimensionId),
	ScorePeriodEndDdim int not null references DateDimensions(DateDimensionId),
	ScoreType varchar(50) NULL,
	InpatientScore float NULL,
	OutpatientScore float NULL,
	PhysicianScore float NULL,
	PharmacyScore float NULL,
	MedicalScore float NULL,
	EmergencyRoomScore float NULL,
	OtherScore float NULL,
	TotalScore float NULL,
	ConcurrentInpatient float NULL,
	ConcurrentOutpatient float NULL,
	ConcurrentPhysician float NULL,
	ConcurrentPharmacy float NULL,
	ConcurrentMedical float NULL,
	ConcurrentInpatientNormalizedToGroup float NULL,
	ConcurrentOutpatientNormalizedToGroup float NULL,
	ConcurrentPhysicianNormalizedToGroup float NULL,
	ConcurrentTotal float NULL
)

GO

exec db.TablePropertySet  'MillimanScores', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'MillimanScores', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.ColumnPropertySet 'MillimanScores', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='health'
exec db.ColumnPropertySet 'MillimanScores', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='health'
exec db.TablePropertySet  'MillimanScores', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'
exec db.ColumnPropertySet 'MillimanScores', 'MemberId', 'Member ID to display on the application, as sent by client', @tableSchema='health'

GO

CREATE TABLE Health.Visits
(
	VisitId int not null identity primary key, 
	RowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default(GetUtcDate()), 
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint  null references Contacts(ContactId),
	MemberId int not null references health.Members(MemberId),
	ForeignId dbo.ForeignIdType,
	VisitType varchar(50),
	VisitStartDdim int not null references DateDimensions(DateDimensionId),
	VisitEndDdim int not null references DateDimensions(DateDimensionId),
	InpatientDays int not null,
	AdmissionType varchar(55),
	AdmissionFromEmergencyRoom bit not null
)

GO

create unique index UX_VisitsForeignId on health.visits(ForeignId) where foreignId is not null and RowStatus='1'

GO

exec db.TablePropertySet  'Visits', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'Visits', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.ColumnPropertySet 'Visits', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='health'
exec db.ColumnPropertySet 'Visits', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='health'
exec db.TablePropertySet  'Visits', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'
exec db.ColumnPropertySet 'Visits', 'MemberId', 'Member ID to display on the application, as sent by client', @tableSchema='health'

GO

create table health.Pharmacy
(
	PharmacyId int not null identity primary key, 
	RowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default(GetUtcDate()), 
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint not null references Contacts(ContactId),
	MemberId int not null references health.Members(MemberId),
	PrescriberProviderContactId bigint not null references Contacts(ContactId),
	TransactionNumber varchar(80) not null,
	NationalDrugCodePackageId int not null references NationalDrugCode.Packages(PackageId),
	PrescriptionWrittenDdim int not null references DateDimensions(DateDimensionId),
	PrescriptionFilledDdim int not null references DateDimensions(DateDimensionId),
	ServiceDdim int not null references DateDimensions(DateDimensionId),
	PaidDdim int not null references DateDimensions(DateDimensionId),
	AllowedAmt money,
	BilledAmt money,
	CoinsuranceAmt money,
	CopayAmt money,
	DeductibleAmt money,
	DispensingFeeAmt money,
	IngredientsCostAmt money,
	StateTaxAmt money,
	UsualCustomaryFeeAmt money,
	PaidAmt money,
	PharmacyDetails dbo.JsonObject
)



GO

create unique index UX_TransactionNumber on health.Pharmacy(TransactionNumber) where RowStatus='1'

GO

exec db.TablePropertySet  'Pharmacy', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'Pharmacy', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='health'
exec db.TablePropertySet  'Pharmacy', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'MemberId', 'Member ID to display on the application, as sent by client', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'PrescriptionWrittenDdim', 'date prescription was written', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'PrescriptionFilledDdim', 'date prescription was filled', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'ServiceDdim', 'date of service', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'PaidDdim', 'date of payment', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'AllowedAmt', 'Amount allowed under contract', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'BilledAmt', 'Gross charges', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'CoinsuranceAmt', 'Coinsurance due from patient', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'CopayAmt', 'Amount collected from the patient as a co-payment.', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'DeductibleAmt', 'Deductible Portion of the Allowed Amount', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'DispensingFeeAmt', 'Dispensing Fee textged by the Pharmacy to the PBM', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'IngredientsCostAmt', 'Cost of ingredients', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'StateTaxAmt', 'State Tax Paid', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'UsualCustomaryFeeAmt', 'Usual and Customary Fee', @tableSchema='health'
exec db.ColumnPropertySet 'Pharmacy', 'PaidAmt', 'Amount paid', @tableSchema='health'




GO

create table Health.MedicalClaims
(
	MedicalClaimId int not null identity primary key,
	RowStatus dbo.RowStatus not null default '1',
	CreatedAtUtc datetime not null default(GetUtcDate()), 
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint  null references Contacts(ContactId),
	MemberId int not null references health.Members(MemberId),
	MedicalClaimNumber varchar(50) not null,
	LineItemsCount int not null,
	ServiceFromDdim int null references DateDimensions(DateDimensionId),
	ServiceToDdim int null references DateDimensions(DateDimensionId)
)

GO

create unique index UX_MedicalClaimNumber on health.medicalclaims(MedicalClaimNumber) where RowStatus='1'

GO

exec db.TablePropertySet  'MedicalClaims', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'MedicalClaims', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.ColumnPropertySet 'MedicalClaims', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='health'
exec db.ColumnPropertySet 'MedicalClaims', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='health'
exec db.TablePropertySet  'MedicalClaims', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'
exec db.ColumnPropertySet 'MedicalClaims', 'MemberId', 'Member ID to display on the application, as sent by client', @tableSchema='health'

GO



create table Health.MedicalClaimLines
(
	MedicalClaimLineId int not null identity primary key,
	MedicalClaimId int not null references health.medicalclaims(medicalclaimid),
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint  null references Contacts(ContactId),
	MemberId int not null references health.Members(MemberId),
	MedicalClaimLineNumber varchar(25) not null,
	VisitId int null references health.visits(visitid),
	ServiceFromDdim int null references DateDimensions(DateDimensionId),
	ServiceToDdim int null references DateDimensions(DateDimensionId),
	PaidDdim int null references DateDimensions(DateDimensionId),
	AdjudicationDdim int null references DateDimensions(DateDimensionId),
	AllowedAmount money,
	BilledAmount money,
	CoordinationOfBenefitsAmount money,
	CoinsuranceAmount money,
	CopayAmount money,
	CoverageChargeAmount money,
	DeductibleAmount money,
	NotCoveredAmount money,
	PPOSavings money,
	OtherSavingsGenerated money,
	PaidAmount money,
	MedicalClaimLineDetails dbo.JsonObject
)

GO

create unique index UX_MedicalClaimNumber on health.MedicalClaimLines(MedicalClaimId, MedicalClaimLineNumber) where RowStatus='1'

GO

exec db.TablePropertySet  'MedicalClaimLines', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'MedicalClaimLines', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.TablePropertySet  'MedicalClaimLines', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'
exec db.ColumnPropertySet 'MedicalClaimLines', 'MemberId', 'Member ID to display on the application, as sent by client', @tableSchema='health'

GO

create table Health.CareAlerts
(
	CareAlertId int not null identity primary key,
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint  null references Contacts(ContactId),
	CareAlertDdim int null references DateDimensions(DateDimensionId),
	CareAlertTypeLid int not null references Lookups(LookupId),
	MetricType varchar(50) null,
	MetricName varchar(50) null
)

GO

exec db.TablePropertySet  'CareAlerts', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'CareAlerts', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.TablePropertySet  'CareAlerts', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'


GO

create table Health.QualityMetrics
(
	QualityMetricId int not null identity primary key,
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint  null references Contacts(ContactId),
	MeasureFromDdim int null references DateDimensions(DateDimensionId),
	MeasureToDdim int null references DateDimensions(DateDimensionId),
	QualityMetricTypeLid int not null references Lookups(LookupId),
	Positive bit not null,
	Numerator float not null,
	Denominator float not null,
	Value as case when positive=1 then 1 else -1 end * Numerator / Denominator persisted
)

GO

exec db.TablePropertySet  'QualityMetrics', '0', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'QualityMetrics', '0', @propertyName='GeneratePoco', @tableSchema='health'
exec db.TablePropertySet  'QualityMetrics', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'


GO




