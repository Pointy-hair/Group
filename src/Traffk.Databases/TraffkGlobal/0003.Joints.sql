CREATE TABLE [dbo].[ReportMetaData](
	[ReportMetaDataId] [int] IDENTITY(-1,-1) NOT NULL primary key,
	[ExternalReportKey] [nvarchar](2000) NOT NULL,
	[ParentReportMetaDataId] [int] NULL,
	[RowStatus] [dbo].[RowStatus] NOT NULL default(1),
	[OwnerContactId] [bigint] NULL,
	[CreatedAtUtc] [datetime] NOT NULL default (getutcdate()),
	[ReportDetails] [dbo].[JsonObject] NOT NULL,
)

GO

exec db.TablePropertySet  'ReportMetaData', '1', @propertyName='AddToDbContext'
exec db.TablePropertySet  'ReportMetaData', '1', @propertyName='GeneratePoco'
exec db.TablePropertySet  'ReportMetaData', 'Global', @propertyName='JointPart'
exec db.ColumnPropertySet 'ReportMetaData', 'CreatedAtUtc', 'Datetime when this entity was created.'
exec db.ColumnPropertySet 'ReportMetaData', 'RowStatus', '1', @propertyName='ImplementsRowStatusSemantics', @tableSchema='dbo'
exec db.ColumnPropertySet 'ReportMetaData', 'RowStatus', 'missing', @propertyName='AccessModifier', @tableSchema='dbo'

GO
