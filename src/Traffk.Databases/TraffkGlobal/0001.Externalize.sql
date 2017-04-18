CREATE USER _TraffkGlobalUser
	FOR LOGIN _TraffkGlobalUser
	WITH DEFAULT_SCHEMA = dbo
GO
EXEC sp_addrolemember N'db_datareader', N'_TraffkGlobalUser'
GO
