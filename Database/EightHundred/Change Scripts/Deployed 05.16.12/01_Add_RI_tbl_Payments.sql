USE [Eighthundred]
GO

BEGIN TRAN
PRINT 'Fixing payments tied to incorrect franchise.'

;WITH [BadPayments] AS
(
SELECT p.PaymentID FROM tbl_Payments p
LEFT JOIN tbl_Franchise f
ON p.franchiseid = f.franchiseid
WHERE f.franchiseid IS NULL
)
UPDATE p
SET p.FranchiseID = j.FranchiseID
FROM tbl_Payments p
INNER JOIN BadPayments bp
ON p.PaymentID = bp.PaymentID
INNER JOIN tbl_Job j
ON j.JobID = p.JobID

PRINT 'Adding foreign key to franchise from tbl_Payments'
IF  NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_Payments_tbl_Franchise]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_Payments]'))
BEGIN
	ALTER TABLE dbo.tbl_Payments ADD CONSTRAINT FK_tbl_Payments_tbl_Franchise FOREIGN KEY (FranchiseID) REFERENCES dbo.tbl_Franchise (FranchiseID) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
END

COMMIT TRAN
