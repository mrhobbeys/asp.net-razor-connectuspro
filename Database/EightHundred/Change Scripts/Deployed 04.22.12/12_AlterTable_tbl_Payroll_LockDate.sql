USE EightHundred

/*
	- Add LockDate to Payroll and remove obsolete column PayrollCompletionDate on Job Table

*/

BEGIN TRY
	BEGIN TRAN

	-- Add new LockDate Column if does not exist
	IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbl_Payroll' AND COLUMN_NAME = 'LockDate')
	BEGIN
		PRINT('Adding Column tbl_Payroll.LockDate')
		ALTER TABLE tbl_Payroll ADD LockDate DATETIME NULL	
		PRINT('Adding Column tbl_Payroll.LockDate - COMPLETE')
	END
	ELSE
	BEGIN
		PRINT('Column tbl_Payroll.LockDate already exists')
	END
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Error: ' + ERROR_MESSAGE()
	RETURN
END CATCH

GO

BEGIN TRY
	--SELECT 
	--	pyr.PayrollID,
	--	pyr.PayrollDate,
	--	pyr.LockDate,
	--	jb.PayrollCompletedDate
	UPDATE pyr
		SET pyr.LockDate = jb.PayrollCompletedDate
	FROM 
		tbl_Payroll pyr
		INNER JOIN tbl_Job_Payroll jp ON jp.PayrollID = pyr.PayrollID
		INNER JOIN tbl_Job jb ON jp.JobID = jb.JobID
	WHERE (1 = 1)
		AND jb.PayrollCompletedDate IS NOT NULL
	
	-- Drop old PayrollCompletionDate Column from Job Table
	IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbl_Job' AND COLUMN_NAME = 'PayrollCompletedDate')
	BEGIN
		PRINT('Dropping Column tbl_Job.PayrollCompletedDate')
		ALTER TABLE tbl_Job DROP COLUMN PayrollCompletedDate
		PRINT('Dropping Column tbl_Job.PayrollCompletedDate - COMPLETE')
	END
	ELSE
	BEGIN
		PRINT('Column tbl_Job.PayrollCompletedDate has already been dropped')
	END
	
		-- Drop old Locked Column from Payroll.  If the lock date is populated, we know it's Locked
	IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tbl_Payroll_LockedYN]') AND type = 'D')
	BEGIN
	ALTER TABLE [dbo].[tbl_Payroll] DROP CONSTRAINT [DF_tbl_Payroll_LockedYN]
	END

	IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbl_Payroll' AND COLUMN_NAME = 'LockedYN')
	BEGIN
		PRINT('Dropping Column tbl_Payroll.LockedYN')
		ALTER TABLE tbl_Payroll DROP COLUMN LockedYN
		PRINT('Dropping Column tbl_Payroll.LockedYN - COMPLETE')
	END
	ELSE
	BEGIN
		PRINT('Column tbl_Payroll.LockedYN has already been dropped')
	END



	PRINT('Succeeded, committing transaction')
	COMMIT
END TRY
BEGIN CATCH
	PRINT('Proc failed, rolling back.  Error Message: ' + ERROR_MESSAGE())
	ROLLBACK
	DECLARE @errorMessage varchar(max)
	SET @errorMessage = ERROR_MESSAGE()
	RAISERROR(@errorMessage,16, 1)
END CATCH









