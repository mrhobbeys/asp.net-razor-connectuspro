-- =============================================
-- Script Template
-- =============================================

USE [EightHundred] 
GO

BEGIN TRAN

BEGIN TRY

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name= 'ParentJobID' AND OBJECT_NAME(OBJECT_ID) = 'TBL_Job')
	ALTER TABLE Tbl_job ADD ParentJobID int
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Error: ' + ERROR_MESSAGE()
	RETURN 
END CATCH 

GO