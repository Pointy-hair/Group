CREATE PROCEDURE [db].[ExternalTableImport]
	@dataSource sysname,
	@schema sysname,
	@table sysname,
	@schemaMetaXml nvarchar(max),
	@type sysname=null,
	@distribution sysname=null,
	@srcSchema sysname=null,
	@srcTable sysname=null
AS
BEGIN

	declare @sm xml
	set @sm = convert(xml, @schemaMetaXml)
	set @srcSchema = coalesce(@srcSchema, @schema)
	set @srcTable = coalesce(@srcTable, @table)
	declare @srcSchemaTable nvarchar(200) = quotename(@srcSchema)+'.'+quotename(@srcTable)
	declare @destSchemaTable nvarchar(200) = quotename(@schema)+'.'+quotename(@table)

	exec db.PrintNow 'ImportExternalTable {s0}.{s1}=>{s2}', @s0=@dataSource, @s1=@srcSchemaTable, @s2=@destSchemaTable

	;with
	pk(TableSchema, TableName, ColumnName) as
	(
		select ccu.Table_schema, ccu.Table_name, ccu.Column_name
		from 
			INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc with (nolock)
				inner join
			INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu with (nolock)
				on tc.constraint_name=ccu.constraint_name 
				and tc.table_schema=ccu.table_schema 
				and tc.table_name=ccu.table_name
		where
			tc.[CONSTRAINT_TYPE]='PRIMARY KEY' 	
	)
	select 
		t.t.value('@schema', 'sysname') TableSchema,
		t.t.value('@name', 'sysname') TableName,
		t.t.value('@tableType', 'varchar(50)') TableType,
		c.c.value('@name', 'sysname') ColumnName,
		c.c.value('@position', 'int') Position,
		c.c.value('@isPrimaryKey', 'bit') IsPrimaryKey,
		c.c.value('@isIdentity', 'bit') IsIdentity,
		c.c.value('@isNullable', 'bit') IsNullable,
		c.c.value('@sqlType', 'sysname') SqlType,
		c.c.value('@maxLen', 'int') MaxLen,
		c.c.value('@refSchema', 'sysname') RefSchema,
		c.c.value('@refTable', 'sysname') RefTable,
		coalesce(pk.ColumnName, k.ColumnName) RefColumn
	into #cols
	from 
		@sm.nodes('/SchemaMeta/Tables/Table') t(t)
			cross apply
		t.t.nodes('Columns/Column') c(c)
			left join
		pk
			on pk.tableschema=c.c.value('@refSchema', 'sysname')
			and pk.tablename=c.c.value('@refTable', 'sysname')
			left join
		[db].[ColumnProperties] k
			on k.propertyname='CustomAttribute'
			and k.propertyvalue='Key'
			and k.SchemaName=c.c.value('@refSchema', 'sysname')
			and k.TableName=c.c.value('@refTable', 'sysname')
	where
		t.t.value('@schema', 'sysname')=@srcSchema and
		t.t.value('@name', 'sysname')=@srcTable
	--order by 1,2,3,5

	exec db.PrintNow 'New table will have {n0} cols', @@rowcount

	--select * from #cols

	declare @meta nvarchar(max)=''
	declare @sql nvarchar(max)
	declare @colNum int=0
	declare @colDef nvarchar(1024)
	declare @isPrimaryKey bit
	declare @columnName sysname
	declare @refSchema sysname
	declare @refTable sysname
	declare @refColumn sysname

	set @sql = 'CREATE EXTERNAL TABLE '+@destSchemaTable+'('

	DECLARE c CURSOR FOR 
	select 
		ColumnName,
		quotename(ColumnName)+' '+
		SqlType+
		case 
			when MaxLen = -1 then '(max)'
			when MaxLen is null then ''
			else '('+cast(MaxLen as varchar(10))+')'
			end +
		case
			when IsNullable=0 then ' not null'
			else ' null'
			end
		colDef,
		IsPrimaryKey,
		RefSchema,
		RefTable,
		RefColumn
	from #cols
	order by Position

	open c

	NEXT_ITEM:

	FETCH NEXT FROM c INTO @columnName, @colDef, @isPrimaryKey, @refSchema, @refTable, @refColumn

	if @@fetch_status = 0
	begin

		if (@isPrimaryKey=1)
		begin
			set @meta = @meta + '

