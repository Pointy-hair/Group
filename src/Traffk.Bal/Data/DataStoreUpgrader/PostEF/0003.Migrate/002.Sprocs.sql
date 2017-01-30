create schema migrate

GO

create view migrate.MemberMap
AS
select distinct m.TenantId, m.MemberId, m.PersonContactId, m.MemberNumber, m.CarrierContactId, c.CarrierId
from
	health.members m (nolock)
		inner join
	dbo.contacts c (nolock)
		on m.carriercontactid=c.contactid

GO

create proc migrate.DateDimensionLink
	@sourceSchema sysname,
	@sourceTable sysname,
	@tenantId int,
	@batchSize int=null
AS
begin

	set nocount on
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	set @batchSize = coalesce(@batchSize, 5000)
	exec db.PrintNow 
		'DateDimensionLink tenantId={n0} batchSize={n1} for table {s0}.{s1}', 
		@tenantId, @batchSize, @s0=@sourceSchema, @s1=@sourceTable

	exec db.AssertNotNull @tenantId

	declare @idField sysname

	select @idField=column_name 
	from information_Schema.columns c 
	where 
		table_schema=@sourceSchema and
		table_name=@sourceTable and
		ordinal_position=1

	select 
		cast(c.column_name as sysname) DimField, 
		cast(ep.value as sysname) BaseField
	into #map
	from
		information_Schema.columns c
			cross apply
		sys.fn_listextendedproperty('BaseField', N'SCHEMA', @sourceSchema, N'TABLE', @sourceTable, N'COLUMN', c.column_name) ep
	where
		table_schema=@sourceSchema and
		table_name=@sourceTable and
		data_type='int'

	alter table #map add Id int not null identity

	select * from #map

	create table #d
	(
		Id int not null identity,
		PkId int not null unique,
		ddim1 int null,
		ddim2 int null,
		ddim3 int null,
		ddim4 int null,
		ddim5 int null,
		ddim6 int null,
		ddim7 int null,
		ddim8 int null,
		ddim9 int null,
		ddim10 int null,
		ddim11 int null,
		ddim12 int null,
		ddim13 int null,
		ddim14 int null,
		ddim15 int null,
		ddim16 int null,
		ddim17 int null,
		ddim18 int null,
		ddim19 int null
	)

	declare @sql nvarchar(max)=''
	declare @sqlb nvarchar(max)=''
	declare @usql nvarchar(max)=''
	declare @dimField sysname
	declare @baseField sysname
	declare @pos int

	declare c cursor
	for
	select * from #map order by Id

	open c

NextRow:
	FETCH NEXT FROM c INTO @dimField, @baseField, @pos

	if @@fetch_status = 0
	begin

		if (len(@usql)>0)
		begin
			set @usql = @usql + ', '
		end
		set @usql = @usql + @dimField + '=d.ddim'+cast(@pos as varchar(10))

		set @sql = @sql + ', dd'+cast(@pos as varchar(10))+'.DateDimensionId'
		set @sqlb = @sqlb +' left join dbo.DateDimensions dd'+ +cast(@pos as varchar(10))+' on src.'+@baseField+'=dd'+ +cast(@pos as varchar(10))+'.CalendarDate and dd'+cast(@pos as varchar(10))+'.TenantId='+cast(@tenantId as varchar(10))

		goto NextRow

	end

	close c
	deallocate c

	if @pos is null
	begin
		exec db.PrintNow 'There are no date dimensions', @pos
		return
	end
	else begin
		exec db.PrintNow 'There are {n0} date dimensions', @pos
	end

	while @pos<19
	begin
		set @sql = @sql +', null'
		set @pos = @pos + 1
	end

	set @sql = 'select src.'+@idField+@sql+' from '+@sourceSchema+'.'+@sourceTable+' src '+@sqlb+' where src.DateDimensionsLinked=0 and src.TenantId='+cast(@tenantId as varchar(10))

	exec db.PrintSql @sql

	insert into #d
	exec(@sql)

	exec db.PrintNow 'Staged {n0} rows', @@rowcount

	select top(100) * from #d

	set @usql = 'update u set DateDimensionsLinked=1, '+@usql+' from '+@sourceSchema + '.'+ @sourceTable +' u inner join #d d on u.'+@idField+'=d.pkid where d.id between @low and @low+@batchSize'

	exec db.PrintSql @usql

	declare @ra int
	set @pos = 0
