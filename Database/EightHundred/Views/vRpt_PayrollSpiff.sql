USE [EightHundred]
GO

/*
 - For Reporting Facade
*/

/****** Object:  View [dbo].[vRpt_PayrollSpiff]    Script Date: 04/07/2012 09:48:41 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_PayrollSpiff]'))
DROP VIEW [dbo].[vRpt_PayrollSpiff]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_PayrollSpiff]    Script Date: 04/07/2012 09:48:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vRpt_PayrollSpiff]
AS

SELECT 
	psp.[PayrollSpiffID]
	,psp.[PayrollSetupID]
	,psp.[JobCodeID]
	,jc.JobCode
	,jc.JobCodeDescription
	,psp.[ServiceProID]
	,emp.Employee
	,psp.[PayTypeID]
	,pt.SpiffPayType PayType
	,psp.[Rate]
	,psp.[DateExpires]
	,psp.[Comments]
	,psp.[AddonYN]
	,psp.[ActiveYN]
FROM 
	[dbo].[tbl_HR_PayrollSpiffs] psp
	INNER JOIN dbo.tbl_PB_JobCodes jc ON psp.JobCodeID = jc.JobCodeID
	INNER JOIN dbo.tbl_Spiff_PayType pt ON psp.PayTypeID = pt.SpiffPayTypeID
	INNER JOIN dbo.tbl_Employee emp ON emp.EmployeeID = psp.ServiceProID


GO


