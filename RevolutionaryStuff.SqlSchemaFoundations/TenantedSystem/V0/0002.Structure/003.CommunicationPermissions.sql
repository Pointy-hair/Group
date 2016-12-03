create table CommunicationTypes
(
	CommunicationTypeId smallint not null primary key,
	CommunicationTypeName dbo.DeveloperName not null unique,
)

GO

exec db.TablePropertySet 'CommunicationTypes', 'Types of communication (i.e. email) that can be used when interacting with a person'
exec db.ColumnPropertySet 'CommunicationTypes', 'CommunicationTypeId', 'Primary key'
exec db.ColumnPropertySet 'CommunicationTypes', 'CommunicationTypeName', 'Name of this type of communication'

GO

create table CommunicationTopics --or perhaps CommunicationInterests
(
	CommunicationTopicId int not null primary key,
	TenantId int not null references Tenants(TenantId),
	CreatedAuditId bigint not null references Audits(AuditId),
	DeletedAuditId bigint null references Audits(AuditId),
	ParentCommunicationTopicId int null references CommunicationTopics(CommunicationTopicId)
)

GO 

exec db.TablePropertySet 'CommunicationTopics', 'The base for a mailing list, communication interest, or a topic that a contact may be interested in receiving.'
exec db.ColumnPropertySet 'CommunicationTopics', 'CommunicationTopicId', 'Primary key'
exec db.ColumnPropertySet 'CommunicationTopics', 'TenantId', 'Foreign key to the tenant that owns this topic'
exec db.ColumnPropertySet 'CommunicationTopics', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'CommunicationTopics', 'DeletedAuditId', 'Foreign key to the action that caused the deletion of this row.  When null, this row is "alive."'
exec db.ColumnPropertySet 'CommunicationTopics', 'ParentCommunicationTopicId', 'When specifid, this is the parent for this node, thus allowing for the creation of a hierarchy'

GO

/*
create table CommunicationPermissionLog
(
	CommunicationPermissionLogId bigint not null primary key,
	TenantId int not null references Tenants(TenantId),
	CreatedAuditId bigint not null references Audits(AuditId),
	CommunicationTypeId smallint not null references CommunicationTypes(CommunicationTypeId),
	CommunicationAllowed bit not null,
	CommunicationTopicId int not null references CommunicationTopics(CommunicationTopicId),
	SpecificEndPointData nvarchar(255) null,
	SpecificContactId bigint null references Contacts(ContactId),
	SpecificContactAddressId bigint null references Addresses(AddressId),
	SpecificContactAddressTypeId smallint null references AddressTypes(AddressTypeId)
)

GO

ALTER TABLE CommunicationPermissionLog
ADD CONSTRAINT SpecificsSpecified CHECK 
(
	(SpecificEndPointData is not null and SpecificContactId is     null) or
	(SpecificEndPointData is     null and SpecificContactId is not null) 
)

GO

exec db.TablePropertySet 'CommunicationPermissionLog', 'Log of permissions granted by end user over communication.  As 2+ "contacts" in the same tenant could have the same address (especially as a contact could be in multiple directories), we need to store and reference the specific communication endpoint so as not to communicate with opted-out contacts.'
exec db.ColumnPropertySet 'CommunicationPermissionLog', 'CommunicationPermissionLogId', 'Primary key'
exec db.ColumnPropertySet 'CommunicationPermissionLog', 'TenantId', 'Foreign key to the tenant that owns this account.  As communication preferences may not be tied to a contact, this is required'
exec db.ColumnPropertySet 'CommunicationPermissionLog', 'CreatedAuditId', 'Foreign key to the action that caused the creation of this row'
exec db.ColumnPropertySet 'CommunicationPermissionLog', 'CommunicationTypeId', 'Foreign key for the type of communication'
exec db.ColumnPropertySet 'CommunicationPermissionLog', 'CommunicationAllowed', 'When true, communication of this sort is allowed. When false, permission was rejected'
exec db.ColumnPropertySet 'CommunicationPermissionLog', 'CommunicationTopicId', 'When null, the permission relates to all types of communications, when specified, it applies to that specific topic'

GO
*/
