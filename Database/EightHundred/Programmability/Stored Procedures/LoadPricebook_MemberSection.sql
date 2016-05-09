-- =============================================
-- Inserts Full Homeguard Membership Section into a given Pricebook
-- =============================================

-- still to do: 
-- 1) update parts table with information from the parts for that pricebook to not re-insert parts 
-- that are already defined for that pricebook
-- 2) Add input options to choose businessguard versus homeguard



USE [EightHundred]
GO

/****** Object:  StoredProcedure [dbo].[PB_LoadPricebook_Membersection]    Script Date: 04/18/2012 09:23:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PB_LoadPricebook_Membersection]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[PB_LoadPricebook_Membersection]
GO

USE [EightHundred]
GO

/****** Object:  StoredProcedure [dbo].[PB_LoadPricebook_Membersection]    Script Date: 04/18/2012 09:23:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





--PB_CopySection 56, 96
CREATE PROC [dbo].[PB_LoadPricebook_Membersection] (@PriceBookID INT, @FranchiseID int)
AS
BEGIN
BEGIN TRAN

DECLARE @LastSection INT
DECLARE @LastSubSection INT
DECLARE @LastJob INT
DECLARE @LastTaskDetail INT
DECLARE @LastPart INT
DECLARE @LastMasterPart INT





	
Declare @PBUploadTree Table(
BookName [varchar](50),	CompanyCodeID [int],	SectionName [varchar](50),	SubSectionName [varchar](50),	JobCode [varchar](50),	JobCodeDescription [varchar](50),	JobCost [money],	JobStdPrice [money],	JobMemberPrice [money],	JobAddonStdPrice [money],	JobAddonMemberPrice [money],	ResAccountCode [varchar](50),	ComAccountCode [varchar](50)
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
           
Declare @detMasterparts TABLE(
           JobCode varchar(10) NOT NULL,
	[MasterPartID] [int] NULL,
	[Qty] [nchar](10) NULL,
	[PartStdPrice] [money] NULL,
	[PartMemberPrice] [money] NULL,
	[PartAddonPrice] [money] NULL,
	[PartAddonMemberPrice] [money] NULL
)           

insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00110', 'HomeGuard Plan - Monthly', 0, 9.95, 9.95, 0, 0, '40090', '40390' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00130', 'HomeGuard Plan - 3 Years', 0, 189, 189, 0, 0, '40090', '40390' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00140', 'HomeGuard Plan - 5 Years', 0, 289, 289, 0, 0, '40090', '40390' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00100', 'HomeGuard Plumbing Analysis', 0, 0, 0, 0, 0, '40000', '40300' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00200', 'HomeGuard HVAC Analysis', 0, 0, 0, 0, 0, '41010', '41100' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00210', 'HomeGuard Agreement, 1 System, Monthly', 0, 22.95, 22.95, 0, 0, '41010', '41110' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00120', 'HomeGuard Plan - 1 Year', 0, 99, 99, 0, 0, '40090', '40390' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00220', 'HomeGuard Agreement, 1 System, 1 Year', 0, 199, 199, 0, 0, '41010', '41110' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00230', 'HomeGuard Agreement, 2 Systems, 1 Year', 0, 289, 289, 0, 0, '41010', '41110' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A00240', 'HomeGuard Agreement, 3 Systems, 1 Year', 0, 379, 379, 0, 0, '41010', '41110' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'BusinessGuard', 'A00500', 'Business Interruption Prevention', 0, 149, 149, 0, 0, '40400', '40400' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A000311', 'HomeGuard Shield 1 System 1year', 0, 239, 239, 239, 239, '40090', '40390' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A000321', 'HomeGuard Shield 2 Systems 1 year', 0, 329, 329, 329, 329, '40090', '40390' )
insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'HomeGuard', 'A000331', 'HomeGuard Shield 3 Systems I year', 0, 389, 389, 0, 0, '40090', '40390' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00110', 'HomeGuard Plan - Monthly', 0, 9.95, 9.95, 0, 0, '40090', '40390' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00130', 'HomeGuard Plan - 3 Years', 0, 189, 189, 0, 0, '40090', '40390' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00140', 'HomeGuard Plan - 5 Years', 0, 289, 289, 0, 0, '40090', '40390' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00100', 'HomeGuard Plumbing Analysis', 0, 0, 0, 0, 0, '40000', '40300' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00200', 'HomeGuard HVAC Analysis', 0, 0, 0, 0, 0, '41010', '41100' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00210', 'HomeGuard Agreement, 1 System, Monthly', 0, 22.95, 22.95, 0, 0, '41010', '41110' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00120', 'HomeGuard Plan - 1 Year', 0, 99, 99, 0, 0, '40090', '40390' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00220', 'HomeGuard Agreement, 1 System, 1 Year', 0, 199, 199, 0, 0, '41010', '41110' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00230', 'HomeGuard Agreement, 2 Systems, 1 Year', 0, 289, 289, 0, 0, '41010', '41110' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A00240', 'HomeGuard Agreement, 3 Systems, 1 Year', 0, 379, 379, 0, 0, '41010', '41110' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A000311', 'HomeGuard Shield 1 System 1year', 0, 239, 239, 239, 239, '40090', '40390' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A000321', 'HomeGuard Shield 2 Systems 1 year', 0, 329, 329, 329, 329, '40090', '40390' )
--insert into @PBUploadTree values ('HomeGuardBook', @FRANCHISEID , 'HomeGuard Member Plans', 'Business Guard I', 'A000331', 'HomeGuard Shield 3 Systems I year', 0, 389, 389, 0, 0, '40090', '40390' )


insert into @detnewpart values ('A000311', 'HG', 'HG-101001', 'HomeGuard Plan - 1 Year', 0, 'n/a', 1, 239, 239, 239, 239 )
insert into @detnewpart values ('A000321', 'HG', 'HG-101001', 'HomeGuard Plan - 1 Year', 0, 'n/a', 1, 329, 329, 329, 329 )
insert into @detnewpart values ('A000331', 'HG', 'HG-101001', 'HomeGuard Plan - 1 Year', 0, 'n/a', 1, 389, 389, 0, 0 )
insert into @detnewpart values ('A00100', 'HG', 'HG-102000', 'HomeGuard Plumbing Inspection', 0, 'n/a', 1, 0, 0, 0, 0 )
insert into @detnewpart values ('A00110', 'HG', 'HG-101000', 'HomeGuard Plan - Monthly', 0, 'n/a', 1, 9.95, 9.95, 9.95, 9.95 )
insert into @detnewpart values ('A00120', 'HG', 'HG-101001', 'HomeGuard Plan - 1 Year', 0, 'n/a', 1, 99, 99, 99, 99 )
insert into @detnewpart values ('A00130', 'HG', 'HG-101002', 'HomeGuard Plan - 3 Years', 0, 'n/a', 1, 189, 189, 189, 189 )
insert into @detnewpart values ('A00140', 'HG', 'HG-101003', 'HomeGuard Plan - 5 Years', 0, 'n/a', 1, 289, 289, 289, 289 )
insert into @detnewpart values ('A00200', 'HG', 'HG-102100', 'HomeGuard HVAC Inspection', 0, 'n/a', 1, 0, 0, 0, 0 )
insert into @detnewpart values ('A00210', 'HG', 'HG-101010', 'HomeGuard Agreement - Monthly', 0, 'n/a', 1, 22.95, 22.95, 22.95, 22.95 )
insert into @detnewpart values ('A00220', 'HG', 'HG-101011', 'HomeGuard Agreement, 1 System, 1 Year', 0, 'n/a', 1, 199, 199, 199, 199 )
insert into @detnewpart values ('A00230', 'HG', 'HG-101012', 'HomeGuard Agreement, 2 Systems, 1 Year', 0, 'n/a', 1, 289, 289, 289, 289 )
insert into @detnewpart values ('A00240', 'HG', 'HG-101013', 'HomeGuard Agreement, 3 Systems, 1 Year', 0, 'n/a', 1, 379, 379, 379, 379 )
insert into @detnewpart values ('A00500', 'HG', 'HG-200000', 'BusinessGuard', 0, 'N/a', 1, 149, 149, 149, 149 )




	
DECLARE @Sections TABLE(ID INT, PriceBookID INT, SectionName VARCHAR(50))
DECLARE @SubSections TABLE(ID INT, SectionID INT, SubSectionName VARCHAR(50))
DECLARE @Jobs TABLE(ID INT, SubSectionID INT, JobCode VARCHAR(10), JobCodeDescription VARCHAR(100), JobCost MONEY, JobStdPrice MONEY, JobMemberPrice MONEY, JobAddonStdPrice MONEY, JobAddonMemberPrice MONEY, ResAccountCode VARCHAR(5), ComAccountCode VARCHAR(5))
DECLARE @TaskDetails TABLE(ID INT, JobCodeID INT, PartID INT, Qty MONEY, PartCost MONEY, PartStdPrice MONEY, PartMemberPrice MONEY, PartAddonStdPrice MONEY, PartAddonMemberPrice MONEY)
DECLARE @MasterParts TABLE([MasterPartID] INT, [ConceptID] INT
				, FranchiseID INT
				, PartCode VARCHAR(25)
				, PartCodeID CHAR(2)
				, PartCost MONEY
				, PartName VARCHAR(100)
				, VendorPartID VARCHAR(20))
DECLARE @PriceBookParts TABLE(
	[ID] [int],
	[PriceBookID] [int] ,
	[MasterPartID] [int] ,
	[PartCost] [money] ,
	[PartStdPrice] [money] ,
	[PartMemberPrice] [money] ,
	[PartAddonStdPrice] [money] ,
	[PartAddonMemberPrice] [money] ,
	[Markup] [float])

SELECT @LastSection = MAX(SectionID) FROM tbl_PB_Section
SELECT @LastSubSection = MAX(SubSectionID) FROM tbl_PB_SubSection
SELECT @LastJob = MAX(JobCodeID) FROM tbl_PB_JobCodes
SELECT @LastTaskDetail = MAX(JobCodeDetailsID) FROM tbl_PB_JobCodes_Details
SELECT @LastPart = MAX(PartID) FROM tbl_PB_Parts
SELECT @LastMasterPart = MAX(MasterPartID) FROM tbl_PB_MasterParts


INSERT INTO @Sections
SELECT ROW_NUMBER() OVER(ORDER BY t.SectionName) + @LastSection
	 , min(@PriceBookID)
	 , t.SectionName
FROM @PBUploadTree t
group by t.SectionName 


INSERT INTO @SubSections
SELECT ROW_NUMBER() OVER(ORDER BY SubSectionName) + @LastSubSection
	 , s.ID AS [SectionID]
	 , SubSectionName
FROM @PBUploadTree t
INNER JOIN @Sections s
ON t.SectionName = s.SectionName
GROUP BY s.ID, SubSectionName

INSERT INTO @Jobs
SELECT ROW_NUMBER() OVER(ORDER BY JobCode) + @LastJob
	 , ss.ID AS [SubSectionID]
	 , JobCode
	 , min(JobCodeDescription) as JobCodeDescription
	 , min(JobCost) as JobCost
	 , min(JobStdPrice) as JobStdPrice
	 , min(JobMemberPrice) as JobMemberPrice
	 , min(JobAddonStdPrice) as JobAddonStdPrice
	 , min(JobAddonMemberPrice) as JobAddonMemberPrice
	 , min(ResAccountCode) as ResAccountCode
	 , min(ComAccountCode) as ComAccountCode
FROM @PBUploadTree t
INNER JOIN @SubSections ss
ON t.SubSectionName = ss.SubSectionName
GROUP BY ss.ID, JobCode 

INSERT INTO @MasterParts
SELECT  ROW_NUMBER() OVER (ORDER BY p.PartCodeID) + @LastMasterPart AS [MasterPartID]
				, min(1) AS [ConceptID]
				, min(@FranchiseID) as [franchiseid]
				, p.PartCode
				, p.PartCodeID
				, MIN(p.PartCost)
				, p.PartName
				, p.VendorPartID
FROM @detnewpart p
group by   p.PartCode, p.PartCodeID, p.PartName, p.VendorPartID 


;WITH 
[allPriceBookParts] AS
(
	SELECT JobCode, MasterPartID, Qty, PartStdPrice, PartMemberPrice, PartAddonPrice, PartAddonMemberPrice
	FROM @detMasterparts
	UNION ALL
	SELECT p.JobCode, mp.MasterPartID, p.Qty, p.PartStdPrice, p.PartMemberPrice, p.PartAddonPrice, p.PartAddonMemberPrice
	FROM @detnewpart p
	INNER JOIN @MasterParts mp
	ON p.PartCode = mp.PartCode 
		AND p.PartCodeID = mp.PartCodeID 
		AND p.VendorPartID = mp.VendorPartID 
		AND p.PartCost = mp.PartCost 
		AND p.PartName = mp.PartName
)
, [allMasterParts] AS
(
	SELECT MasterPartID, FranchiseID, PartCost FROM tbl_PB_MasterParts
	UNION ALL
	SELECT MasterPartID, FranchiseID, PartCost FROM @MasterParts
)

INSERT INTO @PriceBookParts(ID, Markup, MasterPartID, PartAddonMemberPrice, PartAddonStdPrice, PartCost, PartMemberPrice, PartStdPrice, PriceBookID)
SELECT ROW_NUMBER() OVER (ORDER BY MIN(p.JobCode)) + @LastPart AS [PartID]
	, MIN(mu.Markup) AS [Markup]
	, p.MasterPartID
	, MIN(ISNULL(p.PartAddonMemberPrice, mp.PartCost * p.Qty * mu.Markup * (100-0)/100)) AS PartAddonMemberPrice
	, MIN(ISNULL(p.PartAddonPrice, mp.PartCost * p.Qty * mu.Markup)) AS PartAddonPrice
	, MIN(mp.PartCost) AS PartCost
	, MIN(ISNULL(p.PartMemberPrice, mp.PartCost * p.Qty * mu.Markup * (100-0)/100)) AS PartMemberPrice
	, MIN(ISNULL(p.PartStdPrice, mp.PartCost * p.Qty * mu.Markup)) AS PartStdPrice
	, Min(@PriceBookID)
FROM [allPriceBookParts] p
INNER JOIN [allMasterParts] mp
ON mp.MasterPartID = p.MasterPartID

INNER JOIN tbl_PB_Markup mu
ON mp.PartCost >= mu.Lowerbound AND mp.PartCost <= mu.Upperbound
GROUP BY  p.MasterPartID, mp.PartCost

;WITH [allTaskDetails] AS
(
	SELECT JobCode, MasterPartID, Qty, PartStdPrice, PartMemberPrice, PartAddonPrice, PartAddonMemberPrice
	FROM @detMasterparts
	UNION ALL
	SELECT p.JobCode, mp.MasterPartID, p.Qty, p.PartStdPrice, p.PartMemberPrice, p.PartAddonPrice, p.PartAddonMemberPrice
	FROM @detnewpart p
	INNER JOIN @MasterParts mp
	ON p.PartCode = mp.PartCode 
		AND p.PartCodeID = mp.PartCodeID 
		AND p.VendorPartID = mp.VendorPartID 
		AND p.PartCost = mp.PartCost 
		AND p.PartName = mp.PartName
), [justJobs] AS 
(
     SELECT  MIN(id) as ID
      ,  JobCode
	 , min(JobCodeDescription) as JobCodeDescription
	 , min(JobCost) as JobCost
	 , min(JobStdPrice) as JobStdPrice
	 , min(JobMemberPrice) as JobMemberPrice
	 , min(JobAddonStdPrice) as JobAddonStdPrice
	 , min(JobAddonMemberPrice) as JobAddonMemberPrice
	 , min(ResAccountCode) as ResAccountCode
	 , min(ComAccountCode) as ComAccountCode
FROM @Jobs 
group by JobCode 
)

INSERT INTO @TaskDetails(ID, JobCodeID, PartID, Qty, PartCost, PartStdPrice, PartMemberPrice, PartAddonStdPrice, PartAddonMemberPrice)
SELECT  ROW_NUMBER() OVER(ORDER BY atd.JobCode) + @LastTaskDetail 
	 , j.ID as [JobCodeID]
	 , p.ID AS [PartID]
	 , atd.Qty AS [Qty]
	 , p.PartCost
	 , ISNULL(atd.PartStdPrice, p.PartStdPrice)
	 , ISNULL(atd.PartMemberPrice, p.PartMemberPrice)
	 , ISNULL(atd.PartAddonPrice, p.PartAddonStdPrice)
	 , ISNULL(atd.PartAddonMemberPrice, p.PartAddonMemberPrice)
FROM [allTaskDetails] atd
INNER JOIN [justJobs] j
ON j.JobCode = atd.JobCode
INNER JOIN @PriceBookParts p
ON p.MasterPartID = atd.MasterPartID


----Check for duplicate parts on a task.
--SELECT JobCodeID, PartID, COUNT(1) FROM @TaskDetails
--group by JobCodeID, PartID
--having COUNT(1) > 1

--select * from @Jobs
--where ID in (258545, 258552)



SET IDENTITY_INSERT tbl_PB_Section ON
INSERT INTO tbl_PB_Section(ActiveYN, MFlag, PriceBookID, SectionID, SectionName)
SELECT 1, 0, PriceBookID, ID, SectionName
FROM @Sections
SET IDENTITY_INSERT tbl_PB_Section OFF

SET IDENTITY_INSERT tbl_PB_SubSection ON
INSERT INTO tbl_PB_SubSection(ActiveYN, MFlag, SectionID, SubSectionName, SubsectionID)
SELECT 1, 0, SectionID, SubSectionName, ID
FROM @SubSections
SET IDENTITY_INSERT tbl_PB_SubSection OFF

SET IDENTITY_INSERT tbl_PB_JobCodes ON
INSERT INTO tbl_PB_JobCodes( ActiveYN, ComAccountCode, JobAddonMemberPrice, JobAddonStdPrice, JobCode, JobCodeDescription, JobCodeID, JobCost, JobMemberPrice, JobStdPrice, MFlag, ManualPricingYN, ResAccountCode, SubSectionID)
SELECT Distinct 1, j.ComAccountCode, j.JobAddonMemberPrice, j.JobAddonStdPrice, j.JobCode, j.JobCodeDescription, j.ID, j.JobCost, j.JobMemberPrice, j.JobStdPrice, 0, 0, j.ResAccountCode, j.SubSectionID
FROM @Jobs j
SET IDENTITY_INSERT tbl_PB_JobCodes OFF

SET IDENTITY_INSERT tbl_PB_MasterParts ON
INSERT INTO tbl_PB_MasterParts(ActiveYN, ConceptID, FranchiseID, MasterPartID, PartCode, PartCodeID, PartCost, PartName, VendorPartID)
SELECT 1, 1, m.FranchiseID, m.MasterPartID, m.PartCode, m.PartCodeID, m.PartCost, m.PartName, m.VendorPartID
FROM @MasterParts m
SET IDENTITY_INSERT tbl_PB_MasterParts OFF

SET IDENTITY_INSERT tbl_PB_Parts ON
INSERT INTO tbl_PB_Parts(Markup, MasterPartID, PartAddonMemberPrice, PartAddonStdPrice, PartCost, PartID, PartMemberPrice, PartStdPrice, PriceBookID)
SELECT Markup, MasterPartID, PartAddonMemberPrice, PartAddonStdPrice, PartCost, ID, PartMemberPrice, PartStdPrice, PriceBookID FROM
@PriceBookParts
SET IDENTITY_INSERT tbl_PB_Parts OFF

SET IDENTITY_INSERT tbl_PB_JobCodes_Details ON
INSERT INTO tbl_PB_JobCodes_Details(JobCodeDetailsID, JobCodeID, ManualPricingYN, PartAddonMemberPrice, PartAddonStdPrice, PartCost, PartID, PartMemberPrice, PartStdPrice, Qty)
SELECT ID, JobCodeID, 0, PartAddonMemberPrice, PartAddonStdPrice, PartCost, PartID, PartMemberPrice, PartStdPrice, Qty
FROM @TaskDetails
SET IDENTITY_INSERT tbl_PB_JobCodes_Details OFF

-- insert standard extra part for pricebook adjustments
if not exists(Select * from tbl_PB_Parts where PriceBookID = @PriceBookID and MasterPartID = 5350)
INSERT INTO [tbl_PB_Parts]
           ([PriceBookID]
           ,[MasterPartID]
           ,[PartCost]
           ,[PartStdPrice]
           ,[PartMemberPrice]
           ,[PartAddonStdPrice]
           ,[PartAddonMemberPrice]
           ,[Markup])
     VALUES
           ( @PriceBookID 
           , 5350
           , 0
           , 0
           , 0
           , 0
           , 0
           , 0)


--;WITH [DefaultPricing] AS
--(
	
--)

     SELECT  MIN(id) as ID
      ,  JobCode
	 , min(JobCodeDescription) as JobCodeDescription
	 , min(JobCost) as JobCost
	 , min(JobStdPrice) as JobStdPrice
	 , min(JobMemberPrice) as JobMemberPrice
	 , min(JobAddonStdPrice) as JobAddonStdPrice
	 , min(JobAddonMemberPrice) as JobAddonMemberPrice
	 , min(ResAccountCode) as ResAccountCode
	 , min(ComAccountCode) as ComAccountCode
FROM @Jobs 
Group by JobCode 
select * From @Sections
select * From @SubSections
select * From @Jobs
select * from @MasterParts
select * from @PriceBookParts
select * From @TaskDetails



commit TRAN

END





GO


