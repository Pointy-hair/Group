create role i_see_dead_people
create role portal_role
create role tenant_all_reader
create role tenant_all_writer
grant execute to tenant_all_writer
exec sp_addrolemember 'db_datareader', 'tenant_all_reader'
exec sp_addrolemember 'db_datawriter', 'tenant_all_writer'
exec sp_addrolemember 'tenant_all_reader', 'tenant_all_writer'
GO
CREATE USER _TraffkPortalApp
	FOR LOGIN _TraffkPortalApp
	WITH DEFAULT_SCHEMA = dbo
GO
create role _TraffkPortalAppRole
GO
EXEC sp_addrolemember N'_TraffkPortalAppRole', N'_TraffkPortalApp'
EXEC sp_addrolemember N'tenant_all_writer', N'_TraffkPortalAppRole'
EXEC sp_addrolemember N'tenant_all_reader', N'_TraffkPortalAppRole'
GO
CREATE USER _TraffkTenantShardsUser
	FOR LOGIN _TraffkTenantShardsUser
	WITH DEFAULT_SCHEMA = dbo
GO
EXEC sp_addrolemember N'tenant_all_reader', N'_TraffkTenantShardsUser'
GO

CREATE USER _EtlApp
	FOR LOGIN _EtlApp
	WITH DEFAULT_SCHEMA = dbo
GO
create role _EtlAppRole
GO
EXEC sp_addrolemember N'_EtlAppRole', N'_EtlApp'
EXEC sp_addrolemember N'tenant_all_writer', N'_EtlAppRole'
EXEC sp_addrolemember N'tenant_all_reader', N'_EtlAppRole'
GO

CREATE USER _TraffkBackgroundJobServer
	FOR LOGIN _TraffkBackgroundJobServer
	WITH DEFAULT_SCHEMA = dbo
GO
create role _TraffkBackgroundJobServerRole
GO
EXEC sp_addrolemember N'_TraffkBackgroundJobServerRole', N'_TraffkBackgroundJobServer'
EXEC sp_addrolemember N'tenant_all_writer', N'_TraffkBackgroundJobServer'
EXEC sp_addrolemember N'tenant_all_reader', N'_TraffkBackgroundJobServer'


deny all on __ShardManagement.ShardMappingsLocal to _TraffkPortalAppRole
deny all on __ShardManagement.ShardMapsLocal to _TraffkPortalAppRole
deny all on __ShardManagement.ShardsLocal to _TraffkPortalAppRole
grant all on __ShardManagement.ShardMappingsLocal to _TraffkBackgroundJobServerRole
grant all on __ShardManagement.ShardMapsLocal to _TraffkBackgroundJobServerRole
grant all on __ShardManagement.ShardsLocal to _TraffkBackgroundJobServerRole



GO

create schema Security

GO

create FUNCTION security.RowStatusPredicate(@rowStatus char(1))
	RETURNS TABLE
	WITH SCHEMABINDING
AS
	RETURN SELECT 1 AS accessResult
	where
		@rowStatus=1 or
		is_rolemember(N'i_see_dead_people')=1

GO


create proc security.SecurityPoliciesCreate
	@full bit = 0
