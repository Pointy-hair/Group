create view [db].[SprocProperties]
AS
SELECT s.name SchemaName, t.name SprocName, ep.Name PropertyName, value PropertyValue
FROM 
	sys.extended_properties AS ep with(nolock) 
		INNER JOIN 
	sys.procedures AS t with(nolock) 
		ON ep.major_id = t.object_id 
		inner join
	sys.schemas AS s with(nolock) 
		on t.schema_id=s.schema_id
WHERE 
	ep.class = 1 and
	ep.minor_id=0 and
	value is not null and
	value <> ''		
