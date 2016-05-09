USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_CustomerLedgerSummary]    Script Date: 04/16/2012 10:33:10 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_CustomerLedgerSummary]'))
DROP VIEW [dbo].[vRpt_CustomerLedgerSummary]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_CustomerLedgerSummary]    Script Date: 04/16/2012 10:33:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create view [dbo].[vRpt_CustomerLedgerSummary] AS

SELECT ClientId
	, CustomerId
	, SUM(CASE WHEN Type = 'Invoice' THEN Amount ELSE 0 END) AS [Invoices]
	, SUM(CASE WHEN Type = 'Payment' THEN Amount ELSE 0 END) AS [Payments]
	, SUM(ROUND(Amount,2)) AS [OutstandingBalance]
FROM vRpt_CustomerLedger
GROUP BY ClientId, CustomerId
HAVING SUM(ROUND(Amount, 2)) <> 0

GO


