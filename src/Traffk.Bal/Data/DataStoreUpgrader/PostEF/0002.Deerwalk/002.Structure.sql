create schema deerwalk

GO

/*
drop table deerwalk.Eligibility
drop table deerwalk.Pharmacy
drop table deerwalk.MedicalClaims
drop table deerwalk.Demographics
drop table deerwalk.Visits
drop table deerwalk.MemberPCP
drop table deerwalk.Scores
drop table deerwalk.HistoricalScores
drop table deerwalk.Participation
drop table deerwalk.QualityMetrics
drop table deerwalk.HighCostDiagnosis
drop table deerwalk.CareAlerts
*/

create table deerwalk.Eligibility (EligibilityId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.Pharmacy (PharmacyId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.MedicalClaims (MedicalClaimId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.Demographics (DemographicId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.Visits (VisitId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.MemberPCP (MemberPcpId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.Scores (ScoreId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.HistoricalScores (HistoricalScoreId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.Participation (ParticipationId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.QualityMetrics (QualityMetricId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.HighCostDiagnosis (HighCostDiagnosisId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))
create table deerwalk.CareAlerts (CareAlertId bigint not null identity primary key, CreatedAtUtc datetime not null default(GetUtcDate()), TenantId int not null references Tenants(TenantId), ContactId bigint references Contacts(ContactId))

exec db.TablePropertySet  'Eligibility', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'Pharmacy', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'MedicalClaims', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'Demographics', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'Visits', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'MemberPCP', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'Scores', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'HistoricalScores', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'Participation', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'QualityMetrics', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'HighCostDiagnosis', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'
exec db.TablePropertySet  'CareAlerts', '1', @propertyName='AddToDbContext', @tableSchema='deerwalk'

exec db.TablePropertySet  'Eligibility', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'Pharmacy', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'MedicalClaims', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'Demographics', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'Visits', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'MemberPCP', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'Scores', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'HistoricalScores', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'Participation', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'QualityMetrics', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'HighCostDiagnosis', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'
exec db.TablePropertySet  'CareAlerts', '1', @propertyName='GeneratePoco', @tableSchema='deerwalk'


exec db.TablePropertySet  'Eligibility', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'Pharmacy', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'MedicalClaims', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'Demographics', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'Visits', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'MemberPCP', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'Scores', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'HistoricalScores', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'Participation', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'QualityMetrics', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'HighCostDiagnosis', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'
exec db.TablePropertySet  'CareAlerts', 'ITraffkTenanted, IDontCreate', @propertyName='Implements', @tableSchema='deerwalk'


exec db.TablePropertySet  'MemberPCP', 'Beneficiary Provider --- Switching of a member from one provider to next and the next in different point of time. Also need to apply patient attribution logic.', @propertyName='Comment', @tableSchema='deerwalk'
exec db.TablePropertySet  'Scores', 'Milliman Advanced Risk Adjuster Scores', @propertyName='Comment', @tableSchema='deerwalk'
exec db.TablePropertySet  'HistoricalScores', 'Milliman Advanced Risk Adjuster Scores', @propertyName='Comment', @tableSchema='deerwalk'





--Sql
alter table deerwalk.Eligibility add mbr_id varchar(50) not null 
alter table deerwalk.Eligibility add ins_policy_id varchar(50)
alter table deerwalk.Eligibility add mbr_ssn varchar(30)
alter table deerwalk.Eligibility add mbr_first_name varchar(30)
alter table deerwalk.Eligibility add mbr_middle_name varchar(30)
alter table deerwalk.Eligibility add mbr_last_name varchar(30)
alter table deerwalk.Eligibility add mbr_current_status varchar(20)
alter table deerwalk.Eligibility add mbr_gender varchar(2) not null 
alter table deerwalk.Eligibility add mbr_dob date not null 
alter table deerwalk.Eligibility add mbr_street_1 varchar(50)
alter table deerwalk.Eligibility add mbr_street_2 varchar(50)
alter table deerwalk.Eligibility add mbr_city varchar(100)
alter table deerwalk.Eligibility add mbr_county varchar(20)
alter table deerwalk.Eligibility add mbr_state varchar(2)
alter table deerwalk.Eligibility add mbr_zip varchar(12)
alter table deerwalk.Eligibility add mbr_phone varchar(15)
alter table deerwalk.Eligibility add mbr_region_code varchar(32)
alter table deerwalk.Eligibility add mbr_region_name varchar(50)
alter table deerwalk.Eligibility add mbr_relationship_code varchar(5)
alter table deerwalk.Eligibility add mbr_relationship_desc varchar(50)
alter table deerwalk.Eligibility add ins_plan_type_code varchar(20)
alter table deerwalk.Eligibility add ins_plan_type_desc varchar(255)
alter table deerwalk.Eligibility add ins_carrier_id varchar(20)
alter table deerwalk.Eligibility add ins_carrier_name varchar(50)
alter table deerwalk.Eligibility add ins_coverage_type_code varchar(20)
alter table deerwalk.Eligibility add ins_coverage_type_desc varchar(50)
alter table deerwalk.Eligibility add ins_plan_id varchar(20)
alter table deerwalk.Eligibility add ins_plan_desc varchar(100)
alter table deerwalk.Eligibility add ins_emp_group_id varchar(20)
alter table deerwalk.Eligibility add ins_emp_group_name varchar(50)
alter table deerwalk.Eligibility add ins_division_id varchar(20)
alter table deerwalk.Eligibility add ins_division_name varchar(100)
alter table deerwalk.Eligibility add ins_cobra_code varchar(2)
alter table deerwalk.Eligibility add ins_cobra_desc varchar(30)
alter table deerwalk.Eligibility add ins_med_eff_date date not null 
alter table deerwalk.Eligibility add ins_med_term_date date not null 
alter table deerwalk.Eligibility add ins_rx_eff_date date
alter table deerwalk.Eligibility add ins_rx_term_date date
alter table deerwalk.Eligibility add ins_den_eff_date date
alter table deerwalk.Eligibility add ins_den_term_date date
alter table deerwalk.Eligibility add ins_vis_eff_date date
alter table deerwalk.Eligibility add ins_vis_term_date date
alter table deerwalk.Eligibility add ins_ltd_eff_date date
alter table deerwalk.Eligibility add ins_ltd_term_date date
alter table deerwalk.Eligibility add ins_std_eff_date date
alter table deerwalk.Eligibility add ins_std_term_date date
alter table deerwalk.Eligibility add prv_pcp_id varchar(50)
alter table deerwalk.Eligibility add prv_pcp_first_name varchar(100)
alter table deerwalk.Eligibility add prv_pcp_middle_name varchar(30)
alter table deerwalk.Eligibility add prv_pcp_last_name varchar(30)
alter table deerwalk.Eligibility add prv_pcp_site_id varchar(15)
alter table deerwalk.Eligibility add udf1 varchar(100)
alter table deerwalk.Eligibility add udf2 varchar(100)
alter table deerwalk.Eligibility add udf3 varchar(100)
alter table deerwalk.Eligibility add udf4 varchar(100)
alter table deerwalk.Eligibility add udf5 varchar(100)
alter table deerwalk.Eligibility add udf6 varchar(100)
alter table deerwalk.Eligibility add udf7 varchar(100)
alter table deerwalk.Eligibility add udf8 varchar(100)
alter table deerwalk.Eligibility add udf9 varchar(100)
alter table deerwalk.Eligibility add udf10 varchar(100)
alter table deerwalk.Eligibility add udf11 varchar(100)
alter table deerwalk.Eligibility add udf12 varchar(100)
alter table deerwalk.Eligibility add udf13 varchar(100)
alter table deerwalk.Eligibility add udf14 varchar(100)
alter table deerwalk.Eligibility add udf15 varchar(100)
alter table deerwalk.Eligibility add udf16 varchar(100)
alter table deerwalk.Eligibility add udf17 varchar(100)
alter table deerwalk.Eligibility add udf18 varchar(100)
alter table deerwalk.Eligibility add udf19 varchar(100)
alter table deerwalk.Eligibility add udf20 varchar(100)
alter table deerwalk.Eligibility add dw_member_id varchar(50)
alter table deerwalk.Eligibility add dw_rawfilename varchar(500)
alter table deerwalk.Eligibility add udf21 varchar(100)
alter table deerwalk.Eligibility add udf22 varchar(100)
alter table deerwalk.Eligibility add udf23 varchar(100)
alter table deerwalk.Eligibility add udf24 varchar(100)
alter table deerwalk.Eligibility add udf25 varchar(100)
alter table deerwalk.Eligibility add udf26 varchar(100)
alter table deerwalk.Eligibility add udf27 varchar(100)
alter table deerwalk.Eligibility add udf28 varchar(100)
alter table deerwalk.Eligibility add udf29 varchar(100)
alter table deerwalk.Eligibility add udf30 varchar(100)
alter table deerwalk.Eligibility add udf31 varchar(100)
alter table deerwalk.Eligibility add udf32 varchar(100)
alter table deerwalk.Eligibility add udf33 varchar(100)
alter table deerwalk.Eligibility add udf34 varchar(100)
alter table deerwalk.Eligibility add udf35 varchar(100)
alter table deerwalk.Eligibility add udf36 varchar(100)
alter table deerwalk.Eligibility add udf37 varchar(100)
alter table deerwalk.Eligibility add udf38 varchar(100)
alter table deerwalk.Eligibility add udf39 varchar(100)
alter table deerwalk.Eligibility add udf40 varchar(100)
alter table deerwalk.Pharmacy add rev_transaction_num varchar(80) not null 
alter table deerwalk.Pharmacy add ins_plan_type_code varchar(20)
alter table deerwalk.Pharmacy add ins_plan_type_desc varchar(255)
alter table deerwalk.Pharmacy add mbr_id varchar(50) not null 
alter table deerwalk.Pharmacy add mbr_first_name varchar(30)
alter table deerwalk.Pharmacy add mbr_middle_name varchar(30)
alter table deerwalk.Pharmacy add mbr_last_name varchar(30)
alter table deerwalk.Pharmacy add mbr_gender varchar(2) not null 
alter table deerwalk.Pharmacy add mbr_dob date not null 
alter table deerwalk.Pharmacy add mbr_city varchar(20)
alter table deerwalk.Pharmacy add mbr_county varchar(20)
alter table deerwalk.Pharmacy add mbr_zip varchar(10)
alter table deerwalk.Pharmacy add mbr_relationship_code varchar(5)
alter table deerwalk.Pharmacy add mbr_relationship_desc varchar(50)
alter table deerwalk.Pharmacy add ins_carrier_id varchar(20)
alter table deerwalk.Pharmacy add ins_carrier_name varchar(50)
alter table deerwalk.Pharmacy add ins_coverage_type_code varchar(20)
alter table deerwalk.Pharmacy add ins_coverage_type_desc varchar(50)
alter table deerwalk.Pharmacy add ins_emp_group_id varchar(20)
alter table deerwalk.Pharmacy add ins_emp_group_name varchar(100)
alter table deerwalk.Pharmacy add ins_division_id varchar(20)
alter table deerwalk.Pharmacy add ins_division_name varchar(100)
alter table deerwalk.Pharmacy add ins_cobra_code varchar(2)
alter table deerwalk.Pharmacy add ins_cobra_desc varchar(30)
alter table deerwalk.Pharmacy add prv_prescriber_id varchar(20)
alter table deerwalk.Pharmacy add prv_prescriber_first_name varchar(70)
alter table deerwalk.Pharmacy add prv_prescriber_middle_name varchar(30)
alter table deerwalk.Pharmacy add prv_prescriber_last_name varchar(40)
alter table deerwalk.Pharmacy add prv_prescriber_type_Code varchar(10)
alter table deerwalk.Pharmacy add prv_prescriber_type_desc varchar(50)
alter table deerwalk.Pharmacy add prv_dea varchar(30)
alter table deerwalk.Pharmacy add prv_npi varchar(30)
alter table deerwalk.Pharmacy add prv_nabp varchar(30)
alter table deerwalk.Pharmacy add prv_type_code varchar(10)
alter table deerwalk.Pharmacy add svc_written_date date
alter table deerwalk.Pharmacy add svc_filled_date date not null 
alter table deerwalk.Pharmacy add svc_service_date date
alter table deerwalk.Pharmacy add rev_paid_date date
alter table deerwalk.Pharmacy add svc_ndc_code varchar(11)
alter table deerwalk.Pharmacy add svc_ndc_desc varchar(256)
alter table deerwalk.Pharmacy add svc_rx_class_code varchar(12)
alter table deerwalk.Pharmacy add svc_rx_class_desc varchar(200)
alter table deerwalk.Pharmacy add svc_drug_name varchar(200)
alter table deerwalk.Pharmacy add svc_dosage varchar(10)
alter table deerwalk.Pharmacy add svc_drug_strength varchar(100)
alter table deerwalk.Pharmacy add svc_unit_qty int
alter table deerwalk.Pharmacy add svc_days_of_supply int
alter table deerwalk.Pharmacy add svc_label_name varchar(200)
alter table deerwalk.Pharmacy add svc_formulary_plan_code varchar(10)
alter table deerwalk.Pharmacy add svc_formulary_flag varchar(1)
alter table deerwalk.Pharmacy add svc_generic_flag varchar(1)
alter table deerwalk.Pharmacy add svc_mail_order_flag varchar(1)
alter table deerwalk.Pharmacy add svc_refill_qty int
alter table deerwalk.Pharmacy add svc_refill_allowed int
alter table deerwalk.Pharmacy add svc_counter_allow int
alter table deerwalk.Pharmacy add svc_daw_code varchar(10)
alter table deerwalk.Pharmacy add svc_daw_desc varchar(50)
alter table deerwalk.Pharmacy add rev_allowed_amt money
alter table deerwalk.Pharmacy add rev_billed_amt money
alter table deerwalk.Pharmacy add rev_coinsurance_amt money
alter table deerwalk.Pharmacy add rev_copay_amt money
alter table deerwalk.Pharmacy add rev_deductible_amt money
alter table deerwalk.Pharmacy add rev_disp_fee_amt money
alter table deerwalk.Pharmacy add rev_ingred_cost_amt money
alter table deerwalk.Pharmacy add rev_stax_amt money
alter table deerwalk.Pharmacy add rev_usual_cust_amt money
alter table deerwalk.Pharmacy add rev_paid_amt money not null
alter table deerwalk.Pharmacy add rev_adjudication_code varchar(8)
alter table deerwalk.Pharmacy add rev_adjudication_desc varchar(50)
alter table deerwalk.Pharmacy add udf1 varchar(100)
alter table deerwalk.Pharmacy add udf2 varchar(100)
alter table deerwalk.Pharmacy add udf3 varchar(100)
alter table deerwalk.Pharmacy add udf4 varchar(100)
alter table deerwalk.Pharmacy add udf5 varchar(100)
alter table deerwalk.Pharmacy add udf6 varchar(100)
alter table deerwalk.Pharmacy add udf7 varchar(100)
alter table deerwalk.Pharmacy add udf8 varchar(100)
alter table deerwalk.Pharmacy add udf9 varchar(100)
alter table deerwalk.Pharmacy add udf10 varchar(100)
alter table deerwalk.Pharmacy add udf11 varchar(100)
alter table deerwalk.Pharmacy add udf12 varchar(100)
alter table deerwalk.Pharmacy add udf13 varchar(100)
alter table deerwalk.Pharmacy add udf14 varchar(100)
alter table deerwalk.Pharmacy add udf15 varchar(100)
alter table deerwalk.Pharmacy add udf16 varchar(100)
alter table deerwalk.Pharmacy add udf17 varchar(100)
alter table deerwalk.Pharmacy add udf18 varchar(100)
alter table deerwalk.Pharmacy add udf19 varchar(100)
alter table deerwalk.Pharmacy add udf20 varchar(100)
alter table deerwalk.Pharmacy add dw_member_id varchar(50)
alter table deerwalk.Pharmacy add is_makalu_used varchar(20)
alter table deerwalk.Pharmacy add dw_rawfilename varchar(500)
alter table deerwalk.Pharmacy add udf21 varchar(100)
alter table deerwalk.Pharmacy add udf22 varchar(100)
alter table deerwalk.Pharmacy add udf23 varchar(100)
alter table deerwalk.Pharmacy add udf24 varchar(100)
alter table deerwalk.Pharmacy add udf25 varchar(100)
alter table deerwalk.Pharmacy add udf26 varchar(100)
alter table deerwalk.Pharmacy add udf27 varchar(100)
alter table deerwalk.Pharmacy add udf28 varchar(100)
alter table deerwalk.Pharmacy add udf29 varchar(100)
alter table deerwalk.Pharmacy add udf30 varchar(100)
alter table deerwalk.Pharmacy add udf31 varchar(100)
alter table deerwalk.Pharmacy add udf32 varchar(100)
alter table deerwalk.Pharmacy add udf33 varchar(100)
alter table deerwalk.Pharmacy add udf34 varchar(100)
alter table deerwalk.Pharmacy add udf35 varchar(100)
alter table deerwalk.Pharmacy add udf36 varchar(100)
alter table deerwalk.Pharmacy add udf37 varchar(100)
alter table deerwalk.Pharmacy add udf38 varchar(100)
alter table deerwalk.Pharmacy add udf39 varchar(100)
alter table deerwalk.Pharmacy add udf40 varchar(100)
alter table deerwalk.MedicalClaims add rev_claim_id varchar(50) not null 
alter table deerwalk.MedicalClaims add rev_claim_line_id varchar(25)
alter table deerwalk.MedicalClaims add rev_claim_type varchar(15)
alter table deerwalk.MedicalClaims add rev_claim_type_flag char
alter table deerwalk.MedicalClaims add ins_plan_type_code varchar(20)
alter table deerwalk.MedicalClaims add ins_plan_type_desc varchar(255)
alter table deerwalk.MedicalClaims add ins_carrier_id varchar(20)
alter table deerwalk.MedicalClaims add ins_carrier_name varchar(50)
alter table deerwalk.MedicalClaims add ins_coverage_type_code varchar(10)
alter table deerwalk.MedicalClaims add ins_coverage_type_desc varchar(50)
alter table deerwalk.MedicalClaims add ins_plan_id varchar(20)
alter table deerwalk.MedicalClaims add ins_emp_group_id varchar(20)
alter table deerwalk.MedicalClaims add ins_emp_group_name varchar(50)
alter table deerwalk.MedicalClaims add ins_division_id varchar(20)
alter table deerwalk.MedicalClaims add ins_division_name varchar(100)
alter table deerwalk.MedicalClaims add ins_cobra_code varchar(2)
alter table deerwalk.MedicalClaims add ins_cobra_desc varchar(30)
alter table deerwalk.MedicalClaims add mbr_id varchar(50) not null 
alter table deerwalk.MedicalClaims add mbr_ssn varchar(30)
alter table deerwalk.MedicalClaims add mbr_first_name varchar(30)
alter table deerwalk.MedicalClaims add mbr_middle_name varchar(30)
alter table deerwalk.MedicalClaims add mbr_last_name varchar(30)
alter table deerwalk.MedicalClaims add mbr_gender varchar(2) not null 
alter table deerwalk.MedicalClaims add mbr_dob date not null 
alter table deerwalk.MedicalClaims add mbr_street_1 varchar(50)
alter table deerwalk.MedicalClaims add mbr_street_2 varchar(50)
alter table deerwalk.MedicalClaims add mbr_city varchar(50)
alter table deerwalk.MedicalClaims add mbr_county varchar(20)
alter table deerwalk.MedicalClaims add mbr_state varchar(2)
alter table deerwalk.MedicalClaims add mbr_zip varchar(10)
alter table deerwalk.MedicalClaims add mbr_phone varchar(15)
alter table deerwalk.MedicalClaims add mbr_region_code varchar(32)
alter table deerwalk.MedicalClaims add mbr_region_name varchar(50)
alter table deerwalk.MedicalClaims add mbr_relationship_code varchar(10)
alter table deerwalk.MedicalClaims add mbr_relationship_desc varchar(50)
alter table deerwalk.MedicalClaims add prv_service_provider_id varchar(30)
alter table deerwalk.MedicalClaims add prv_npi varchar(30)
alter table deerwalk.MedicalClaims add prv_tin varchar(30)
alter table deerwalk.MedicalClaims add prv_type_desc varchar(70)
alter table deerwalk.MedicalClaims add prv_first_name varchar(100)
alter table deerwalk.MedicalClaims add prv_middle_name varchar(30)
alter table deerwalk.MedicalClaims add prv_last_name varchar(40)
alter table deerwalk.MedicalClaims add prv_gender varchar(2)
alter table deerwalk.MedicalClaims add prv_native_language varchar(30)
alter table deerwalk.MedicalClaims add prv_network_code varchar(10)
alter table deerwalk.MedicalClaims add prv_network_name varchar(50)
alter table deerwalk.MedicalClaims add prv_phone varchar(20)
alter table deerwalk.MedicalClaims add prv_speciality_1_code varchar(10)
alter table deerwalk.MedicalClaims add prv_Specialty_1_desc varchar(100)
alter table deerwalk.MedicalClaims add prv_speciality_2_code varchar(10)
alter table deerwalk.MedicalClaims add prv_Specialty_2_desc varchar(100)
alter table deerwalk.MedicalClaims add prv_speciality_3_code varchar(10)
alter table deerwalk.MedicalClaims add prv_Specialty_3_desc varchar(100)
alter table deerwalk.MedicalClaims add prv_street_1 varchar(128)
alter table deerwalk.MedicalClaims add prv_street_2 varchar(128)
alter table deerwalk.MedicalClaims add prv_city varchar(32)
alter table deerwalk.MedicalClaims add prv_county varchar(32)
alter table deerwalk.MedicalClaims add prv_state varchar(2)
alter table deerwalk.MedicalClaims add prv_zip varchar(10)
alter table deerwalk.MedicalClaims add prv_in_network_flag char
alter table deerwalk.MedicalClaims add prv_pcp_id varchar(30)
alter table deerwalk.MedicalClaims add prv_pcp_first_name varchar(100)
alter table deerwalk.MedicalClaims add prv_pcp_middle_name varchar(30)
alter table deerwalk.MedicalClaims add prv_pcp_last_name varchar(50)
alter table deerwalk.MedicalClaims add svc_pos_code varchar(2) not null 
alter table deerwalk.MedicalClaims add svc_pos_desc varchar(50)
alter table deerwalk.MedicalClaims add svc_diag_1_code varchar(8) not null 
alter table deerwalk.MedicalClaims add svc_diag_1_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_diag_2_code varchar(30)
alter table deerwalk.MedicalClaims add svc_diag_2_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_diag_3_code varchar(30)
alter table deerwalk.MedicalClaims add svc_diag_3_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_diag_4_code varchar(30)
alter table deerwalk.MedicalClaims add svc_diag_4_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_diag_5_code varchar(30)
alter table deerwalk.MedicalClaims add svc_diag_5_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_diag_6_code varchar(30)
alter table deerwalk.MedicalClaims add svc_diag_6_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_diag_7_code varchar(30)
alter table deerwalk.MedicalClaims add svc_diag_7_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_diag_8_code varchar(30)
alter table deerwalk.MedicalClaims add svc_diag_8_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_diag_9_code varchar(30)
alter table deerwalk.MedicalClaims add svc_diag_9_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_procedure_type varchar(10)
alter table deerwalk.MedicalClaims add svc_procedure_code varchar(10) not null 
alter table deerwalk.MedicalClaims add svc_procedure_desc varchar(200)
alter table deerwalk.MedicalClaims add svc_rev_code varchar(5)
alter table deerwalk.MedicalClaims add svc_rev_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_cpt_code varchar(5)
alter table deerwalk.MedicalClaims add svc_cpt_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_icd_proc_1_code varchar(10)
alter table deerwalk.MedicalClaims add svc_icd_proc_1_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_icd_proc_2_code varchar(5)
alter table deerwalk.MedicalClaims add svc_icd_proc_2_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_drg_type_code varchar(10)
alter table deerwalk.MedicalClaims add svc_drg_type_Desc varchar(100)
alter table deerwalk.MedicalClaims add svc_drg_code varchar(7)
alter table deerwalk.MedicalClaims add svc_drg_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_hcpcs_code varchar(5)
alter table deerwalk.MedicalClaims add svc_hcpcs_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_modifier_code varchar(8)
alter table deerwalk.MedicalClaims add svc_modifier_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_modifier_2_code varchar(8)
alter table deerwalk.MedicalClaims add svc_modifier_2_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_modifier_3_code varchar(8)
alter table deerwalk.MedicalClaims add svc_modifier_3_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_tos_code varchar(5)
alter table deerwalk.MedicalClaims add svc_tos_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_discharge_code varchar(20)
alter table deerwalk.MedicalClaims add svc_discharge_desc varchar(100)
alter table deerwalk.MedicalClaims add svc_service_qty int
alter table deerwalk.MedicalClaims add svc_ip_days int
alter table deerwalk.MedicalClaims add svc_covered_days int
alter table deerwalk.MedicalClaims add svc_admit_type varchar(6)
alter table deerwalk.MedicalClaims add svc_service_frm_date date not null 
alter table deerwalk.MedicalClaims add svc_service_to_date date
alter table deerwalk.MedicalClaims add rev_adjudication_date date
alter table deerwalk.MedicalClaims add rev_paid_date date
alter table deerwalk.MedicalClaims add svc_benefit_code varchar(10)
alter table deerwalk.MedicalClaims add svc_benefit_desc varchar(100)
alter table deerwalk.MedicalClaims add rev_allowed_amt money
alter table deerwalk.MedicalClaims add rev_billed_amt money
alter table deerwalk.MedicalClaims add rev_cob_paid_amt money
alter table deerwalk.MedicalClaims add rev_coinsurance_amt money
alter table deerwalk.MedicalClaims add rev_copay_amt money
alter table deerwalk.MedicalClaims add rev_coverage_charge_amt money
alter table deerwalk.MedicalClaims add rev_deductible_amt money
alter table deerwalk.MedicalClaims add rev_not_covered_amt money
alter table deerwalk.MedicalClaims add rev_other_savings money
alter table deerwalk.MedicalClaims add rev_ppo_savings money
alter table deerwalk.MedicalClaims add rev_paid_amt money not null
alter table deerwalk.MedicalClaims add rev_pay_type varchar(4)
alter table deerwalk.MedicalClaims add rev_check_num varchar(20)
alter table deerwalk.MedicalClaims add svc_pre_authorization varchar(50)
alter table deerwalk.MedicalClaims add rev_adjudication_code varchar(8)
alter table deerwalk.MedicalClaims add rev_adjudication_desc varchar(50)
alter table deerwalk.MedicalClaims add mbr_mrn varchar(30)
alter table deerwalk.MedicalClaims add mbr_hicn varchar(11)
alter table deerwalk.MedicalClaims add rev_bill_type_code varchar(3)
alter table deerwalk.MedicalClaims add rev_bill_type_desc varchar(100)
alter table deerwalk.MedicalClaims add udf1 varchar(100)
alter table deerwalk.MedicalClaims add udf2 varchar(100)
alter table deerwalk.MedicalClaims add udf3 varchar(100)
alter table deerwalk.MedicalClaims add udf4 varchar(100)
alter table deerwalk.MedicalClaims add udf5 varchar(100)
alter table deerwalk.MedicalClaims add udf6 varchar(100)
alter table deerwalk.MedicalClaims add udf7 varchar(100)
alter table deerwalk.MedicalClaims add udf8 varchar(100)
alter table deerwalk.MedicalClaims add udf9 varchar(100)
alter table deerwalk.MedicalClaims add udf10 varchar(100)
alter table deerwalk.MedicalClaims add udf11 varchar(100)
alter table deerwalk.MedicalClaims add udf12 varchar(100)
alter table deerwalk.MedicalClaims add udf13 varchar(100)
alter table deerwalk.MedicalClaims add udf14 varchar(100)
alter table deerwalk.MedicalClaims add udf15 varchar(100)
alter table deerwalk.MedicalClaims add udf16 varchar(100)
alter table deerwalk.MedicalClaims add udf17 varchar(100)
alter table deerwalk.MedicalClaims add udf18 varchar(100)
alter table deerwalk.MedicalClaims add udf19 varchar(100)
alter table deerwalk.MedicalClaims add udf20 varchar(100)
alter table deerwalk.MedicalClaims add dw_vendor_name varchar(20)
alter table deerwalk.MedicalClaims add dw_admrule varchar(6)
alter table deerwalk.MedicalClaims add proc1_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add proc1_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add proc1_Subgrouper_id varchar(100)
alter table deerwalk.MedicalClaims add proc1_Subgrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add rev_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add rev_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add rev_subgrouper_id varchar(100)
alter table deerwalk.MedicalClaims add rev_subgrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add cpt_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add cpt_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add cpt_subgrouper_id varchar(100)
alter table deerwalk.MedicalClaims add cpt_subgrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add icd1_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add icd1_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add icd1_subgrouper_id varchar(100)
alter table deerwalk.MedicalClaims add icd1_subgrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add icd2_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add icd2_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add icd2_subgrouper_id varchar(100)
alter table deerwalk.MedicalClaims add icd2_subgrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add drg_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add drg_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add drg_subgrouper_id varchar(100)
alter table deerwalk.MedicalClaims add drg_subgrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add hcpcs_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add hcpcs_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add hcpcs_subgrouper_id varchar(100)
alter table deerwalk.MedicalClaims add hcpcs_subgrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag1_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag1_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag1_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag1_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag2_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag2_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag2_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag2_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag3_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag3_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag3_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag3_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag4_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag4_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag4_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag4_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag5_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag5_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag5_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag5_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag6_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag6_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag6_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag6_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag7_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag7_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag7_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag7_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag8_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag8_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag8_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag8_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag9_grouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag9_grouper_desc varchar(100)
alter table deerwalk.MedicalClaims add diag9_supergrouper_id varchar(100)
alter table deerwalk.MedicalClaims add diag9_supergrouper_desc varchar(100)
alter table deerwalk.MedicalClaims add cpt_betos varchar(20)
alter table deerwalk.MedicalClaims add cpt_betos_grouper varchar(100)
alter table deerwalk.MedicalClaims add cpt_betos_sub_grouper varchar(100)
alter table deerwalk.MedicalClaims add hcpcs_betos varchar(100)
alter table deerwalk.MedicalClaims add hcpcs_betos_grouper varchar(100)
alter table deerwalk.MedicalClaims add hcpcs_betos_sub_grouper varchar(100)
alter table deerwalk.MedicalClaims add rev_betos varchar(100)
alter table deerwalk.MedicalClaims add rev_betos_grouper varchar(100)
alter table deerwalk.MedicalClaims add rev_betos_sub_grouper varchar(100)
alter table deerwalk.MedicalClaims add icd1_betos varchar(100)
alter table deerwalk.MedicalClaims add icd1_betos_grouper varchar(100)
alter table deerwalk.MedicalClaims add icd1_betos_sub_grouper varchar(100)
alter table deerwalk.MedicalClaims add icd2_betos varchar(100)
alter table deerwalk.MedicalClaims add icd2_betos_grouper varchar(100)
alter table deerwalk.MedicalClaims add icd2_betos_sub_grouper varchar(100)
alter table deerwalk.MedicalClaims add drg_betos varchar(100)
alter table deerwalk.MedicalClaims add drg_betos_grouper varchar(100)
alter table deerwalk.MedicalClaims add drg_betos_sub_grouper varchar(100)
alter table deerwalk.MedicalClaims add proc7_betos varchar(100)
alter table deerwalk.MedicalClaims add proc7_betos_grouper varchar(100)
alter table deerwalk.MedicalClaims add proc7_betos_sub_grouper varchar(100)
alter table deerwalk.MedicalClaims add dw_creation_date date
alter table deerwalk.MedicalClaims add dw_update_date date
alter table deerwalk.MedicalClaims add dw_rawfilename varchar(255)
alter table deerwalk.MedicalClaims add dw_recievedmonth varchar(10)
alter table deerwalk.MedicalClaims add visit_id int
alter table deerwalk.MedicalClaims add dw_member_id varchar(50)
alter table deerwalk.MedicalClaims add is_makalu_used varchar(20)
alter table deerwalk.MedicalClaims add udf21 varchar(100)
alter table deerwalk.MedicalClaims add udf22 varchar(100)
alter table deerwalk.MedicalClaims add udf23 varchar(100)
alter table deerwalk.MedicalClaims add udf24 varchar(100)
alter table deerwalk.MedicalClaims add udf25 varchar(100)
alter table deerwalk.MedicalClaims add udf26 varchar(100)
alter table deerwalk.MedicalClaims add udf27 varchar(100)
alter table deerwalk.MedicalClaims add udf28 varchar(100)
alter table deerwalk.MedicalClaims add udf29 varchar(100)
alter table deerwalk.MedicalClaims add udf30 varchar(100)
alter table deerwalk.MedicalClaims add udf31 varchar(100)
alter table deerwalk.MedicalClaims add udf32 varchar(100)
alter table deerwalk.MedicalClaims add udf33 varchar(100)
alter table deerwalk.MedicalClaims add udf34 varchar(100)
alter table deerwalk.MedicalClaims add udf35 varchar(100)
alter table deerwalk.MedicalClaims add udf36 varchar(100)
alter table deerwalk.MedicalClaims add udf37 varchar(100)
alter table deerwalk.MedicalClaims add udf38 varchar(100)
alter table deerwalk.MedicalClaims add udf39 varchar(100)
alter table deerwalk.MedicalClaims add udf40 varchar(100)
alter table deerwalk.Demographics add dw_record_id int
alter table deerwalk.Demographics add dw_account_id varchar(50)
alter table deerwalk.Demographics add dw_client_id varchar(10)
alter table deerwalk.Demographics add dw_member_id varchar(50)
alter table deerwalk.Demographics add mbr_id varchar(50) not null 
alter table deerwalk.Demographics add mbr_ssn varchar(30)
alter table deerwalk.Demographics add mbr_first_name varchar(100)
alter table deerwalk.Demographics add mbr_middle_name varchar(100)
alter table deerwalk.Demographics add mbr_last_name varchar(100)
alter table deerwalk.Demographics add mbr_current_status varchar(10)
alter table deerwalk.Demographics add mbr_gender varchar(10) not null 
alter table deerwalk.Demographics add mbr_dob date not null 
alter table deerwalk.Demographics add mbr_street_1 varchar(100)
alter table deerwalk.Demographics add mbr_street_2 varchar(100)
alter table deerwalk.Demographics add mbr_city varchar(100)
alter table deerwalk.Demographics add mbr_county varchar(100)
alter table deerwalk.Demographics add mbr_state varchar(100)
alter table deerwalk.Demographics add mbr_zip varchar(100)
alter table deerwalk.Demographics add mbr_phone varchar(100)
alter table deerwalk.Demographics add mbr_region_code varchar(100)
alter table deerwalk.Demographics add mbr_region_name varchar(100)
alter table deerwalk.Demographics add mbr_relationship_code varchar(10)
alter table deerwalk.Demographics add mbr_relationship_desc varchar(50)
alter table deerwalk.Demographics add dw_rawfilename varchar(100)
alter table deerwalk.Demographics add dw_recievedmonth varchar(100)
alter table deerwalk.Demographics add dw_vendor_name varchar(100)
alter table deerwalk.Visits add dw_record_id int
alter table deerwalk.Visits add dw_account_id varchar(50)
alter table deerwalk.Visits add dw_client_id varchar(50)
alter table deerwalk.Visits add dw_member_id varchar(50)
alter table deerwalk.Visits add mbr_id varchar(50) not null 
alter table deerwalk.Visits add mbr_visit_type varchar(50)
alter table deerwalk.Visits add mbr_start_date date
alter table deerwalk.Visits add mbr_end_date date
alter table deerwalk.Visits add value varchar(20)
alter table deerwalk.Visits add admission_type varchar(55)
alter table deerwalk.Visits add ip_days numeric
alter table deerwalk.Visits add admission_from_er varchar(1)
alter table deerwalk.MemberPCP add dw_record_id int
alter table deerwalk.MemberPCP add dw_account_id varchar(50)
alter table deerwalk.MemberPCP add dw_client_id varchar(10)
alter table deerwalk.MemberPCP add dw_member_id varchar(50)
alter table deerwalk.MemberPCP add mbr_id varchar(50) not null 
alter table deerwalk.MemberPCP add pcp_name varchar(100)
alter table deerwalk.MemberPCP add pcp_npi varchar(100)
alter table deerwalk.MemberPCP add start_date date
alter table deerwalk.MemberPCP add end_date date
alter table deerwalk.Scores add dw_record_id int
alter table deerwalk.Scores add dw_account_id varchar(50)
alter table deerwalk.Scores add dw_client_id varchar(50)
alter table deerwalk.Scores add dw_member_id varchar(50)
alter table deerwalk.Scores add mbr_id varchar(50) not null 
alter table deerwalk.Scores add score_type varchar(50)
alter table deerwalk.Scores add score_start_date date
alter table deerwalk.Scores add score_end_date date
alter table deerwalk.Scores add ip_score float
alter table deerwalk.Scores add op_score float
alter table deerwalk.Scores add phy_score float
alter table deerwalk.Scores add rx_score float
alter table deerwalk.Scores add med_score float
alter table deerwalk.Scores add total_score float
alter table deerwalk.Scores add concurrent_total float
alter table deerwalk.Scores add erScore float
alter table deerwalk.Scores add otherScore float
alter table deerwalk.Scores add concurrentInpatient float
alter table deerwalk.Scores add concurrentMedical float
alter table deerwalk.Scores add concurrentOutpatient float
alter table deerwalk.Scores add concurrentPharmacy float
alter table deerwalk.Scores add concurrentPhysician float
alter table deerwalk.Scores add concurrentIpNormalizedToGroup float
alter table deerwalk.Scores add concurrentOpNormalizedToGroup float
alter table deerwalk.Scores add concurrentPhyNormalizedToGroup float
alter table deerwalk.HistoricalScores add dw_record_id int
alter table deerwalk.HistoricalScores add dw_account_id varchar(50)
alter table deerwalk.HistoricalScores add dw_client_id varchar(50)
alter table deerwalk.HistoricalScores add dw_member_id varchar(50)
alter table deerwalk.HistoricalScores add mbr_id varchar(50) not null 
alter table deerwalk.HistoricalScores add score_type varchar(50)
alter table deerwalk.HistoricalScores add score_start_date date
alter table deerwalk.HistoricalScores add score_end_date date
alter table deerwalk.HistoricalScores add ip_score float
alter table deerwalk.HistoricalScores add op_score float
alter table deerwalk.HistoricalScores add phy_score float
alter table deerwalk.HistoricalScores add rx_score float
alter table deerwalk.HistoricalScores add med_score float
alter table deerwalk.HistoricalScores add total_score float
alter table deerwalk.HistoricalScores add concurrent_total float
alter table deerwalk.HistoricalScores add erScore float
alter table deerwalk.HistoricalScores add otherScore float
alter table deerwalk.HistoricalScores add concurrentInpatient float
alter table deerwalk.HistoricalScores add concurrentMedical float
alter table deerwalk.HistoricalScores add concurrentOutpatient float
alter table deerwalk.HistoricalScores add concurrentPharmacy float
alter table deerwalk.HistoricalScores add concurrentPhysician float
alter table deerwalk.HistoricalScores add concurrentIpNormalizedToGroup float
alter table deerwalk.HistoricalScores add concurrentOpNormalizedToGroup float
alter table deerwalk.HistoricalScores add concurrentPhyNormalizedToGroup float
alter table deerwalk.Participation add dw_record_id int
alter table deerwalk.Participation add dw_account_id varchar(50)
alter table deerwalk.Participation add dw_client_id varchar(50)
alter table deerwalk.Participation add dw_member_id varchar(50)
alter table deerwalk.Participation add mbr_id varchar(50) not null 
alter table deerwalk.Participation add program_type varchar(50)
alter table deerwalk.Participation add program_code varchar(20) not null 
alter table deerwalk.Participation add program_name varchar(50)
alter table deerwalk.Participation add program_status varchar(20)
alter table deerwalk.Participation add program_start_date date
alter table deerwalk.Participation add program_end_date date
alter table deerwalk.QualityMetrics add dw_record_id int not null 
alter table deerwalk.QualityMetrics add dw_member_id varchar(50) not null 
alter table deerwalk.QualityMetrics add memberFirstName varchar(30)
alter table deerwalk.QualityMetrics add memberLastName varchar(30)
alter table deerwalk.QualityMetrics add memberGender varchar(5)
alter table deerwalk.QualityMetrics add memberDOB date
alter table deerwalk.QualityMetrics add measureId varchar(50)
alter table deerwalk.QualityMetrics add measureDesc varchar(200)
alter table deerwalk.QualityMetrics add PositiveNegative varchar(50)
alter table deerwalk.QualityMetrics add measureName varchar(200)
alter table deerwalk.QualityMetrics add startDate date
alter table deerwalk.QualityMetrics add EndDate date
alter table deerwalk.QualityMetrics add numerator varchar(1)
alter table deerwalk.QualityMetrics add denomenator varchar(1)
alter table deerwalk.HighCostDiagnosis add dw_record_id int
alter table deerwalk.HighCostDiagnosis add dw_account_id varchar(50)
alter table deerwalk.HighCostDiagnosis add dw_client_id varchar(50)
alter table deerwalk.HighCostDiagnosis add dw_member_id varchar(50)
alter table deerwalk.HighCostDiagnosis add mbr_id varchar(50)
alter table deerwalk.HighCostDiagnosis add Diagnosis_code varchar(10)
alter table deerwalk.HighCostDiagnosis add Paid_Amount money
alter table deerwalk.HighCostDiagnosis add SuperGrouperDescription varchar(50)
alter table deerwalk.HighCostDiagnosis add GrouperDescription varchar(50)
alter table deerwalk.CareAlerts add dw_record_id int
alter table deerwalk.CareAlerts add dw_member_id varchar(50)
alter table deerwalk.CareAlerts add mbr_id varchar(50)
alter table deerwalk.CareAlerts add first_name varchar(50)
alter table deerwalk.CareAlerts add last_name varchar(20)
alter table deerwalk.CareAlerts add middle_name varchar(10)
alter table deerwalk.CareAlerts add mbr_dob date
alter table deerwalk.CareAlerts add mbr_gender varchar(10)
alter table deerwalk.CareAlerts add mbr_status varchar(10)
alter table deerwalk.CareAlerts add mbr_relationship varchar(50)
alter table deerwalk.CareAlerts add pcp_full_name varchar(50)
alter table deerwalk.CareAlerts add mbr_age int
alter table deerwalk.CareAlerts add mbr_months varchar(3)
alter table deerwalk.CareAlerts add care_alert_startDate date
alter table deerwalk.CareAlerts add care_alert_id varchar(50)
alter table deerwalk.CareAlerts add care_alert_desc varchar(200)
alter table deerwalk.CareAlerts add metric_Type varchar(50)
alter table deerwalk.CareAlerts add metric_name varchar(50)



--Description
exec db.ColumnPropertySet 'Eligibility', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_policy_id', 'Policy Number for Member', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_ssn', 'Member SSN', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_first_name', 'Member first name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_middle_name', 'Member middle name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_last_name', 'Member last name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_current_status', ' Current status of member', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_gender', 'Member gender', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_dob', 'Member date of Birth', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_street_1', 'Member Street Address 1', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_street_2', 'Member Street Address 2', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_city', 'Member City', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_county', 'Member County', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_state', 'Abbreviation of State', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_zip', 'Zip code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_phone', 'Member Phone', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_region_code', 'Member Region code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_region_name', 'Member Region', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_relationship_code', 'Relationship Code to the Subscriber; subscriber(01), spouse (02),child (03), other (04)', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_relationship_desc', 'Relationship Description to the Subscriber, Dependent, Spouse', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_plan_type_code', 'Plan type code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_plan_type_desc', 'Plan type name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_carrier_id', 'TPA/ASO/HMO', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_carrier_name', 'TPA/ASO/HMO name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_coverage_type_code', 'Coverage type', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_coverage_type_desc', 'Coverage type name; infer from code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_plan_id', 'Plan id of insurance', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_plan_desc', 'Plan name of insurance', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_emp_group_id', 'Identification of the group the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_emp_group_name', 'Name of the group the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_division_id', 'Identification of the division the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_division_name', 'Name of the group the division  subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_cobra_code', 'Status Code of the Employee - Not Specified : 00, Working : 01, Terminated : 02', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_cobra_desc', 'Status of the Employee - Working, Terminated, etc', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_med_eff_date', 'Effective date for medical plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_med_term_date', 'Termination date for medical plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_rx_eff_date', 'Effective date for drug plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_rx_term_date', 'Termination date for drug plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_den_eff_date', 'Effective date for dental plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_den_term_date', 'Termination date for dental plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_vis_eff_date', 'Effective date for vision plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_vis_term_date', 'Termination date for vision plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_ltd_eff_date', 'Effective date for long term disability plan plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_ltd_term_date', 'Termination date for long term disability plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_std_eff_date', 'Effective date for short term disability plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_std_term_date', 'Termination date for short term disability plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_id', 'Primary Care Physician identification number', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_first_name', 'Primary Care Physician First Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_middle_name', 'Primary Care Physician Middle Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_last_name', 'Primary Care Physician Last Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_site_id', 'PCP Location ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf1', 'User Defined Field 1', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf2', 'User Defined Field 2', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf3', 'User Defined Field 3', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf4', 'User Defined Field 4', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf5', 'User Defined Field 5', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf6', 'User Defined Field 6', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf7', 'User Defined Field 7', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf8', 'User Defined Field 8', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf9', 'User Defined Field 9', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf10', 'User Defined Field 10', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf11', 'User Defined Field 11', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf12', 'User Defined Field 12', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf13', 'User Defined Field 13', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf14', 'User Defined Field 14', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf15', 'User Defined Field 15', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf16', 'User Defined Field 16', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf17', 'User Defined Field 17', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf18', 'User Defined Field 18', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf19', 'User Defined Field 19', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf20', 'User Defined Field 20', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'dw_rawfilename', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf21', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf22', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf23', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf24', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf25', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf26', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf27', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf28', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf29', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf30', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf31', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf32', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf33', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf34', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf35', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf36', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf37', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf38', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf39', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf40', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_transaction_num', 'Number generated by claim syst1)em', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_plan_type_code', 'Plan type code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_plan_type_desc', 'Plan type desciption', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_id', 'Member identification number', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_first_name', 'Member first name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_middle_name', 'Member middle name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_last_name', 'Member last name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_gender', 'Member gender', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_dob', 'Member date of Birth', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_city', 'Member City', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_county', 'Member County', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_zip', 'Zip code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_relationship_code', 'Relationship Code to the Subscriber; subscriber(01), spouse (02),child (03), other (04)', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_relationship_desc', 'Relationship Description to the Subscriber, Dependent, Spouse', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_carrier_id', 'PBM', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_carrier_name', 'PBM name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_coverage_type_code', 'Coverage type', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_coverage_type_desc', 'Coverage type name; infer from code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_emp_group_id', 'Identification of the group the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_emp_group_name', 'Name of the group the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_division_id', 'Identification of the division the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_division_name', 'Name of the group the division  subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_cobra_code', 'Status Code of Employee - Not Specified : 00, Working : 01, Terminated : 02', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_cobra_desc', 'Status of the Employee - Working, Terminated, etc', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_id', 'Member prescriber', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_first_name', 'Prescriber First Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_middle_name', 'Prescriber Middle Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_last_name', 'Prescriber Last Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_type_Code', 'Prescriber Provider Type if present.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_type_desc', 'Prescriber Provider Type Description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_dea', 'Provider dea number', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_npi', 'National Provider ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_nabp', 'Provider nabp number', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_type_code', 'Provider Type Code; I/P/A', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_written_date', 'date prescription was written', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_filled_date', 'date prescription was filled', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_service_date', 'date of service', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_paid_date', 'date of payment', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_ndc_code', 'National Drug Code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_ndc_desc', 'National Drug Code description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_rx_class_code', 'Pharmacy class code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_rx_class_desc', 'Pharmacy class code descirption', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_drug_name', 'Drug name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_dosage', 'Drug dose', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_drug_strength', 'Drug Strength', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_unit_qty', 'Quanitiy of physical unit', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_days_of_supply', 'Prescription supply based in Days', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_label_name', 'Label Name of Prescription', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_formulary_plan_code', 'Formulary Plan Code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_formulary_flag', 'Formulary flag', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_generic_flag', 'Brand / Generic Indicator', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_mail_order_flag', 'Mail Order Flag', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_refill_qty', 'Per refill quantity', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_refill_allowed', 'Number of Refills allowed', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_counter_allow', 'Allowance Provided at the Pharmacy Sales Counter', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_daw_code', 'Dispensed as Written Instructions', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_daw_desc', 'Dispensed as Written Instructions Description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_allowed_amt', 'Amount allowed under contract', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_billed_amt', 'Gross charges', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_coinsurance_amt', 'Coinsurance due from patient', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_copay_amt', 'Amount collected from the patient as a co-payment.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_deductible_amt', 'Deductible Portion of the Allowed Amount ', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_disp_fee_amt', 'Dispensing Fee textged by the Pharmacy to the PBM', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_ingred_cost_amt', 'Cost of ingredients', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_stax_amt', 'State Tax Paid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_usual_cust_amt', 'Usual and Customary Fee', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_paid_amt', 'Amount paid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_adjudication_code', 'Adjudication code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_adjudication_desc', 'Adjudication description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf1', 'User Defined Field 1', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf2', 'User Defined Field 2', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf3', 'User Defined Field 3', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf4', 'User Defined Field 4', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf5', 'User Defined Field 5', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf6', 'User Defined Field 6', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf7', 'User Defined Field 7', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf8', 'User Defined Field 8', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf9', 'User Defined Field 9', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf10', 'User Defined Field 10', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf11', 'User Defined Field 11', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf12', 'User Defined Field 12', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf13', 'User Defined Field 13', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf14', 'User Defined Field 14', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf15', 'User Defined Field 15', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf16', 'User Defined Field 16', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf17', 'User Defined Field 17', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf18', 'User Defined Field 18', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf19', 'User Defined Field 19', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf20', 'User Defined Field 20', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'is_makalu_used', 'Boolean Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'dw_rawfilename', 'Source Filename', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf21', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf22', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf23', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf24', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf25', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf26', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf27', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf28', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf29', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf30', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf31', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf32', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf33', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf34', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf35', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf36', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf37', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf38', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf39', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf40', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_claim_id', 'Number generated by claim system', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_claim_line_id', 'Number of line numbers for this claim', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_claim_type', 'Should be HCFA 1500 or UB04, Dental, Vision, STD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_claim_type_flag', 'Claim type description; 0: professional or 1: institutional', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_plan_type_code', 'Plan type code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_plan_type_desc', 'Plan type description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_carrier_id', 'TPA/ASO/HMO', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_carrier_name', 'TPA/ASO/HMO name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_coverage_type_code', 'Coverage type', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_coverage_type_desc', 'Coverage type name; infer from code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_plan_id', 'Plan id of insurance', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_emp_group_id', 'Identification of the group the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_emp_group_name', 'Name of the group the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_division_id', 'Identification of the division the subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_division_name', 'Name of the group the division  subscriber is employed with', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_cobra_code', 'Status Code of the Employee - Not Specified : 00, Working : 01, Terminated : 02', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_cobra_desc', 'Status of the Employee - Working, Terminated, etc', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_id', 'Member identification number', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_ssn', 'Member SSN', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_first_name', 'Member first name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_middle_name', 'Member middle name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_last_name', 'Member last name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_gender', 'Member gender', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_dob', 'Member date of Birth', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_street_1', 'Member Street Address 1', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_street_2', 'Member Street Address 2', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_city', 'Member City', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_county', 'Member County', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_state', 'Abbreviation of State', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_zip', 'Zip code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_phone', 'Member Phone', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_region_code', 'Member Region code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_region_name', 'Member Region', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_relationship_code', 'Relationship Code to the Subscriber; subscriber(01), spouse (02),child (03), other (04)', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_relationship_desc', 'Relationship description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_service_provider_id', 'Provider of services for ClaimType=HIC/PHYSICIANS or DENTAL', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_npi', 'National Provider ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_tin', 'Provider Tax ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_type_desc', 'Provider Type Name; Institutional / Professional / Ancillary', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_first_name', 'First Name of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_middle_name', 'Middle name of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_last_name', 'Last Name of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_gender', 'Gender of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_native_language', 'Provider  Native Language', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_network_code', 'Network Code Provider Paid Through', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_network_name', 'Network Name Provider Paid through', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_phone', 'Phone of Provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_speciality_1_code', 'First Specialty of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_Specialty_1_desc', 'First Specialty of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_speciality_2_code', 'Second Specialty of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_Specialty_2_desc', 'Second Specialty of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_speciality_3_code', 'Third Specialty of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_Specialty_3_desc', 'Third Specialty of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_street_1', 'Provider first address line', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_street_2', 'Provider second address line', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_city', 'City of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_county', 'County of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_state', 'Provider State', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_zip', 'Zip code of provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_in_network_flag', 'Identifies if Provider is - 0: in Network or 1: out of network', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_pcp_id', 'Primary Care Physician identification number', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_pcp_first_name', 'Primary Care Physician First Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_pcp_middle_name', 'Primary Care Physician Middle Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_pcp_last_name', 'Primary Care Physician Last Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_pos_code', 'Place of Service code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_pos_desc', 'Place of Service description; from Master POS table. ', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_1_code', 'Primary ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_1_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_2_code', 'Secondary ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_2_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_3_code', 'Tertiary ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_3_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_4_code', '4th ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_4_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_5_code', '5th ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_5_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_6_code', '6th ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_6_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_7_code', '7th ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_7_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_8_code', '8th ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_8_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_9_code', '9th ICD', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_9_desc', 'Diagnosis Description; From master ICD9 table. For home grown codes, use client description.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_procedure_type', 'Procedure code type - CPT4, Revenue, HCPCS, DRG, RUG (Resource Utilization Group)', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_procedure_code', 'Procedure code; CPT, HCPCS, ICD, REV, DRG in order', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_procedure_desc', 'Procedure description; From master Procedure table', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_rev_code', 'Revenue code ', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_rev_desc', 'Revenue code description; From master procedure table', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_cpt_code', 'CPT code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_cpt_desc', 'CPT code description; From master procedure table', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_icd_proc_1_code', 'First ICD procedure code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_icd_proc_1_desc', 'First ICD procedure description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_icd_proc_2_code', 'Second ICD procedure code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_icd_proc_2_desc', 'Second ICD procedure description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_drg_type_code', 'DRG Type Code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_drg_type_Desc', 'DRG Type Description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_drg_code', 'Diagnosis related group code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_drg_desc', 'Diagnosis related group description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_hcpcs_code', 'HCPCS code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_hcpcs_desc', 'HCPCS description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_modifier_code', 'CPT4 modifier code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_modifier_desc', 'CPT4 description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_modifier_2_code', 'modifier code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_modifier_2_desc', 'modifier description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_modifier_3_code', 'modifier code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_modifier_3_desc', 'modifier description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_tos_code', 'Type of service code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_tos_desc', 'Type of service description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_discharge_code', 'Type of discharge code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_discharge_desc', 'Type of discharge description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_service_qty', 'Service quantity', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_ip_days', 'Inpatient stay days', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_covered_days', 'IP days covered by the insurance', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_admit_type', 'Internal codes', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_service_frm_date', 'From date', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_service_to_date', 'To date / Thru date', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_adjudication_date', 'date the claim was adjudicated', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_paid_date', 'date of payment', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_benefit_code', 'Benefit Code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_benefit_desc', 'Benefit Code description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_allowed_amt', 'Amount allowed under contract', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_billed_amt', 'Gross charges', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_cob_paid_amt', 'Coordination of benefits on the medical plan', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_coinsurance_amt', 'Coinsurance due from patient', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_copay_amt', 'Amount collected from the patient as a co-payment.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_coverage_charge_amt', 'Network usage charge', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_deductible_amt', 'Deductible Portion of the Allowed Amount ', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_not_covered_amt', 'Billed Charges not covered under the Member policy', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_other_savings', 'Other Savings generated', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_ppo_savings', 'PPO Savings', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_paid_amt', 'Amount paid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_pay_type', 'Fee for service vs Capitated (FFS or CAP)', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_check_num', 'Insurance check number', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_pre_authorization', 'Authorization Number from Insurance Company', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_adjudication_code', 'Adjudication code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_adjudication_desc', 'Adjudication description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_mrn', 'Patient Number issued by Provider', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_hicn', 'Health Insurance Claim Number to identify Medicare Patients', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_bill_type_code', 'Type of Bill.', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_bill_type_desc', 'Description out of master table for Bill type', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf1', 'User Defined Field 1', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf2', 'User Defined Field 2', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf3', 'User Defined Field 3', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf4', 'User Defined Field 4', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf5', 'User Defined Field 5', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf6', 'User Defined Field 6', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf7', 'User Defined Field 7', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf8', 'User Defined Field 8', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf9', 'User Defined Field 9', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf10', 'User Defined Field 10', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf11', 'User Defined Field 11', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf12', 'User Defined Field 12', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf13', 'User Defined Field 13', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf14', 'User Defined Field 14', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf15', 'User Defined Field 15', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf16', 'User Defined Field 16', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf17', 'User Defined Field 17', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf18', 'User Defined Field 18', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf19', 'User Defined Field 19', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf20', 'User Defined Field 20', @tableSchema='deerwalk'



























































































exec db.ColumnPropertySet 'MedicalClaims', 'visit_id', 'To map with visit table (dw_record_id)', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'is_makalu_used', 'Boolean Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf21', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf22', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf23', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf24', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf25', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf26', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf27', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf28', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf29', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf30', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf31', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf32', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf33', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf34', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf35', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf36', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf37', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf38', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf39', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf40', 'User Defined Field', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_account_id', 'Account id', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_client_id', 'Clientid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_ssn', 'Member SSN', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_first_name', 'Member first name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_middle_name', 'Member middle name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_last_name', 'Member last name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_current_status', ' Current status of member', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_gender', 'Member gender', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_dob', 'Member date of Birth', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_street_1', 'Member Street Address 1', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_street_2', 'Member Street Address 2', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_city', 'Member City', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_county', 'Member County', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_state', 'Abbreviation of State', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_zip', 'Zip code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_phone', 'Member Phone', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_region_code', 'Member Region code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_region_name', 'Member Region', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_relationship_code', 'Relationship Code to the Subscriber; subscriber(01), spouse (02),child (03), other (04)', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_relationship_desc', 'Relationship Description to the Subscriber, Dependent, Spouse', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_rawfilename', 'Filename from vendor', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_recievedmonth', 'Month when data is recieved', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_vendor_name', 'Data Vendor Name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'dw_account_id', 'Account id', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'dw_client_id', 'Clientid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'mbr_visit_type', 'Where the visit was made', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'mbr_start_date', 'Date when the visit started', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'mbr_end_date', 'Date when the visit ended', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'value', 'Units of the visit', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'admission_type', 'Admission type', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'ip_days', 'Inpatient days', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'admission_from_er', 'Yes or no on admissions from ER (Options : Y/N)', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'dw_account_id', 'Account id', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'dw_client_id', 'Clientid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MemberPCP', 'pcp_npi', 'May be null', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'start_date', 'May be null', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'end_date', 'May be null', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'dw_account_id', 'Account id', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'dw_client_id', 'Clientid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'score_type', 'Score scope ', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'score_start_date', 'Risk calculation start date', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'score_end_date', 'Risk calculation end  date', @tableSchema='deerwalk'




exec db.ColumnPropertySet 'Scores', 'med_score', 'IP+OP+PHY', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'total_score', 'Med+Rx', @tableSchema='deerwalk'











exec db.ColumnPropertySet 'HistoricalScores', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'dw_account_id', 'Account id', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'dw_client_id', 'Clientid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'score_type', 'Score scope ', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'score_start_date', 'Risk calculation start date', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'score_end_date', 'Risk calculation end  date', @tableSchema='deerwalk'




exec db.ColumnPropertySet 'HistoricalScores', 'med_score', 'IP+OP+PHY', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'total_score', 'Med+Rx', @tableSchema='deerwalk'











exec db.ColumnPropertySet 'Participation', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'dw_account_id', 'Account id', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'dw_client_id', 'Clientid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_type', 'Type of Program for participation', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_code', 'Code to identify program', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_name', 'Name of the program', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_status', 'Current status of the program', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_start_date', 'Program start date', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_end_date', 'Program end  date', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'












exec db.ColumnPropertySet 'HighCostDiagnosis', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'dw_account_id', 'Account id', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'dw_client_id', 'Clientid', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'Diagnosis_code', 'Diagnosis Code', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'Paid_Amount', 'Paid Amount', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'SuperGrouperDescription', 'Infections', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'GrouperDescription', 'Tuberculosis', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'dw_record_id', 'Auto-increment number-a unique identifier for Makalu engine', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'dw_member_id', 'Member ID', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'mbr_id', 'Member ID to display on the application, as sent by client', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'first_name', 'Member first name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'last_name', 'Member last name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'middle_name', 'Member middle name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'mbr_dob', 'Member date of birth', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'CareAlerts', 'mbr_status', 'Active or Termed', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'mbr_relationship', 'Relationship', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'pcp_full_name', 'PCP name', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'mbr_age', 'Age of member', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'mbr_months', 'Member Months', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'care_alert_startDate', 'Care Alert Date', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'care_alert_id', 'Care Alert Id', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'care_alert_desc', 'Care Alert Description', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'metric_Type', 'Metric type', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'metric_name', 'Metric Name', @tableSchema='deerwalk'




exec db.ColumnPropertySet 'Eligibility', 'mbr_id', '9916897', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Eligibility', 'mbr_ssn', '811619', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_first_name', 'BEVERLY', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_middle_name', 'George', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_last_name', 'BARRETT', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_current_status', 'active', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_gender', 'M', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_dob', '31597', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_street_1', '5621 TEAKWOOD ROAD', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Eligibility', 'mbr_city', 'Lakeworth', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_county', 'Lexington', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_state', 'FL', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_zip', '34746', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'mbr_phone', '7802966511', @propertyName='SampleData', @tableSchema='deerwalk'



exec db.ColumnPropertySet 'Eligibility', 'mbr_relationship_desc', 'Dependent', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_plan_type_code', 'com', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_plan_type_desc', 'Commercial', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_carrier_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_carrier_name', 'Harry TPA', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_coverage_type_code', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_coverage_type_desc', 'Family', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_plan_id', 'M720000-M', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_plan_desc', 'Family', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_emp_group_id', '3198508', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_emp_group_name', 'Deerwalk', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'Eligibility', 'ins_cobra_code', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_cobra_desc', 'Working', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_med_eff_date', '39814', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_med_term_date', '30/09/2011', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_rx_eff_date', '39819', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_rx_term_date', '30/06/2011', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_den_eff_date', '39821', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_den_term_date', '40550', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_vis_eff_date', '39821', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_vis_term_date', '40550', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_ltd_eff_date', '39821', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_ltd_term_date', '40550', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_std_eff_date', '39821', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'ins_std_term_date', '40550', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_id', '5687456598', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_first_name', 'Ashay', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_middle_name', 'Kumar', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_last_name', 'Thakur', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'prv_pcp_site_id', '120', @propertyName='SampleData', @tableSchema='deerwalk'




















exec db.ColumnPropertySet 'Eligibility', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'





















exec db.ColumnPropertySet 'Pharmacy', 'rev_transaction_num', '90272068301', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_plan_type_code', 'Commercial', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Pharmacy', 'mbr_id', '345677', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_first_name', 'BEVERLY', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_middle_name', 'George', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_last_name', 'BARRETT', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_gender', 'M', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_dob', '31597', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_city', 'Lakeworth', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_county', 'Lexington', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'mbr_zip', '34746', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Pharmacy', 'mbr_relationship_desc', 'Dependent', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Pharmacy', 'ins_carrier_name', 'Walgreens', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_coverage_type_code', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_coverage_type_desc', 'Family', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_emp_group_id', '3198508', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_emp_group_name', 'Deerwalk', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'Pharmacy', 'ins_cobra_code', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'ins_cobra_desc', 'Working', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_id', '381882404-014', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_first_name', 'Sanket', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_middle_name', 'Lal', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_prescriber_last_name', 'Shrestha', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'Pharmacy', 'prv_dea', 'CC15772', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'prv_npi', '5687456598', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Pharmacy', 'prv_type_code', 'I', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_written_date', '20/01/2011', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_filled_date', '40696', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_service_date', '40699', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_paid_date', '40700', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_ndc_code', '2416502', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_ndc_desc', 'Evista 60 Mg Tablet', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_rx_class_code', '77', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_rx_class_desc', 'Anticoagulants', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_drug_name', 'Warfarin Sodium', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Pharmacy', 'svc_drug_strength', '60 mg', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_unit_qty', '5', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_days_of_supply', '10', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'Pharmacy', 'svc_formulary_flag', 'Y', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_generic_flag', 'N', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'svc_mail_order_flag', 'Y', @propertyName='SampleData', @tableSchema='deerwalk'





exec db.ColumnPropertySet 'Pharmacy', 'rev_allowed_amt', '800', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_billed_amt', '500', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_coinsurance_amt', '10', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_copay_amt', '5', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_deductible_amt', '5', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_disp_fee_amt', '6', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_ingred_cost_amt', '10', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_stax_amt', '20', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_usual_cust_amt', '30', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_paid_amt', '400', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_adjudication_code', 'P', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'rev_adjudication_desc', 'Paid', @propertyName='SampleData', @tableSchema='deerwalk'




















exec db.ColumnPropertySet 'Pharmacy', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'is_makalu_used', 'True for Non-EM members  and False for EM members', @propertyName='SampleData', @tableSchema='deerwalk'





















exec db.ColumnPropertySet 'MedicalClaims', 'rev_claim_id', 'AAA6819', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_claim_line_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_claim_type', ' Prof', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_claim_type_flag', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_plan_type_code', 'com', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_plan_type_desc', 'Commercial', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_carrier_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_carrier_name', 'Harry TPA', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_coverage_type_code', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_coverage_type_desc', 'Family', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_plan_id', 'M720000-M', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_emp_group_id', '3198508', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_emp_group_name', 'Deerwalk', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'MedicalClaims', 'ins_cobra_code', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'ins_cobra_desc', 'Working', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_id', '345677', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_ssn', '811619', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_first_name', 'BEVERLY', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_middle_name', 'George', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_last_name', 'BARRETT', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_gender', 'M', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_dob', '31597', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_street_1', '5621 TEAKWOOD ROAD', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'mbr_city', 'Lakeworth', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_county', 'Lexington', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_state', 'FL', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_zip', '34746', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'mbr_phone', '7802966511', @propertyName='SampleData', @tableSchema='deerwalk'




exec db.ColumnPropertySet 'MedicalClaims', 'prv_service_provider_id', '772698', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_npi', '5687456598', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_tin', '381882404', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_type_desc', 'Institutional', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_first_name', 'Dilli', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_middle_name', 'Raj ', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_last_name', 'Ghimire', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_gender', 'M', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'prv_network_code', 'PPOM', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'prv_phone', '7802222334', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_speciality_1_code', '1054', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_Specialty_1_desc', 'Radiology', @propertyName='SampleData', @tableSchema='deerwalk'






exec db.ColumnPropertySet 'MedicalClaims', 'prv_city', 'Saginaw', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_county', 'Lexington', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_state', 'MA', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_zip', '2420', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'prv_in_network_flag', '0', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'prv_pcp_first_name', 'Meredith', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'prv_pcp_last_name', 'Gray', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_pos_code', '21', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_pos_desc', 'Inpatient', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_1_code', '272', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'svc_diag_2_code', '401.1', @propertyName='SampleData', @tableSchema='deerwalk'















exec db.ColumnPropertySet 'MedicalClaims', 'svc_procedure_type', 'HCPCS', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_procedure_code', 'G0107', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_procedure_desc', 'Fecal-Occult Blood Test', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_rev_code', 'R002', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_rev_desc', 'Total Charge', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_cpt_code', '100', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_cpt_desc', 'Anes-Salivary Glands InclBx', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_icd_proc_1_code', '9432', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_icd_proc_1_desc', 'Hypnotherapy', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'MedicalClaims', 'svc_drg_type_code', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_drg_type_Desc', 'MS-DRG, DRG', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_drg_code', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_drg_desc', 'HEART TRANSPLANT OR IMPLANT OF HEART ASSIST SYSTEM W MCC', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_hcpcs_code', 'G0107', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_hcpcs_desc', 'Fecal-Occult Blood Test', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_modifier_code', '90', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_modifier_desc', 'Lab send out', @propertyName='SampleData', @tableSchema='deerwalk'




exec db.ColumnPropertySet 'MedicalClaims', 'svc_tos_code', '85', @propertyName='SampleData', @tableSchema='deerwalk'




exec db.ColumnPropertySet 'MedicalClaims', 'svc_ip_days', '12', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_covered_days', '3', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'svc_service_frm_date', '39823', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_service_to_date', '40128', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_adjudication_date', '40211', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_paid_date', '40239', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_benefit_code', '105', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'svc_benefit_desc', 'Emergency and Urgent Care Services', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_allowed_amt', '180', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_billed_amt', '100', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_cob_paid_amt', '10', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_coinsurance_amt', '5', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_copay_amt', '5', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_coverage_charge_amt', '30', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_deductible_amt', '5', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_not_covered_amt', '30', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_other_savings', '10', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_ppo_savings', '10', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_paid_amt', '300', @propertyName='SampleData', @tableSchema='deerwalk'



exec db.ColumnPropertySet 'MedicalClaims', 'rev_adjudication_code', 'P', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'rev_adjudication_desc', 'Paid', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'MedicalClaims', 'rev_bill_type_code', '110', @propertyName='SampleData', @tableSchema='deerwalk'
















































































































exec db.ColumnPropertySet 'MedicalClaims', 'visit_id', '17', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'is_makalu_used', 'True for Non-EM members  and False for EM members', @propertyName='SampleData', @tableSchema='deerwalk'




















exec db.ColumnPropertySet 'Demographics', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_account_id', '1027', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_client_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_id', '9916897', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_ssn', '811619', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_first_name', 'BEVERLY', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_middle_name', 'George', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_last_name', 'BARRETT', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_current_status', 'active', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_gender', 'M', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_dob', '31597', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_street_1', '5621 TEAKWOOD ROAD', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Demographics', 'mbr_city', 'Lakeworth', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_county', 'Lexington', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_state', 'FL', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_zip', '34746', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Demographics', 'mbr_phone', '7802966511', @propertyName='SampleData', @tableSchema='deerwalk'



exec db.ColumnPropertySet 'Demographics', 'mbr_relationship_desc', 'Dependent', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Demographics', 'dw_recievedmonth', '201106', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Visits', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'dw_account_id', '1027', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'dw_client_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'mbr_id', '9916897', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'mbr_visit_type', 'ER,office etc.', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'Visits', 'value', '20', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Visits', 'admission_type', 'Maternity, Medical', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'Visits', 'admission_from_er', 'Y', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'dw_account_id', '1027', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'dw_client_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MemberPCP', 'mbr_id', '9916897', @propertyName='SampleData', @tableSchema='deerwalk'




exec db.ColumnPropertySet 'Scores', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'dw_account_id', '1027', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'dw_client_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'mbr_id', '9916897', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Scores', 'score_type', 'Group ID, ALL', @propertyName='SampleData', @tableSchema='deerwalk'



















exec db.ColumnPropertySet 'HistoricalScores', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'dw_account_id', '1027', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'dw_client_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'mbr_id', '9916897', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HistoricalScores', 'score_type', 'Group ID, ALL', @propertyName='SampleData', @tableSchema='deerwalk'



















exec db.ColumnPropertySet 'Participation', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'dw_account_id', '1027', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'dw_client_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'mbr_id', '9916897', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_type', 'Disease Management, Wellness ', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_code', 'C , A, PC', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_name', 'CAD, ASTHMA, Preventive Care', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Participation', 'program_status', 'Open, Ongoing, Closed', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'QualityMetrics', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'memberFirstName', 'Mark', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'memberLastName', 'Hinds', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'memberGender', 'F', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'memberDOB', 'YYYY-MM-DD', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'measureId', '123', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'measureDesc', '3 or more ER Visits in the last 6 months   ', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'PositiveNegative', '0=Negative metric; 1=Positive metric', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'measureName', 'Utilization', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'startDate', 'YYYY-MM-DD', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'EndDate', 'YYYY-MM-DD', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'numerator', '0', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'QualityMetrics', 'denomenator', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'dw_account_id', '1027', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'dw_client_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'mbr_id', '9916897', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'Diagnosis_code', '123.12', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'Paid_Amount', '123456.99', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'SuperGrouperDescription', 'Infections', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'HighCostDiagnosis', 'GrouperDescription', 'Infectious Diseases', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'dw_record_id', '1', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'dw_member_id', 'Hash Encrypted', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'mbr_id', '15435', @propertyName='SampleData', @tableSchema='deerwalk'



exec db.ColumnPropertySet 'CareAlerts', 'mbr_dob', 'yyyy-mm-dd', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'mbr_gender', 'Male, Female', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'CareAlerts', 'mbr_relationship', 'Employee, Dependent', @propertyName='SampleData', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'CareAlerts', 'mbr_age', '30', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'mbr_months', '11', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'care_alert_startDate', 'yyyy-mm-dd', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'CareAlerts', 'metric_Type', 'Positive Metric or Negative Metric', @propertyName='SampleData', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'CareAlerts', 'metric_name', 'Wellness, Hypertension', @propertyName='SampleData', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'Eligibility', 'udf1', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf2', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf3', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf4', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf5', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf6', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf7', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf8', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf9', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf10', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf11', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf12', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf13', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf14', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf15', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf16', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf17', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf18', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf19', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf20', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'Eligibility', 'udf21', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf22', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf23', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf24', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf25', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf26', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf27', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf28', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf29', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf30', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf31', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf32', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf33', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf34', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf35', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf36', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf37', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf38', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf39', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Eligibility', 'udf40', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'


exec db.ColumnPropertySet 'Pharmacy', 'udf1', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf2', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf3', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf4', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf5', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf6', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf7', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf8', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf9', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf10', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf11', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf12', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf13', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf14', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf15', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf16', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf17', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf18', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf19', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf20', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'



exec db.ColumnPropertySet 'Pharmacy', 'udf21', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf22', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf23', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf24', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf25', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf26', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf27', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf28', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf29', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf30', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf31', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf32', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf33', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf34', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf35', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf36', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf37', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf38', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf39', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'Pharmacy', 'udf40', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'udf1', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf2', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf3', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf4', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf5', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf6', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf7', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf8', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf9', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf10', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf11', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf12', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf13', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf14', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf15', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf16', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf17', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf18', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf19', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf20', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'

exec db.ColumnPropertySet 'MedicalClaims', 'udf21', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf22', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf23', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf24', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf25', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf26', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf27', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf28', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf29', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf30', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf31', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf32', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf33', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf34', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf35', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf36', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf37', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf38', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf39', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'
exec db.ColumnPropertySet 'MedicalClaims', 'udf40', 'UserDefinedData', @propertyName='CustomAttribute', @tableSchema='deerwalk'




create index IX_MbrId on [deerwalk].[CareAlerts] (TenantId, mbr_id)
create unique index UX_MbrId on [deerwalk].[Demographics] (TenantId, mbr_id)
create unique index UX_MbrId on [deerwalk].[Eligibility] (TenantId, mbr_id)
create unique index UX_DwMemberId on [deerwalk].[Eligibility] (TenantId, dw_member_id)
create unique index UX_MbrId on [deerwalk].[HighCostDiagnosis] (TenantId, mbr_id)
create index IX_MbrId on [deerwalk].[HistoricalScores] (TenantId, mbr_id)
create index IX_MbrId on [deerwalk].[MedicalClaims] (TenantId, mbr_id)
create unique index UX_MbrId on [deerwalk].[MemberPCP] (TenantId, mbr_id)
create index IX_MbrId on [deerwalk].[Participation] (TenantId, mbr_id)
create index IX_MbrId on [deerwalk].[Pharmacy] (TenantId, mbr_id)
create index IX_DwMemberId on [deerwalk].[QualityMetrics] (TenantId, dw_member_id)
create unique index UX_MbrId on [deerwalk].[Scores] (TenantId, mbr_id)
create index IX_MbrId on [deerwalk].[Visits] (TenantId, mbr_id)




alter table deerwalk.Eligibility add MbrDobDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'MbrDobDateDimId', 'mbr_dob', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsMedEffDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsMedEffDateDateDimId', 'ins_med_eff_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsMedTermDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsMedTermDateDateDimId', 'ins_med_term_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsRxEffDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsRxEffDateDateDimId', 'ins_rx_eff_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsRxTermDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsRxTermDateDateDimId', 'ins_rx_term_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsDenEffDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsDenEffDateDateDimId', 'ins_den_eff_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsDenTermDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsDenTermDateDateDimId', 'ins_den_term_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsVisEffDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsVisEffDateDateDimId', 'ins_vis_eff_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsVisTermDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsVisTermDateDateDimId', 'ins_vis_term_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsLtdEffDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsLtdEffDateDateDimId', 'ins_ltd_eff_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsLtdTermDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsLtdTermDateDateDimId', 'ins_ltd_term_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsStdEffDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsStdEffDateDateDimId', 'ins_std_eff_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Eligibility add InsStdTermDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Eligibility', 'InsStdTermDateDateDimId', 'ins_std_term_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Pharmacy add MbrDobDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Pharmacy', 'MbrDobDateDimId', 'mbr_dob', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Pharmacy add SvcWrittenDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Pharmacy', 'SvcWrittenDateDateDimId', 'svc_written_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Pharmacy add SvcFilledDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Pharmacy', 'SvcFilledDateDateDimId', 'svc_filled_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Pharmacy add SvcServiceDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Pharmacy', 'SvcServiceDateDateDimId', 'svc_service_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Pharmacy add RevPaidDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Pharmacy', 'RevPaidDateDateDimId', 'rev_paid_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MedicalClaims add MbrDobDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MedicalClaims', 'MbrDobDateDimId', 'mbr_dob', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MedicalClaims add SvcServiceFrmDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MedicalClaims', 'SvcServiceFrmDateDateDimId', 'svc_service_frm_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MedicalClaims add SvcServiceToDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MedicalClaims', 'SvcServiceToDateDateDimId', 'svc_service_to_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MedicalClaims add RevAdjudicationDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MedicalClaims', 'RevAdjudicationDateDateDimId', 'rev_adjudication_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MedicalClaims add RevPaidDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MedicalClaims', 'RevPaidDateDateDimId', 'rev_paid_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MedicalClaims add DwCreationDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MedicalClaims', 'DwCreationDateDateDimId', 'dw_creation_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MedicalClaims add DwUpdateDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MedicalClaims', 'DwUpdateDateDateDimId', 'dw_update_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Demographics add MbrDobDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Demographics', 'MbrDobDateDimId', 'mbr_dob', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Visits add MbrStartDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Visits', 'MbrStartDateDateDimId', 'mbr_start_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Visits add MbrEndDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Visits', 'MbrEndDateDateDimId', 'mbr_end_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MemberPCP add StartDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MemberPCP', 'StartDateDateDimId', 'start_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.MemberPCP add EndDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'MemberPCP', 'EndDateDateDimId', 'end_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Scores add ScoreStartDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Scores', 'ScoreStartDateDateDimId', 'score_start_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Scores add ScoreEndDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Scores', 'ScoreEndDateDateDimId', 'score_end_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.HistoricalScores add ScoreStartDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'HistoricalScores', 'ScoreStartDateDateDimId', 'score_start_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.HistoricalScores add ScoreEndDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'HistoricalScores', 'ScoreEndDateDateDimId', 'score_end_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Participation add ProgramStartDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Participation', 'ProgramStartDateDateDimId', 'program_start_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.Participation add ProgramEndDateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'Participation', 'ProgramEndDateDateDimId', 'program_end_date', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.QualityMetrics add MemberdobDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'QualityMetrics', 'MemberdobDateDimId', 'memberDOB', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.QualityMetrics add StartdateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'QualityMetrics', 'StartdateDateDimId', 'startDate', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.QualityMetrics add EnddateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'QualityMetrics', 'EnddateDateDimId', 'EndDate', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.CareAlerts add MbrDobDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'CareAlerts', 'MbrDobDateDimId', 'mbr_dob', @propertyName='BaseField', @tableSchema='deerwalk'
alter table deerwalk.CareAlerts add CareAlertStartdateDateDimId int null references DateDimensions(DateDimensionId);  exec db.ColumnPropertySet 'CareAlerts', 'CareAlertStartdateDateDimId', 'care_alert_startDate', @propertyName='BaseField', @tableSchema='deerwalk'

alter table deerwalk.CareAlerts add DateDimensionsLinked bit not null default 0
alter table deerwalk.Demographics add DateDimensionsLinked bit not null default 0
alter table deerwalk.Eligibility add DateDimensionsLinked bit not null default 0
alter table deerwalk.HighCostDiagnosis add DateDimensionsLinked bit not null default 0
alter table deerwalk.HistoricalScores add DateDimensionsLinked bit not null default 0
alter table deerwalk.MedicalClaims add DateDimensionsLinked bit not null default 0
alter table deerwalk.MemberPCP add DateDimensionsLinked bit not null default 0
alter table deerwalk.Participation add DateDimensionsLinked bit not null default 0
alter table deerwalk.Pharmacy add DateDimensionsLinked bit not null default 0
alter table deerwalk.QualityMetrics add DateDimensionsLinked bit not null default 0
alter table deerwalk.Scores add DateDimensionsLinked bit not null default 0
alter table deerwalk.Visits add DateDimensionsLinked bit not null default 0


