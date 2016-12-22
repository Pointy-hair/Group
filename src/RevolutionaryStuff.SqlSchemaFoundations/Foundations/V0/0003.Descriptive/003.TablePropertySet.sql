create proc [db].[TablePropertySet]
	@tableName sysname,
	@propertyVal sql_variant,
	@tableSchema sysname=null,
	@propertyName sysname=null
as
begin

	set @propertyName = coalesce(@propertyName, 'Comment')
	set @tableSchema = coalesce(@tableSchema, 'dbo')

	declare @cnt int

	select @cnt=count(*)
	from sys.fn_listextendedproperty(@propertyName, N'SCHEMA', @tableSchema, N'TABLE', @tableName, null, null)

	if (@cnt>0)
	begin
		EXEC sys.sp_dropextendedproperty   @name=@propertyName, @level0type=N'SCHEMA',@level0name=@tableSchema, @level1type=N'TABLE',@level1name=@tableName
	end
	EXEC sys.sp_addextendedproperty  @name=@propertyName, @value=@propertyVal, @level0type=N'SCHEMA',@level0name=@tableSchema, @level1type=N'TABLE',@level1name=@tableName

end
