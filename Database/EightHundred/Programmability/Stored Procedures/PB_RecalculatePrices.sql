USE [EightHundred]
GO
/****** Object:  StoredProcedure [dbo].[PB_RecalculatePrices]    Script Date: 03/30/2012 13:12:28 ******/
DROP PROCEDURE [dbo].[PB_RecalculatePrices]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
Author: Tejas Shah
Date: 2011-12-20
*/
--PB_RecalculatePrices 56
CREATE PROC [dbo].[PB_RecalculatePrices]
	@PriceBookID INT
AS
BEGIN
	BEGIN TRY
		
		--;with cte as(
		--SELECT	jd.JobCodeID,
		--		j.JobCode,
		--		ss.SubSectionName,
		--		s.SectionName,
				
		--		CAST(SUM(jd.PartCost) AS MONEY) AS PartCost,
		--		CAST(SUM(jd.PartStdPrice) AS MONEY) AS PartStdPrice,
		--		CAST(SUM(jd.PartMemberPrice) AS MONEY) AS PartMemberPrice,
		--		CAST(SUM(jd.PartAddonStdPrice) AS MONEY) AS PartAddonStdPrice,
		--		CAST(SUM(jd.PartAddonMemberPrice) AS MONEY) AS PartAddonMemberPrice
		--FROM tbl_PB_JobCodes j
		--INNER JOIN tbl_PB_JobCodes_Details jd ON jd.JobCodeID = j.JobCodeID
		--INNER JOIN tbl_PB_SubSection ss ON ss.SubsectionID = j.SubSectionID
		--INNER JOIN tbl_PB_Section s ON s.sectionID = ss.SectionID
		--	AND s.PriceBookID = @PriceBookID
		--	--AND j.JobCode = 'F10010'
		--GROUP BY 
		--		jd.JobCodeID,
		--		j.JobCode,
		--		ss.SubSectionName,
		--		s.SectionName
		--)
		----SELECT	c.SectionName,
		----		c.SubSectionName,
		----		c.*,
				
		----		j.JobCost,
		----		j.JobStdPrice,
		----		j.JobMemberPrice,
		----		j.JobAddonStdPrice,
		----		j.JobAddonMemberPrice
		--UPDATE	j		
		--SET		JobCost = c.PartCost,
		--		JobStdPrice = c.PartStdPrice,
		--		JobMemberPrice = c.PartMemberPrice,
		--		JobAddonStdPrice = c.PartAddonStdPrice,
		--		JobAddonMemberPrice = c.PartAddonMemberPrice 
		--FROM	cte c
		--INNER JOIN tbl_PB_JobCodes j ON j.JobCodeID = c.JobCodeID
		--WHERE 
		--	(
		--		c.PartCost <> j.JobCost
		--		OR
		--		c.PartStdPrice <> j.JobStdPrice
		--		OR 
		--		c.PartMemberPrice <> j.JobMemberPrice
		--		OR
		--		c.PartAddonStdPrice <> j.JobAddonStdPrice
		--		OR 
		--		c.PartAddonMemberPrice <> j.JobAddonMemberPrice	
		--)
		EXEC RecalculateTree 0, @PriceBookID
			
	END TRY
	BEGIN CATCH
				
		DECLARE @msg VARCHAR(MAX)
		SELECT @msg = ERROR_MESSAGE()
		
		RAISERROR('Error occured in PB_RecalculatePrices, %s', 1, 16, @msg)
			
	END CATCH
END
GO
