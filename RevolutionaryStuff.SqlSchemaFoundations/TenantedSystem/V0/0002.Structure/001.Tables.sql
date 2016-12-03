/*
use d1
exec db.SchemaTablesPreview 'dbo'
select * from db.schemameta
select *, 'drop table dbo.'+table_name from INFORMATION_SCHEMA.tables where table_Schema='dbo'

ALTER TABLE Audits drop CONSTRAINT ActorUserId_FK

*/

create table Tenants
(
	TenantId smallint not null primary key identity,
	CreatedAtUtc datetime not null default(getutcdate()),
	DeletedAtUtc datetime null,
	TenantName dbo.Title not null
)

GO

create unique index Uniqueness on Tenants(TenantName) where DeletedAtUtc is null

GO

exec db.TablePropertySet 'Tenants', 'Information about each branded implementation of the system.'
exec db.ColumnPropertySet 'Tenants', 'TenantId', 'Primary key'
exec db.ColumnPropertySet 'Tenants', 'CreatedAtUtc', 'The time when this tenant was created'
exec db.ColumnPropertySet 'Tenants', 'DeletedAtUtc', 'When non-null, the time this tenant was deleted'
exec db.ColumnPropertySet 'Tenants', 'TenantName', 'Recognizable title for the tenant'

GO

insert into Tenants (TenantName) values ('_Bootstrap');

GO

/*
TODO: This table will get large, create partitioning key based on TenantId
*/
create table Audits
(
	AuditId bigint not null identity primary key,
	TenantId smallint not null references Tenants(TenantId),
	CreatedAtUtc datetime not null default(getutcdate()),
	ActorUserId bigint not null
)

GO

exec db.TablePropertySet 'Audits', 'Information about an event'
exec db.ColumnPropertySet 'Audits', 'AuditId', 'Primary key'
exec db.ColumnPropertySet 'Audits', 'TenantId', 'Foreign key to tenant to which this belongs'
exec db.ColumnPropertySet 'Audits', 'CreatedAtUtc', 'The time when this action was caused'
exec db.ColumnPropertySet 'Audits', 'ActorUserId', 'Foreign key to the user who caused this action'

GO

insert into Audits (TenantId, ActorUserId) values (1, 1);

GO

create table Accounts
(
	AccountId int not null identity primary key,
	TenantId smallint not null references Tenants(TenantId),
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	ParentAccountId int null references Accounts(AccountId),
	AccountName dbo.Title not null
)

GO

create unique index Uniqueness on Accounts(TenantId, ParentAccountId, AccountName) where DeletedAuditId is null

GO

exec db.TablePropertySet 'Accounts', 'A (sub)division of related users that may have common billing/permission systems.'
exec db.ColumnPropertySet 'Accounts', 'AccountId', 'Primary key'
exec db.ColumnPropertySet 'Accounts', 'TenantId', 'Foreign key to the tenant that owns this account'
exec db.ColumnPropertySet 'Accounts', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'Accounts', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'Accounts', 'ParentAccountId', 'Foreign key to a parent account'
exec db.ColumnPropertySet 'Accounts', 'AccountName', 'Human recognizable name for an account'

GO

insert into Accounts (TenantId, CreatedAuditId, AccountName) values (1, 1, '_BootstrapAccount');

GO

create table ApplicationTypes
(
	ApplicationTypeId smallint not null primary key,
	ApplicationTypeName dbo.DeveloperName not null unique,
	--TODO: Add information about "Singleton-ness".  Should this be at a tenant or root account basis or account?  Perhaps we need bits for each
)

GO

exec db.TablePropertySet 'ApplicationTypes', '1', @propertyName='RS_DontCreateFromCode'
exec db.TablePropertySet 'ApplicationTypes', '1', @propertyName='RS_DontDeleteFromCode'
exec db.TablePropertySet 'ApplicationTypes', 'Type of application that runs this system'
exec db.ColumnPropertySet 'ApplicationTypes', 'ApplicationTypeId', 'Primary key'
exec db.ColumnPropertySet 'ApplicationTypes', 'ApplicationTypeName', 'Name of this application'

GO

insert into ApplicationTypes
(ApplicationTypeId, ApplicationTypeName)
values
(1, 'TenantSetupSite');

GO

CREATE TABLE ApplicationTypePermissions
(
	ApplicationTypePermissionId smallint not null primary key,
	ApplicationTypeId smallint not null references ApplicationTypes(ApplicationTypeId),
	PermissionName dbo.DeveloperName not null
)

GO

