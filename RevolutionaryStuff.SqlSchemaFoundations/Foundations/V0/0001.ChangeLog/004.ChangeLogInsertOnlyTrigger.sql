CREATE TRIGGER [db].[ChangeLogInsertOnly]
ON [db].[ChangeLog] 
for delete, update
AS

	ROLLBACK TRAN
	raiserror('DatabaseChangeLog is a log table.  Items cannot be deleted or changed', 16, 1)

GO

ALTER TABLE [db].[ChangeLog] ENABLE TRIGGER [ChangeLogInsertOnly]
GO
