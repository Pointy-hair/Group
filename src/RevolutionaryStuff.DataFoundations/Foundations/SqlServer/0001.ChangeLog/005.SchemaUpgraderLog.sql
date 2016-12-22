create table db.SchemaUpgraderLog
(
	SchemaUpgraderLogId int not null identity primary key,
	CreatedAt datetime not null default(getutcdate()),
	Namespace varchar(255) not null,
	Path varchar(255) not null,
	Title varchar(255) not null,
	PreChangeLogId int not null references db.changelog(changelogid),
	PostChangeLogId int null references db.changelog(changelogid)
)

GO

create unique index Uniqueness on db.SchemaUpgraderLog(Namespace, Path)

GO

create proc db.SchemaLogUpgradeBegin
	@namespace varchar(255),
	@path varchar(255),
	@title varchar(255)
as
begin

	insert into db.SchemaUpgraderLog
	(namespace, path, title, prechangelogid)
	select @namespace, @path, @title, max(changelogid)
	from
		db.changelog

	select @@identity

end	

GO

create proc db.SchemaLogUpgradeEnd
	@schemaUpgraderLogId int
as
begin

	update sul
	set
		PostChangeLogId = cl.changelogid
	from
		db.SchemaUpgraderLog sul
			cross apply
		(select max(changelogid) changelogid from db.changelog) cl
	where
		sul.SchemaUpgraderLogId=@schemaUpgraderLogId

end

GO

create proc db.SchemaRequirementExists
	@namespace varchar(255),
	@path varchar(255)
as
begin

	select cast (1 as bit)
	from
		db.SchemaUpgraderLog
	where
		[namespace]=@namespace and
		[path] like @path+'%'
end

GO

create proc db.SchemaRequirementMaxVersionGet
	@namespace varchar(255)
as
begin

	select top(1) [path]
	from
		db.SchemaUpgraderLog
	where
		[namespace]=@namespace
	order by 
		[path] desc

end

GO
