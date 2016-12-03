/*
In master
create login traffk_tenant_all_reporting with password='jfkldsaj#$R%3fvdjoiog43j-9g9jvJJoawi'
create login traffk_tenant_all_portal with password='vfiodVOPghj3irjgv0324fickCJEIWabhv'
create login traffk_tenant_1_reporting with password='jfkldsaj#$R%3fvdjoiog43j-9g9jvJJoawi'
create login traffk_tenant_1_portal with password='vfiodVOPghj3irjgv0324fickCJEIWabhv'
*/


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

/*
select frag
from
(
	select 'ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON '+table_schema+'.'+table_name+',' frag, table_schema, table_name
	from information_schema.columns where column_name='tenantid' 
	union all
	select 'ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON '+table_schema+'.'+table_name+',' frag, table_schema, table_name
	from information_schema.columns where column_name='tenantid' 
) a
order by table_schema, table_name
*/

create SECURITY POLICY Security.tenantAccessPolicy
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.Applications,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.Applications,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.AspNetRoles,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.AspNetRoles,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.AspNetUsers,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.AspNetUsers,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.CommunicationBlasts,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.CommunicationBlasts,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.Contacts,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.Contacts,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.DateDimensions,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.DateDimensions,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.Jobs,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.Jobs,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.MessageTemplates,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.MessageTemplates,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.SystemCommunications,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.SystemCommunications,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.Templates,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.Templates,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.Tenants,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.Tenants,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON dbo.UserActivities,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON dbo.UserActivities,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.CareAlerts,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.CareAlerts,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Demographics,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Demographics,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Eligibility,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Eligibility,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.HighCostDiagnosis,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.HighCostDiagnosis,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.HistoricalScores,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.HistoricalScores,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.MedicalClaims,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.MedicalClaims,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.MemberPCP,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.MemberPCP,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Participation,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Participation,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Pharmacy,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Pharmacy,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.QualityMetrics,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.QualityMetrics,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Scores,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Scores,
	ADD BLOCK PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Visits,
	ADD FILTER PREDICATE Security.TenantPredicate(TenantId) ON deerwalk.Visits

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
