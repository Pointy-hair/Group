	create proc [db].[ExecuteSqlIfColumnNotExists]
		@tableName nvarchar(128),
		@columnName nvarchar(128),
		@sql nvarchar(max)
	as
	begin

	if (not(exists(select 1 from information_schema.columns where table_name=@tableName and column_name=@columnName))) 	EXEC dbo.sp_executesql @statement = @sql

	end
