USE [EightHundred]
GO

BEGIN TRAN

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE name='CreateDate' AND OBJECT_NAME(Object_id) = 'tbl_Deposits')
	ALTER TABLE tbl_Deposits ADD CreateDate DATETIME
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE name='ExceptionComments' AND OBJECT_NAME(Object_id) = 'tbl_Deposits')
	ALTER TABLE tbl_Deposits ADD ExceptionComments NVARCHAR(4000)
GO

UPDATE tbl_Deposits SET CreateDate = DepositDate WHERE CreateDate IS NULL

ALTER TABLE tbl_Deposits ALTER COLUMN CreateDate DATETIME NOT NULL

IF  NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tbl_Deposits_CreateDate]') AND type = 'D')
	ALTER TABLE [dbo].[tbl_Deposits] ADD  CONSTRAINT [DF_tbl_Deposits_CreateDate]  DEFAULT (GETDATE()) FOR [CreateDate]

IF EXISTS(SELECT 1 FROM sys.columns WHERE name='Timestamp' AND OBJECT_NAME(Object_id) = 'tbl_Deposits')
	ALTER TABLE tbl_Deposits DROP COLUMN [Timestamp]
GO

COMMIT TRAN