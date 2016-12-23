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

GO

create proc [db].[SprocParameterPropertySet]
	@sprocName sysname,
	@parameterName sysname,
	@propertyVal nvarchar(3750),
	@sprocSchema sysname=null,
	@propertyName sysname=null
as
begin
	
	set @propertyName = coalesce(@propertyName, 'MS_Description')
	set @sprocSchema = coalesce(@sprocSchema, 'dbo')

	declare @cnt int

	select @cnt=count(*)
	from sys.fn_listextendedproperty(@propertyName, N'SCHEMA', @sprocSchema, N'PROCEDURE', @sprocName, N'PARAMETER', @parameterName)

	if (@cnt>0)
	begin
	exec db.printnow 'a'
		EXEC sys.sp_dropextendedproperty   @name=@propertyName, @level0type=N'SCHEMA',@level0name=@sprocSchema, @level1type=N'PROCEDURE',@level1name=@sprocName, @level2type=N'PARAMETER',@level2name=@parameterName
	end
	if (@propertyVal is not null)
	begin
	exec db.printnow 'b'
		EXEC sys.sp_addextendedproperty  @name=@propertyName, @value=@propertyVal, @level0type=N'SCHEMA',@level0name=@sprocSchema, @level1type=N'PROCEDURE',@level1name=@sprocName, @level2type=N'PARAMETER',@level2name=@parameterName
	end

end

GO
