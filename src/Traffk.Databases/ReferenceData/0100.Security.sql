CREATE USER _TraffkPortalApp
	FOR LOGIN _TraffkPortalApp
	WITH DEFAULT_SCHEMA = dbo
GO

CREATE USER _EtlApp
	FOR LOGIN _EtlApp
	WITH DEFAULT_SCHEMA = dbo
GO

create role _EtlAppRole

GO

exec sp_addrolemember 'db_datareader', '_TraffkPortalApp'
exec sp_addrolemember '_EtlAppRole', '_EtlApp'
exec sp_addrolemember 'db_datareader', '_EtlAppRole'
exec sp_addrolemember 'db_datawriter', '_EtlAppRole'

GO