create unique index Uniqueness on ApplicationTypePermissions(ApplicationTypeId, PermissionName);

GO

exec db.TablePropertySet 'ApplicationTypePermissions', '1', @propertyName='RS_DontCreateFromCode'
exec db.TablePropertySet 'ApplicationTypePermissions', '1', @propertyName='RS_DontDeleteFromCode'
exec db.TablePropertySet 'ApplicationTypePermissions', 'Permissions that can be applied to an application'
exec db.ColumnPropertySet 'ApplicationTypePermissions', 'ApplicationTypePermissionId', 'Primary key'
exec db.ColumnPropertySet 'ApplicationTypePermissions', 'ApplicationTypeId', 'Foreign key to the type of application'
exec db.ColumnPropertySet 'ApplicationTypePermissions', 'PermissionName', 'The name of the permission'

GO

insert into ApplicationTypePermissions (ApplicationTypePermissionId, ApplicationTypeId, PermissionName)
values
(1, 1, 'Login'),
(2, 1, 'CanCreateNewTenants');

GO

create table ApplicationInstances
(
	ApplicationInstanceId int not null identity primary key,
	TenantId smallint not null references Tenants(TenantId),
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	ApplicationTypeId smallint not null references ApplicationTypes(ApplicationTypeId),
	AccountId int null references Accounts(AccountId),
	AnonymousUserRoleId int null,
	ApplicationInstanceName dbo.Title not null
)

GO

create unique index Uniqueness on ApplicationInstances(TenantId, ApplicationTypeId, AccountId, ApplicationInstanceName) where DeletedAuditId is null

GO

exec db.TablePropertySet 'ApplicationInstances', 'Type of application that runs this system'
exec db.ColumnPropertySet 'ApplicationInstances', 'ApplicationInstanceId', 'Primary key'
exec db.ColumnPropertySet 'ApplicationInstances', 'TenantId', 'Foreign key to the tenant that owns this account'
exec db.ColumnPropertySet 'ApplicationInstances', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'ApplicationInstances', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'ApplicationInstances', 'ApplicationTypeId', 'Foreign key to the type of this application'
exec db.ColumnPropertySet 'ApplicationInstances', 'AccountId', 'Foreign key to the account that owns this application.  When null, it is owned solely by the tenant'
exec db.ColumnPropertySet 'ApplicationInstances', 'AnonymousUserRoleId', 'Foreign key to the role which should be used when this application is accessed by an anonymous user'
exec db.ColumnPropertySet 'ApplicationInstances', 'ApplicationInstanceName', 'Human readible name of the application'

GO

insert into ApplicationInstances
(TenantId, CreatedAuditId, ApplicationTypeId, AccountId, ApplicationInstanceName)
values
(1, 1, 1, 1, 'TenantCreatorApplicationInstance');

GO

create table ApplicationInstanceEntryPoints
(
	ApplicationInstanceEntryPointId int not null identity primary key,
	ApplicationInstanceId int not null references ApplicationInstances(ApplicationInstanceId),
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	IsActive bit not null default(0),
	AllowNonLocalConnections bit not null default(0),
	Port int not null,
	Hostname nvarchar(1024) not null,
	_HostnameHash as cast(HASHBYTES('MD5', lower(hostname)) as binary(16)) persisted --required due to the potential size of the hostname
	--Do we need a protocol name?
)

GO

create unique index Uniqueness on ApplicationInstanceEntryPoints(_HostnameHash, Port) where DeletedAuditId is null;

GO

exec db.TablePropertySet 'ApplicationInstanceEntryPoints', 'Addresses used to access an application'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'ApplicationInstanceEntryPointId', 'Primary key'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'ApplicationInstanceId', 'Foreign key to the application instance this to which this entrypoint is applicable'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'IsActive', 'When 1, this entrypoint is active, when 0, it is not.'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'AllowNonLocalConnections', 'When 1, connections from remote computers may be allowed, when 0, they are blocked.'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'Hostname', 'An internet host that should respond to this application'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', 'Port', 'A port that is listening on the host for connections'
exec db.ColumnPropertySet 'ApplicationInstanceEntryPoints', '_HostnameHash', 'Computed column for parts of this table that represent uniqueness.  This uses MD5 to keep under the SQL Server max conditions for unique keys, not for any crypto purposes.'

GO

insert into ApplicationInstanceEntryPoints
(ApplicationInstanceId, CreatedAuditId, DeletedAuditId, Hostname, Port, IsActive, AllowNonLocalConnections)
values
(1,1,null, 'localhost', 54321, 1, 0);

