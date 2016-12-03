exec db.SchemaTablesPreview 'dbo'

/*
insert into Tenants
(Name, CreatedAuditId)
values
('Revolutionary Stuff Data Delivery Service Instance');
*/

GO

insert into ApplicationTypes
(ApplicationTypeId, ApplicationTypeName)
values
(1, 'Website');

GO

insert into CommunicationTypes
(CommunicationTypeId, CommunicationTypeName)
values
(1, 'All'),
(2, 'Email'),
(3, 'Text'),
(4, 'Voice'),
(5, 'Mail'),
(6, 'InPerson');

GO

create table Countries
(
	CountryId int not null identity primary key,
)

GO

exec db.TablePropertySet 'Countries', 'ISO 3166-1 Countries.  See https://en.wikipedia.org/wiki/ISO_3166-1'

GO

create table States
(
	StateId int not null identity primary key,
	CountryId int not null references Countries(CountryId),
)

GO

exec db.TablePropertySet 'States', 'ISO 3166-2 Country Subdivisions.  See https://en.wikipedia.org/wiki/ISO_3166-1'

GO

create table AddressTypes
(
	AddressTypeId smallint not null primary key,
	AddressTypeName developername unique
)

GO

insert into AddressTypes
(AddressTypeId, AddressTypeName)
values
(1, 'Physical'),
(2, 'Email'),
(3, 'Phone');

GO

create table Addresses
(
	AddressId bigint not null identity primary key,
	CreatedAt datetime not null default(getutcdate()),
	AddressTypeId smallint not null references AddressTypes(AddressTypeId),
	CanBeUsedForCommunication bit null
)

GO

create table PhysicalAddresses
(
	AddressId bigint not null primary key references Addresses(AddressId),
	Address1 nvarchar(255) null,
	Address2 nvarchar(255) null,
	Address3 nvarchar(255) null,
	Address4 nvarchar(255) null,
	City nvarchar(255) null,
	StateId int null references States(StateId),
	PostalCode nvarchar(20) null,
	CountryId int null references Countries(CountryId),
)

GO

create table EmailAddresses
(
	AddressId bigint not null primary key references Addresses(AddressId),
	EmailAddress dbo.EmailAddress not null
)

GO

create table PhoneNumberTypes
(
	PhoneNumberTypeId smallint not null primary key,
	PhoneNumberTypeName dbo.DeveloperName unique
)

GO

insert into PhoneNumberTypes
(PhoneNumberTypeId,PhoneNumberTypeName)
values
(1, 'Mobile'),
(2, 'Landline'),
(3, 'Fax');

GO

create table PhoneNumbers
(
	AddressId bigint not null primary key references Addresses(AddressId),
	PhoneNumberTypeId smallint null references PhoneNumberTypes(PhoneNumberTypeId),
	PhoneNumber varchar(20) not null,
	Extension varchar(20) not null
)

GO

create table Directories
(
	DirectoryId int not null identity primary key,
	CreatedAt datetime not null default(getutcdate()),	
)

GO

create table AccountDirectories
(
	AccountId int not null references Accounts(AccountId),
	DirectoryId int not null references Directories(DirectoryId)
)

GO

create unique index Uniqueness on AccountDirectories(AccountId, DirectoryId)

-- later on we may want contacts to have their own directories

GO

create table Contacts
(
	ContactId bigint not null identity primary key,
	CreatedAt datetime not null default(getutcdate()),
	DirectoryId int not null references Directories(DirectoryId), -- Should I have a DirectoryId or a rel table?
	Name nvarchar(255) null
)

GO

create table ContactAddresses
(
	ContactAddressId bigint not null identity primary key,
	ContactId bigint not null references Contacts(ContactId),
	AddressId bigint not null references Addresses(AddressId),
	ValidFrom datetime null,
	ValidUntil datetime null,
)

GO

create unique index Uniqueness on ContactAddresses(ContactId, AddressId)

GO

