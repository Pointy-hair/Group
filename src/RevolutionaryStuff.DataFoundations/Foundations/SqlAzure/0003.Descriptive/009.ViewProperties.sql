CREATE proc [db].[ViewPropertySet]
	@viewName sysname,
	@propertyVal sql_variant,
	@viewSchema sysname=null,
	@propertyName sysname=null,
	@remove bit=0
as
begin

	set @propertyName = coalesce(@propertyName, 'Comment')
	set @viewSchema = coalesce(@viewSchema, 'dbo')

	declare @cnt int

	select @cnt=count(*)
	from sys.fn_listextendedproperty(@propertyName, N'SCHEMA', @viewSchema, N'VIEW', @viewName, null, null)

	if (@cnt>0)
	begin
		EXEC sys.sp_dropextendedproperty   @name=@propertyName, @level0type=N'SCHEMA',@level0name=@viewSchema, @level1type=N'VIEW',@level1name=@viewName
	end
	if (@remove=1) return;
	EXEC sys.sp_addextendedproperty  @name=@propertyName, @value=@propertyVal, @level0type=N'SCHEMA',@level0name=@viewSchema, @level1type=N'VIEW',@level1name=@viewName

end

GO

create view [db].[ViewProperties]
AS
SELECT s.name SchemaName, t.name ViewName, ep.Name PropertyName, value PropertyValue
FROM 
	sys.extended_properties AS ep with(nolock) 
		INNER JOIN 
	sys.views AS t with(nolock) 
		ON ep.major_id = t.object_id 
		inner join
	sys.schemas AS s with(nolock) 
		on t.schema_id=s.schema_id
WHERE 
	ep.class = 1 and
	ep.minor_id=0 and
	value is not null and
	value <> ''		

GO