GO

create table UserTypes
(
	UserTypeId tinyint not null primary key,
	UserTypeName dbo.DeveloperName not null unique,
)

GO

exec db.TablePropertySet 'UserTypes', '1', @propertyName='RS_DontCreateFromCode'
exec db.TablePropertySet 'UserTypes', '1', @propertyName='RS_DontDeleteFromCode'
exec db.TablePropertySet 'UserTypes', 'Type of application that runs this system'
exec db.ColumnPropertySet 'UserTypes', 'UserTypeId', 'Primary key'
exec db.ColumnPropertySet 'UserTypes', 'UserTypeName', 'Name of this user type'

GO

insert into UserTypes
(UserTypeId, UserTypeName)
values
(1, 'DBA'),
(2, 'SystemUtility'),
(3, 'AnonymousUser'),
(4, 'EndUser');

GO

create table Users
(
	UserId bigint not null identity primary key,
	TenantId smallint null references Tenants(TenantId),
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	UserTypeId tinyint not null references UserTypes(UserTypeId),
	LockoutEnabled bit not null default(0),
	LockoutEndDateUtc datetime null,
	UserName dbo.Title not null,
	EmailAddress dbo.EmailAddress null
)

GO

create unique index UniquenessUsername on Users(TenantId, UserName) where DeletedAuditId is null;

GO

alter table Users
ADD CONSTRAINT UserTypeTenantCheck CHECK 
(
	(UserTypeId in (1,2) and TenantId is null) or
	(UserTypeId in (3,4) and TenantId is not null) 
)

GO

exec db.TablePropertySet 'Users', '1', @propertyName='RS_DeleteViaAudit'
exec db.TablePropertySet 'Users', 'Human, agent, and group based users/logins.'
exec db.ColumnPropertySet 'Users', 'UserId', 'Primary key.'
exec db.ColumnPropertySet 'Users', 'TenantId', 'Foreign key to the tenant that owns this account.  NULL is for usertypes that can span all tenants.'
exec db.ColumnPropertySet 'Users', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'Users', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'Users', 'UserTypeId', 'Foreign key to user type'
exec db.ColumnPropertySet 'Users', 'UserName', 'The name of the user; oftentimes, this will be an email address'
exec db.ColumnPropertySet 'Users', 'EmailAddress', 'Primary email address associated with this account'
exec db.ColumnPropertySet 'Users', 'LockoutEndDateUtc', 'When null or in the past user can login.  Otherwise, this is the time until which the user can login'

GO

insert into users
(TenantId, CreatedAuditId, UserTypeId, UserName)
values
(1, 1, 4, 'BootstrapUser'),
(null, 1, 1, ORIGINAL_LOGIN());

GO

--select * from users

create table UserAccounts
(
	UserAccountId bigint not null identity primary key,
	TenantId smallint not null references Tenants(TenantId),
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	UserId bigint not null references Users(UserId),
	AccountId int not null references Accounts(AccountId),
	IsActive bit not null default(0),
)

GO 

create unique index Uniqueness on UserAccounts(TenantId, UserId, AccountId) where DeletedAuditId is null;

GO

exec db.TablePropertySet 'UserAccounts', '1', @propertyName='RS_DeleteViaAudit'
exec db.TablePropertySet 'UserAccounts', 'Given that a user is a "human" and could work for multiple organizations, this maps that user to a distinct organization.'
exec db.ColumnPropertySet 'UserAccounts', 'UserAccountId', 'Primary key'
exec db.ColumnPropertySet 'UserAccounts', 'TenantId', 'Foreign key to the tenant that owns this account'
exec db.ColumnPropertySet 'UserAccounts', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'UserAccounts', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'UserAccounts', 'UserId', 'Foreign key to user'
exec db.ColumnPropertySet 'UserAccounts', 'AccountId', 'Foreign key to the account'
exec db.ColumnPropertySet 'UserAccounts', 'IsActive', 'When 1, this entrypoint is active, when 0, it is not.'

GO

insert into UserAccounts
(TenantId, CreatedAuditId, UserId, AccountId)
values
(1,1,1,1)

GO

CREATE TABLE UserClaims
(
	UserClaimId bigint not null identity primary key,
	UserId bigint not null references Users(UserId),
	ClaimType nvarchar(max) NULL,
	ClaimValue nvarchar(max) NULL
)

GO

exec db.TablePropertySet 'UserClaims', '1', @propertyName='RS_DeleteViaDelete'
exec db.ColumnPropertySet 'UserClaims', 'UserClaimId', 'Primary key'
exec db.ColumnPropertySet 'UserClaims', 'UserId', 'Foreign key to user'

