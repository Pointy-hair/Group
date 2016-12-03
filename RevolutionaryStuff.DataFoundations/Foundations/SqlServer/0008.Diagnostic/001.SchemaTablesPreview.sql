create proc [db].[SchemaTablesPreview]
	@schema sysname='dbo',
	@tableNameExpr varchar(500)=null,
	@maxRows int=null,
	@countsOnly bit=null
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	set nocount on;

	set @maxRows = coalesce(@maxRows, 100)
	set @countsOnly = coalesce(@countsOnly,0)
	set @tableNameExpr = coalesce(@tableNameExpr,'.')

	create table #counts
	(
		TableName sysname not null,
		Cnt int not null
	)

	declare @tableName sysname
	declare @cnt int
	declare @sql nvarchar(max)

	DECLARE x CURSOR FOR 
	select quotename (table_name)
	from
		information_schema.tables 
	where 
		table_type='base table' and 
		table_schema=@schema and 
		dbo.RegexIsMatch(@tableNameExpr, 1, table_name)=1
	order by
		table_name

	OPEN x

NEXT_ITEM:
	FETCH NEXT FROM x INTO @tableName

	IF @@FETCH_STATUS = 0
	BEGIN
		
		set @sql = 'set @cnt=(select count(*) from '+@schema+'.'+@tableName+')'
		exec sp_executesql @sql, N'@cnt int output', @cnt output

		insert into #counts
		(TableName, Cnt)
		values
		(@tableName, @cnt)

		goto NEXT_ITEM;

	END

	close x


	select * 
	from #counts
	order by tablename


	if (@countsOnly=0)
	begin


		OPEN x

NEXT_ITEM2:
		FETCH NEXT FROM x INTO @tableName

		IF @@FETCH_STATUS = 0
		BEGIN
		
			select @schema [Schema], @tableName [Table], @cnt [Cnt]

			set @sql='select top('+cast((case when @cnt>(@maxRows+50) then @maxRows else @cnt end) as varchar(10))+') * from '+@schema+'.'+@tableName+' order by 1'
			exec sp_executesql @sql

			goto NEXT_ITEM2;

		END

		close x

	end

	deallocate x

end