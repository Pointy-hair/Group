CREATE TRIGGER [db].[SchemaUpgraderNonDeletable]
ON [db].[SchemaUpgraderLog] 
for delete
AS

	ROLLBACK TRAN
	raiserror('SchemaUpgraderLog prevents items from being deleted', 16, 1)

GO

ALTER TABLE [db].[SchemaUpgraderLog] ENABLE TRIGGER SchemaUpgraderNonDeletable
GO