GO

CREATE TABLE Roles
(
	RoleId int not null identity primary key,
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	AccountId int not null references Accounts(AccountId),
	RoleName dbo.Title not null
)

GO

create unique index Uniqueness on Roles(AccountId, RoleName) where DeletedAuditId is null;

GO

exec db.TablePropertySet 'Roles', '1', @propertyName='RS_DeleteViaAudit'
exec db.TablePropertySet 'Roles', 'Account defined roles (a named collection of privileges).'
exec db.ColumnPropertySet 'Roles', 'RoleId', 'Primary key'
exec db.ColumnPropertySet 'Roles', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'Roles', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'Roles', 'AccountId', 'Foreign key to the account'
exec db.ColumnPropertySet 'Roles', 'RoleName', 'A human recognizable name for this role'

GO

insert into Roles
(CreatedAuditId, AccountId, RoleName)
values
(1, 1, 'SuperUser');

GO

CREATE TABLE RolePermissions
(
	RolePermissionId int not null identity primary key,
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	RoleId int not null references Roles(RoleId),
	ApplicationTypeId smallint null references ApplicationTypes(ApplicationTypeId),
	ApplicationInstanceId int null references ApplicationInstances(ApplicationInstanceId),
	ApplicationTypePermissionId smallint not null references ApplicationTypePermissions(ApplicationTypePermissionId),
	FlowsIntoChildAccounts bit not null default(0)
)

GO

create unique index Uniqueness on RolePermissions(RoleId, ApplicationTypeId, ApplicationInstanceId, ApplicationTypePermissionId) where DeletedAuditId is null;

GO

exec db.TablePropertySet 'RolePermissions', '1', @propertyName='RS_DeleteViaAudit'
exec db.TablePropertySet 'RolePermissions', 'Permissions that are assigned to a role.'
exec db.ColumnPropertySet 'RolePermissions', 'RolePermissionId', 'Primary key'
exec db.ColumnPropertySet 'RolePermissions', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'RolePermissions', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'RolePermissions', 'RoleId', 'Foreign key to the role to which the permission applies'
exec db.ColumnPropertySet 'RolePermissions', 'ApplicationTypeId', 'Foreign key to the application type to which this applies'
exec db.ColumnPropertySet 'RolePermissions', 'ApplicationInstanceId', 'Foreign key to the specific application instance to which this applies.  When null, it applies to all application instances of the application type'
exec db.ColumnPropertySet 'RolePermissions', 'ApplicationTypePermissionId', 'Foreign key to the permission being granted'
exec db.ColumnPropertySet 'RolePermissions', 'FlowsIntoChildAccounts', 'When 1, this permission flows into all child accounts, when 0, it is restricted to the directly implied useraccount account'

GO

insert into RolePermissions
(CreatedAuditId, RoleId, ApplicationTypeId, ApplicationInstanceId, ApplicationTypePermissionId)
values
(1, 1, 1, 1, 1),
(1, 1, 1, 1, 2);

GO

CREATE TABLE UserAccountRoles
(
	UserAccountRoleId bigint not null identity primary key,
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	UserAccountId bigint not null references UserAccounts(UserAccountId),
	RoleId int not null references Roles(RoleId),
)

GO

exec db.TablePropertySet 'UserAccountRoles', '1', @propertyName='RS_DeleteViaAudit'
exec db.TablePropertySet 'UserAccountRoles', 'Roles assigned to a user account.'
exec db.ColumnPropertySet 'UserAccountRoles', 'UserAccountRoleId', 'Primary key'
exec db.ColumnPropertySet 'UserAccountRoles', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'UserAccountRoles', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.'
exec db.ColumnPropertySet 'UserAccountRoles', 'UserAccountId', 'Foreign key to the useraccount to which this applies'
exec db.ColumnPropertySet 'UserAccountRoles', 'RoleId', 'Foreign key to the role to which the permission applies'

GO

insert into UserAccountRoles
(CreatedAuditId, UserAccountId, RoleId)
values
(1,1,1);

GO

create table Plugins
(
	PluginId int not null identity primary key,
	TenantId smallint null references Tenants(TenantId),
	PluginName dbo.DeveloperName not null unique,
	PluginInitializationData nvarchar(max) null,
)

GO

