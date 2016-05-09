USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_JobCode]    Script Date: 04/22/2012 14:40:51 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_JobCode]'))
DROP VIEW [dbo].[vRpt_JobCode]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_JobCode]    Script Date: 04/22/2012 14:40:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vRpt_JobCode]
AS

SELECT 
	-- PriceBook
	pb.FranchiseID
	,pb.PriceBookID
	,pb.BookName		PriceBookName
	,pb.ActiveBookYN	PriceBookActiveYN
	
	--	pb.* 
	--,'-->'
	--,pbs.*
	--,'-->'
	--,pbss.*
	--,'-->'
	
	-- Job Code
	,pbjc.JobCodeID
	,pbjc.ActiveYN
	,pbjc.JobCode
	,pbjc.JobCodeDescription
--	,part.*
FROM 
	tbl_Franchise fr
	INNER JOIN tbl_PriceBook pb ON fr.FranchiseID = pb.FranchiseID
	INNER JOIN tbl_PB_Section pbs ON pb.PriceBookID = pbs.PriceBookID
	INNER JOIN tbl_PB_SubSection pbss ON pbs.SectionID = pbss.SectionID
	INNER JOIN tbl_PB_JobCodes pbjc ON pbjc.SubSectionID = pbss.SubsectionID
WHERE (1 = 1) 




GO

