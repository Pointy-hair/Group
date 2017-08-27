create table dbo.Zips
(
	ZipId int not null identity primary key,
	ParentZipId int null references Zips(ZipId),
	ZipFormat dbo.DeveloperName not null,
	CountryId int not null,
	ZipCode nvarchar(20) not null,
)

GO

create unique index UX_ZipCode on dbo.Zips(CountryId, ZipCode)

GO