exec db.TablePropertySet 'Plugins', 'A code based plugin to the application'
exec db.ColumnPropertySet 'Plugins', 'PluginId', 'Primary key'
exec db.ColumnPropertySet 'Plugins', 'TenantId', 'Foreign key to the tenant that can use this plugin, when null, all tenants can use it'
exec db.ColumnPropertySet 'Plugins', 'PluginName', 'A name corresponding to what the code exposes'
exec db.ColumnPropertySet 'Plugins', 'PluginInitializationData', 'Data used to initialize the plugin'

GO

create table Providers
(
	ProviderId int not null identity primary key,
	TenantId smallint null references Tenants(TenantId),
	PluginId int not null references PlugIns(PluginId),
	ProviderInitializationData nvarchar(max),
)

GO

exec db.TablePropertySet 'Providers', 'An "instantiated" plugin'
exec db.ColumnPropertySet 'Providers', 'ProviderId', 'Primary key'
exec db.ColumnPropertySet 'Providers', 'TenantId', 'Foreign key to the tenant that can use this plugin, when null, all tenants can use it'
exec db.ColumnPropertySet 'Providers', 'PluginId', 'Foreign key to the underlying plugin'
exec db.ColumnPropertySet 'Providers', 'ProviderInitializationData', 'Data used to initialize the plugin'

GO

create table Passwords
(
	PasswordId bigint not null identity primary key,
	CreatedAuditId bigint not null references Audits(AuditId),
--	PasswordProviderId int not null references Providers(ProviderId),
	UserId bigint not null references Users(UserId),
	MustChangeAtNextLogin bit not null default(0),
	IsCurrent bit not null,
	ExpiresAt datetime null,
	PasswordData nvarchar(512) null
)

GO

create unique index Uniqueness on Passwords(UserId) where IsCurrent=1;

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

--can't delete
exec db.TablePropertySet 'UserTokenTypes', '1', @propertyName='RS_DontCreateFromCode'
exec db.TablePropertySet 'UserTokenTypes', '1', @propertyName='RS_DontDeleteFromCode'

GO

create table UserTokens
(
	UserTokenId bigint not null identity primary key,
	CreatedAuditId bigint not null references Audits(AuditId),
	UserTokenTypeId smallint not null references UserTokenTypes(UserTokenTypeId),
	Token varchar(40) not null unique,
	UserId bigint not null references Users(UserId),
	UserAccountId bigint null references UserAccounts(UserAccountId),
	ValidUntilUtc datetime null,
	ClaimedAuditId bigint null references Audits(AuditId),
	AppData nvarchar(max)
)

GO

exec db.TablePropertySet 'UserTokens', 'A limited use token that is associated with a user that will allows an action'
exec db.ColumnPropertySet 'UserTokens', 'UserTokenId', 'Primary key.'
exec db.ColumnPropertySet 'UserTokens', 'Token', 'The unique token'
exec db.ColumnPropertySet 'UserTokens', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'UserTokens', 'ValidUntilUtc', 'Datetime when this row expires and should no longer be considered'
exec db.ColumnPropertySet 'UserTokens', 'ClaimedAuditId', 'Foreign key to the action that caused the claiming of the reset'
exec db.ColumnPropertySet 'UserTokens', 'UserId', 'The user to who this token is attached'
exec db.ColumnPropertySet 'UserTokens', 'UserAccountId', 'When relevant, the user account to who this token is attached'
exec db.ColumnPropertySet 'UserTokens', 'UserTokenTypeId', 'Foreign key to the type of token that has been granted'
exec db.ColumnPropertySet 'UserTokens', 'AppData', 'Application specific data attached to the token'

GO

ALTER TABLE Audits ADD CONSTRAINT ActorUserId_FK FOREIGN KEY (ActorUserId) REFERENCES Users(UserId);
ALTER TABLE ApplicationInstances ADD CONSTRAINT AnonymousUserRoleId_FK FOREIGN KEY (AnonymousUserRoleId) REFERENCES Roles(RoleId)

GO

--select * from db.schemameta

/*

insert into Plugins
(TenantId, PluginName, PluginInitializationData)
values
(null, 'SaltHashPasswordProviderFactory', null);

insert into Providers
(TenantId, PluginId, ProviderInitializationData)
values
(null, 1, 'sha256,128');

insert into ApplicationInstanceEntryPoints
(
	ApplicationInstanceId,
	CreatedAuditId,
	IsActive,
	AllowNonLocalConnections,
	Port,
	Hostname
)
values
(1,1,1,0,54321, 'localhost')

select * from applicationtypes
select * from applicationinstances
select * from ApplicationInstanceEntryPoints

select * from audits
select * from users
select * from passwords

*/