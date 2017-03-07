create view db.TraffkGlobalCols 
as
select * from information_schema.columns (nolock)

GO
