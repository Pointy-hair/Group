create view [db].[SchemaMeta]
as
select *
from
(
	select [Tables], Sprocs
	from
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
						c.ordinal_position '@position',
						coalesce(pk.isPk,0) '@isPrimaryKey',
						COLUMNPROPERTY(object_id(c.table_schema+'.'+c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') '@isIdentity',
						cast(case when c.is_nullable='YES' then 1 else 0 end as bit) '@isNullable',
						c.data_type '@sqlType',
						c.character_maximum_length '@maxLen',
						c.column_default '@default',
						ref.TABLE_SCHEMA '@refSchema',
						ref.TABLE_NAME '@refTable',
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
							outer apply
						(
							select  
								--fk.table_Schema, fk.table_name, fk.COLUMN_NAME,
								ref.TABLE_SCHEMA, ref.TABLE_NAME
							from 
								[INFORMATION_SCHEMA].[CONSTRAINT_COLUMN_USAGE] fk with (nolock)
									inner join
								[INFORMATION_SCHEMA].[REFERENTIAL_CONSTRAINTS] rc with (nolock)
									on rc.CONSTRAINT_NAME=fk.CONSTRAINT_NAME
									and rc.CONSTRAINT_SCHEMA=fk.CONSTRAINT_SCHEMA
									and rc.CONSTRAINT_CATALOG=fk.CONSTRAINT_CATALOG
									inner join
								[INFORMATION_SCHEMA].[CONSTRAINT_TABLE_USAGE] ref with (nolock)
									on ref.CONSTRAINT_NAME=rc.UNIQUE_CONSTRAINT_NAME
									and ref.CONSTRAINT_SCHEMA=rc.UNIQUE_CONSTRAINT_SCHEMA
									and ref.CONSTRAINT_CATALOG=rc.UNIQUE_CONSTRAINT_CATALOG
							where
								fk.table_schema=c.TABLE_SCHEMA and						
								fk.table_name=c.table_name and						
								fk.COLUMN_NAME=c.COLUMN_NAME						
						) ref
					where
						c.table_name=t.table_name and
						c.table_schema=t.table_schema
					order by 
						c.ordinal_position
					for xml path ('Column'), type
				) Columns,
				(
					select  
						fk.table_Schema '@schema', 
						fk.table_name '@table', 
						fk.COLUMN_NAME '@column'
					from 
						[INFORMATION_SCHEMA].[CONSTRAINT_COLUMN_USAGE] fk with (nolock)
							inner join
						[INFORMATION_SCHEMA].[REFERENTIAL_CONSTRAINTS] rc with (nolock)
							on rc.CONSTRAINT_NAME=fk.CONSTRAINT_NAME
							and rc.CONSTRAINT_SCHEMA=fk.CONSTRAINT_SCHEMA
							and rc.CONSTRAINT_CATALOG=fk.CONSTRAINT_CATALOG
							inner join
						[INFORMATION_SCHEMA].[CONSTRAINT_TABLE_USAGE] ref with (nolock)
							on ref.CONSTRAINT_NAME=rc.UNIQUE_CONSTRAINT_NAME
							and ref.CONSTRAINT_SCHEMA=rc.UNIQUE_CONSTRAINT_SCHEMA
							and ref.CONSTRAINT_CATALOG=rc.UNIQUE_CONSTRAINT_CATALOG
					where
						ref.TABLE_SCHEMA = t.table_Schema and
						ref.TABLE_NAME = t.table_name
					for xml path ('Collection'), type
				) Collections
			from 
				INFORMATION_SCHEMA.tables t with (nolock)
			order by
				t.table_Schema,
				t.table_name
			for xml path('Table'), type
		) [Tables]
	) a,
	(
		select
		(
			select
				r.routine_Schema '@schema',
				r.routine_name '@name',
				(
					select 
						p.PropertyName '@name',
						p.PropertyValue '@value'
					from 
						db.SprocProperties p with (nolock)
					where
						p.SchemaName = r.specific_schema and
						p.SprocName = r.routine_name
					for xml path ('Property'), type
				) Properties,
				(
					select 
						p.ordinal_position '@position',
						p.parameter_mode '@mode',
						p.parameter_name '@name',
						p.data_type '@sqlType',
						p.character_maximum_length '@maxLen',
						(
							select 
								pp.PropertyName '@name',
								pp.PropertyValue '@value'
							from 
								db.SprocParameterProperties pp with (nolock)
							where
								pp.SchemaName = r.specific_schema and
								pp.SprocName = r.routine_name and
								pp.ParameterName = p.parameter_name
							for xml path ('Property'), type
						) Properties
					from 
						information_Schema.parameters p with (nolock)
					where
						r.specific_name=p.specific_name and
						r.specific_schema=p.specific_schema
					order by 
						p.ordinal_position
					for xml path ('Arg'), type
				) Args
			from 
				INFORMATION_SCHEMA.routines r with (nolock)
			where
				r.routine_type='PROCEDURE'
			order by
				r.routine_Schema,
				r.routine_name
			for xml path('Sproc'), type
		) Sprocs
	) b
	for xml path('SchemaMeta'), type
) x(SchemaMeta)

GO


