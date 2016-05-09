-- =============================================
-- Script Template
-- =============================================
USE [DB_10668_Calls]
GO

BEGIN TRAN

BEGIN TRY

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name= 'VirtualPhoneNumber' AND OBJECT_NAME(OBJECT_ID) = 'LookupScript')
	ALTER TABLE LookupScript ADD VirtualPhoneNumber bit
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Error: ' + ERROR_MESSAGE()
	RETURN 
END CATCH 

GO

BEGIN TRY
UPDATE LookupScript
SET VirtualPhoneNumber = 0
WHERE VirtualPhoneNumber IS NULL


IF  NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_LookupScript_VirtualPhoneNumber]') AND type = 'D')
	ALTER TABLE [dbo].[LookupScript] ADD  CONSTRAINT [DF_LookupScript_VirtualPhoneNumber]  DEFAULT (0) FOR [VirtualPhoneNumber]


IF EXISTS (SELECT 1 FROM sys.columns WHERE name= 'VirtualPhoneNumber' AND OBJECT_NAME(OBJECT_ID) = 'LookupScript')
	ALTER TABLE LookupScript ALTER COLUMN VirtualPhoneNumber Bit NOT NULL

END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Error: ' + ERROR_MESSAGE()
	
END CATCH


Commit TRAN
