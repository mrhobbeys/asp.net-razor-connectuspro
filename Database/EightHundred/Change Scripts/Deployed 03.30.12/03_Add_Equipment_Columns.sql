USE [EightHundred]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Job' AND name = 'AgeOfHouse')
	ALTER TABLE tbl_Job ADD AgeOfHouse CHAR(1)

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Job' AND name = 'AgeOfHVAC')
	ALTER TABLE tbl_Job ADD AgeOfHVAC CHAR(1)

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Job' AND name = 'AgeOfWaterHeater')
	ALTER TABLE tbl_Job ADD AgeOfWaterHeater CHAR(1)