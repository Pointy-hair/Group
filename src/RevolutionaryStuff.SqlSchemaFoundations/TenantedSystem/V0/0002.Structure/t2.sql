/*
use d1
exec db.SchemaTablesPreview 'dbo'
select * from INFORMATION_SCHEMA.tables where table_Schema='dbo'
*/

create table Audits
(
	AuditId bigint not null identity primary key,
	CreatedAtUtc datetime not null default(getutcdate()),
	ActorUserId bigint not null
)

exec db.TablePropertySet 'Audits', 'Information about an event'
exec db.ColumnPropertySet 'Audits', 'AuditId', 'Primary key'
exec db.ColumnPropertySet 'Audits', 'CreatedAtUtc', 'The time when this action was caused'
exec db.ColumnPropertySet 'Audits', 'ActorUserId', 'Foreign key to the user who caused this action'

GO

insert into Audits
(ActorUserId)
values
(1);

GO

create table Tenants
(
	TenantId smallint not null primary key identity,
	CreatedAuditId bigint not null references Audits(AuditId),
	TenantName dbo.Title not null,
)

GO

exec db.TablePropertySet 'Tenants', 'Information about each branded implementation of the system.'
exec db.ColumnPropertySet 'Tenants', 'TenantId', 'Primary key'
exec db.ColumnPropertySet 'Tenants', 'TenantName', 'Recognizable title for the tenant'
exec db.ColumnPropertySet 'Tenants', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'

insert into Tenants
(CreatedAuditId, TenantName)
values
(1, 'Bootstrap');

GO

create table EntityTypes
(
	EntityTypeId bigint not null primary key,
	EntityTypeNameCanBeNull bit not null default(1),
	EntityTypeNameMustBeUniqueCaseSensitive bit not null default(0),
	EntityTypeNameMustBeUniqueCaseInsensitive bit not null default(0),
)

GO

exec db.TablePropertySet 'EntityTypes', 'Types of entities.  This is an entity subclass.'
exec db.ColumnPropertySet 'EntityTypes', 'EntityTypeId', 'Primary key'
exec db.ColumnPropertySet 'EntityTypes', 'EntityTypeNameCanBeNull', 'When 1, the name is allowed to be null, when 0, it must be given.'
exec db.ColumnPropertySet 'EntityTypes', 'EntityTypeNameMustBeUniqueCaseSensitive', 'When 1, the name must be case sensitive unique, when 0, this is not a requirement.'
exec db.ColumnPropertySet 'EntityTypes', 'EntityTypeNameMustBeUniqueCaseInsensitive', 'When 1, the name must be case insensitive unique, when 0, this is not a requirement.'

GO

insert into EntityTypes
(EntityTypeId, EntityTypeNameCanBeNull, EntityTypeNameMustBeUniqueCaseSensitive, EntityTypeNameMustBeUniqueCaseInsensitive)
values
(1, 0, 1, 0); --entitytype

GO

create table Entities
(
	EntityId bigint not null identity primary key,
	EntityTypeId bigint not null references EntityTypes(EntityTypeId),
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	ParentEntityId bigint null references Entities(EntityId),
	TenantId smallint null references Tenants(TenantId),
	EntityName dbo.Title null,
	_EntityTypeNameCanBeNull bit not null,
	_EntityTypeNameMustBeUniqueCaseSensitive bit not null,
	_EntityTypeNameMustBeUniqueCaseInsensitive bit not null,
	_UK as
	(
		cast(HASHBYTES('MD5',
			case 
				when _EntityTypeNameMustBeUniqueCaseSensitive = 1 then EntityName
				when _EntityTypeNameMustBeUniqueCaseInsensitive = 1 then lower(EntityName)
				else EntityName
				end
			) as binary(16))	
	) persisted
)

GO

create unique index Uniqueness on Entities(TenantId, _UK) 
where 
	DeletedAuditId is null and 
	EntityName is not null

GO

create trigger InsertUniqueness on Entities instead of insert
as
begin

	insert into Entities
	select 
		i.EntityTypeId, i.CreatedAuditId, i.DeletedAuditId, i.ParentEntityId, i.TenantId, i.EntityName,
		et.EntityTypeNameCanBeNull,
		et.EntityTypeNameMustBeUniqueCaseSensitive,
		et.EntityTypeNameMustBeUniqueCaseInsensitive
	from
		inserted i
			inner join
		entitytypes et
			on et.EntityTypeId=i.EntityTypeId

