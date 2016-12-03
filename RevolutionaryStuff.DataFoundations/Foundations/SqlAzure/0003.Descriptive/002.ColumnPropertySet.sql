CREATE proc [db].[ColumnPropertySet]
	@tableName sysname,
	@columnName sysname,
	@propertyVal nvarchar(3750),
	@tableSchema sysname=null,
	@propertyName sysname=null
as
begin
	
	set @propertyName = coalesce(@propertyName, 'MS_Description')
	set @tableSchema = coalesce(@tableSchema, 'dbo')

	declare @cnt int

	select @cnt=count(*)
	from sys.fn_listextendedproperty(@propertyName, N'SCHEMA', @tableSchema, N'TABLE', @tableName, N'COLUMN', @columnName)

	if (@cnt>0)
	begin
		EXEC sys.sp_dropextendedproperty @name=@propertyName, @level0type=N'SCHEMA',@level0name=@tableSchema, @level1type=N'TABLE',@level1name=@tableName, @level2type=N'COLUMN',@level2name=@columnName
	end
	EXEC sys.sp_addextendedproperty @name=@propertyName, @value=@propertyVal ,  @level0type=N'SCHEMA',@level0name=@tableSchema, @level1type=N'TABLE',@level1name=@tableName, @level2type=N'COLUMN',@level2name=@columnName

end

GO
