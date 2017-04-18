CREATE TABLE [dbo].[Releases](
	[ReleaseId] [int] IDENTITY(1,1) NOT NULL primary key,
	[AppType] [dbo].[DeveloperName] NOT NULL,
	[ReleaseDate] [date] NULL,
	[ReleaseName] [dbo].[Title] NULL,
	[ReleaseNotes] [nvarchar](max) NULL
)

GO

exec db.ColumnPropertySet 'Releases', 'AppType', 'AppTypes', @propertyName='EnumType'

GO

CREATE TABLE [dbo].[ReleaseChanges](
	[ReleaseChangeId] [int] IDENTITY(1,1) NOT NULL primary key,
	[ReleaseId] [int] NOT NULL REFERENCES [dbo].[Releases] ([ReleaseId]) ON DELETE CASCADE,
	[ChangeType] [dbo].[Title] NULL,
	[Title] [dbo].[Title] NULL,
	[Order] [int] NULL
)

GO

