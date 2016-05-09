-- =============================================
-- Restore the Demo Database
-- =============================================
USE [master]
GO

ALTER DATABASE [EightHundred] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
GO

RESTORE DATABASE [EightHundred] 
FROM  DISK = N'C:\sqlbackup\eighthundred.bak' WITH  FILE = 1,  
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

CREATE USER [CONN950\SvcConnectusPro_STAG FOR LOGIN [CONN952\SvcConnectusPro_DEMO]
GO

EXEC sp_addrolemember N'db_owner', N'CONN952\SvcConnectusPro_DEMO'


USE [EightHundred]
GO

update tbl_Franchise 
set FranchiseStatusID = 10
where FranchiseID <> 51 and FranchiseID <> 38 and FranchiseID <> 56

GO
update tbl_Franchise 
set LegalName = 'AAA Plumbing'
where FranchiseID = 38 
GO
update tbl_Franchise 
set LegalName = 'AAA Heating and Cooling'
where FranchiseID = 51
GO
select * from tbl_Franchise
GO