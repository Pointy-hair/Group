create table Releases
(
	ReleaseId int not null identity primary key,
	ApplicationType dbo.developername,
	ReleaseDate Date,
	ReleaseName dbo.Title,
	ReleaseNotes nvarchar(max) default null
)

GO

exec db.TablePropertySet  'Releases', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'Releases', '1', @propertyName='GeneratePoco'

GO

create table ReleaseChanges
(
	ReleaseChangeId int not null identity primary key,
	ReleaseId int not null references Releases(ReleaseId) on delete cascade,
	ChangeType dbo.Title,
	Title dbo.Title,
	[Order] int default null 
)

GO

exec db.TablePropertySet  'ReleaseChanges', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'ReleaseChanges', '1', @propertyName='GeneratePoco'

GO
