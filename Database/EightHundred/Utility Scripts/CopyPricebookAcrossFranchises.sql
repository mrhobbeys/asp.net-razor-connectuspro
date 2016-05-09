DECLARE @PriceBookID INT
DECLARE @DestinationFranchiseId INT

SET @PriceBookID = 260
SET @DestinationFranchiseId = 50

BEGIN TRAN

INSERT INTO tbl_PBU_Tree
SELECT  jc.JobCodeID
		, pb.BookName
		, @DestinationFranchiseId
		, s.SectionName
		, ss.SubSectionName
		, jc.JobCode
		, jc.JobCodeDescription
		, jc.JobCost
		, jc.JobStdPrice
		, jc.JobMemberPrice
		, jc.JobAddonStdPrice
		, jc.JobAddonMemberPrice
		, jc.ResAccountCode
		, jc.ComAccountCode
FROM tbl_PriceBook pb
INNER JOIN tbl_PB_Section s
ON pb.PriceBookID = s.PriceBookID
INNER JOIN tbl_PB_SubSection ss
ON ss.SectionID = s.SectionID
INNER JOIN tbl_PB_JobCodes jc
ON jc.SubSectionID = ss.SubSectionID
WHERE pb.PriceBookID = @PriceBookID
ORDER BY pb.BookName


INSERT INTO tbl_PBU_Det_NewParts_ManualPrices
SELECT t.TreeID
	, mp.PartCodeID
	, mp.PartCode
	, mp.PartName
	, mp.PartCost
	, mp.VendorPartID
	, jcd.Qty
	, jcd.PartStdPrice
	, jcd.PartMemberPrice
	, jcd.PartAddonStdPrice
	, jcd.PartAddonMemberPrice
FROM tbl_PriceBook pb
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
INNER JOIN tbl_PBU_Tree t
ON t.SectionName = s.SectionName AND t.SubSectionName = ss.SubSectionName AND t.JobCode = jc.JobCode AND t.HashCode = jc.JobCodeID
WHERE pb.PriceBookID = @PriceBookID
ORDER BY jc.JobCode

COMMIT TRAN
