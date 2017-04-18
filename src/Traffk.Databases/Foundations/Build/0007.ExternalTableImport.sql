CREATE PROCEDURE [db].ExternalTableImport
	@dataSource sysname,
	@schema sysname,
	@table sysname,
	@columnProviderView nvarchar(200),
	@type sysname=null,
	@distribution sysname=null,
	@srcSchema sysname=null,
	@srcTable sysname=null
AS
BEGIN

	set @srcSchema = coalesce(@srcSchema, @schema)
	set @srcTable = coalesce(@srcTable, @table)
	declare @srcSchemaTable nvarchar(200) = quotename(@srcSchema)+'.'+quotename(@srcTable)
	declare @destSchemaTable nvarchar(200) = quotename(@schema)+'.'+quotename(@table)

	exec db.PrintNow 'ImportExternalTable {s0}.{s1}=>{s2}', @s0=@dataSource, @s1=@srcSchemaTable, @s2=@destSchemaTable

	declare @sql nvarchar(max)


	declare @colNum int=0
	declare @colDef nvarchar(1024)

	create table #cols
	(
		table_Schema nvarchar(128) null,
		table_name sysname not null,
		column_name sysname not null,
		data_type nvarchar(128) null,
		character_maximum_length int null,
		is_nullable varchar(3) null,
		ordinal_position int null
	)

	set @sql = 'select table_Schema, table_name, column_name, data_type, character_maximum_length, is_nullable, ordinal_position from '+@columnProviderView
	exec db.PrintSql @sql

	insert into #cols
	exec(@sql)

	set @sql = 'CREATE EXTERNAL TABLE '+@destSchemaTable+'('

	DECLARE c CURSOR FOR 
	select 
		quotename(column_name)+' '+
		data_type+
		case 
			when character_maximum_length = -1 then '(max)'
			when character_maximum_length is null then ''
			else '('+cast(character_maximum_length as varchar(10))+')'
			end +
		case
			when is_nullable='NO' then ' not null'
			else ' null'
			end
		colDef
	from #cols
	where table_Schema=@srcSchema and table_name=@srcTable
	order by ordinal_position

	open c

	NEXT_ITEM:

	FETCH NEXT FROM c INTO @colDef

	if @@fetch_status = 0
	begin

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

	select @colNum=count(*) 
	from sys.schemas 
	where name=@schema

	if (@colNum=0)
	begin

		declare @s nvarchar(max)
		set @s = 'create schema '+quotename(@schema)
		exec db.PrintSql @s
		exec(@s)

	end

	select @colNum=count(*) 
	from information_Schema.tables 
	where table_schema=@schema and table_name=@table

	if (@colNum=0)
	begin

		exec db.PrintSql @sql
		exec(@sql)

	end


END