NextUpdate:
	
--	execute sp_executesql N'select top(10) * from #d where id between @low and @low+@batchSize', N'@low int, @batchSize int', @low=@pos, @batchSize=@batchSize
	execute sp_executesql @usql, N'@low int, @batchSize int', @low=@pos, @batchSize=@batchSize
	set @ra = @@rowcount
	exec db.PrintNow 'Updated {n0} rows', @ra
	set @pos = @pos + @batchSize
	if (@ra>0) goto NextUpdate


end

GO

create proc migrate.MoveRows
	@sourceSchema sysname,
	@sourceTable sysname,
	@destSchema sysname,
	@destTable sysname,
	@tenantId int,
	@batchSize int=null
as
begin

	set nocount on
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	set @batchSize = coalesce(@batchSize, 5000)
	exec db.PrintNow 
		'MoveRows tenantId={n0} batchSize={n1} from table {s0}.{s1} to {s2}.{s3}', 
		@tenantId, @batchSize, @s0=@sourceSchema, @s1=@sourceTable, @s2=@destSchema, @s3=@destTable

	exec db.AssertNotNull @tenantId

	declare @col_length int

	select @col_length=count(*) from information_Schema.tables where table_schema=@sourceSchema and table_name=@sourceTable
	if (@col_length<>1)
	begin
		
		exec db.PrintNow 'MoveRows {s0}.{s1} DOES NOT EXIST', @s0=@sourceSchema, @s1=@sourceTable
		return;

	end

	declare @column_name sysname
	declare @data_type nvarchar(100)

	declare @sql nvarchar(max)=''
	declare @sqlb nvarchar(max)=''

	declare c cursor
	for
	select src.column_name, dst.DATA_TYPE, dst.CHARACTER_MAXIMUM_LENGTH
	from
		information_schema.columns src
			inner join
		information_schema.columns dst
			on src.column_name=dst.column_name
	where
		src.table_schema=@sourceSchema and
		src.table_name=@sourceTable and
		dst.table_schema=@DestSchema and
		dst.table_name=@DestTable
	order by
		src.ordinal_position

	open c

NextRow:
	FETCH NEXT FROM c INTO @column_name, @data_type, @col_length

	if @@fetch_status = 0
	begin

		set @sql = @sql + ', '+@column_name

		set @sqlb = @sqlb + ', try_cast(src.'+@column_name+' as '+@data_type
		if (@data_type='varchar' or @data_type='nvarchar')
		begin
			set @sqlb = @sqlb +'('+cast(@col_length as varchar(10))+')'
		end
		set @sqlb = @sqlb +')'
		goto NextRow

	end

	close c

	deallocate c

	set @sql = 'insert into '+@destSchema+'.'+@destTable+' (createdatutc, tenantid'+@sql+') select getutcdate(), '+cast(@tenantId as varchar(10))+@sqlb+' from '+@sourceSchema+'.'+@sourceTable+' src'

	exec db.PrintSql @sql

	exec(@sql)

	exec db.PrintNow 'Moved {n0} rows', @@rowcount

end

GO

create proc migrate.[ContactsCreateFromDeerwalkSetContactIds]
	@tenantId int,
	@deerwalkTableName sysname,
	@deerwalkPrimaryKeyFieldName sysname,
	@deerwalkMemberIdFieldName sysname,
	@batchSize int,
	@contactTableKeyFieldName sysname=null
