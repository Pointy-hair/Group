create view [db].[ColumnProperties]
AS
SELECT s.name SchemaName, t.name TableName, c.name ColumnName, ep.name PropertyName, value PropertyValue
FROM 
	sys.extended_properties AS ep with(nolock) 
		INNER JOIN 
	sys.tables AS t with(nolock) 
		ON ep.major_id = t.object_id 
		INNER JOIN 
	sys.schemas AS s with(nolock) 
		on t.schema_id=s.schema_id 
		INNER JOIN
	sys.columns AS c with(nolock) 
		ON ep.major_id = c.object_id 
		AND ep.minor_id = c.column_id
WHERE 
	class = 1 and
	value is not null and
	value <> ''				
