create schema CmsGov

GO

--https://www.cms.gov/Medicare/Provider-Enrollment-and-Certification/MedicareProviderSupEnroll/Downloads/TaxonomyCrosswalk.pdf
--put into Lookups
create table CmsGov.MedicareSpecialtyCodes
(
	MedicareSpecialtyCodeId int not null primary key,
	MedicareSpecialtyCode char(2) not null unique,
	MedicareSpecialtySupplierTypeDescription varchar(100)
)

GO

exec db.TablePropertySet  'MedicareSpecialtyCodes', '1', @propertyName='AddToDbContext', @tableSchema='CmsGov'
exec db.TablePropertySet  'MedicareSpecialtyCodes', '1', @propertyName='GeneratePoco', @tableSchema='CmsGov'
exec db.TablePropertySet  'MedicareSpecialtyCodes', 'IDontCreate', @propertyName='Implements', @tableSchema='CmsGov'

GO

insert into CmsGov.MedicareSpecialtyCodes
(MedicareSpecialtyCodeId, MedicareSpecialtyCode, MedicareSpecialtySupplierTypeDescription)
values
	(1, '01', 'Physician/General Practice'),
	(2, '02', 'Physician/General Surgery'),
	(3, '03', 'Physician/Allergy/ Immunology'),
	(4, '04', 'Physician/Otolaryngology'),
	(5, '05', 'Physician/Anesthesiology'),
	(6, '06', 'Physician/Cardiovascular Disease (Cardiology)'),
	(7, '07', 'Physician/Dermatology'),
	(8, '08', 'Physician/Family Practice'),
	(9, '09', 'Physician/Interventional Pain Management'),
	(10, '10', 'Physician/Gastroenterology'),
	(11, '11', 'Physician/Internal Medicine'),
	(12, '12', 'Physician/Osteopathic Manipulative Medicine'),
	(13, '13', 'Physician/Neurology'),
	(14, '14', 'Physician/Neurosurgery'),
	(15, '15', 'Speech Language Pathologist'),
	(16, '16', 'Physician/Obstetrics & Gynecology'),
	(17, '17', 'Physician/Hospice and Palliative Care'),
	(18, '18', 'Physician/Ophthalmology'),
	(19, '19', 'Oral Surgery (Dentist only)'),
	(20, '20', 'Physician/Orthopedic Surgery'),
	(22, '22', 'Physician/Pathology'),
	(23, '23', 'Physician/Sports Medicine'),
	(24, '24', 'Physician/Plastic and Reconstructive Surgery'),
	(25, '25', 'Physician/Physical Medicine and Rehabilitation'),
	(26, '26', 'Physician/Psychiatry'),
	(27, '27', 'Physician/Geriatric Psychiatry'),
	(28, '28', 'Physician/Colorectal Surgery (Proctology)'),
	(29, '29', 'Physician/Pulmonary Disease'),
	(30, '30', 'Physician/Diagnostic Radiology'),
	(32, '32', 'Anesthesiology Assistant'),
	(33, '33', 'Physician/Thoracic Surgery'),
	(34, '34', 'Physician/Urology'),
	(35, '35', 'Chiropractic'),
	(36, '36', 'Physician/Nuclear Medicine'),
	(37, '37', 'Physician/Pediatric Medicine'),
	(38, '38', 'Physician/Geriatric Medicine'),
	(39, '39', 'Physician/Nephrology'),
	(40, '40', 'Physician/Hand Surgery'),
	(41, '41', 'Optometry'),
	(42, '42', 'Certified Nurse Midwife'),
	(43, '43', 'Certified Registered Nurse Anesthetist (CRNA)'),
	(44, '44', 'Physician/Infectious Disease'),
	(45, '45', 'Mammography Center'),
	(46, '46', 'Physician/Endocrinology'),
	(47, '47', 'Independent Diagnostic Testing Facility (IDTF)'),
	(48, '48', 'Podiatry'),
	(49, '49', 'Ambulatory Surgical Center'),
	(50, '50', 'Nurse Practitioner'),
	(51, '51', 'Medical Supply Company with Orthotist'),
	(52, '52', 'Medical Supply Company with Prosthetist'),
	(53, '53', 'Medical Supply Company with Orthotist-Prosthetist'),
	(54, '54', 'Other Medical Supply Company'),
	(55, '55', 'Individual Certified Orthotist'),
	(56, '56', 'Individual Certified Prosthetist'),
	(57, '57', 'Individual Certified Prosthetist-Orthotist'),
	(58, '58', 'Medical Supply Company with Pharmacist'),
	(59, '59', 'Ambulance Service Provider'),
	(60, '60', 'Public Health or Welfare Agency'),
	(61, '61', 'Voluntary Health or Charitable Agency[1]'),
	(62, '62', 'Psychologist, Clinical'),
	(63, '63', 'Portable X-Ray Supplier'),
	(64, '64', 'Audiologist'),
	(65, '65', 'Physical Therapist in Private Practice'),
	(66, '66', 'Physician/Rheumatology'),
	(67, '67', 'Occupational Therapist in Private Practice'),
	(68, '68', 'Psychologist, Clinical'),
	(69, '69', 'Clinical Laboratory'),
	(70, '70', 'Clinic or Group Practice'),
	(71, '71', 'Registered Dietitian or Nutrition Professional'),
	(72, '72', 'Physician/Pain Management'),
	(73, '73', 'Mass Immunizer Roster Biller[2]'),
	(74, '74', 'Radiation Therapy Center'),
	(75, '75', 'Slide Preparation Facility'),
	(76, '76', 'Physician/Peripheral Vascular Disease'),
	(77, '77', 'Physician/Vascular Surgery'),
	(78, '78', 'Physician/Cardiac Surgery'),
	(79, '79', 'Physician/Addiction Medicine'),
	(80, '80', 'Licensed Clinical Social Worker'),
	(81, '81', 'Physician/Critical Care (Intensivists)'),
	(82, '82', 'Physician/Hematology'),
	(83, '83', 'Physician/Hematology-Oncology'),
	(84, '84', 'Physician/Preventive Medicine'),
	(85, '85', 'Physician/Maxillofacial Surgery'),
	(86, '86', 'Physician/Neuropsychiatry'),
	(87, '87', 'All Other Suppliers'),
	(88, '88', 'Unknown Supplier/Provider Specialty[4]'),
	(89, '89', 'Certified Clinical Nurse Specialist'),
	(90, '90', 'Physician/Medical Oncology'),
	(91, '91', 'Physician/Surgical Oncology'),
	(92, '92', 'Physician/Radiation Oncology'),
	(93, '93', 'Physician/Emergency Medicine'),
	(94, '94', 'Physician/Interventional Radiology'),
	(95, '95', 'Advance Diagnostic Imaging'),
	(96, '96', 'Optician'),
	(97, '97', 'Physician Assistant'),
	(98, '98', 'Physician/Gynecological Oncology'),
	(99, '99', 'Physician/Undefined Physician type[6]'),
	(160, 'A0', 'Hospital-General'),
	(161, 'A1', 'Skilled Nursing Facility'),
	(162, 'A2', 'Intermediate Care Nursing Facility'),
	(163, 'A3', 'Other Nursing Facility'),
	(164, 'A4', 'Home Health Agency'),
	(165, 'A5', 'Pharmacy'),
	(166, 'A6', 'Medical Supply Company with Respiratory Therapist'),
	(167, 'A7', 'Department Store'),
	(168, 'A8', 'Grocery Store'),
	(169, 'A9', 'Indian Health Service facility[13]'),
	(177, 'B1', 'Oxygen supplier'),
	(178, 'B2', 'Pedorthic personnel'),
	(179, 'B3', 'Medical supply company with pedorthic personnel'),
	(180, 'B4', 'Rehabilitation Agency'),
	(192, 'C0', 'Physician, Sleep Medicine'),
	(195, 'C3', 'Interventional Cardiology');