exec db.ColumnPropertySet '''+@table+''', '''+@columnName+''', ''Key'', @propertyName=''CustomAttribute'', @tableSchema='''+@schema+''''
		end
		if (@refColumn is not null)
		begin
			set @meta = @meta + '

exec db.ColumnPropertySet '''+@table+''', '''+@columnName+''', '''+@refSchema+'.'+@refTable+'('+@refColumn+')'', @propertyName=''LinksTo'', @tableSchema='''+@schema+''''
		end

		set @sql = @sql + '
'
		if (@colNum>0)
		begin
			set @sql = @sql + ', '
		end

		set @colNum = @colNum + 1

		set @sql = @sql + @colDef
		
		goto NEXT_ITEM

	end

	close c
	
	deallocate c	

	set @sql = @sql + '
) with (
	DATA_SOURCE = '+quotename(@dataSource)+',
	SCHEMA_NAME = '''+@srcSchema+''',
	OBJECT_NAME = '''+@srcTable+''''+
	(case when @type is null then '' else ', TYPE='+@type end)+
	(case when @distribution is null then '' else ', DISTRIBUTION='+@distribution end)+'
)'

	declare @propertyName nvarchar(255)
	declare @propertyVal nvarchar(max)

	declare c2 cursor for
	select 
		c.c.value('@name', 'sysname') ColumnName,
		p.p.value('@name', 'nvarchar(200)') PropertyName,
		p.p.value('@value', 'nvarchar(max)') PropertyValue
	from 
		@sm.nodes('/SchemaMeta/Tables/Table') t(t)
			cross apply
		t.t.nodes('Columns/Column') c(c)
			cross apply
		c.c.nodes('Properties/Property') p(p)
	where
		t.t.value('@schema', 'sysname')=@srcSchema and
		t.t.value('@name', 'sysname')=@srcTable

	open c2

	NEXT_PROP:

	FETCH NEXT FROM c2 INTO @columnName, @propertyName, @propertyVal

	if @@fetch_status = 0
	begin

		set @meta = @meta + '

exec db.ColumnPropertySet '''+@table+''', '''+@columnName+''', '''+replace(@propertyVal,'''','''''')+''', @propertyName='''+@propertyName+''', @tableSchema='''+@schema+''''
		

		goto NEXT_PROP

	end

	close c2

	deallocate c2

	declare c3 cursor for
	select 
		p.p.value('@name', 'nvarchar(200)') PropertyName,
		p.p.value('@value', 'nvarchar(max)') PropertyValue
	from 
		@sm.nodes('/SchemaMeta/Tables/Table') t(t)
			cross apply
		t.t.nodes('Properties/Property') p(p)
	where
		t.t.value('@schema', 'sysname')=@srcSchema and
		t.t.value('@name', 'sysname')=@srcTable

	open c3

	NEXT_TPROP:

	FETCH NEXT FROM c3 INTO @propertyName, @propertyVal

	if @@fetch_status = 0
	begin

		set @meta = @meta + '

exec db.TablePropertySet '''+@table+''', '''+replace(@propertyVal,'''','''''')+''', @propertyName='''+@propertyName+''', @tableSchema='''+@schema+''''
		

		goto NEXT_TPROP

	end

	close c3

	deallocate c3

	select @colNum=count(*) 
	from sys.schemas 
	where name=@schema

	if (@colNum=0)
	begin

		set @sql = 'create schema '+quotename(@schema)+'
GO
'

	end

	select @colNum=count(*) 
	from information_Schema.tables 
	where table_schema=@schema and table_name=@table

	if (@colNum=0)
	begin

		set @sql = @sql + @meta
		exec db.PrintSql @sql, 1
		exec(@sql)

	end


END
