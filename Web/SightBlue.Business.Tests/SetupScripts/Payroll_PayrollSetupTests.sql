USE [EightHundred]
GO

/****** Object:  StoredProcedure [dbo].[UnitTest_PayrollSetup]    Script Date: 04/13/2012 07:37:21 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnitTest_PayrollSetup]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UnitTest_PayrollSetup]
GO

USE [EightHundred]
GO

/****** Object:  StoredProcedure [dbo].[UnitTest_PayrollSetup]    Script Date: 04/13/2012 07:37:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Bryan Panjavan
-- Create date: 2012.04.13
-- Description:	For Automated Integration Testing.
--					Not perfect, but helps re-run the same setup that I'm currently using for Unit Tests
--				2012.04.13 - Converted to Stored Proc for automation
-- =============================================
CREATE PROCEDURE [dbo].[UnitTest_PayrollSetup]
	@scenarioNumber varchar(50)	-- Ex A1, A2, etc.
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DECLARE @franchiseID int
SET @franchiseID = 39	-- Dallas TX

BEGIN TRY
	BEGIN TRAN

	/*
	[SETUP - Part 1] - Clear existing records
	*/
	DELETE FROM tbl_HR_PayrollSpiffs WHERE PayrollSetupID IN
	(
		SELECT PayrollSetupID FROM tbl_HR_PayrollSetup WHERE FranchiseID = @franchiseID
	)
	DELETE FROM tbl_HR_PayrollSetup WHERE FranchiseID = @franchiseID


	/*[Section 2] - [Scenario A1] - No Payroll  */
	IF (@scenarioNumber = 'A1')
	BEGIN
		PRINT ('Do Nothing')
	END

	/*[Section 2] - [Scenario A2] - One Payroll Setup  */
	IF (@scenarioNumber = 'A2')
	BEGIN
		
		INSERT INTO [dbo].[tbl_HR_PayrollSetup]
			   ([FranchiseID]
			   ,[LaborCommission]
			   ,[PartsCommission]
			   ,[OvertimeStarts]
			   ,[OvertimeMethod]
			   ,[OTMultiplier])
		 VALUES
			   (
				@franchiseID	--<FranchiseID, int,>
			   ,-1			--<LaborCommission, real,>
			   ,-1			--<PartsCommission, real,>
			   ,40				--<OvertimeStarts, real,>
			   ,1				--<OvertimeMethod, int,>
			   ,1.5				--<OTMultiplier, real,>
			   )
	END
	/*[Section 2] - [Scenario A3] - One Payroll Setup for specific franchise*/
	IF (@scenarioNumber = 'A3')
	BEGIN
		
		INSERT INTO [dbo].[tbl_HR_PayrollSetup]
			   ([FranchiseID]
			   ,[LaborCommission]
			   ,[PartsCommission]
			   ,[OvertimeStarts]
			   ,[OvertimeMethod]
			   ,[OTMultiplier])
		 VALUES
			   (
				@franchiseID	--<FranchiseID, int,>
			   ,-1			--<LaborCommission, real,>
			   ,-1			--<PartsCommission, real,>
			   ,40				--<OvertimeStarts, real,>
			   ,1				--<OvertimeMethod, int,>
			   ,1.5				--<OTMultiplier, real,>
			   )
	END

	/*[Section 2] - [Scenario A5] - One Payroll Setup for specific franchise already existing*/
	IF (@scenarioNumber = 'A5')
	BEGIN
		
		INSERT INTO [dbo].[tbl_HR_PayrollSetup]
			   ([FranchiseID]
			   ,[LaborCommission]
			   ,[PartsCommission]
			   ,[OvertimeStarts]
			   ,[OvertimeMethod]
			   ,[OTMultiplier])
		 VALUES
			   (
				@franchiseID	--<FranchiseID, int,>
			   ,-1			--<LaborCommission, real,>
			   ,-1			--<PartsCommission, real,>
			   ,40				--<OvertimeStarts, real,>
			   ,1				--<OvertimeMethod, int,>
			   ,1.5				--<OTMultiplier, real,>
			   )
	END

	/*[Section 2] - [Scenario A8] - One Payroll Setup for specific franchise already existing, so that we can add one SPIFF*/
	IF (@scenarioNumber = 'A8')
	BEGIN
		
		INSERT INTO [dbo].[tbl_HR_PayrollSetup]
			   ([FranchiseID]
			   ,[LaborCommission]
			   ,[PartsCommission]
			   ,[OvertimeStarts]
			   ,[OvertimeMethod]
			   ,[OTMultiplier])
		 VALUES
			   (
				@franchiseID	--<FranchiseID, int,>
			   ,-1			--<LaborCommission, real,>
			   ,-1			--<PartsCommission, real,>
			   ,40				--<OvertimeStarts, real,>
			   ,1				--<OvertimeMethod, int,>
			   ,1.5				--<OTMultiplier, real,>
			   )
	END

	/*[Section 2] - [Scenario A9] - One Payroll Setup for specific franchise already existing, With one SPIFF that we update*/
	IF (@scenarioNumber = 'A9')
	BEGIN

	DECLARE @payrollSetupID int	
		INSERT INTO [dbo].[tbl_HR_PayrollSetup]
			   ([FranchiseID]
			   ,[LaborCommission]
			   ,[PartsCommission]
			   ,[OvertimeStarts]
			   ,[OvertimeMethod]
			   ,[OTMultiplier])
		 VALUES
			   (
				@franchiseID	--<FranchiseID, int,>
			   ,-1			--<LaborCommission, real,>
			   ,-1			--<PartsCommission, real,>
			   ,40				--<OvertimeStarts, real,>
			   ,1				--<OvertimeMethod, int,>
			   ,1.5				--<OTMultiplier, real,>
			   )
	SET @payrollSetupID = SCOPE_IDENTITY()

	INSERT INTO [dbo].[tbl_HR_PayrollSpiffs]
			   ([PayrollSetupID]
			   ,[JobCodeID]
			   ,[ServiceProID]
			   ,[PayTypeID]
			   ,[Rate]
			   ,[DateExpires]
			   ,[Comments]
			   ,[AddonYN]
			   ,[ActiveYN])
		 VALUES
			   (@payrollSetupID
			   ,65834
			   ,172
			   ,0
			   ,1.450
			   ,'5/2/2012'
			   ,'Test Existing Spiff'
			   ,1
			   ,1
			   )
	END

	PRINT('Succeeded, committing transaction')
	COMMIT
END TRY
BEGIN CATCH
	PRINT('Proc failed, rolling back.  Error Message: ' + ERROR_MESSAGE())
	ROLLBACK
END CATCH

END

GO

