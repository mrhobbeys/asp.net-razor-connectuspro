USE [EightHundred]
GO

/*
 - For Reporting Facade
*/

/****** Object:  View [dbo].[vRpt_Employee]    Script Date: 04/07/2012 09:48:41 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_Employee]'))
DROP VIEW [dbo].[vRpt_Employee]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_Employee]    Script Date: 04/07/2012 09:48:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vRpt_Employee]
AS

SELECT
	fr.FranchiseID
	,fr.FranchiseNUmber
	,fr.LegalName	FranchiseLegalName
	--,fr.*
	--,'-->'
	,emp.EmployeeID
	,emp.Employee
	,emp.CommissionRate
	,emp.ServiceProYN
	,ActiveYN
FROM
	tbl_Franchise fr
	LEFT JOIN tbl_Employee emp ON fr.FranchiseID = emp.FranchiseID
WHERE (1 = 1)


GO