end

GO

exec db.TablePropertySet 'Entities', 'Based table for major system entities.  Allows for simplistic linkage to basic functionality like metadata, relationships, audititing, ...'
exec db.ColumnPropertySet 'Entities', 'EntityId', 'Primary key'
exec db.ColumnPropertySet 'Entities', 'EntityTypeId', 'Foreign key to the type of entity'
exec db.ColumnPropertySet 'Entities', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'Entities', 'ParentEntityId', 'Foreign key to the parent of this row'
exec db.ColumnPropertySet 'Entities', 'TenantId', 'Foreign key to the tenant to which this record belongs'
exec db.ColumnPropertySet 'Entities', 'EntityName', 'A title for this entity'
exec db.ColumnPropertySet 'Entities', '_UK', 'If entityname uniquess is to be enforced (per the entitytype table), this is the "calculated" unique key.'

GO

insert into Entities (EntityTypeId, CreatedAuditId, TenantId, EntityName) values (1, 1, null, 'urn:revolutionarystuff.FDSA.types.entityType');

GO

ALTER TABLE EntityTypes ADD CONSTRAINT EntityId_FK FOREIGN KEY (EntityTypeId) REFERENCES Entities(EntityId)

GO

create proc EntityTypeCreate
	@tenantId smallint,
	@entityTypeName dbo.developerName,
	@auditId bigint,
	@entityTypeNameCanBeNull bit,
	@entityTypeNameMustBeUniqueCaseSensitive bit,
	@entityTypeNameMustBeUniqueCaseInsensitive bit,
	@parentEntityTypeId bigint=null,
	@entityId bigint output
AS
BEGIN

	set @entityId = null

	begin tran

		insert into entities 
		(entityTypeId, CreatedAuditId, ParentEntityId, TenantId, EntityName)
		values
		(1, @auditId, @parentEntityTypeId, @tenantId, @entityTypeName)

		set @entityId = @@IDENTITY

		insert into EntityTypes
		(
			EntityTypeId,
			EntityTypeNameCanBeNull,
			EntityTypeNameMustBeUniqueCaseSensitive,
			EntityTypeNameMustBeUniqueCaseInsensitive
		)
		values
		(@entityId, @entityTypeNameCanBeNull, @entityTypeNameMustBeUniqueCaseSensitive, @entityTypeNameMustBeUniqueCaseInsensitive)

	commit tran

END

GO

declare @entityId bigint
exec EntityTypeCreate null, 'urn:revolutionarystuff.FDSA.types.account', 1, 0, 0, 1, @entityId=@entityId output
exec EntityTypeCreate null, 'urn:revolutionarystuff.FDSA.types.user', 1, 0, 0, 1, @entityId=@entityId output
select * from entities

GO

--this is a helper sproc that will cease to exist after this script completes
create proc BootstrapEntityCreate
	@expectedEntityId bigint,
	@entityTypeName dbo.DeveloperName,
	@entityName dbo.Title=null
as
begin

	declare @entityTypeId bigint

	select @entityTypeId=entityId 
	from entities
	where entitytypeid=1 and entityname like '%.'+@entityTypeName

	insert into Entities
	(EntityTypeId, CreatedAuditId, TenantId, EntityName)
	values
	(@entityTypeId, 1, 1, @entityName);

	if (@expectedEntityId is not null)
	begin
		exec db.AssertEquals @expectedEntityId, @@identity
	end

end



GO

select * from EntityTypes
select * from Entities

GO

create table Accounts
(
	AccountId bigint not null primary key references Entities(EntityId),
)

GO

exec BootstrapEntityCreate null, 'account', 'Bootstrap Account'
exec BootstrapEntityCreate null, 'user', 'Bootstrap User'

insert into Accounts
values
(1);

GO


exec db.TablePropertySet 'Accounts', 'A (sub)division of related users that may have common billing/permission systems.  This is an entity subclass.'
exec db.ColumnPropertySet 'Accounts', 'AccountId', 'Primary key'

GO

create table ApplicationTypes
(
	ApplicationTypeId smallint not null primary key,
	ApplicationTypeName dbo.DeveloperName not null unique,
)

