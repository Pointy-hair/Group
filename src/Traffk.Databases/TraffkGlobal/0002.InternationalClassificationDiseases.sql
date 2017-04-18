create schema InternationalClassificationDiseases
GO
create table InternationalClassificationDiseases.ICD10
(
	Icd10Id int not null identity primary key,
	ParentIcd10Id int null references InternationalClassificationDiseases.ICD10(Icd10Id),
	DiagnosisCode varchar(16) not null unique,
	DiagnosisDescription nvarchar(255) not null
)
GO