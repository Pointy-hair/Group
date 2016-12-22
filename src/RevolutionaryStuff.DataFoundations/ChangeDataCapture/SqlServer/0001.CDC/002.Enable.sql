--http://www.sqlshack.com/how-to-track-changes-in-sql-server/

GO

EXEC sys.sp_cdc_enable_db  

GO

CREATE proc [db].[ChangeDataCaptureEnable]
	@schema sysname,
	@tableExpr nvarchar(100)='.'
as
begin

	declare @table sysname

	DECLARE x CURSOR FOR 

	select distinct t.name
	--select t.*
	from 
		sys.schemas s
			inner join
		sys.tables t
			on s.schema_id=t.schema_id
			and t.is_ms_shipped=0
			and t.is_tracked_by_cdc=0
			left join
		db.TableProperties tp
			on tp.schemaname=s.name
			and tp.tablename=t.name
			and tp.PropertyName='ChangeDataCapture'
			and tp.PropertyValue='0'
	where
		s.name=@schema and
		(tp.PropertyValue is null or tp.PropertyValue<>0) and
		dbo.RegexIsMatch(@tableExpr, 1, t.name)=1

	OPEN x

NEXT_ITEM:
	FETCH NEXT FROM x INTO @table

	IF @@FETCH_STATUS = 0
	BEGIN

		EXEC sys.sp_cdc_enable_table   
			@source_schema = @schema,
			@source_name   = @table,
			@role_name     = NULL,
			@supports_net_changes = 1;

		exec db.PrintNow 'Enabled Change Data Tracking on {s0}.{s1}', @s0=@schema, @s1=@table

		goto NEXT_ITEM;

	END

	CLOSE x

	deallocate x

end

GO
