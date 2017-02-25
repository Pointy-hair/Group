create schema NationalDrugCode

GO

create table NationalDrugCode.Labelers
(
	LabelerId int not null identity primary key,
	LabelerCode varchar(10) not null unique,
	LabelerName nvarchar(255),
)

GO

exec db.TablePropertySet  'Labelers', 'IDontCreate', @propertyName='Implements', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Labelers', '0', @propertyName='AddToDbContext', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Labelers', '0', @propertyName='GeneratePoco', @tableSchema='NationalDrugCode'

GO

create table NationalDrugCode.Products
(
	ProductId int not null identity primary key,
	LabelerId int not null references NationalDrugCode.Labelers(LabelerId),
	ProductUid varchar(50) not null unique,
	ProductNDC varchar(16) not null,
	ProductTypeName varchar(64),
	ProprietaryName nvarchar(1024),
	ProprietaryNameSuffix nvarchar(255) null,
	NonProprietaryName nvarchar(1024),
	DosageFormName varchar(64),
	RouteName varchar(255),
	StartMarketingDate date null,
	EndMarketingDate date null,
	MarketingCategoryName varchar(64),
	ApplicationNumber varchar(11),
	SubstanceName nvarchar(max),
	StrengthNumber nvarchar(4000),
	StrengthUnit nvarchar(4000),
	PharmClasses nvarchar(max),
	DEASchedule varchar(4)
)

GO

exec db.TablePropertySet  'Products', 'IDontCreate', @propertyName='Implements', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Products', '1', @propertyName='AddToDbContext', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Products', '1', @propertyName='GeneratePoco', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Products', 'http://www.fda.gov/Drugs/InformationOnDrugs/ucm254527.htm', @propertyName='Source', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'ProductUid', 'ProductUID is a concatenation of the NDCproduct code and SPL documentID. It is included to help prevent duplicate rows from appearing when joining the product and package files together.  It has no regulatory value or significance.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'ProductNDC', 'The labeler code and product code segments of the National Drug Code number, separated by a hyphen. Asterisks are no longer used or included within the product code segment to indicate certain configurations of the NDC.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'ProductTypeName', 'Indicates the type of product, such as Human Prescription Drug or Human OTC Drug. This data element corresponds to the "Document Type" of the SPL submission for the listing. The complete list of codes and translations can be found at', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'ProprietaryName', 'Also known as the trade name. It is the name of the product chosen by the labeler.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'ProprietaryNameSuffix', 'A suffix to the proprietary name, a value here should be appended to the ProprietaryName field to obtain the complete name of the product. This suffix is often used to distinguish characteristics of a product such as extended release (“XR”) or sleep aid (“PM”). Although many companies follow certain naming conventions for suffices, there is no recognized standard.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'NonProprietaryName', 'Sometimes called the generic name, this is usually the active ingredient(s) of the product.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'DosageFormName', 'The translation of the DosageForm Code submitted by the firm. The complete list of codes and translations can be found www.fda.gov/edrls under Structured Product Labeling Resources.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'RouteName', 'The translation of the Route Code submitted by the firm, indicating route of administration. The complete list of codes and translations can be found at www.fda.gov/edrls under Structured Product Labeling Resources.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'StartMarketingDate', 'This is the date that the labeler indicates was the start of its marketing of the drug product.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'EndMarketingDate', 'This is the date the product will no longer be available on the market. If a product is no longer being manufactured, in most cases, the FDA recommends firms use the expiration date of the last lot produced as the EndMarketingDate, to reflect the potential for drug product to remain available after manufacturing has ceased. Products that are the subject of ongoing manufacturing will not ordinarily have any EndMarketingDate. Products with a value in the EndMarketingDate will be removed from the NDC Directory when the EndMarketingDate is reached.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'MarketingCategoryName', 'Product types are broken down into several potential Marketing Categories, such as NDA/ANDA/BLA, OTC Monograph, or Unapproved Drug. One and only one Marketing Category may be chosen for a product, not all marketing categories are available to all product types. Currently, only final marketed product categories are included.  The complete list of codes and translations can be found at www.fda.gov/edrls under Structured Product Labeling Resources.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'ApplicationNumber', 'This corresponds to the NDA, ANDA, or BLA number reported by the labeler for products which have the corresponding Marketing Category designated. If the designated Marketing Category is OTC Monograph Final or OTC Monograph Not Final, then the Application number will be the CFR citation corresponding to the appropriate Monograph (e.g. “part 341”). For unapproved drugs, this field will be null.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'SubstanceName', 'This is the active ingredient list. Each ingredient name is the preferred term of the UNII code submitted.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'StrengthNumber', 'These are the strength values (to be used with units below) of each active ingredient, listed in the same order as the SubstanceName field above.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'StrengthUnit', 'These are the units to be used with the strength values above, listed in the same order as the SubstanceName and SubstanceNumber.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'PharmClasses', 'These are the reported pharmacological class categories corresponding to the SubstanceNames listed above.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Products', 'DEASchedule', 'This is the assigned DEA Schedule number as reported by the labeler. Values are CI, CII, CIII, CIV, and CV.', @tableSchema='NationalDrugCode'

GO

create table NationalDrugCode.Packages
(
	PackageId int not null identity primary key,
	Productid int NOT NULL references NationalDrugCode.Products(ProductId),
	ProductNDC varchar(16) not null,
    NDCPackageCode varchar(16) NOT NULL,
    PackageDescription nvarchar(max) NOT NULL,
	CmsCode  AS (replace(case when substring([NDCPackageCode],(6),(1))='-' AND substring([NDCPackageCode],(10),(1))='-' then (substring([NDCPackageCode],(1),(6))+'0')+substring([NDCPackageCode],(7),(6)) when substring([NDCPackageCode],(6),(1))='-' AND substring([NDCPackageCode],(11),(1))='-' then (substring([NDCPackageCode],(1),(11))+'0')+substring([NDCPackageCode],(12),(1)) when substring([NDCPackageCode],(5),(1))='-' then '0'+[NDCPackageCode] else '???' end,'-','')) PERSISTED
)

GO

exec db.TablePropertySet  'Packages', 'IDontCreate', @propertyName='Implements', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Packages', '1', @propertyName='AddToDbContext', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Packages', '1', @propertyName='GeneratePoco', @tableSchema='NationalDrugCode'
exec db.TablePropertySet  'Packages', 'http://www.fda.gov/Drugs/InformationOnDrugs/ucm254528.htm', @propertyName='Source', @tableSchema='NationalDrugCode'

exec db.ColumnPropertySet 'Packages', 'ProductNDC', 'The labeler code and product code segments of the National Drug Code number, separated by a hyphen. Asterisks are no longer used or included within the product code segment to indicate certain configurations of the NDC.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Packages', 'NDCPackageCode', 'The labeler code, product code, and package code segments of the National Drug Code number, separated by hyphens. Asterisks are no longer used or included within the product and package code segments to indicate certain configurations of the NDC.', @tableSchema='NationalDrugCode'
exec db.ColumnPropertySet 'Packages', 'PackageDescription', 'A description of the size and type of packaging in sentence form. Multilevel packages will have the descriptions concatenated together.  For example: 4 BOTTLES in 1 CARTON/100 TABLETS in 1 BOTTLE.', @tableSchema='NationalDrugCode'

GO