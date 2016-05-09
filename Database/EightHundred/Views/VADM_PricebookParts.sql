-- =============================================
-- Script Template
-- =============================================
USE [eighthundred]
GO

/****** Object:  View [dbo].[VADM_Pricebook_Parts]    Script Date: 04/26/2012 18:06:37 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VADM_PricebookParts]'))
DROP VIEW [dbo].[VADM_PricebookParts]
GO

USE [eighthundred]
GO

/****** Object:  View [dbo].[VADM_PricebookParts]    Script Date: 04/26/2012 18:06:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[VADM_PricebookParts]
AS
SELECT     TOP (100) PERCENT jc.JobCode, mp.PartCodeID, mp.PartCode, mp.PartName, mp.PartCost, mp.VendorPartID, jcd.Qty, jcd.PartStdPrice, jcd.PartMemberPrice, 
                      jcd.PartAddonStdPrice, jcd.PartAddonMemberPrice, s.PriceBookID
FROM         dbo.tbl_PriceBook AS pb INNER JOIN
                      dbo.tbl_PB_Section AS s ON pb.PriceBookID = s.PriceBookID INNER JOIN
                      dbo.tbl_PB_SubSection AS ss ON ss.SectionID = s.SectionID INNER JOIN
                      dbo.tbl_PB_JobCodes AS jc ON jc.SubSectionID = ss.SubsectionID INNER JOIN
                      dbo.tbl_PB_JobCodes_Details AS jcd ON jc.JobCodeID = jcd.JobCodeID INNER JOIN
                      dbo.tbl_PB_Parts AS p ON p.PartID = jcd.PartID INNER JOIN
                      dbo.tbl_PB_MasterParts AS mp ON mp.MasterPartID = p.MasterPartID
ORDER BY jc.JobCode

GO