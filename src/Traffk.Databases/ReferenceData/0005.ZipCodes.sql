create schema ZipCodes

GO

create table ZipCodes.Zip5
(
	Zip5Id int not null identity primary key,
	ZipCode Char(5) not null,
	PrimaryRecord Char(1) not null,
	Population Integer not null,
	HouseholdsPerZipcode Integer not null,
	WhitePopulation Integer not null,
	BlackPopulation Integer not null,
	HispanicPopulation Integer not null,
	AsianPopulation Integer not null,
	HawaiianPopulation Integer not null,
	IndianPopulation Integer not null,
	OtherPopulation Integer not null,
	MalePopulation Integer not null,
	FemalePopulation Integer not null,
	PersonsPerHousehold Decimal(4, 2) not null,
	AverageHouseValue Integer not null,
	IncomePerHousehold Integer not null,
	MedianAge Decimal(3, 1) not null,
	MedianAgeMale Decimal(3, 1) not null,
	MedianAgeFemale Decimal(3, 1) not null,
	Latitude Decimal(12, 6) not null,
	Longitude Decimal(12, 6) not null,
	Elevation Integer not null,
	State Char(2) not null,
	StateFullName VarChar(35) not null,
	CityType Char(1) not null,
	CityAliasAbbreviation VarChar(13) not null,
	AreaCode VarChar(55) not null,
	City VarChar(35) not null,
	CityAliasName VarChar(35) not null,
	County VarChar(45) not null,
	CountyFIPS Char(5) not null,
	StateFIPS Char(2) not null,
	TimeZone Char(2) not null,
	DayLightSaving Char(1) not null,
	MSA VarChar(35) not null,
	PMSA Char(4) not null,
	CSA Char(3) not null,
	CBSA Char(5) not null,
	CBSA_DIV Char(5) not null,
	CBSA_Type Char(5) not null,
	CBSA_Name VarChar(150) not null,
	MSA_Name VarChar(150) not null,
	PMSA_Name VarChar(150) not null,
	Region VarChar(10) not null,
	Division VarChar(20) not null,
	MailingName Char(1) not null,
	NumberOfBusinesses Integer not null,
	NumberOfEmployees Integer not null,
	BusinessFirstQuarterPayroll Integer not null,
	BusinessAnnualPayroll Integer not null,
	BusinessEmpolymentFlag Char(1) not null,
	GrowthRank Integer not null, --was "Number"
	GrowingCountiesA Integer not null,
	GrowingCountiesB Integer not null,
	GrowthIncreaseNumber Integer not null,
	GrowthIncreasePercentage Decimal(3, 1) not null,
	CBSAPopulation Integer not null,
	CBSADivisionPopulation Integer not null,
	CongressionalDistrict VarChar(150) not null,
	CongressionalLandArea VarChar(150) not null,
	DeliveryResidential Integer not null,
	DeliveryBusiness Integer not null,
	DeliveryTotal Integer not null,
	PreferredLastLineKey VarChar(10) not null,
	ClassificationCode Char(1) not null,
	MultiCounty Char(1) not null,
	CSAName VarChar(255) not null,
	CBSA_Div_Name VarChar(255) not null,
	CityStateKey Char(6) not null,
	PopulationEstimate Integer not null,
	LandArea Decimal(12,6) not null,
	WaterArea Decimal(12,6) not null,
	CityAliasCode VarChar(5) not null,
	CityMixedCase VarChar(35) not null,
	CityAliasMixedCase VarChar(35) not null,
	BoxCount Integer not null,
	SFDU Integer not null,
	MFDU Integer not null,
	StateANSI VarChar(2) not null,
	CountyANSI VarChar(3) not null,
	--ZIPIntro VarChar(15) not null,
	ZIPIntroDate datetime null,
	--AliasIntroDate VarChar(15) not null,
	AliasIntroDate datetime null,
	FacilityCode VarChar(1) not null,
	CityDeliveryIndicator VarChar(1) not null,
	CarrierRouteRateSortation VarChar(1) not null,
	FinanceNumber VarChar(6) not null,
	UniqueZIPName VarChar(1) not null,
	SSAStateCountyCode VarChar(5) not null,
	MedicareCBSACode VarChar(5) not null,
	MedicareCBSAName VarChar(255) not null,
	MedicareCBSAType VarChar(5) not null,
	MarketRatingAreaID Integer not null,
	LocationPoint as geography::Point([Latitude], [Longitude], 4326) persisted
)

