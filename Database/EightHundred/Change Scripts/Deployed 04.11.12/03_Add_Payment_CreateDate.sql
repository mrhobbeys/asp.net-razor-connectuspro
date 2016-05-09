USE [EightHundred]
GO

BEGIN TRAN

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE name='CreateDate' AND OBJECT_NAME(Object_id) = 'tbl_Payments')
	ALTER TABLE tbl_Payments ADD CreateDate DATETIME
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE name='ExceptionComments' AND OBJECT_NAME(Object_id) = 'tbl_Payments')
	ALTER TABLE tbl_Payments ADD ExceptionComments NVARCHAR(4000)
GO

UPDATE tbl_Payments SET CreateDate = PaymentDate WHERE CreateDate IS NULL

ALTER TABLE tbl_Payments ALTER COLUMN CreateDate DATETIME NOT NULL

IF  NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tbl_Payments_CreateDate]') AND type = 'D')
	ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_CreateDate]  DEFAULT (GETDATE()) FOR [CreateDate]

IF EXISTS(SELECT 1 FROM sys.columns WHERE name='Timestamp' AND OBJECT_NAME(Object_id) = 'tbl_Payments')
	ALTER TABLE tbl_Payments DROP COLUMN [Timestamp]
GO

COMMIT TRAN