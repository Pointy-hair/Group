create proc db.FindAgentJobs
	@commandTextExpr varchar(max)
as
begin

	select j.job_id, j.name, j.description, j.enabled, js.step_id, js.step_name, js.command
	from
		msdb.dbo.sysjobs j
			inner join
		msdb.dbo.sysjobsteps js
			on js.job_id=j.job_id
	where
		dbo.regexismatch(@commandTextExpr, 1, js.command)=1 or
		dbo.regexismatch(@commandTextExpr, 1, js.step_name)=1 or
		dbo.regexismatch(@commandTextExpr, 1, j.name)=1 or
		dbo.regexismatch(@commandTextExpr, 1, j.description)=1

end

GO

create proc [db].[FindTables]
	@tablePattern nvarchar(128),
	@schemaPattern nvarchar(128)=null
as
begin

	select * 
	from information_schema.tables 
	where
		(@tablePattern is null or dbo.RegexIsMatch(@tablePattern, 1, table_name)=1) and
		(@schemaPattern is null or dbo.RegexIsMatch(@schemaPattern, 1, table_schema)=1)
	order by table_name
	
end

GO

create proc [db].[FindSprocText]
@pattern nvarchar(255)
AS

SELECT ROUTINE_SCHEMA [Schema], ROUTINE_NAME Name, ROUTINE_DEFINITION [Definition]
FROM INFORMATION_SCHEMA.ROUTINES with (nolock)
WHERE 
	@pattern is not null and 
	ROUTINE_DEFINITION is not null and
	dbo.RegexIsMatch(@pattern, 1, ROUTINE_DEFINITION)=1
order by 1,2

GO

CREATE proc [db].[FindSprocs]
	@routineName nvarchar(128),
	@schema nvarchar(128)=null
as
begin

	select * 
	from information_schema.routines 
	where
		(@routineName is null or dbo.RegexIsMatch(@routineName, 1, routine_name)=1) and 
		(@schema is null or dbo.RegexIsMatch(@schema, 1, routine_schema)=1) 
	order by routine_name

end

GO

CREATE proc [db].[FindCols]
	@tableName nvarchar(128),
	@columnName nvarchar(128)=null,
	@tableSchema nvarchar(128)='^dbo$'
as
begin

	select * 
	from information_schema.columns 
	where
		(@tableName is null or dbo.RegexIsMatch(@tableName, 1, table_name)=1) and
		(@columnName is null or dbo.RegexIsMatch(@columnName, 1, column_Name)=1) and
		(@tableSchema is null or dbo.RegexIsMatch(@tableSchema, 1, table_schema)=1)
	order by table_name, column_name 
	
end

GO