GO

create table CmsGov.HealthCareProviderTaxonomyCodeCrosswalk
(
	HealthCareProviderTaxonomyCodeCrosswalkId int not null identity primary key,
	MedicareSpecialtyCodeId int not null references CmsGov.MedicareSpecialtyCodes(MedicareSpecialtyCodeId),
	HealthCareProviderTaxonomyCode char(10) not null
)

GO

	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208D00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='01';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208600000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086H0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0120X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0122X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0105X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0102X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086X0206X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0127X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0129X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208G00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '204F00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208C00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207T00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '204E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207X00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XS0114X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XX0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XS0106X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XS0117X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XX0801X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XP3100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XX0005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208200000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2082S0099X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2082S0105X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207WX0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='02';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207K00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='03';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207KA0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='03';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207KI0005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='03';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207Y00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='04';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207YS0123X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='04';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207YX0602X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='04';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207YX0905X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='04';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207YX0901X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='04';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207YP0228X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='04';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207YX0007X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='04';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207YS0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='04';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207L00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='05';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207LA0401X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='05';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207LC0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='05';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207LH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='05';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207LP2900X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='05';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207LP3000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='05';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RC0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='06';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207N00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='07';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207NI0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='07';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ND0101X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='07';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ND0900X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='07';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207NP0225X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='07';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207NS0135X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='07';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207Q00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QA0401X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QA0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QA0505X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QB0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QG0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QS0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QS1201X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='08';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208VP0014X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='09';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RG0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='10';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207R00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RA0401X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RA0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RA0201X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RB0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RC0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RI0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RC0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RC0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RE0101X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RG0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RG0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RH0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RH0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RI0008X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RI0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RM1200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RX0202X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RN0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RP1001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RR0500X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RS0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RS0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RT0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='11';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '204D00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='12';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '204C00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='12';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084N0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='13';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084N0402X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='13';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084A2900X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='13';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207T00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='14';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '235Z00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='15';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207V00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VB0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VC0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VF0040X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VX0201X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VG0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VM0101X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VX0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VE0102X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='16';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086H0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='17';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='17';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207LH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='17';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='17';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='17';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084H0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='17';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207PH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='17';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207W00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='18';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '1223S0112X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='19';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207X00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='20';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XS0114X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='20';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XX0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='20';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XS0106X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='20';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XS0117X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='20';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XX0801X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='20';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XP3100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='20';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XX0005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='20';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZP0101X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZP0102X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZB0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZP0104X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZC0006X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZP0105X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZC0500X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZD0900X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZF0201X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZH0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZI0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZM0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZP0007X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZN0500X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207ZP0213X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='22';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207XX0005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207PS0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QS0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RS0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '204C00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080S0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2081P0301X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2081S0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2083S0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084S0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='23';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208200000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='24';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2082S0099X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='24';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2082S0105X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='24';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208100000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='25';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2081H0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='25';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2081N0008X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='25';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2081P2900X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='25';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2081P0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='25';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2081P0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='25';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2081S0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='25';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P0800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='26';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P0301X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='26';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QG0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='27';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208C00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='28';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RP1001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='29';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2085R0202X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='30';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '367H00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='32';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208G00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='33';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208800000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='34';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2088P0231X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='34';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2088F0040X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='34';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111N00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NI0013X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NI0900X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NN0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NN1001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NX0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NX0800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NP0017X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NR0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NR0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NS0005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '111NT0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='35';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207U00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='36';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207UN0903X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='36';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207UN0901X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='36';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207UN0902X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='36';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208000000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080A0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080I0007X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0006X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080H0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080T0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080N0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0008X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0201X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0202X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0203X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0204X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0205X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0206X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0207X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0208X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0210X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0214X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080P0216X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080T0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080S0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080S0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='37';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RG0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='38';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QG0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='38';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RN0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='39';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0105X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='40';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2082S0105X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='40';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '152W00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='41';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '152WC0802X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='41';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '152WL0500X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='41';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '152WX0102X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='41';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '152WP0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='41';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '152WS0006X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='41';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '152WV0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='41';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '367A00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='42';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '367500000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='43';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RI0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='44';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QR0208X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='45';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QR0207X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='45';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RE0101X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='46';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '293D00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='47';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '213E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='48';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '213ES0103X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='48';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '213ES0131X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='48';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '213EG0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='48';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '213EP1101X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='48';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '213EP0504X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='48';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '213ER0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='48';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '213ES0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='48';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QA1903X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='49';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363L00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LA2100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LA2200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LC1500X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LC0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LF0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LG0600X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LN0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LN0005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LX0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LX0106X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LP0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LP0222X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LP1700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LP2300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LP0808X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LS0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363LW0102X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='50';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '335E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='51';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '335E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='52';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '335E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='53';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332B00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='54';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '222Z00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='55';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '224P00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='56';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '222Z00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='57';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '224P00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='57';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332B00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '333600000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336C0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336C0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336C0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336H0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336I0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336L0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336M0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336M0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336N0007X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336S0011X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='58';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '341600000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='59';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3416A0800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='59';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3416L0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='59';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3416S0300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='59';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '251K00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='60';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '251V00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='61';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103T00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TA0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TA0700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TC0700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TC2200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TB0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TC1900X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TE1000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TE1100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TF0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TF0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TP2701X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TH0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TH0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TM1700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TM1800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TP0016X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TP0814X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TP2700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TR0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TS0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TW0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '106E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '106S00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='62';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '335V00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='63';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '231H00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='64';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '231HA2400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='64';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225100000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251C2600X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251E1300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251E1200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251G0304X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251H1200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251H1300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251N0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251X0800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251P0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2251S0007X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='65';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RR0500X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='66';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225X00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XR0403X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XE0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XE1200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XF0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XG0600X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XH1200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XH1300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XL0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XM0800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XN1300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XP0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '225XP0019X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='67';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '103TC0700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='68';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '291U00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='69';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QM1300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='70';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QP2000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='70';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '193200000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='70';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '193400000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='70';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '133V00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='71';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '133VN1006X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='71';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '133VN1004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='71';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '133VN1005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='71';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208VP0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='72';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QR0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='74';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '247200000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='75';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0129X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='76';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086S0129X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='77';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208G00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='78';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207L00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='79';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QA0401X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='79';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RA0401X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='79';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084A0401X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='79';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '1041C0700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='80';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RC0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='81';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RH0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='82';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RH0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='83';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2083A0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='84';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2083T0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='84';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2083X0100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='84';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2083P0500X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='84';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2083P0901X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='84';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2083S0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='84';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2083P0011X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='84';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '204E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='85';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084A0401X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P0802X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084B0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P0804X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084N0600X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084D0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084F0202X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P0805X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084H0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P0005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084N0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084N0402X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084N0008X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P2900X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P0800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084P0015X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084S0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084S0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084V0102X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='86';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332B00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='87';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364S00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SA2100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SA2200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SC2300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SC1501X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SC0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SE0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SE1400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SF0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SG0600X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SH1100X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SH0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SI0800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SL0600X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SM0705X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SN0000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SN0800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SX0106X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SX0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SX0204X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP1700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP2800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP0808X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP0809X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP0807X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP0810X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP0811X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP0812X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SP0813X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SR0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SS0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364ST0500X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '364SW0102X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='89';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RX0202X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='90';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2086X0206X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='91';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2085R0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='92';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207P00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='93';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207PE0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='93';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207PH0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='93';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207PT0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='93';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207PP0204X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='93';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207PS0010X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='93';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207PE0005X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='93';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2085R0204X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='94';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2085H0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='94';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '156FX1800X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='96';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363A00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='97';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363AM0700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='97';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '363AS0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='97';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207VX0201X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='98';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '208D00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='99';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '282N00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '282N00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '282NC2000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '282E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '283Q00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '283X00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '282N00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	--insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2843000000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '275N00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '273R00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '273Y00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '284300000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '282NC0060X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '314000000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A1';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332B00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A2';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '313M00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A3';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '251E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '251E00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '333600000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336C0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336C0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336C0004X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336H0001X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336I0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336L0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336M0002X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336M0003X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336N0007X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '3336S0011X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '1835C0205X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '1835P0200X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A5';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332B00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A6';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332B00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A7';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332B00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='A8';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332BX2000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B1';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '222Z00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B2';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '224P00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B2';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '332B00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B3';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QR0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '335U00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QM0801X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QR0401X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QE0700X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QF0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '251G00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '291U00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '291900000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QR0400X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '282J00000X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '261QR1300X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='B4';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207QS1201X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='C0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RS0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='C0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207YS0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='C0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2080S0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='C0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '2084S0012X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='C0';
	insert into CmsGov.HealthCareProviderTaxonomyCodeCrosswalk select MedicareSpecialtyCodeId, '207RI0011X' from CmsGov.MedicareSpecialtyCodes where MedicareSpecialtyCode='C3';