GO

exec db.TablePropertySet 'ApplicationTypes', '1', @propertyName='RS_DontCreateFromEF'
exec db.TablePropertySet 'ApplicationTypes', 'Type of application that runs this system'
exec db.ColumnPropertySet 'ApplicationTypes', 'ApplicationTypeId', 'Primary key'
exec db.ColumnPropertySet 'ApplicationTypes', 'ApplicationTypeName', 'Name of this application'

GO

insert into ApplicationTypes
(ApplicationTypeId, ApplicationTypeName)
values
(1, 'TenantSetupSite');

GO

create table ApplicationInstances
(
	ApplicationInstanceId int not null identity primary key,
	TenantId smallint not null references Tenants(TenantId),
	CreatedAuditId bigint not null references Audits(AuditId),
	ApplicationTypeId smallint not null references ApplicationTypes(ApplicationTypeId),
)

GO

exec db.TablePropertySet 'ApplicationInstances', 'Type of application that runs this system'
exec db.ColumnPropertySet 'ApplicationInstances', 'ApplicationInstanceId', 'Primary key'
exec db.ColumnPropertySet 'ApplicationInstances', 'ApplicationTypeId', 'Foreign key to the type of this application'
exec db.ColumnPropertySet 'ApplicationInstances', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'ApplicationInstances', 'TenantId', 'Foreign key to the tenant that owns this account'

GO

insert into ApplicationInstances
(TenantId, CreatedAuditId, ApplicationTypeId)
values
(1, 1, 1);

GO

create table ApplicationInstanceEntryPoints
(
	ApplicationInstanceEntryPointId int not null identity primary key,
	ApplicationInstanceId int not null references ApplicationInstances(ApplicationInstanceId),
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	Hostname nvarchar(1024) not null,
	Port int not null,
	_UK as cast(HASHBYTES('MD5', Hostname+':'+cast(Port as nvarchar(20))) as binary(16)) persisted
)

GO

create unique index Uniqueness on ApplicationInstanceEntryPoints(_UK) where DeletedAuditId is null;

GO

exec db.TablePropertySet 'ApplicationInstanceEntryPoints', 'Type of application that runs this system'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'ApplicationInstanceEntryPointId', 'Primary key'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'ApplicationInstanceId', 'Foreign key to the application instance this to which this entrypoint is applicable'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'Hostname', 'An internet host that should respond to this application'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'Port', 'A port that is listening on the host for connections'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', '_UK', 'Computed column for parts of this table that represent uniqueness.  This uses MD5 to keep under the SQL Server max conditions for unique keys, not for any crypto purposes.'

GO

insert into ApplicationInstanceEntryPoints
(ApplicationInstanceId, CreatedAuditId, DeletedAuditId, Hostname, Port)
values
(1,1,null, 'localhost', 54321);

GO

create table UserTypes
(
	UserTypeId smallint not null primary key,
	UserTypeName dbo.DeveloperName not null unique,
)

GO

exec db.TablePropertySet 'UserTypes', '1', @propertyName='RS_DontCreateFromEF'

GO

insert into UserTypes
(UserTypeId, UserTypeName)
values
(1, 'Bootstrap');

GO

create table Users
(
	UserId bigint not null primary key references Entities(EntityId),
	TenantId bigint not null references Tenants(TenantId),
	DeletedAuditId bigint null references Audits(AuditId),
	UserTypeId smallint not null references UserTypes(UserTypeId),
	UserName nvarchar(128) not null,
	LockoutEnabled bit not null default(0),
	LockoutEndDateUtc datetime default(null),
)

GO

create unique index Uniqueness on Users(TenantId, UserName) where DeletedAuditId is null;

GO

exec BootstrapEntityCreate 3, 3;

insert into Users
(UserId, TenantId, UserTypeId, UserName)
values
(3,1,1,'Bootstrap');

GO

exec db.TablePropertySet 'Users', '1', @propertyName='RS_DeleteViaAudit'
exec db.TablePropertySet 'Users', '1', @propertyName='RS_DontCreateFromEF'
exec db.TablePropertySet 'Users', 'Human, agent, and group based users.  This is an entity subclass.'

GO

ALTER TABLE Audits ADD CONSTRAINT ActorUserId_FK FOREIGN KEY (ActorUserId) REFERENCES Users(UserId)

