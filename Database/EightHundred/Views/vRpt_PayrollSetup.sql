USE [EightHundred]
GO

/*
 - For Reporting Facade to Payroll Setup

*/

/****** Object:  View [dbo].[vRpt_PayrollSetup]    Script Date: 04/07/2012 09:48:41 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_PayrollSetup]'))
DROP VIEW [dbo].[vRpt_PayrollSetup]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_PayrollSetup]    Script Date: 04/07/2012 09:48:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vRpt_PayrollSetup]
AS

SELECT 
	ps.[PayrollSetupID]
	,ps.[FranchiseID]
	,ps.[OvertimeStarts]
	,ps.[OvertimeMethod] OvertimeMethodID
	,ps.[OTMultiplier]
	,om.OvertimeMethod
FROM 
	[dbo].[tbl_HR_PayrollSetup] ps
	INNER JOIN dbo.tbl_OvertimeMethod om ON ps.OvertimeMethod = om.OvertimeMethodID


GO