as
begin

	set @batchSize = coalesce(@batchSize, 5000)
	set @contactTableKeyFieldName = coalesce(@contactTableKeyFieldName, 'MemberId')
	exec db.PrintNow 
		'ContactsCreateFromDeerwalkSetContactIds tenantId={n0}, deerwalkTableName={s0} deerwalkPrimaryKeyFieldName={s1}, deerwalkMemberIdFieldName={s2}, contactTableKeyFieldName={s3}', 
		@tenantId, @s0=@deerwalkTableName, @s1=@deerwalkPrimaryKeyFieldName, @s2=@deerwalkMemberIdFieldName, @s3=@contactTableKeyFieldName

	exec db.AssertNotNull @tenantId

	create table #m
	(
		Id int not null identity,
		DwId bigint not null,
		ContactId bigint not null
	)

	declare @sql nvarchar(max)

	set @sql=
	'
	select dw.'+@deerwalkPrimaryKeyFieldName+', c.ContactId
	from
		dbo.Contacts c
			left join
		deerwalk.'+@deerwalkTableName+' dw
			on c.'+@contactTableKeyFieldName+'=dw.'+@deerwalkMemberIdFieldName+'
			and c.TenantId=dw.TenantId
	where
		dw.ContactId is null and
		c.TenantId='+cast(@tenantId as varchar(10))+' and
		dw.TenantId='+cast(@tenantId as varchar(10))+'
	'

	exec db.PrintNow @sql

	insert into #m
	exec(@sql)

	exec db.PrintNow 'Staged {n0} updates to {s0}', @@rowcount, @s0=@deerwalkTableName
	declare @n int=0
	declare @rc int

Again:

	set @sql='
	update dw
	set
		ContactId=m.ContactId
	from	
		deerwalk.'+@deerwalkTableName+' dw
			inner join
		#m m 
			on m.DwId=dw.'+@deerwalkPrimaryKeyFieldName+'
			and m.Id>'+cast(@n as varchar(10))+' and m.id<='+cast ((@n+@batchSize) as varchar(10))+'
	where
		dw.TenantId='+cast(@tenantId as varchar(10))+'
	'

	exec db.PrintNow @sql
	exec(@sql)

	set @rc = @@rowcount

	exec db.PrintNow 'Updated {n0} {s0} records', @rc, @s0=@deerwalkTableName

	set @n = @n + @batchSize

	if (@rc>0) goto Again


end

GO

create proc [migrate].[ContactsCreateFromDeerwalkEligibility]
	@tenantId int,
	@batchSize int=null
AS
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	set @batchSize = coalesce(@batchSize, 5000)
	exec db.PrintNow 'ContactsCreateFromDeerwalkEligibility tenantId={n0} batchSize={n1}', @tenantId, @batchSize 
	exec db.AssertNotNull @tenantId

	select
		ltrim(rtrim(
			case when mbr_first_name is null or mbr_first_name='' then '' else mbr_first_name+' ' end +
			case when mbr_middle_name is null or mbr_middle_name='' then '' else mbr_middle_name+' ' end +
			case when mbr_last_name is null or mbr_last_name='' then '' else mbr_last_name end
		)) FullName,
		mbr_id MemberId,
		dw_member_id DeerwalkMemberId,
		j.ContactDetails
	into #z
	from
		deerwalk.Eligibility e
			cross apply
		(
		select 
			case 
				when e.mbr_gender='M' then 'Male'
				when e.mbr_gender='F' then 'Female'
				else null
				end [person.gender],
			try_cast(e.mbr_dob as date) [person.dateOfBirth],
			e.mbr_first_name [person.name.firstName],
			e.mbr_middle_name [person.name.middleName],
			e.mbr_last_name [person.name.lastName],
			(
				select 
					i.mbr_phone [phoneNumber]	
				from deerwalk.eligibility i 
				where i.EligibilityId=e.EligibilityId
				for json path
			) phoneNumbers,
			(
				select 
					i.mbr_street_1 [addressLine1],
					i.mbr_street_2 [addressLine2],	
					i.mbr_city [city],
					i.mbr_state [state],	
					i.mbr_zip [postalCode]	
				from deerwalk.eligibility i 
				where i.EligibilityId=e.EligibilityId
				for json path
			) addresses
		for json path, WITHOUT_ARRAY_WRAPPER
		) j(ContactDetails)
			left join
		dbo.contacts c
			on c.MemberId=e.mbr_id
			and c.tenantId=@tenantId
	where
		c.contactId is null and
		e.TenantId=@tenantId

	exec db.PrintNow 'Staged {n0} eligiblity records into #z', @@rowcount

	alter table #z add Id int not null identity

	declare @n int=0
	declare @rc int

