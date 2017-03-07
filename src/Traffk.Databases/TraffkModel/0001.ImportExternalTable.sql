create EXTERNAL TABLE db.TraffkGlobalCols
(
	[TABLE_CATALOG] nvarchar(128) null
	,[TABLE_SCHEMA] nvarchar(128) null
	,[TABLE_NAME] nvarchar(128) not null
	,[COLUMN_NAME] nvarchar(128) null
	,[ORDINAL_POSITION] int null
	,[COLUMN_DEFAULT] nvarchar(4000) null
	,[IS_NULLABLE] varchar(3) null
	,[DATA_TYPE] nvarchar(128) null
	,[CHARACTER_MAXIMUM_LENGTH] int null
	,[CHARACTER_OCTET_LENGTH] int null
	,[NUMERIC_PRECISION] tinyint null
	,[NUMERIC_PRECISION_RADIX] smallint null
	,[NUMERIC_SCALE] int null
	,[DATETIME_PRECISION] smallint null
	,[CHARACTER_SET_CATALOG] nvarchar(128) null
	,[CHARACTER_SET_SCHEMA] nvarchar(128) null
	,[CHARACTER_SET_NAME] nvarchar(128) null
	,[COLLATION_CATALOG] nvarchar(128) null
	,[COLLATION_SCHEMA] nvarchar(128) null
	,[COLLATION_NAME] nvarchar(128) null
	,[DOMAIN_CATALOG] nvarchar(128) null
	,[DOMAIN_SCHEMA] nvarchar(128) null
	,[DOMAIN_NAME] nvarchar(128) null
)
WITH (DATA_SOURCE = [DataSrc_RefData])

GO

create PROCEDURE [db].TraffkGlobalTableImport
	@schema sysname,
	@table sysname
AS
BEGIN

	declare @dataSource sysname = 'DataSrc_RefData'

	exec db.PrintNow 'ImportExternalTable {s0}.{s1}.{s2}', @s0=@dataSource, @s1=@schema, @s2=@table

	declare @sql nvarchar(max)

	set @sql = 'CREATE EXTERNAL TABLE '+quotename(@schema)+'.'+quotename(@table)+'('

	declare @colNum int=0
	declare @colDef nvarchar(1024)

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
	from db.TraffkGlobalCols
	where table_Schema=@schema and table_name=@table
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
) with (DATA_SOURCE = '+quotename(@dataSource)+')'

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

GO

exec [db].TraffkGlobalTableImport 'ISO3166', 'Countries'

GO
