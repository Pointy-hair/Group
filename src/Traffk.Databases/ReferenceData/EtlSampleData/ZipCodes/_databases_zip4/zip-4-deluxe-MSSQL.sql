/*
------------------------------------------------------------------
Provider:  Zip-Codes.com
Product:   U.S. ZIP +4 Database Deluxe
------------------------------------------------------------------
This SQL Creates a new table named ZIP4, 
related indexes, and extended column information.

This script is designed to work with MS SQL Server 2000, 2005, & 2008.

Actions:
  1.) Drop Table ZIP4 if it exists
  2.) Creates Table named ZIP4

Last Updated: 7/1/2012
------------------------------------------------------------------
*/


/* 1.) Drop Table if it Exists */
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ZIP4]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ZIP4]
GO



/* 2.) Create Table */
CREATE TABLE [dbo].[ZIP4](
	[ZipCode] [char](5) NULL,
	[UpdateKey] [varchar](10) NULL,
	[Action] [char](1) NULL,
	[RecordType] [varchar](1) NULL,
	[CarrierRoute] [varchar](4) NULL,
	[StPreDirAbbr] [varchar](2) NULL,
	[StName] [varchar](28) NULL,
	[StSuffixAbbr] [varchar](4) NULL,
	[StPostDirAbbr] [varchar](2) NULL,
	[AddressPrimaryLowNumber] [varchar](10) NULL,
	[AddressPrimaryHighNumber] [varchar](10) NULL,
	[AddressPrimaryOddEven] [varchar](1) NULL,
	[BuildingName] [varchar](40) NULL,
	[AddressSecAbbr] [varchar](4) NULL,
	[AddressSecLowNumber] [varchar](10) NULL,
	[AddressSecHighNumber] [varchar](10) NULL,
	[AddressSecOddEven] [varchar](1) NULL,
	[Plus4Low] [varchar](4) NULL,
	[Plus4High] [varchar](4) NULL,
	[BaseAlternateCode] [varchar](1) NULL,
	[LACSStatus] [varchar](1) NULL,
	[GovernmentBuilding] [varchar](1) NULL,
	[FinanceNumber] [varchar](6) NULL,
	[State] [varchar](2) NULL,
	[CountyFIPS] [varchar](3) NULL,
	[CongressionalDistrict] [varchar](2) NULL,
	[MunicipalityKey] [varchar](6) NULL,
	[UrbanizationKey] [varchar](6) NULL,
	[PreferredLastLineKey] [varchar](6) NULL,
	[ToLatitude] [decimal](18, 10) NULL,
	[FromLatitude] [decimal](18, 10) NULL,
	[ToLongitude] [decimal](18, 10) NULL,
	[FromLongitude] [decimal](18, 10) NULL,
	[CensusTract] [varchar](15) NULL,
	[CensusBlock] [varchar](15) NULL,
	[TLID] [varchar](15) NULL,
	[LatLonMultiMatch] [varchar](1) NULL,
	[CenLat] [decimal](18, 10) NULL,
	[CenLon] [decimal](18, 10) NULL,
	[BlockGroup] [varchar](15) NULL
) ON [PRIMARY]
GO
