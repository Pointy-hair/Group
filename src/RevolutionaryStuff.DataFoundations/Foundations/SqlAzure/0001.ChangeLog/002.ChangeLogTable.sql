CREATE TABLE db.[ChangeLog]
(
	ChangeLogId int not null identity primary key,
	[EventType] [varchar](50) NOT NULL,
	[ObjectName] [varchar](256) NOT NULL,
	[ObjectType] [varchar](25) NOT NULL,
	[SqlCommand] [varchar](max) NOT NULL,
	[EventDate] [datetime] NOT NULL,
	[LoginName] [varchar](256) NOT NULL,
	[Data] [xml] NULL
)
GO
