﻿-- =============================================
-- Script Template
-- =============================================
USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_AccountingDetail]    Script Date: 04/10/2012 10:58:57 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_AccountingDetail]'))
DROP VIEW [dbo].[vRpt_AccountingDetail]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_AccountingDetail]    Script Date: 04/10/2012 10:58:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vRpt_AccountingDetail]
AS
SELECT      row_number() OVER (ORDER BY dbo.vRpt_Job.ClientID) AS uID, dbo.vRpt_Job.ClientID, dbo.vRpt_JobDetail.AccountCode, dbo.tbl_Account_Codes.AccountName, dbo.vRpt_Job.TicketNumber AS JOBCOUNT, 
                      SUM(dbo.vRpt_JobDetail.Amount) AS TotalSales, dbo.vRpt_Job.ServiceName, dbo.vRpt_Job.BusinessType, MIN(DISTINCT dbo.vRpt_Job.CallCompleted) AS JobDate, 
                      dbo.tbl_Job.WSRCompletedDate
FROM         dbo.tbl_Job INNER JOIN
                      dbo.vRpt_JobDetail ON dbo.tbl_Job.JobID = dbo.vRpt_JobDetail.JobID LEFT OUTER JOIN
                      dbo.tbl_Account_Codes ON dbo.vRpt_JobDetail.AccountCode = dbo.tbl_Account_Codes.AccountCode RIGHT OUTER JOIN
                      dbo.vRpt_Job ON dbo.vRpt_JobDetail.JobID = dbo.vRpt_Job.TicketNumber
WHERE     (dbo.tbl_Job.LockedYN = 1)
GROUP BY dbo.vRpt_JobDetail.AccountCode, dbo.vRpt_Job.ClientID, dbo.vRpt_Job.ServiceName, dbo.vRpt_Job.BusinessType, dbo.tbl_Account_Codes.AccountName, 
                      dbo.tbl_Job.WSRCompletedDate, dbo.vRpt_Job.TicketNumber
HAVING      (NOT (dbo.tbl_Job.WSRCompletedDate IS NULL))

GO