InsertContactBatch:
	insert into Contacts
	(
		TenantId, ContactType, CreatedAtUtc, FullName, MemberId, DeerwalkMemberId, ContactDetails
	)
	select @tenantId, 'Person', getutcdate(), z.FullName, z.MemberId, z.DeerwalkMemberId, z.ContactDetails
	from #z z
	where z.id>@n and z.id<=@n+@batchSize

	set @rc = @@rowcount

	exec db.PrintNow 'Created {n0} contacts', @rc

	set @n = @n + @batchSize

	if (@rc>0) goto InsertContactBatch;

	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'CareAlerts', 'CareAlertId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'Demographics', 'DemographicId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'Eligibility', 'EligibilityId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'HighCostDiagnosis', 'HighCostDiagnosisId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'HistoricalScores', 'HistoricalScoreId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'MedicalClaims', 'MedicalClaimId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'MemberPCP', 'MemberPcpId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'Participation', 'ParticipationId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'Pharmacy', 'PharmacyId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'QualityMetrics', 'QualityMetricId', 'dw_member_id', @batchSize, 'DeerwalkMemberId'
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'Scores', 'ScoreId', 'mbr_id', @batchSize
	exec migrate.ContactsCreateFromDeerwalkSetContactIds @tenantId, 'Visits', 'VisitId', 'mbr_id', @batchSize

end

GO

create proc migrate.[DeerwalkDataImport]
	@tenantId int,
	@createContactsAfterImport bit=1
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	declare @batchSize int=5000

	exec db.PrintNow 'DeerwalkDataImport tenantId={n0} createContactsAfterImport={b0}', @tenantId, @batchSize, @b0=@createContactsAfterImport
	exec db.AssertNotNull @tenantId

	exec migrate.MoveRows 'i', 'CareAlerts', 'deerwalk', 'CareAlerts', @tenantId
	exec migrate.MoveRows 'i', 'Demographics', 'deerwalk', 'Demographics', @tenantId
	exec migrate.MoveRows 'i', 'Eligibility', 'deerwalk', 'Eligibility', @tenantId
	exec migrate.MoveRows 'i', 'HighCost', 'deerwalk', 'HighCostDiagnosis', @tenantId
	exec migrate.MoveRows 'i', 'HistoricalScores', 'deerwalk', 'HistoricalScores', @tenantId
	exec migrate.MoveRows 'i', 'Medical', 'deerwalk', 'MedicalClaims', @tenantId
	exec migrate.MoveRows 'i', 'PCP', 'deerwalk', 'MemberPCP', @tenantId
	exec migrate.MoveRows 'i', 'Participation', 'deerwalk', 'Participation', @tenantId
	exec migrate.MoveRows 'i', 'Pharmacy', 'deerwalk', 'Pharmacy', @tenantId
	exec migrate.MoveRows 'i', 'QualityMetrics', 'deerwalk', 'QualityMetrics', @tenantId
	exec migrate.MoveRows 'i', 'Scores', 'deerwalk', 'Scores', @tenantId
	exec migrate.MoveRows 'i', 'Visits', 'deerwalk', 'Visits', @tenantId

	exec migrate.DateDimensionLink 'deerwalk', 'eligibility', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'CareAlerts', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'Demographics', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'Eligibility', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'HighCostDiagnosis', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'HistoricalScores', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'MedicalClaims', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'MemberPCP', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'Participation', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'Pharmacy', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'QualityMetrics', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'Scores', @tenantId
	exec migrate.DateDimensionLink 'deerwalk', 'Visits', @tenantId

	if (@createContactsAfterImport=1)
	begin
		exec migrate.ContactsCreateFromDeerwalkEligibility @tenantId, @batchSize
	end

end

GO

