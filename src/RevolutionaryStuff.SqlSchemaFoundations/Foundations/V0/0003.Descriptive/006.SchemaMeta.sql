create view [db].[SchemaMeta]
as
select *
from
(
	select
	(
		select
		(
			select
				t.table_Schema '@schema',
				t.table_name '@name',
				(
					select 
						p.PropertyName '@name',
						p.PropertyValue '@value'
					from 
						db.TableProperties p with (nolock)
					where
						p.SchemaName = t.table_schema and
						p.TableName = t.table_name
					for xml path ('Property'), type
				) Properties,
				(
					select 
						c.column_name '@name',
						coalesce(pk.isPk,0) '@isPrimaryKey',
						cast(case when c.is_nullable='YES' then 1 else 0 end as bit) '@isNullable',
						c.data_type '@sqlType',
						c.character_maximum_length '@maxLen',
						c.column_default '@default',
						(
							select 
								p.PropertyName '@name',
								p.PropertyValue '@value'
							from 
								db.ColumnProperties p with (nolock)
							where
								p.SchemaName = c.table_schema and
								p.TableName = c.table_name and
								p.ColumnName = c.column_name
							for xml path ('Property'), type
						) Properties
					from 
						information_Schema.columns c with (nolock)
							outer apply
						(
							select 1 isPk
							from 
								INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc with (nolock)
									inner join
								INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu with (nolock)
									on tc.constraint_name=ccu.constraint_name 
									and tc.table_schema=ccu.table_schema 
									and tc.table_name=ccu.table_name
							where
								tc.[CONSTRAINT_TYPE]='PRIMARY KEY' and
								tc.table_schema=c.table_schema and
								tc.table_name=c.table_name and
								ccu.column_name=c.column_name
						) pk

					where
						c.table_name=t.table_name and
						c.table_schema=t.table_schema
					for xml path ('Column'), type
				) Columns
			from 
				INFORMATION_SCHEMA.tables t with (nolock)
			for xml path('Table'), type
		) for xml path('Tables'), type
	) for xml path('SchemaMeta'), type
) x(SchemaMeta)


