CREATE proc [db].RoutineReplace
	@routineName nvarchar(128),
	@sql nvarchar(max),
	@routineSchema sysname=null
as
begin

	set @sql = ltrim(rtrim(@sql))
	set @sql = right(@sql, len(@sql)-6)

	if (exists(select 1 from INFORMATION_SCHEMA.ROUTINES where routine_name=@routineName and (SPECIFIC_SCHEMA=@routineSchema or @routineSchema is null)))
	begin

		set @sql = 'ALTER ' + @sql

	end
	else begin

		set @sql = 'CREATE ' + @sql

	end
	
	EXEC dbo.sp_executesql @statement = @sql

end