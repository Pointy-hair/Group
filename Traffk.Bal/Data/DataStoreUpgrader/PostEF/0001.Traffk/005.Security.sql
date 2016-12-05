/*
In master
create login traffk_tenant_all_reporting with password='jfkldsaj#$R%3fvdjoiog43j-9g9jvJJoawi'
create login traffk_tenant_all_portal with password='vfiodVOPghj3irjgv0324fickCJEIWabhv'
create login traffk_tenant_1_reporting with password='jfkldsaj#$R%3fvdjoiog43j-9g9jvJJoawi'
create login traffk_tenant_1_portal with password='vfiodVOPghj3irjgv0324fickCJEIWabhv'
*/

create role i_see_dead_people
create role portal_role
grant execute on dbo.GetFieldCounts to portal_role
create role tenant_all_reader
create role tenant_all_writer
grant execute to tenant_all_writer
create user tenant_all_portal for login traffk_tenant_all_portal with default_schema=[dbo]
create user tenant_all_reporting for login traffk_tenant_all_reporting with default_schema=[dbo]
exec sp_addrolemember 'db_datareader', 'tenant_all_reader'
exec sp_addrolemember 'db_datawriter', 'tenant_all_writer'
exec sp_addrolemember 'tenant_all_reader', 'tenant_all_writer'
exec sp_addrolemember 'tenant_all_writer', 'tenant_all_portal'
exec sp_addrolemember 'tenant_all_reader', 'tenant_all_reporting'

GO

create FUNCTION security.TenantPredicate(@tenantId int)
	RETURNS TABLE
	WITH SCHEMABINDING
AS
	RETURN SELECT 1 AS accessResult
	where
		1=is_rolemember('tenant_'+cast(@tenantId as varchar(10))+'_reader') or
		1=is_rolemember('tenant_all_reader') or
		1=is_rolemember('db_owner') or
		current_user='dbo'

GO

create FUNCTION security.RowStatusPredicate(@rowStatus char(1))
	RETURNS TABLE
	WITH SCHEMABINDING
AS
	RETURN SELECT 1 AS accessResult
	where
		@rowStatus not in ('d', 'p') or
		1=is_rolemember('i_see_dead_people') or
		1=is_rolemember('db_owner') or
		current_user='dbo'

GO

create FUNCTION security.TenantAndStatusPredicate(@tenantId int, @rowStatus char(1))
	RETURNS TABLE
	WITH SCHEMABINDING
AS
	RETURN SELECT 1 AS accessResult
	where
		1=is_rolemember('db_owner') or
		current_user='dbo' or
		(
			(
				@rowStatus not in ('d', 'p') or
				1=is_rolemember('i_see_dead_people')			
			) and
			(
				1=is_rolemember('tenant_'+cast(@tenantId as varchar(10))+'_reader') or
				1=is_rolemember('tenant_all_reader')
			)		
		)

GO




/*

;with
Frags (SchemaName, TableName, Frag) as
(
	select 
		t.table_schema SchemaName, 
		t.table_name TableName,
		--TenantRule.TenantIdColumnName,
		--StatusRule.StatusColumnName,
		case
			when TenantIdColumnName is not null and StatusColumnName is not null
			then 'security.TenantAndStatusPredicate('+TenantIdColumnName+', '+StatusColumnName+')'
			when TenantIdColumnName is not null
			then 'security.TenantPredicate('+TenantIdColumnName+')'
			when StatusColumnName is not null
			then 'security.RowStatusPredicate('+StatusColumnName+')'
			else null
			end
		Frag
	from
		information_schema.tables t
			outer apply
		(
			select 'TenantId' TenantIdColumnName
			from information_schema.columns c
			where 
				c.column_name='TenantId' and
				c.table_schema=t.table_schema and
				c.table_name=t.table_name
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
),
NonNullFrags(SchemaTable, Frag) as
(
	select SchemaName+'.'+TableName, Frag from frags where frag is not null
)
select Pred +',' Pred
from 
(
	select 'add filter predicate '+frag+' on '+SchemaTable Pred, SchemaTable
	from NonNullFrags
	union all
	select 'add block predicate '+frag+' on '+SchemaTable Pred, SchemaTable
	from NonNullFrags
) z
order by SchemaTable, Pred

*/

--drop SECURITY POLICY Security.dataAccessPolicy

