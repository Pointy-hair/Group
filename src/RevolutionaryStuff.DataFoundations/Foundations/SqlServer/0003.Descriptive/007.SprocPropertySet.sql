create proc [db].[SprocPropertySet]
	@sprocName sysname,
	@propertyVal sql_variant,
	@routineSchema sysname=null,
	@propertyName sysname=null
as
begin

	set @propertyName = coalesce(@propertyName, 'Comment')
	set @routineSchema = coalesce(@routineSchema, 'dbo')

	declare @cnt int

	select @cnt=count(*)
	from sys.fn_listextendedproperty(@propertyName, N'SCHEMA', @routineSchema, N'PROCEDURE', @sprocName, null, null)

	if (@cnt>0)
	begin
		EXEC sys.sp_dropextendedproperty   @name=@propertyName, @level0type=N'SCHEMA',@level0name=@routineSchema, @level1type=N'PROCEDURE',@level1name=@sprocName
	end
	EXEC sys.sp_addextendedproperty  @name=@propertyName, @value=@propertyVal, @level0type=N'SCHEMA',@level0name=@routineSchema, @level1type=N'PROCEDURE',@level1name=@sprocName

end
