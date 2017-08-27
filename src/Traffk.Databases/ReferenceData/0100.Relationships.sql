alter table dbo.Zips Add foreignKey CountryId references [ISO3166].[Countries](CountryId)
 
GO
