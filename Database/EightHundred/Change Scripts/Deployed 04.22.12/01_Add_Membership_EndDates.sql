USE [EightHundred]
GO

BEGIN TRAN
UPDATE tbl_Customer_Members
SET EndDate = DATEADD(year, 1, StartDate)
WHERE StartDate IS NOT NULL AND EndDate IS NULL
COMMIT TRAN