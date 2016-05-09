CREATE PROCEDURE PB_LoadFromExtractTables AS

BEGIN TRAN

DELETE FROM tbl_PBU_Det_MasterParts_ManualPrices
DELETE FROM tbl_PBU_Det_NewParts_ManualPrices
DELETE FROM tbl_PBU_Tree

INSERT INTO tbl_PBU_Tree
SELECT  0
		, BookName
		, CompanyCodeID
		, SectionName
		, SubSectionName
		, JobCode
		, JobCodeDescription
		, JobCost
		, JobStdPrice
		, JobMemberPrice
		, JobAddonStdPrice
		, JobAddonMemberPrice
		, ResAccountCode
		, ComAccountCode
FROM tbl_PBU_Extract


INSERT INTO tbl_PBU_Det_NewParts_ManualPrices
SELECT t.TreeID
	, PartCodeID
	, PartCode
	, PartName
	, PartCost
	, VendorPartID
	, Qty
	, PartStdPrice
	, PartMemberPrice
	, PartAddonPrice
	, PartAddonMemberPrice
FROM tbl_PBU_Extract_Det_NewParts_ManualPrices emp
INNER JOIN tbl_PBU_Extract e
ON e.JobCode = emp.JobCode
INNER JOIN tbl_PBU_Tree t
ON t.SectionName = e.SectionName AND t.SubSectionName = e.SubSectionName AND t.JobCode = e.JobCode
ORDER BY emp.JobCode

COMMIT TRAN
