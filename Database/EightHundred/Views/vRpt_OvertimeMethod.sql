USE [EightHundred]
GO

/*
 - For Reporting Facade
*/

/****** Object:  View [dbo].[vRpt_OvertimeMethod]    Script Date: 04/07/2012 09:48:41 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_OvertimeMethod]'))
DROP VIEW [dbo].[vRpt_OvertimeMethod]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_OvertimeMethod]    Script Date: 04/07/2012 09:48:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vRpt_OvertimeMethod]
AS

SELECT OvertimeMethodID, OvertimeMethod FROM tbl_OvertimeMethod


GO



