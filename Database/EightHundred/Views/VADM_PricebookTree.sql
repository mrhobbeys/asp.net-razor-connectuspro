-- =============================================
-- Script Template
-- =============================================
USE [EightHundred]
GO

/****** Object:  View [dbo].[VADM_PricebookTree]    Script Date: 04/26/2012 18:11:49 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VADM_PricebookTree]'))
DROP VIEW [dbo].[VADM_PricebookTree]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[VADM_PricebookTree]    Script Date: 04/26/2012 18:11:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[VADM_PricebookTree]
AS
SELECT     TOP (100) PERCENT pb.PriceBookID, pb.BookName, s.SectionName, ss.SubSectionName, jc.JobCode, jc.JobCodeDescription, jc.JobCost, jc.JobStdPrice, 
                      jc.JobMemberPrice, jc.JobAddonStdPrice, jc.JobAddonMemberPrice, jc.ResAccountCode, jc.ComAccountCode
FROM         dbo.tbl_PriceBook AS pb INNER JOIN
                      dbo.tbl_PB_Section AS s ON pb.PriceBookID = s.PriceBookID INNER JOIN
                      dbo.tbl_PB_SubSection AS ss ON ss.SectionID = s.SectionID INNER JOIN
                      dbo.tbl_PB_JobCodes AS jc ON jc.SubSectionID = ss.SubsectionID
ORDER BY pb.BookName

GO