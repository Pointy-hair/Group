CREATE TYPE [dbo].[BatchDeleteType] AS TABLE(
	SchemaName sysname not NULL,
	TableName sysname not NULL,
	RowId bigint not NULL
)

GO

CREATE proc [dbo].[BatchDelete]
	@tenantId int,
	@items dbo.BatchDeleteType readonly,
	@deleteAuditId bigint,
	@batchSize int=null
AS
BEGIN

	set nocount on
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 

	declare @schemaTable varchar(255)
	declare @primaryKeyColumnName sysname
	declare @ids nvarchar(max)
	declare @sql nvarchar(max)
	set @batchSize = coalesce(@batchSize, 1000)

	select distinct *
	into #items
	from @items

	if (@@rowcount=0) return;

	alter table #items add Id int not null identity

	DECLARE x CURSOR FOR 
	select b.SchemaName+'.'+b.TableName SchemaTable, c.COLUMN_NAME PrimaryKeyColumnName, b.ids
	from
		(
			select schemaname, tablename, Id/@batchSize batchNum, dbo.CsvLongs2Table(RowId) ids
			from #items
			group by schemaname, tablename, Id/@batchSize
		) b
			left join
		INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc with (nolock)
			on CONSTRAINT_TYPE='PRIMARY KEY'
			and tc.table_Schema=b.SchemaName
			and tc.table_name=b.TableName
			left join
		INFORMATION_SCHEMA.COLUMNS c with (nolock)
			on c.TABLE_SCHEMA=tc.TABLE_SCHEMA
			and c.TABLE_NAME=tc.TABLE_NAME
			and c.DATA_TYPE in ('bigint', 'smallint', 'int')

	OPEN x

NEXT_REC:
	FETCH NEXT FROM x INTO @schemaTable, @primaryKeyColumnName, @ids

	IF @@FETCH_STATUS = 0
	BEGIN

		set @sql = 'update '+@schemaTable+' set deletedauditid='+@deleteAuditId+' where '+@primaryKeyColumnName+' in ('+@ids+')'
		exec db.printnow @sql
		exec(@sql)

		goto NEXT_REC
	END

	CLOSE x
	DEALLOCATE x

END
