-- =============================================
-- Script Template
-- =============================================
USE [EightHundred]
GO

/****** Object:  StoredProcedure [dbo].[pb_copyPriceBookAcrossClients]    Script Date: 04/30/2012 17:35:47 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pb_copyPriceBookAcrossClients]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[pb_copyPriceBookAcrossClients]
GO

USE [EightHundred]
GO

/****** Object:  StoredProcedure [dbo].[pb_copyPriceBookAcrossClients]    Script Date: 04/30/2012 17:35:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create proc [dbo].[pb_copyPriceBookAcrossClients](@PriceBookID INT, @DestinationFranchiseId INT)
AS
Begin
--SET @PriceBookID = 260
--SET @DestinationFranchiseId = 50

Declare @PBUploadTree Table(
BookName [varchar](50),	
CompanyCodeID [int],	
SectionName [varchar](50),	
SubSectionName [varchar](50),	
JobCode [varchar](50),	
JobCodeDescription [varchar](100),	
JobCost [money],	
JobStdPrice [money],	
JobMemberPrice [money],	
JobAddonStdPrice [money],	
JobAddonMemberPrice [money],	
ResAccountCode [varchar](5),	
ComAccountCode [varchar](5)
)	

Declare @detnewpart Table(
           JobCode varchar(10)
           ,PartCodeID char(2)
           ,PartCode varchar(25)
           ,PartName varchar(100)
           ,PartCost money
           ,VendorPartID varchar(20)
           ,Qty nchar(10)
           ,PartStdPrice money
           ,PartMemberPrice money
           ,PartAddonPrice money
           ,PartAddonMemberPrice money)



BEGIN TRAN

INSERT INTO @PBUploadTree
SELECT pb.BookName, @DestinationFranchiseId,s.SectionName, ss.SubSectionName, jc.JobCode, jc.JobCodeDescription, jc.JobCost, jc.JobStdPrice, jc.JobMemberPrice, jc.JobAddonStdPrice, jc.JobAddonMemberPrice,jc.ResAccountCode, jc.ComAccountCode FROM tbl_PriceBook pb
INNER JOIN tbl_PB_Section s
ON pb.PriceBookID = s.PriceBookID
INNER JOIN tbl_PB_SubSection ss
ON ss.SectionID = s.SectionID
INNER JOIN tbl_PB_JobCodes jc
ON jc.SubSectionID = ss.SubSectionID
WHERE pb.PriceBookID = @PriceBookID
ORDER BY pb.BookName

INSERT INTO @detnewpart
SELECT jc.JobCode, mp.PartCodeID, mp.PartCode, mp.PartName, mp.PartCost, mp.VendorPartID, jcd.Qty, jcd.PartStdPrice, jcd.PartMemberPrice, jcd.PartAddonStdPrice, jcd.PartAddonMemberPrice FROM tbl_PriceBook pb
INNER JOIN tbl_PB_Section s
ON pb.PriceBookID = s.PriceBookID
INNER JOIN tbl_PB_SubSection ss
ON ss.SectionID = s.SectionID
INNER JOIN tbl_PB_JobCodes jc
ON jc.SubSectionID = ss.SubSectionID
INNER JOIN tbl_PB_JobCodes_Details jcd
ON jc.JobCodeID = jcd.JobCodeID
INNER JOIN tbl_PB_Parts p
ON p.PartID = jcd.PartID
INNER JOIN tbl_PB_MasterParts mp
ON mp.MasterPartID = p.MasterPartID
WHERE pb.PriceBookID = @PriceBookID
ORDER BY jc.JobCode


select t.jobcode, t.jobcodedescription, count(t.jobcode) as frequency from @pbuploadTree t group by jobcode, t.jobcodedescription having count(t.jobcode) > 2 


--INSERT INTO eighthundred_new..tbl_PBU_Tree
select * from @pbuploadTree
--INSERT INTO eighthundred_new..tbl_PBU_Det_NewParts_ManualPrices
select * from @detnewpart

rollback TRAN
end

GO