GO

create index ix_Zip5 on ZipCodes.Zip5(ZipCode)

GO

exec db.ColumnPropertySet 'Zip5', 'ZipCode', '00000-99999 Five digit numeric ZIP Code of the area.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'PrimaryRecord', 'Character ‘P’ denoting if this row is a Primary Record or not. Absence of character denotes a non-primary record.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'Population', 'The population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'HouseholdsPerZipcode', 'The estimated number of households of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'WhitePopulation', 'The estimated White/Caucasian Population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'BlackPopulation', 'The estimated Black/African American Population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'HispanicPopulation', 'The estimated Hispanic Population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'AsianPopulation', 'The estimated Asian Population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'HawaiianPopulation', 'The estimated Hawaiian or Pacific Islander Population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'IndianPopulation', 'The estimated American Indian and Alaska Native Population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'OtherPopulation', 'The estimated Population of all other races for the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MalePopulation', 'The estimated Male Population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'FemalePopulation', 'The estimated Female Population of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'PersonsPerHousehold', 'The estimated average number of persons per household of the ZIP Code based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'AverageHouseValue', 'The median house value of the county.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'IncomePerHousehold', 'The median household income.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MedianAge', 'The median age for all people based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MedianAgeMale', 'The median age for males based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MedianAgeFemale', 'The median age for females based on 2010 Census data.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'Latitude', 'Geographic coordinate as a point measured in degrees north or south of the equator.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'Longitude', 'Geographic coordinate as a point measured in degrees east or west of the Greenwich Meridian.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'Elevation', 'The average elevation of the county.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'State', '2 letter state name abbreviation.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'StateFullName', 'The full US State Name.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CityType', 'Indicates the type of locale such as Post Office, Stations, or Branch.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CityAliasAbbreviation', '13 Character abbreviation for the city alias name.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'AreaCode', 'The telephone area codes available in this ZIP Code.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'City', 'Name of the city as designated by the USPS.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CityAliasName', 'Alias name of the city if it exists.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'County', 'Name of County or Parish this ZIP Code resides in.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CountyFIPS', 'FIPS code for the County this ZIP Code resides in.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'StateFIPS', 'FIPS code for the State this ZIP Code resides in.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'TimeZone', 'Hours past Greenwich Time Zone this ZIP Code belongs to.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'DayLightSaving', 'Flag indicating whether this ZIP Code observes daylight savings.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MSA', 'Metropolitan Statistical Area number assigned by Census.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'PMSA', 'Primary Metropolitan Statistical Area Number.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CSA', 'Core Statistical Area. This area is a group of MSA''s combined into a population core area.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CBSA', 'Core Based Statistical Area Number.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CBSA_DIV', 'Core Based Statistical Area Division.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CBSA_Type', 'Core Based Statistical Area Type (Metro or Micro).', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CBSA_Name', 'Core Based Statistical Area Name or Title.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MSA_Name', 'Primary Metropolitan Statistical Area name or Title.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'PMSA_Name', 'Primary Metropolitan Statistical Area name or Title.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'Region', 'A geographic area consisting of several States defined by the U.S. Department of Commerce, Bureau of the Census. The States are grouped into four regions and nine divisions.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'Division', 'A geographic area consisting of several States defined by the U.S. Department of Commerce, Bureau of the Census. The States are grouped into four regions and nine divisions.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MailingName', 'Yes or No (Y/N) flag indicating whether or not the USPS accepts this City Alias Name for mailing purposes.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'NumberOfBusinesses', 'The total number of Business Establishments for this ZIP Code. Data taken from Census for 2015 Business Patterns.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'NumberOfEmployees', 'The total number of employees for this ZIP Code. Data taken from Census for 2015 Business Patterns.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'BusinessFirstQuarterPayroll', 'The total payroll for the first quarter this ZIP Code in $1000. Data taken from Census for 2015 Business Patterns.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'BusinessAnnualPayroll', 'The total annual payroll for this ZIP Code in $1000. Data taken from Census for 2015 Business Patterns.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'BusinessEmpolymentFlag', 'Due to confidentiality, some areas do not have actual figures for employment information. Employment and payroll data are replaced by zeroes with the Employment Flag denoting employment size class. ', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'GrowthRank', 'The rank in which this county is growing according to the US Census.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'GrowingCountiesA', 'The estimated number of housing units in this county as of July 1, 2010. Source: Population Division, U.S. Census Bureau.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'GrowingCountiesB', 'The estimated number of housing units in this county as of July 1, 2011. Source: Population Division, U.S. Census Bureau.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'GrowthIncreaseNumber', 'The change in housing units from 2003 to 2004, expressed as a number.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'GrowthIncreasePercentage', 'The change in housing units from 2003 to 2004, expressed as a percentage.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CBSAPopulation', 'The estimated population for the selected CBSA. ', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CBSADivisionPopulation', 'The estimated population for the selected CBSA Division.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CongressionalDistrict', 'This field shows which Congressional Districts the ZIP Code belongs to. Currently set for 115th Congress.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CongressionalLandArea', 'This field provides the approximate land area covered by the Congressional District for which the ZIP Code belongs to.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'DeliveryResidential', 'The number of active residential delivery mailboxes and centralized units for this ZIP Code. This excludes PO Boxes and all other contract box types. ', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'DeliveryBusiness', 'The number of active business delivery mailboxes and centralized units for this ZIP Code. This excludes PO Boxes and all other contract box types.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'DeliveryTotal', 'The total number of delivery receptacles for this ZIP Code. This includes curb side mailboxes, centralized units, PO Boxes, NDCBU, and contract boxes.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'PreferredLastLineKey', 'Links this record with other products ZIP-Codes.com offers.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'ClassificationCode', 'The classification type of this ZIP Code.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MultiCounty', 'Flag indicating whether this ZIP Code crosses county lines.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CSAName', 'The name of the CSA this ZIP Code is in.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CBSA_Div_Name', 'The name of the CBSA Division this ZIP Code is in.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CityStateKey', 'Links this record with other products ZIP-Codes.com offers such as the ZIP+4.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'PopulationEstimate', 'An up to the month population estimate for this ZIP Code.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'LandArea', 'The land area of this ZIP Code in Square Miles.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'WaterArea', 'The water area of this ZIP Code in Square Miles.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CityAliasCode', 'Code indication the type of the city alias name for this record. Record can be Abbreviations, Universities, Government, and more.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CityMixedCase', 'The city name in mixed case (i.e. Not in all uppercase letters).', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CityAliasMixedCase', 'The city alias name in mixed case (i.e. Not in all uppercase letters).', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'BoxCount', 'Total count of PO Box deliveries in this ZIP Code', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'SFDU', 'Total count of single family deliveries in this ZIP Code; generally analogous to homes', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MFDU', 'Total count of multifamily deliveries in this ZIP Code; generally analogous to apartments', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'StateANSI', 'ANSI code for the State this ZIP Code resides in.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CountyANSI', 'ANSI code for the County/Parish this ZIP Code resides in.', @tableSchema='ZipCodes'
--exec db.ColumnPropertySet 'Zip5', 'ZIPIntro', 'The date this ZIP Code was introduced into the database.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'ZIPIntroDate', 'The date this ZIP Code was introduced into the database.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'AliasIntroDate', 'The date this City Alias was introduced for this ZIP Code.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'FacilityCode', 'The type of locale identified in the city/state name.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CityDeliveryIndicator', 'Specifies whether or not a post office has city-delivery carrier routes.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'CarrierRouteRateSortation', 'Identifies where automation Carrier Route rates are available.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'FinanceNumber', 'A code assigned to Postal Service facilities.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'UniqueZIPName', 'Field that specifies whether the City State Record contains the organization name for a unique ZIP Code.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'SSAStateCountyCode', 'The State County Code as set by the Social Security Administration for Medicare and Medicaid.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MedicareCBSACode', 'The CBSA Code for this ZIP Code as set by the Centers for Medicare and Medicaid Services.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MedicareCBSAName', 'The CBSA Name  for this ZIP Code as set by the Centers for Medicare and Medicaid Services.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MedicareCBSAType', 'The CBSA type for this ZIP Code as set by the Centers for Medicare and Medicaid Services. Metro or Micro.', @tableSchema='ZipCodes'
exec db.ColumnPropertySet 'Zip5', 'MarketRatingAreaID', 'The ID of the Insurance Market Rating Area as set by the Centers for Medicare & Medicaid Services.', @tableSchema='ZipCodes'