GO

CREATE TABLE UserClaims
(
	UserClaimId bigint not null identity primary key,
	UserId bigint not null references Users(UserId),
	ClaimType nvarchar(max) NULL,
	ClaimValue nvarchar(max) NULL
)

GO

exec db.TablePropertySet 'Users', '1', @propertyName='RS_DeleteViaDelete'

GO

CREATE TABLE Roles
(
	RoleId int not null identity primary key,
	AccountId bigint not null references Accounts(AccountId),
	RoleName nvarchar(255) not null
)

GO

CREATE TABLE UserRoles
(
	UserRoleId bigint not null identity primary key,
	UserId bigint not null references Users(UserId),
	RoleId int not null references Roles(RoleId),
)

GO

create table Plugins
(
	PluginId int not null identity primary key,
	PluginName dbo.DeveloperName unique
)

GO


create table Passwords
(
	PasswordId bigint not null identity primary key,
	CreatedAuditId bigint not null references Audits(AuditId),
	PasswordProviderId int not null references Plugins(PluginId),
	UserId bigint not null references Users(UserId),
	IsPasswordPci31Compliant bit not null default(0),
	MustChangeAtNextLogin bit not null default(0),
	ExpiresAt datetime null,
	PasswordData nvarchar(255) null
)

GO

/*
How do I go about handling application specific metadata/custom fields?  And what about md that needs a changelog?
For instance, passwordpolicy xml that should be applied to a tennant?
*/

/*
	PasswordPolicy { complexity, multi-factor, reset token durations, crypto schemes }
	AccountPackages { # of users available/purchased }
*/

GO 

create table UserTokenTypes
(
	UserTokenTypeId smallint primary key,
	UserTokenTypeName dbo.DeveloperName unique
)

GO

insert into UserTokenTypes
values
(1, 'PasswordReset'),
(2, 'EmailConfirmation'),
(3, 'Invitation'),
(4, 'ChangePhoneNumber'),
(5, 'Custom'),
(6, 'TwoFactor');

GO

exec db.TablePropertySet 'UserTokenTypes', '1', @propertyName='RS_DontCreateFromEF'

GO

create table UserTokens
(
	UserTokenId bigint not null identity primary key,
	Token varchar(40) not null unique,
	UserId bigint not null references Users(UserId),
	UserTokenTypeId smallint not null references UserTokenTypes(UserTokenTypeId),
	CreatedAuditId bigint not null references Audits(AuditId),
	ValidUntilUtc datetime null,
	DeletedAuditId bigint null references Audits(AuditId),
	ClaimedAuditId bigint null references Audits(AuditId),
	AppData nvarchar(max)
)

GO


exec db.TablePropertySet 'UserTokens', 'A limited use token that is associated with a user that will allows an action'
exec db.ColumnPropertySet 'UserTokens', 'UserTokenId', 'Primary key.'
exec db.ColumnPropertySet 'UserTokens', 'Token', 'The unique token'
exec db.ColumnPropertySet 'UserTokens', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'UserTokens', 'ValidUntilUtc', 'Datetime when this row expires and should no longer be considered'
exec db.ColumnPropertySet 'UserTokens', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.  This happens if a user requests a token but one is still outstanding.'
exec db.ColumnPropertySet 'UserTokens', 'ClaimedAuditId', 'Foreign key to the action that caused the claiming of the reset'
exec db.ColumnPropertySet 'UserTokens', 'UserId', 'The user whose account it unlocked by a reset command'
exec db.ColumnPropertySet 'UserTokens', 'UserTokenTypeId', 'Foreign key to the type of token that has been granted'

GO

create table InvitationUserTokens
(
	UserTokenId bigint not null primary key references UserTokens(UserTokenId),
	AccountId bigint not null references Accounts(AccountId),
)

GO

exec db.TablePropertySet 'InvitationUserTokens', 'Invitations to join an account as a user'
exec db.ColumnPropertySet 'InvitationUserTokens', 'UserTokenId', 'Primary key for this table AND foreign key to the underlying UserToken'
exec db.ColumnPropertySet 'InvitationUserTokens', 'AccountId', 'Foreign key to the account which was used to send this invitation'

GO

drop proc BootstrapEntityCreate

GO
