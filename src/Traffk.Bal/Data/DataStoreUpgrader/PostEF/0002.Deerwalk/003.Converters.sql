create proc ContactsCreateFromDeerwalkSetContactIds
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

CREATE proc [migrate].[ContactsCreateFromDeerwalkEligibility]
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
		j.ContactDetails,
		case 
			when e.mbr_gender='M' then 'Male'
			when e.mbr_gender='F' then 'Female'
			else null
			end Gender,
		try_cast(e.mbr_dob as date) DateOfBirth,
		cast(null as varchar(255)) Prefix,
		e.mbr_first_name FirstName,
		e.mbr_middle_name MiddleName,
		e.mbr_last_name LastName,
		cast(null as varchar(255)) Suffix,
		rank () over (partition by mbr_id order by ins_med_eff_date desc, ins_med_term_date desc, dw_rawfilename desc) r
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
		TenantId, ContactType, CreatedAtUtc, FullName, MemberId, DeerwalkMemberId, ContactDetails,
		Gender, DateOfBirth, Prefix, FirstName, MiddleName, LastName, Suffix
	)
	select 
		@tenantId, 'urn:traffk.com/person/member', getutcdate(), 
		FullName, MemberId, DeerwalkMemberId, ContactDetails,
		Gender, DateOfBirth, Prefix, FirstName, MiddleName, LastName, Suffix
	from
	(
		select z.FullName, z.MemberId, z.DeerwalkMemberId, z.ContactDetails,
			z.Gender, z.DateOfBirth, z.Prefix, z.FirstName, z.MiddleName, z.LastName, z.Suffix,
			rank () over (partition by MemberId order by Id desc) rr
		from #z z
		where z.r=1 and z.id>@n and z.id<=@n+@batchSize
	) b where rr=1

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

create proc DeerwalkDataImport
	@tenantId int,
	@createContactsAfterImport bit=1
as
begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	declare @batchSize int=5000

	exec db.PrintNow 'DeerwalkDataImport tenantId={n0} createContactsAfterImport={b0}', @tenantId, @batchSize, @b0=@createContactsAfterImport
	exec db.AssertNotNull @tenantId

	insert into deerwalk.MedicalClaims select getutcdate(), @tenantId, null, i.* from i.Medical i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.MedicalClaims', @@rowcount

	insert into deerwalk.Eligibility select getutcdate(), @tenantId, null, i.* from i.Eligibility i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.Eligibility', @@rowcount

	insert into deerwalk.Pharmacy select getutcdate(), @tenantId, null, i.* from i.Pharmacy i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.Pharmacy', @@rowcount

	insert into deerwalk.Demographics select getutcdate(), @tenantId, null, i.* from i.Demographics i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.Demographics', @@rowcount

	insert into deerwalk.Visits select getutcdate(), @tenantId, null, i.* from i.Visits i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.Visits', @@rowcount

	insert into deerwalk.MemberPCP select getutcdate(), @tenantId, null, i.* from i.PCP i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.MemberPCP', @@rowcount

	insert into deerwalk.Scores select getutcdate(), @tenantId, null, i.* from i.Scores i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.Scores', @@rowcount

	insert into deerwalk.HistoricalScores select getutcdate(), @tenantId, null, i.* from i.HistoricalScores i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.HistoricalScores', @@rowcount

	insert into deerwalk.Participation select getutcdate(), @tenantId, null, i.* from i.Participation i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.Participation', @@rowcount

	insert into deerwalk.QualityMetrics select getutcdate(), @tenantId, null, i.* from i.QualityMetrics i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.QualityMetrics', @@rowcount

	insert into deerwalk.HighCostDiagnosis select getutcdate(), @tenantId, null, i.* from i.HighCost i where paid_amount not like '%e%'
	exec db.PrintNow 'Inserted {n0} items into deerwalk.HighCostDiagnosis', @@rowcount

	insert into deerwalk.CareAlerts select getutcdate(), @tenantId, null, i.* from i.CareAlerts i
	exec db.PrintNow 'Inserted {n0} items into deerwalk.CareAlerts', @@rowcount

	if (@createContactsAfterImport=1)
	begin
		exec ContactsCreateFromDeerwalkEligibility @tenantId, @batchSize
	end

end

GO