AS
begin

	set nocount on

	set @full = coalesce(@full,0)

	DECLARE c CURSOR FOR 

	with
	Frags (SchemaName, TableName, Frag) as
	(
		select 
			t.table_schema SchemaName, 
			t.table_name TableName,
			--TenantRule.TenantIdColumnName,
			--StatusRule.StatusColumnName,
			case
				--when TenantIdColumnName is not null and StatusColumnName is not null
				--then 'security.TenantAndStatusPredicate('+TenantIdColumnName+', '+StatusColumnName+')'
				--when TenantIdColumnName is not null
				--then 'security.TenantPredicate('+TenantIdColumnName+')'
				when StatusColumnName is not null
				then 'security.RowStatusPredicate('+StatusColumnName+')'
				else null
				end
			Frag
		from
			information_schema.tables t
				outer apply
			(
/*
				select 'TenantId' TenantIdColumnName
				from db.ColumnProperties c
				where 
					PropertyName='Implements' and 
					cast(PropertyValue as varchar(max))like '%ITraffkTenanted%' and
					c.SchemaName=t.table_schema and
					c.TableName=t.table_name	
*/
				select 'TenantId' TenantIdColumnName
				from information_schema.columns c
				where 
					c.column_name='TenantId' and
					c.table_schema=t.table_schema and
					c.table_name=t.table_name and
					c.table_schema in ('dbo', 'health', 'deerwalk')
			) TenantRule
				outer apply
			(
				select ColumnName StatusColumnName
				from db.ColumnProperties c
				where 
					PropertyName='ImplementsRowStatusSemantics' and 
					PropertyValue='1' and
					c.SchemaName=t.table_schema and
					c.TableName=t.table_name	
			) StatusRule
		where
			t.table_type='BASE TABLE'
	),
	NonNullFrags(SchemaName, TableName, SchemaTable, Frag) as
	(
		select SchemaName, TableName, SchemaName+'.'+TableName, Frag from frags where frag is not null
	)
	select 
		'create security policy security.'+
		case when SchemaName='dbo' then '' else SchemaName end+
		TableName+
		' '+
		string_agg(Pred, ','),
		'security.'+case when SchemaName='dbo' then '' else SchemaName end+ TableName SecurityPolicyName,
		sp.object_id
	from 
	(
		select 'add filter predicate '+frag+' on '+SchemaTable Pred, SchemaTable, SchemaName, TableName
		from NonNullFrags
		/*
		union all
		select 'add block predicate '+frag+' on '+SchemaTable Pred, SchemaTable, SchemaName, TableName
		from NonNullFrags
		*/
	) z
		left join
	[sys].[security_policies] sp
		on sp.name=(case when SchemaName='dbo' then '' else SchemaName end+TableName)
	group by SchemaTable, SchemaName, TableName, sp.object_id
	order by 1

	OPEN c

	declare @sql nvarchar(max)
	declare @securityPolicyName sysname
	declare @objectId int

	NEXT_ITEM:

	FETCH NEXT FROM c INTO @sql, @securityPolicyName, @objectId

	if @@fetch_status = 0
	begin

		if (@objectId is not null)
		begin

			if (@full=0) goto NEXT_ITEM

			set @sql = 'drop security policy '+@securityPolicyName+'; '+@sql

		end

		exec db.PrintSql @sql, 0
--		exec (@sql)

		goto NEXT_ITEM

	end

	close c

	deallocate c

end

GO

create proc db.RowStatusTriggersCreate
	@full bit = 0
AS
begin

	set nocount on

	set @full = coalesce(@full,0)

	declare @sql nvarchar(max)
	declare @schemaName sysname
	declare @tableName sysname
	declare @columnName sysname
	declare @primaryKeyColumnName sysname
	declare @triggerName sysname
	declare @objectId int

	DECLARE c CURSOR FOR 

	select c.SchemaName, c.TableName, c.ColumnName, c.TriggerName, PrimaryKeyColumnName, t.TriggerId
	from 
		(
			select SchemaName, TableName, ColumnName, TableName+'InsteadOfDelete' TriggerName
			from db.ColumnProperties
			where 
				PropertyName='ImplementsRowStatusSemantics' and 
				PropertyValue='1'	
		) c
			inner join
		(
			select tc.table_schema SchemaName, tc.table_name TableName, ccu.Column_Name PrimaryKeyColumnName
			from 
				INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc with (nolock)
					inner join
				INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu with (nolock)
					on tc.constraint_name=ccu.constraint_name 
					and tc.table_schema=ccu.table_schema 
					and tc.table_name=ccu.table_name
			where
				tc.[CONSTRAINT_TYPE]='PRIMARY KEY'
		) pk
			on pk.SchemaName=c.SchemaName
			and pk.TableName=c.TableName
			left join
		(
			select s.Name SchemaName, ta.Name TableName, tr.Name TriggerName, tr.object_id TriggerId
			from 
				sys.triggers tr
					inner join
				sys.tables ta
					on ta.object_id=tr.parent_id
					inner join
				sys.schemas s
					on s.schema_id=ta.schema_id	
		) t
			on t.schemaName=c.schemaName
			and t.tableName=c.tableName
			and t.TriggerName=c.triggerName
	order by 1,2,3

	OPEN c

	NEXT_ITEM:

	FETCH NEXT FROM c INTO @schemaName, @tableName, @columnName, @triggerName, @primaryKeyColumnName, @objectId

	if @@fetch_status = 0
	begin

		set @sql ='
'+case when @objectId is null then 'CREATE' else 'ALTER' end+' TRIGGER ['+@schemaName+'].['+@triggerName+'] ON ['+@schemaName+'].['+@tableName+']  INSTEAD OF DELETE  
AS  
BEGIN     
	SET NOCOUNT ON;     

	update u   
	set
		'+@columnName+'=''d''   
	from
		'+@schemaName+'.'+@tableName+' u     
			inner join
		deleted d
			on d.'+@primaryKeyColumnName+'=u.'+@primaryKeyColumnName+'   
	where    
		u.'+@columnName+' not in (''p'', ''d'')     
		
END		
		'

		if (@full=1 or @objectId is null)
		begin
			exec db.PrintSql @sql, 0
			exec (@sql)
		end

		goto NEXT_ITEM

	end

	close c

	deallocate c

end

GO

/*
exec db.RowStatusTriggersCreate
exec security.SecurityPoliciesCreate
*/

GO