create SECURITY POLICY Security.dataAccessPolicy
	add block predicate security.TenantPredicate(TenantId) on dbo.Applications,
	add filter predicate security.TenantPredicate(TenantId) on dbo.Applications,
	add block predicate security.TenantPredicate(TenantId) on dbo.AspNetRoles,
	add filter predicate security.TenantPredicate(TenantId) on dbo.AspNetRoles,
	add block predicate security.TenantAndStatusPredicate(TenantId, UserRowStatus) on dbo.AspNetUsers,
	add filter predicate security.TenantAndStatusPredicate(TenantId, UserRowStatus) on dbo.AspNetUsers,
	add block predicate security.TenantAndStatusPredicate(TenantId, CommunicationBlastLinkRowStatus) on dbo.CommunicationBlastLinks,
	add filter predicate security.TenantAndStatusPredicate(TenantId, CommunicationBlastLinkRowStatus) on dbo.CommunicationBlastLinks,
	add block predicate security.TenantAndStatusPredicate(TenantId, CommunicationBlastRowStatus) on dbo.CommunicationBlasts,
	add filter predicate security.TenantAndStatusPredicate(TenantId, CommunicationBlastRowStatus) on dbo.CommunicationBlasts,
	add block predicate security.TenantPredicate(TenantId) on dbo.Contacts,
	add filter predicate security.TenantPredicate(TenantId) on dbo.Contacts,
	add block predicate security.TenantPredicate(TenantId) on dbo.DateDimensions,
	add filter predicate security.TenantPredicate(TenantId) on dbo.DateDimensions,
	add block predicate security.TenantAndStatusPredicate(TenantId, JobRowStatus) on dbo.Jobs,
	add filter predicate security.TenantAndStatusPredicate(TenantId, JobRowStatus) on dbo.Jobs,
	add block predicate security.TenantPredicate(TenantId) on dbo.MessageTemplates,
	add filter predicate security.TenantPredicate(TenantId) on dbo.MessageTemplates,
	add block predicate security.TenantPredicate(TenantId) on dbo.SystemCommunications,
	add filter predicate security.TenantPredicate(TenantId) on dbo.SystemCommunications,
	add block predicate security.TenantPredicate(TenantId) on dbo.Templates,
	add filter predicate security.TenantPredicate(TenantId) on dbo.Templates,
	add block predicate security.TenantAndStatusPredicate(TenantId, TenantRowStatus) on dbo.Tenants,
	add filter predicate security.TenantAndStatusPredicate(TenantId, TenantRowStatus) on dbo.Tenants,
	add block predicate security.TenantPredicate(TenantId) on dbo.UserActivities,
	add filter predicate security.TenantPredicate(TenantId) on dbo.UserActivities,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.CareAlerts,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.CareAlerts,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.Demographics,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.Demographics,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.Eligibility,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.Eligibility,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.HighCostDiagnosis,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.HighCostDiagnosis,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.HistoricalScores,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.HistoricalScores,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.MedicalClaims,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.MedicalClaims,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.MemberPCP,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.MemberPCP,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.Participation,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.Participation,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.Pharmacy,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.Pharmacy,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.QualityMetrics,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.QualityMetrics,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.Scores,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.Scores,
	add block predicate security.TenantPredicate(TenantId) on deerwalk.Visits,
	add filter predicate security.TenantPredicate(TenantId) on deerwalk.Visits

GO

alter proc security.TenantSecurityCreate
	@tenantId int
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	exec db.PrintNow 
		'TenantSecurityCreate tenantId={n0}',
		@tenantId;

	exec db.AssertNotNull @tenantId

	declare @n int
	select @n=count(*) from tenants where tenantId=@tenantId

	exec db.AssertEquals 1, @n, 'That tenant was not found'

	declare @sql nvarchar(max)

	declare @readerRole sysname
	declare @writerRole sysname

	set @readerRole = 'tenant_'+cast(@tenantId as varchar(10))+'_reader'
	set @writerRole = 'tenant_'+cast(@tenantId as varchar(10))+'_writer'

	set @sql = 'create role '+@readerRole
	EXECUTE sp_executesql @sql

	set @sql = 'create role '+@writerRole
	EXECUTE sp_executesql @sql

	exec sp_addrolemember @readerRole, @writerRole

	declare @u sysname

	declare @db sysname
	select @db = ORIGINAL_DB_NAME()

	set @u = 'tenant_'+cast(@tenantId as varchar(10))+'_portal'
	set @sql = 'CREATE USER '+@u+' for login [traffk_'+@u+'] WITH DEFAULT_SCHEMA=[dbo]'
	EXECUTE sp_executesql @sql
	exec sp_addrolemember @writerRole, @u
	exec sp_addrolemember 'db_datawriter', @u
	exec sp_addrolemember 'db_datareader', @u
	exec sp_addrolemember 'portal_role', @u

	set @u = 'tenant_'+cast(@tenantId as varchar(10))+'_reporting'
	set @sql = 'CREATE USER '+@u+' for login [traffk_'+@u+'] WITH DEFAULT_SCHEMA=[dbo]'
	EXECUTE sp_executesql @sql
	exec sp_addrolemember @readerRole, @u
	exec sp_addrolemember 'db_datareader', @u

end

GO


/*
select 
	a.*,
	'CREATE TRIGGER '+SchemaName+'.'+TableName+'InsteadOfDelete
       ON '+SchemaName+'.'+TableName+'
INSTEAD OF DELETE
AS
BEGIN

	SET NOCOUNT ON;

	update u
	set
		'+RowStatusColumnName+'=''d''
	from
		'+SchemaName+'.'+TableName+' u
			inner join
		deleted d
			on d.'+PrimaryKeyColumnName+'=u.'+PrimaryKeyColumnName+'
	where
		u.'+RowStatusColumnName+' not in (''p'', ''d'')
 
END' InsteadOfDeleteTriggerDdl
from
(	 
	select 
		c.SchemaName, c.TableName, 
		ccu.column_name PrimaryKeyColumnName, 
		c.ColumnName RowStatusColumnName
	from 
		db.ColumnProperties c with (nolock)
			inner join
		INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc with (nolock)
			on tc.table_schema=c.schemaname
			and tc.table_name=c.TableName
			inner join
		INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu with (nolock)
			on tc.constraint_name=ccu.constraint_name 
			and tc.table_schema=ccu.table_schema 
			and tc.table_name=ccu.table_name
	where
		c.PropertyName='ImplementsRowStatusSemantics' and 
		c.PropertyValue='1' and 
		tc.[CONSTRAINT_TYPE]='PRIMARY KEY' 
) a
order by 1,2

*/


