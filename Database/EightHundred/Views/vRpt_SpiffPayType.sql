USE [EightHundred]
GO

/*
 - For Reporting Facade
*/

/****** Object:  View [dbo].[vRpt_SpiffPayType]    Script Date: 04/07/2012 09:48:41 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_SpiffPayType]'))
DROP VIEW [dbo].[vRpt_SpiffPayType]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_SpiffPayType]    Script Date: 04/07/2012 09:48:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vRpt_SpiffPayType]
AS

SELECT 
	pt.SpiffPayTypeID
	,pt.SpiffPayType 
FROM 
	tbl_Spiff_PayType pt

GO


