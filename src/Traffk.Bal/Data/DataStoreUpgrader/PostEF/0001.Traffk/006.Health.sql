create schema health

GO

create table health.Eligibility
(
	EligibilityId int not null identity primary key, 
	CreatedAtUtc datetime not null default(GetUtcDate()), 
	TenantId int not null references Tenants(TenantId), 
	ContactId bigint  null references Contacts(ContactId),
	MemberId nvarchar(50) not null,
	MemberAddressId int references Addresses(AddressId),
	InsurancePolicyId nvarchar(50) not null,
	SocialSecurityNumber nvarchar(30) null,
	DateOfBirthDdm int null references DateDimensions(DateDimensionId)
)

GO

exec db.TablePropertySet  'Eligibility', '1', @propertyName='AddToDbContext', @tableSchema='health'
exec db.TablePropertySet  'Eligibility', '1', @propertyName='GeneratePoco', @tableSchema='health'
exec db.TablePropertySet  'Eligibility', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='health'

GO

exec db.ColumnPropertySet 'Eligibility', 'MemberId', 'Member ID to display on the application, as sent by client', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'InsurancePolicyId', 'Policy Number for Member', @tableSchema='health'
exec db.ColumnPropertySet 'Eligibility', 'SocialSecurityNumber', 'Member SSN', @tableSchema='health'

GO

