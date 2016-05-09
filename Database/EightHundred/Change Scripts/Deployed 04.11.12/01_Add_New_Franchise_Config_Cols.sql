USE [EightHundred]
GO

BEGIN TRAN

BEGIN TRY

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name= 'MaxRoyaltyDeferDays' AND OBJECT_NAME(OBJECT_ID) = 'tbl_Franchise')
	ALTER TABLE tbl_Franchise ADD MaxRoyaltyDeferDays INT
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name= 'AccountingVarianceDays' AND OBJECT_NAME(OBJECT_ID) = 'tbl_Franchise')
	ALTER TABLE tbl_Franchise ADD AccountingVarianceDays INT
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Error: ' + ERROR_MESSAGE()
	RETURN 
END CATCH 

GO

BEGIN TRY
UPDATE tbl_Franchise
SET MaxRoyaltyDeferDays = 30
WHERE MaxRoyaltyDeferDays IS NULL

UPDATE tbl_Franchise
SET AccountingVarianceDays = 7
WHERE AccountingVarianceDays IS NULL


IF  NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tbl_Franchise_MaxRoyaltyDeferDays]') AND type = 'D')
	ALTER TABLE [dbo].[tbl_Franchise] ADD  CONSTRAINT [DF_tbl_Franchise_MaxRoyaltyDeferDays]  DEFAULT (30) FOR [MaxRoyaltyDeferDays]

IF  NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tbl_Franchise_AccountingVarianceDays]') AND type = 'D')
	ALTER TABLE [dbo].[tbl_Franchise] ADD  CONSTRAINT [DF_tbl_Franchise_AccountingVarianceDays]  DEFAULT (7) FOR [AccountingVarianceDays]

IF EXISTS (SELECT 1 FROM sys.columns WHERE name= 'MaxRoyaltyDeferDays' AND OBJECT_NAME(OBJECT_ID) = 'tbl_Franchise')
	ALTER TABLE tbl_Franchise ALTER COLUMN MaxRoyaltyDeferDays INT NOT NULL

IF EXISTS (SELECT 1 FROM sys.columns WHERE name= 'AccountingVarianceDays' AND OBJECT_NAME(OBJECT_ID) = 'tbl_Franchise')
	ALTER TABLE tbl_Franchise ALTER COLUMN AccountingVarianceDays INT NOT NULL
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Error: ' + ERROR_MESSAGE()
	
END CATCH


COMMIT TRAN
