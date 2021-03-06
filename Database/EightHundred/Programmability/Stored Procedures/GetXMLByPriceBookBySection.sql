USE [EightHundred]
GO
/****** Object:  StoredProcedure [dbo].[GetXMLByPriceBookBySection]    Script Date: 03/30/2012 13:12:27 ******/
DROP PROCEDURE [dbo].[GetXMLByPriceBookBySection]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC GetXMLByPriceBookBySection @PriceBookID = 41, @SectionID = 778, @XML=NULL
CREATE PROC [dbo].[GetXMLByPriceBookBySection]
	@PriceBookID INT,
	@SectionID INT,
	@XML VARCHAR(MAX)=NULL OUT
As
BEGIN 

	;WITH [crazinessSubSections] AS
	(
		SELECT SubSectionID, SubSectionName, 0 AS [IsFake]
		FROM tbl_PB_SubSection 
		WHERE SectionID = @SectionID
		UNION ALL
		SELECT -1 * s.sectionid, 'BusinessGuard', 1
		FROM tbl_pb_subsection ss
		INNER JOIN tbl_pb_section s
		ON ss.sectionid = s.sectionid
		WHERE s.sectionname like '%HomeGuard%'
		AND s.SectionID = @SectionID
		GROUP BY s.sectionid
		HAVING COUNT(CASE WHEN ss.subsectionname like '%BusinessGuard%' THEN 1 END) = 0
	)
	, [crazinessJobTasks] AS
	(
		SELECT	  jc.JobCodeID
				, jc.JobCode
				, jc.JobCodeDescription
				, jc.JobMemberPrice
				, jc.JobStdPrice
				, jc.JobAddonMemberPrice
				, jc.JobAddonStdPrice
				, jc.JobCost
				, ss.SubSectionID
				, ss.SubSectionName
				, ss.SectionID
				, jc.ActiveYN
				, 0 AS [IsFake]
				, CONVERT(BIT, CASE WHEN ss.SubSectionName LIKE '%HomeGuard%' THEN 1 ELSE 0 END) AS [IsHomeGuard]
		FROM tbl_PB_JobCodes jc
		INNER JOIN tbl_PB_SubSection ss
		ON jc.SubSectionID = ss.SubSectionID
		WHERE ss.SectionID = @SectionID AND jc.ActiveYN = 1 AND ss.ActiveYN = 1
		UNION ALL 
		SELECT -1 * ss.SubSectionID, 'A00110', 'HomeGuard - Do Not Use', 0, 0, 0, 0, 0, ss.SubSectionID, ss.SubSectionName, ss.SectionID, 1, 1, 1 
		FROM tbl_PB_SubSection ss
		WHERE ss.SectionID = @SectionID AND ss.SubSectionName LIKE '%HomeGuard%'
		UNION ALL 
		SELECT SubSectionID, 'A00500','Business Interruption Prevention', 0, 0, 0, 0, 0, SubSectionID, SubSectionName, -1 * SubSectionID, 1, 1, 0 FROM [crazinessSubSections] WHERE IsFake = 1
		
	)
	, [crazinessJobTaskDetails] AS 
	(
		SELECT jd.JobCodeID, jd.JobCodeDetailsID, mp.PartCode, mp.PartName, jd.Qty, jd.PartMemberPrice, jd.PartStdPrice, jd.PartAddonMemberPrice, jd.PartAddonStdPrice, jd.PartCost 
		FROM tbl_PB_JobCodes_Details jd
		INNER JOIN tbl_PB_Parts p ON p.PartID = jd.PartID
		INNER JOIN tbl_PB_MasterParts mp ON mp.MasterPartID = p.MasterPartID
		INNER JOIN [crazinessJobTasks] jt ON jt.JobCodeID = jd.JobCodeID AND [IsFake] = 0
		UNION ALL 
		SELECT -1 * SubSectionID, -1, 'HG-101000', 'HomeGuard - Do Not Use', 0, 0, 0, 0, 0, 0
		FROM [crazinessJobTasks] WHERE IsFake = 1 AND IsHomeGuard = 1
		UNION ALL 
		SELECT SubSectionID, -1, 'HG-200000', 'BusinessGuard', 0, 0, 0, 0, 0, 0
		FROM [crazinessJobTasks] WHERE IsFake = 1 AND IsHomeGuard = 0
	)

	SELECT @XML = CAST( 
	(SELECT	p.FranchiseID AS 'FranchiseID',
			(
				SELECT	REPLACE(REPLACE(ss.SubSectionName,',','-'),'/','-') AS 'PriceBookSubSection',
						(
							SELECT	REPLACE(j.JobCode,',','-') + ',' + REPLACE(REPLACE(j.JobCodeDescription,',','-'),'/','-') AS 'PBTask',
									CONVERT(VARCHAR, j.JobMemberPrice, 1) AS 'JobMemberPrice',
									CONVERT(VARCHAR, j.JobStdPrice, 1) AS 'JobStdPrice',
									CONVERT(VARCHAR, j.JobAddonMemberPrice, 1) AS 'JobAddonMemberPrice',
									CONVERT(VARCHAR, j.JobAddonStdPrice, 1) AS 'JobAddonStdPrice',
									CONVERT(VARCHAR, j.JobCost, 1) AS JobCost,
									CASE WHEN j.IsHomeGuard = 1
										THEN CASE WHEN j.JobCodeDescription LIKE '%Inspection%' OR j.JobCodeDescription LIKE '%Prevention%' 
											 THEN 'False' ELSE 'True' END
										ELSE NULL END AS [HomeGuardPlanYN],
									(
										SELECT  jd.PartCode AS 'PartCode',
												REPLACE(REPLACE(jd.PartName,',','-'),'/','-') AS 'PartName',
												CONVERT(DECIMAL(10,4), jd.Qty) AS 'QTY',
												CONVERT(VARCHAR, jd.PartMemberPrice, 1) AS 'PartMemberPrice',
												CONVERT(VARCHAR, jd.PartStdPrice, 1) AS 'PartStdPrice',
												CONVERT(VARCHAR, jd.PartAddonMemberPrice, 1) AS 'PartAddonMemberPrice',
												CONVERT(VARCHAR, jd.PartAddonStdPrice, 1) AS 'PartAddonStdPrice',
												CONVERT(VARCHAR, jd.PartCost, 1) AS 'PartCost'
										FROM [crazinessJobTaskDetails] jd
										WHERE	jd.JobCodeID = j.JobCodeID
										ORDER BY jd.JobCodeDetailsID
										FOR XML PATH('Part'), ROOT ('TaskParts'), TYPE
									)
							from [crazinessJobTasks] j
							WHERE j.SubSectionID = ss.SubsectionID
							order by j.JobCodeID
							FOR XML PATH('Task'), TYPE
						)
				from [crazinessSubSections] ss
				ORDER BY ss.IsFake, ss.SubsectionID
				FOR XML PATH('SubSection'), TYPE
			)
		FROM tbl_PB_Section s 
		INNER JOIN tbl_PriceBook p ON p.PriceBookID = s.PriceBookID
		WHERE s.SectionID = @SectionID
		ORDER BY s.SectionID
		FOR XML PATH(''), ROOT ('PriceBook')
	) AS VARCHAR(MAX))
	
	--SELECT @XML
	
END
GO
