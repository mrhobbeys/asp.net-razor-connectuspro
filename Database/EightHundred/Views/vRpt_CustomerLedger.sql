USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_CustomerLedger]    Script Date: 04/16/2012 10:31:14 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_CustomerLedger]'))
DROP VIEW [dbo].[vRpt_CustomerLedger]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_CustomerLedger]    Script Date: 04/16/2012 10:31:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vRpt_CustomerLedger] AS

WITH [Invoices] AS
(
SELECT 
	  j.FranchiseID AS ClientId
	, j.CustomerId AS [CustomerId]
	, j.JobId AS LineId
	, j.JobID AS [ReferenceId]
	, 'Invoice' AS [Type], TotalSales AS [Amount]
	, ROUND(j.TotalSales - SUM(p.PaymentAmount), 2) AS [ActualBalance]
	, ROUND(j.Balance, 2) AS [StoredBalance]
	, CallCompleted AS [RecordedDate]
	, CONVERT(BIT, CASE WHEN j.TotalSales - SUM(p.PaymentAmount) <> 0 OR j.TotalSales - SUM(p.PaymentAmount) <> j.Balance THEN 1 ELSE 0 END) AS [IsOutstanding]
FROM tbl_job j
INNER JOIN tbl_Payments p
ON j.JobID = p.JobID
WHERE j.StatusID <> 15
GROUP BY j.JobID, j.FranchiseID, j.CustomerID, j.TotalSales, j.CallCompleted, j.Balance, j.LockedYN
)
, [Payments] AS
(
SELECT i.ClientId
	, i.CustomerId
	,  PaymentId AS LineId
	, i.LineId AS [ReferenceId]
	, 'Payment' AS [Type]
	, ISNULL(PaymentAmount, 0) * -1 AS [Amount]
	, 0 AS [StoredBalance]
	, 0 AS [ActualBalance]
	, PaymentDate AS [PostDate] 
	, i.IsOutstanding
FROM tbl_Payments p INNER JOIN
[Invoices] i
ON i.LineId = p.JobID
)
, [LedgerItems] AS
(
	SELECT * FROM Invoices
	UNION ALL
	SELECT * FROM Payments
)
, [Ordered] AS
(
	SELECT ROW_NUMBER() OVER(PARTITION BY CustomerId ORDER BY RecordedDate DESC, LineId DESC) AS Sequence, *
	FROM [LedgerItems]
)

SELECT
	o.ClientId
	, o.CustomerId
	, o.LineId
	, o.ReferenceId
	, o.Type
	, o.Amount
	, o.ActualBalance
	, o.StoredBalance
	, o.RecordedDate
	, o.IsOutstanding
	, ROUND(o.Amount, 2) + ISNULL(SUM(ROUND(o1.Amount, 2)), 0) AS [RunningBalance]
	, o.Sequence
FROM Ordered [o]
LEFT JOIN [Ordered] o1
ON o.CustomerId = o1.CustomerId AND o.Sequence < o1.Sequence
GROUP BY o.Sequence, o.ClientId, o.CustomerId, o.LineId, o.ReferenceId, o.Type, o.Amount, o.ActualBalance, o.StoredBalance, o.RecordedDate, o.IsOutstanding

GO
