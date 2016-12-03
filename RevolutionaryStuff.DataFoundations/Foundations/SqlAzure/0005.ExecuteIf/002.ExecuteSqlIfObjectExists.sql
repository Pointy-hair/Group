CREATE proc [db].[ExecuteSqlIfObjectExists]
	@objectName nvarchar(128),
	@sql nvarchar(max)
as
begin

	if (exists(select 1 from sys.objects where name=@objectName))	EXEC dbo.sp_executesql @statement = @sql

end
