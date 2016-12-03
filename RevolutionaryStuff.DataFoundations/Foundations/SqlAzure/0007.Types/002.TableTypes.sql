CREATE TYPE [dbo].[IntListType] AS TABLE(
	[Val] [int] NULL,
	[Pos] [int] NULL
)

GO

CREATE TYPE [dbo].[StringListType] AS TABLE(
	[Val] [nvarchar](max) NULL,
	[Pos] [int] NULL
)

GO

CREATE TYPE [dbo].[BigIntListType] AS TABLE(
	[Val] [bigint] NULL,
	[Pos] [int] NULL
)

GO

CREATE TYPE [dbo].[GuidListType] AS TABLE(
	[Val] [uniqueidentifier] NULL,
	[Pos] [int] NULL
)

GO

CREATE TYPE [dbo].[DateListType] AS TABLE(
	[Val] [datetime] NULL,
	[Pos] [int] NULL
)

GO
