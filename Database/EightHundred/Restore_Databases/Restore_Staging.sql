﻿-- =============================================
-- Script Template
-- =============================================
USE [master]
GO

ALTER DATABASE [EightHundred] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
GO

RESTORE DATABASE [EightHundred] 
FROM  DISK = N'C:\backups\eighthundred.bak' WITH  FILE = 1,  
NOUNLOAD,  STATS = 10, REPLACE
GO

ALTER DATABASE [EightHundred] SET RECOVERY SIMPLE WITH NO_WAIT
GO

ALTER DATABASE [EightHundred] SET  MULTI_USER WITH NO_WAIT
GO

USE [EightHundred]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'CONN951\SvcConnectusPro_PROD')
DROP USER [CONN951\SvcConnectusPro_PROD]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'CONN950\SvcConnectusPro_STAG')
DROP USER [CONN950\SvcConnectusPro_STAG]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'CONN952\SvcConnectusPro_TRA')
DROP USER [CONN952\SvcConnectusPro_TRA]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'CONN952\SvcConnectusPro_DEV')
DROP USER [CONN952\SvcConnectusPro_DEV]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'CONN952\SvcConnectusPro_DEMO')
DROP USER [CONN952\SvcConnectusPro_DEMO]
GO

CREATE USER [CONN950\SvcConnectusPro_STAG FOR LOGIN [CONN950\SvcConnectusPro_STAG]
GO

EXEC sp_addrolemember N'db_owner', N'CONN950\SvcConnectusPro_STAG'