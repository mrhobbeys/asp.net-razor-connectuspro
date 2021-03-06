USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_JobDetail]    Script Date: 04/14/2012 22:43:07 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_JobDetail]'))
DROP VIEW [dbo].[vRpt_JobDetail]
GO

USE [EightHundred]
GO


/****** Object:  View [dbo].[vRpt_JobDetail]    Script Date: 04/14/2012 22:43:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
/*
	2012.04.15 - BPanjavan - Added JobTaskID to list.  EF is doing a Select DISTINCT so this is a work-around

*/
CREATE VIEW [dbo].[vRpt_JobDetail]
AS
SELECT
	  JobID
	, j.ClientID
	, AccountCode
	, BusinessType
	,jt.JobTaskID
	, jt.Price AS Rate
	, jt.Quantity AS Quantity
	, jt.Price * jt.Quantity AS Amount
	, jt.JobCode
	, jt.JobCodeDescription
	, jt.AddOnYN as IsAddOn
FROM  dbo.vRpt_Job AS j INNER JOIN
      dbo.tbl_Job_Tasks jt ON j.TicketNumber = jt.JobID
      


GO