GO

/*
https://www.cms.gov/Medicare/Coding/HCPCSReleaseCodeSets/Alpha-Numeric-HCPCS-Items/2017-Alpha-Numeric-HCPCS-File.html
https://www.cms.gov/Medicare/Coding/HCPCSReleaseCodeSets/Alpha-Numeric-HCPCS-Items/2017-Record-Layout.html
HCPCS17_recordlayout.txt
*/

create table CmsGov.PricingIndicatorCodes
(
	PricingIndicatorCodeId int not null primary key,
	PricingIndicatorCode char(2) not null unique,
	Header varchar(100),
	Description varchar(255)
)

GO

insert into CmsGov.PricingIndicatorCodes
values
(0,  '00', null, 'Service not separately priced by part B (e.G., services not covered, bundled, used by part a only, etc.)'),
(11, '11', 'Physician Fee Schedule', 'Price established using national rvu''s'),
(12, '12', 'Physician Fee Schedule', 'Price established using national anesthesia base units'),
(13, '13', 'Physician Fee Schedule', 'Price established by carriers (e.G., not otherwise classified, individual determination, carrier discretion)'),
(21, '21', 'Clinical Lab Fee Schedule', 'Price subject to national limitation amount'),
(22, '22', 'Clinical Lab Fee Schedule', 'Price established by carriers (e.G., gap-fills, carrier established panels)'),
(31, '31', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Frequently serviced DME (price subject to floors and ceilings)'),
(32, '32', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Inexpensive & routinely purchased DME (price subject to floors and ceilings)'),
(33, '33', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Oxygen and oxygen equipment (price subject to floors and ceilings)'),
(34, '34', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'DME supplies (price subject to floors and ceilings)'),
(35, '35', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Surgical dressings (price subject to floors and ceilings)'),
(36, '36', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Capped rental DME (price subject to floors and ceilings)'),
(37, '37', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Ostomy, tracheostomy and urological supplies (price subject to floors and ceilings)'),
(38, '38', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Orthotics, prosthetics, prosthetic devices & vision services (price subject to floors and ceilings)'),
(39, '39', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Parenteral and Enteral Nutrition'),
(45, '45', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Customized DME items'),
(46, '46', 'Durable Medical Equipment, Prosthetics, Orthotics, Supplies And Surgical Dressings', 'Carrier priced (e.g., not otherwise classified, individual determination, carrier discretion, gap-filled  amounts)'),
(51, '51', 'Other', 'Drugs'),
(52, '52', 'Other', 'Reasonable charge'),
(53, '53', 'Other', 'Statute'),
(54, '54', 'Other', 'Vaccinations'),
(55, '55', 'Other', 'Splints and Casts (effec 10/1/2014)'),
(56, '56', 'Other', 'IOL''s inserted in a physician''s office (eff 10/1/2014)'),
(57, '57', 'Other', 'Other carrier priced'),
(99, '99', 'Other', 'Value not established')

GO

create table CmsGov.LabCertificationCodes
(
	LabCertificationCodeId int not null primary key,
	ParentLabCertificationCodeId int null references CmsGov.LabCertificationCodes(LabCertificationCodeId),
	LabCertificationCode char(3) not null unique,
	SpecialtyCertificationCategory varchar(100) not null
)

GO

insert into CmsGov.LabCertificationCodes
values
(010, null, '010', 'Histocompatibility testing'),
(100, null, '100', 'Microbiology'),
(110, 100, '110', 'Bacteriology'),
(115, 100, '115', 'Mycobacteriology'),
(120, 100, '120', 'Mycology'),
(130, 100, '130', 'Parasitology'),
(140, 100, '140', 'Virology'),
(150, 100, '150', 'Other microbiology'),
(200, null, '200', 'Diagnostic immunology'),
(210, 200, '210', 'Syphilis serology'),
(220, 200, '220', 'General immunology'),
(300, null, '300', 'Chemistry'),
(310, 300, '310', 'Routine chemistry'),
(320, 300, '320', 'Urinalysis'),
(330, 300, '330', 'Endocrinology'),
(340, 300, '340', 'Toxicology'),
(350, 300, '350', 'Other chemistry'),
(400, null, '400', 'Hematology'),
(500, null, '500', 'Immunohematology'),
(510, 500, '510', 'Abo group & RH type'),
(520, 500, '520', 'Antibody detection (transfusion)'),
(530, 500, '530', 'Antibody detection (nontransfusion)'),
(540, 500, '540', 'Antibody identification'),
(550, 500, '550', 'Compatibility testing'),
(560, null, '560', 'Other immunohematology'),
(600, null, '600', 'Pathology'),
(610, 600, '610', 'Histopathology'),
(620, 600, '620', 'Oral pathology'),
(630, 600, '630', 'Cytology'),
(800, null, '800', 'Radiobioassay'),
(900, null, '900', 'Clinical cytogenetics')

GO

create table CmsGov.MedicareOutpatientGroupsPaymentGroupCodes
(
	MogId int not null primary key,
	MogCode char(3) not null unique,
	MogCategory varchar(100),
	MogTitle varchar(100) not null
)

GO

insert into CmsGov.MedicareOutpatientGroupsPaymentGroupCodes
values
(000, '000', 'No MOG applies', 'No MOG applies'),
(102, '102', 'Integumentary', 'Level II needle biopsy/aspiration'),
(112, '112', 'Integumentary', 'Level II incision and drainage'),
(132, '132', 'Integumentary', 'Level II debridement/destruction'),
(142, '142', 'Integumentary', 'Level II excision/biopsy'),
(143, '143', 'Integumentary', 'Level III excision/biopsy'),
(151, '151', 'Integumentary', 'Level I skin repair'),
(152, '152', 'Integumentary', 'Level II skin repair'),
(153, '153', 'Integumentary', 'Level III skin repair'),
(160, '160', 'Integumentary', 'Incision/excision breast'),
(169, '169', 'Integumentary', 'Breast reconstruction/mastectomy'),
(201, '201', 'Musculoskeletal', 'Level I skull and facial bone procedures'),
(202, '202', 'Musculoskeletal', 'Level II skull and facial bone procedures'),
(211, '211', 'Musculoskeletal', 'Level I hand musculoskeletal procedures'),
(212, '212', 'Musculoskeletal', 'Level II hand musculoskeletal procedures'),
(221, '221', 'Musculoskeletal', 'Level I foot musculoskeletal procedures'),
(222, '222', 'Musculoskeletal', 'Level II foot musculoskeletal procedures'),
(231, '231', 'Musculoskeletal', 'Level I musculoskeletal procedures'),
(232, '232', 'Musculoskeletal', 'Level II musculoskeletal procedures'),
(233, '233', 'Musculoskeletal', 'Level III musculoskeletal procedures'),
(241, '241', 'Musculoskeletal', 'Level I arthroscopy'),
(242, '242', 'Musculoskeletal', 'Level II arthroscopy'),
(260, '260', 'Musculoskeletal', 'Closed treatment fracture finger/toe/trunk'),
(269, '269', 'Musculoskeletal', 'Closed treatment fracture/dislocation/except finger/toe/trunk'),
(270, '270', 'Musculoskeletal', 'Open/percutaneous treatment fracture or dislocation'),
(279, '279', 'Musculoskeletal', 'Bone/joint manipulation under anesthesia'),
(280, '280', 'Musculoskeletal', 'Bunion procedures'),
(289, '289', 'Musculoskeletal', 'Arthroplasty'),
(290, '290', 'Musculoskeletal', 'Arthroplasty with prosthesis'),
(302, '302', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Level II ENT procedures'),
(303, '303', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Level III ENT procedures'),
(304, '304', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Level IV ENT procedures'),
(309, '309', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Implantation of cochlear device (ASC rate doesnot include cost of implant)'),
(310, '310', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Nasal cauterization/packing'),
(319, '319', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Tonsil/adenoid procedures'),
(322, '322', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Level II endoscopy upper airway'),
(323, '323', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Level III endoscopy upper airway'),
(329, '329', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Endoscopy lower airway'),
(330, '330', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Thoracentesis/lavage procedures'),
(350, '350', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Placement transvenous caths/cutdown'),
(359, '359', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Removal/revision, pacemaker/vascular device'),
(360, '360', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Vascular ligation'),
(369, '369', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Vascular repair/fistula construction'),
(370, '370', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Lymph node excisions'),
(379, '379', 'ENT/Respiratory/Cardiovascular/Lymphatic/Endocrine', 'Thyroid/lymphadenectomy procedures'),
(400, '400', 'Digestive', 'Esophageal dilation without endoscopy'),
(410, '410', 'Digestive', 'Esophagoscopy'),
(421, '421', 'Digestive', 'Level I upper GI endoscopy/intubation'),
(422, '422', 'Digestive', 'Level II upper GI endoscopy/intubation'),
(429, '429', 'Digestive', 'Lower GI endoscopy'),
(430, '430', 'Digestive', 'Anoscopy and diagnostic sigmoidoscopy'),
(439, '439', 'Digestive', 'Therapeutic proctosigmoidoscopy'),
(440, '440', 'Digestive', 'Small intestine endoscopy'),
(449, '449', 'Digestive', 'Percutaneous biliary endoscopic procedures'),
(450, '450', 'Digestive', 'Endoscopic retrograde cholangio-pancreatography (ERCP)'),
(460, '460', 'Digestive', 'Hernia/hydrocele procedures'),
(472, '472', 'Digestive', 'Level II anal/rectal procedures'),
(473, '473', 'Digestive', 'Level III anal/rectal procedures'),
(480, '480', 'Digestive', 'Peritoneal and abdominal procedures'),
(490, '490', 'Digestive', 'Tube procedures'),
(501, '501', 'Urinary/Genital', 'Level I laparoscopy'),
(502, '502', 'Urinary/Genital', 'Level II laparoscopy'),
(509, '509', 'Urinary/Genital', 'Lithotripsy'),
(511, '511', 'Urinary/Genital', 'Level I cystourethroscopy and other genitourinary procedures'),
(512, '512', 'Urinary/Genital', 'Level II cystourethroscopy and other genitourinary procedures'),
(513, '513', 'Urinary/Genital', 'Level III cystourethroscopy and other genitourinary procedures'),
(521, '521', 'Urinary/Genital', 'Level I urethral procedures'),
(522, '522', 'Urinary/Genital', 'Level II urethral procedures'),
(530, '530', 'Urinary/Genital', 'Circumcision'),
(539, '539', 'Urinary/Genital', 'Penile procedures'),
(540, '540', 'Urinary/Genital', 'Insertion of penile prosthesis (ASC rate does include cost of implant)'),
(549, '549', 'Urinary/Genital', 'Testes/epididymis procedures'),
(550, '550', 'Urinary/Genital', 'Prostrate biopsy'),
(562, '562', 'Urinary/Genital', 'Level II female reproductive procedures'),
(563, '563', 'Urinary/Genital', 'Level III female reproductive procedures'),
(570, '570', 'Urinary/Genital', 'Surgical hysteroscopy'),
(579, '579', 'Urinary/Genital', 'D & C'),
(580, '580', 'Urinary/Genital', 'Spontaneous abortion'),
(589, '589', 'Urinary/Genital', 'Therapeutic abortion'),
(602, '602', 'Nervous/Eye', 'Level II nervous system injections'),
(609, '609', 'Nervous/Eye', 'Revision/removal neurological device'),
(610, '610', 'Nervous/Eye', 'Implantation of neurostimulator electrodes (ASC rate does not include cost of implant)'),
(619, '619', 'Nervous/Eye', 'Implantation of neurological devices (asc rate does not include cost of implant)'),
(621, '621', 'Nervous/Eye', 'Level I nerve procedures'),
(622, '622', 'Nervous/Eye', 'Level II nerve procedures'),
(629, '629', 'Nervous/Eye', 'Spinal tap'),
(639, '639', 'Nervous/Eye', 'Laser eye procedures except retinal'),
(640, '640', 'Nervous/Eye', 'Cataract procedures'),
(649, '649', 'Nervous/Eye', 'Cataract procedures with IOL insert (includes $150 insert)'),
(651, '651', 'Nervous/Eye', 'Level I anterior segment eye procedures'),
(652, '652', 'Nervous/Eye', 'Level II anterior segment eye procedures'),
(659, '659', 'Nervous/Eye', 'Corneal transplant (ASC rate includes price of transplant)'),
(660, '660', 'Nervous/Eye', 'Posterior segment eye procedures'),
(669, '669', 'Nervous/Eye', 'Strabismus/muscle procedures'),
(673, '673', 'Nervous/Eye', 'Level III eye procedure'),
(674, '674', 'Nervous/Eye', 'Level IV eye procedure'),
(680, '680', 'Nervous/Eye', 'Vitrectomy'),
(689, '689', 'Nervous/Eye', 'Implantation/replacement of intraitreal drug')

GO

create table CmsGov.HealthcareCommonProcedureCodingSystemCodes
(
	HcpcsId int not null identity primary key,
	RecordIdentificationCode char(11) not null unique,
	HcpcsCode char(5) not null,
	SequenceNumber int not null,
	IsPrimary bit not null default(0),
	ShortDescription varchar(255),
	LongDescription varchar(max),
	PricingIndicatorCodeId int null references CmsGov.PricingIndicatorCodes(PricingIndicatorCodeId),
	MogId int null references CmsGov.MedicareOutpatientGroupsPaymentGroupCodes(MogId),
	LabCertificationCodeId int null references CmsGov.LabCertificationCodes(LabCertificationCodeId),
)

GO

--http://www.cms.gov/Research-Statistics-Data-and-Systems/Statistics-Trends-and-Reports/MedicareFeeforSvcPartsAB/Downloads/BETOSDescCodes.pdf
create table CmsGov.BerensonEggersTypeOfServices
(
	BetosId int not null primary key,
	BetosCode varchar(5) not null unique,
	Category varchar(50) not null,
	SubCategory varchar(200) not null
)

GO

insert into CmsGov.BerensonEggersTypeOfServices
(BetosId, BetosCode, Category, SubCategory)
values
(1001, 'M1A', 'Evaluation and Management', 'OFFICE VISITS - NEW'),
(1002, 'M1B', 'Evaluation and Management', 'OFFICE VISITS - ESTABLISHED'),
(1003, 'M2A', 'Evaluation and Management', 'HOSPITAL VISIT - INITIAL'),
(1004, 'M2B', 'Evaluation and Management', 'HOSPITAL VISIT - SUBSEQUENT'),
(1005, 'M2C', 'Evaluation and Management', 'HOSPITAL VISIT - CRITICAL CARE'),
(1006, 'M3', 'Evaluation and Management', 'EMERGENCY ROOM VISIT'),
(1007, 'M4A', 'Evaluation and Management', 'HOME VISIT'),
(1008, 'M4B', 'Evaluation and Management', 'NURSING HOME VISIT'),
(1009, 'M5A', 'Evaluation and Management', 'SPECIALIST - PATHOLOGY'),
(1010, 'M5B', 'Evaluation and Management', 'SPECIALIST - PSYCHIATRY'),
(1011, 'M5C', 'Evaluation and Management', 'SPECIALIST - OPHTHALMOLOGY'),
(1012, 'M5D', 'Evaluation and Management', 'SPECIALIST - OTHER'),
(1013, 'M6', 'Evaluation and Management', 'CONSULTATIONS'),
(2001, 'P0', 'Procedures', 'ANESTHESIA'),
(2002, 'P1A', 'Procedures', 'MAJOR PROCEDURE - BREAST'),
(2003, 'P1B', 'Procedures', 'MAJOR PROCEDURE - COLECTOMY'),
(2004, 'P1C', 'Procedures', 'MAJOR PROCEDURE - CHOLECYSTECTOMY'),
(2005, 'P1D', 'Procedures', 'MAJOR PROCEDURE - TURP'),
(2006, 'P1E', 'Procedures', 'MAJOR PROCEDURE - HYSTERECTOMY'),
(2007, 'P1F', 'Procedures', 'MAJOR PROCEDURE - EXPLOR/DECOMPR/EXCISDISC'),
(2008, 'P1G', 'Procedures', 'MAJOR PROCEDURE - OTHER'),
(2009, 'P2A', 'Procedures', 'MAJOR PROCEDURE, CARDIOVASCULAR - CABG'),
(2010, 'P2B', 'Procedures', 'MAJOR PROCEDURE, CARDIOVASCULAR - ANEURYSM REPAIR'),
(2011, 'P2C', 'Procedures', 'MAJOR PROCEDURE, CARDIOVASCULAR - THROMBOENDARTERECTOMY'),
(2012, 'P2D', 'Procedures', 'MAJOR PROCEDURE, CARDIOVASCULAR - CORONARY ANGIOPLASTY(PTCA)'),
(2013, 'P2E', 'Procedures', 'MAJOR PROCEDURE, CARDIOVASCULAR - PACEMAKER INSERTION'),
(2014, 'P2F', 'Procedures', 'MAJOR PROCEDURE, CARDIOVASCULAR - OTHER'),
(2015, 'P3A', 'Procedures', 'MAJOR PROCEDURE, ORTHOPEDIC - HIP FRACTURE REPAIR'),
(2016, 'P3B', 'Procedures', 'MAJOR PROCEDURE, ORTHOPEDIC - HIP REPLACEMENT'),
(2017, 'P3C', 'Procedures', 'MAJOR PROCEDURE, ORTHOPEDIC - KNEE REPLACEMENT'),
(2018, 'P3D', 'Procedures', 'MAJOR PROCEDURE, ORTHOPEDIC - OTHER'),
(2019, 'P4A', 'Procedures', 'EYE PROCEDURE - CORNEAL TRANSPLANT'),
(2020, 'P4B', 'Procedures', 'EYE PROCEDURE - CATARACT REMOVAL/LENS INSERTION'),
(2021, 'P4C', 'Procedures', 'EYE PROCEDURE - RETINAL DETACHMENT'),
(2022, 'P4D', 'Procedures', 'EYE PROCEDURE - TREATMENT OF RETINAL LESIONS'),
(2023, 'P4E', 'Procedures', 'EYE PROCEDURE - OTHER'),
(2024, 'P5A', 'Procedures', 'AMBULATORY PROCEDURES - SKIN'),
(2025, 'P5B', 'Procedures', 'AMBULATORY PROCEDURES - MUSCULOSKELETAL'),
(2026, 'P5C', 'Procedures', 'AMBULATORY PROCEDURES - INGUINAL HERNIA REPAIR'),
(2027, 'P5D', 'Procedures', 'AMBULATORY PROCEDURES - LITHOTRIPSY'),
(2028, 'P5E', 'Procedures', 'AMBULATORY PROCEDURES - OTHER'),
(2029, 'P6A', 'Procedures', 'MINOR PROCEDURES - SKIN'),
(2030, 'P6B', 'Procedures', 'MINOR PROCEDURES - MUSCULOSKELETAL'),
(2031, 'P6C', 'Procedures', 'MINOR PROCEDURES - OTHER (MEDICARE FEE SCHEDULE)'),
(2032, 'P6D', 'Procedures', 'MINOR PROCEDURES - OTHER (NON-MEDICARE FEE SCHEDULE)'),
(2033, 'P7A', 'Procedures', 'ONCOLOGY - RADIATION THERAPY'),
(2034, 'P7B', 'Procedures', 'ONCOLOGY - OTHER'),
(2035, 'P8A', 'Procedures', 'ENDOSCOPY - ARTHROSCOPY'),
(2036, 'P8B', 'Procedures', 'ENDOSCOPY - UPPER GASTROINTESTINAL'),
(2037, 'P8C', 'Procedures', 'ENDOSCOPY - SIGMOIDOSCOPY'),
(2038, 'P8D', 'Procedures', 'ENDOSCOPY - COLONOSCOPY'),
(2039, 'P8E', 'Procedures', 'ENDOSCOPY - CYSTOSCOPY'),
(2040, 'P8F', 'Procedures', 'ENDOSCOPY - BRONCHOSCOPY'),
(2041, 'P8G', 'Procedures', 'ENDOSCOPY - LAPAROSCOPIC CHOLECYSTECTOMY'),
(2042, 'P8H', 'Procedures', 'ENDOSCOPY - LARYNGOSCOPY'),
(2043, 'P8I', 'Procedures', 'ENDOSCOPY - OTHER'),
(2044, 'P9A', 'Procedures', 'DIALYSIS SERVICES (MEDICARE FEE SCHEDULE)'),
(2045, 'P9B', 'Procedures', 'DIALYSIS SERVICES (NON-MEDICARE FEE SCHEDULE)'),
(3001, 'I1A', 'Imaging', 'STANDARD IMAGING - CHEST'),
(3002, 'I1B', 'Imaging', 'STANDARD IMAGING - MUSCULOSKELETAL'),
(3003, 'I1C', 'Imaging', 'STANDARD IMAGING - BREAST'),
(3004, 'I1D', 'Imaging', 'STANDARD IMAGING - CONTRAST GASTROINTESTINAL'),
(3005, 'I1E', 'Imaging', 'STANDARD IMAGING - NUCLEAR MEDICINE'),
(3006, 'I1F', 'Imaging', 'STANDARD IMAGING - OTHER'),
(3007, 'I2A', 'Imaging', 'ADVANCED IMAGING - CAT: HEAD'),
(3008, 'I2B', 'Imaging', 'ADVANCED IMAGING - CAT: OTHER'),
(3009, 'I2C', 'Imaging', 'ADVANCED IMAGING - MRI: BRAIN'),
(3010, 'I2D', 'Imaging', 'ADVANCED IMAGING - MRI: OTHER'),
(3011, 'I3A', 'Imaging', 'ECHOGRAPHY - EYE'),
(3012, 'I3B', 'Imaging', 'ECHOGRAPHY - ABDOMEN/PELVIS'),
(3013, 'I3C', 'Imaging', 'ECHOGRAPHY - HEART'),
(3014, 'I3D', 'Imaging', 'ECHOGRAPHY - CAROTID ARTERIES'),
(3015, 'I3E', 'Imaging', 'ECHOGRAPHY - PROSTATE, TRANSRECTAL'),
(3016, 'I3F', 'Imaging', 'ECHOGRAPHY - OTHER'),
(3017, 'I4A', 'Imaging', 'IMAGING/PROCEDURE - HEART,INCLUDING CARDIAC CATHETERIZATION'),
(3018, 'I4B', 'Imaging', 'IMAGING/PROCEDURE - OTHER'),
(4001, 'T1A', 'Tests', 'LAB TESTS - ROUTINE VENIPUNCTURE (NON MEDICARE FEE SCHEDULE)'),
(4002, 'T1B', 'Tests', 'LAB TESTS - AUTOMATED GENERAL PROFILES'),
(4003, 'T1C', 'Tests', 'LAB TESTS - URINALYSIS'),
(4004, 'T1D', 'Tests', 'LAB TESTS - BLOOD COUNTS'),
(4005, 'T1E', 'Tests', 'LAB TESTS - GLUCOSE'),
(4006, 'T1F', 'Tests', 'LAB TESTS - BACTERIAL CULTURES'),
(4007, 'T1G', 'Tests', 'LAB TESTS - OTHER (MEDICARE FEE SCHEDULE)'),
(4008, 'T1H', 'Tests', 'LAB TESTS - OTHER (NON-MEDICARE FEE SCHEDULE)'),
(4009, 'T2A', 'Tests', 'OTHER TESTS - ELECTROCARDIOGRAMS'),
(4010, 'T2B', 'Tests', 'OTHER TESTS - CARDIOVASCULAR STRESS TESTS'),
(4011, 'T2C', 'Tests', 'OTHER TESTS - EKG MONITORING'),
(4012, 'T2D', 'Tests', 'OTHER TESTS - OTHER'),
(5001, 'D1A', 'Durable Medical Equipment', 'MEDICAL/SURGICAL SUPPLIES'),
(5002, 'D1B', 'Durable Medical Equipment', 'HOSPITAL BEDS'),
(5003, 'D1C', 'Durable Medical Equipment', 'OXYGEN AND SUPPLIES'),
(5004, 'D1D', 'Durable Medical Equipment', 'WHEELCHAIRS'),
(5005, 'D1E', 'Durable Medical Equipment', 'OTHER DME'),
(5006, 'D1F', 'Durable Medical Equipment', 'ORTHOTIC DEVICES'),
(5007, 'D1G', 'Durable Medical Equipment', 'DRUGS ADMINISTERED THROUGH DME'),
(6001, 'O1A', 'Other', 'AMBULANCE'),
(6002, 'O1B', 'Other', 'CHIROPRACTIC'),
(6003, 'O1C', 'Other', 'ENTERAL AND PARENTERAL'),
(6004, 'O1D', 'Other', 'CHEMOTHERAPY'),
(6005, 'O1E', 'Other', 'OTHER DRUGS'),
(6006, 'O1F', 'Other', 'VISION, HEARING AND SPEECH SERVICES'),
(6007, 'O1G', 'Other', 'INFLUENZA IMMUNIZATION'),
(7001, 'Y1', 'Exceptions/Unclassified', 'OTHER - MEDICARE FEE SCHEDULE'),
(7002, 'Y2', 'Exceptions/Unclassified', 'OTHER - NON-MEDICARE FEE SCHEDULE'),
(7003, 'Z1', 'Exceptions/Unclassified', 'LOCAL CODES 4. Z2 UNDEFINED CODES');

GO

