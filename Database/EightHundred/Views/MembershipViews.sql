-- =============================================
-- Script Template
-- =============================================
USE [EightHundred]
GO
/****** Object:  View [dbo].[vRPT_MembershipInfo]    Script Date: 04/11/2012 17:21:28 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRPT_MembershipInfo]'))
DROP VIEW [dbo].[vRPT_MembershipInfo]
GO
/****** Object:  View [dbo].[vRpt_Membership]    Script Date: 04/11/2012 17:21:28 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_Membership]'))
DROP VIEW [dbo].[vRpt_Membership]
GO
/****** Object:  View [dbo].[vRpt_MembershipInspections]    Script Date: 04/11/2012 17:21:28 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_MembershipInspections]'))
DROP VIEW [dbo].[vRpt_MembershipInspections]
GO
/****** Object:  View [dbo].[VPartial_Membertype_ACTuneUpTemplate]    Script Date: 04/11/2012 17:21:27 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VPartial_Membertype_ACTuneUpTemplate]'))
DROP VIEW [dbo].[VPartial_Membertype_ACTuneUpTemplate]
GO
/****** Object:  View [dbo].[VPartial_Membertype_PlumbingInspectionTemplate]    Script Date: 04/11/2012 17:21:27 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VPartial_Membertype_PlumbingInspectionTemplate]'))
DROP VIEW [dbo].[VPartial_Membertype_PlumbingInspectionTemplate]
GO
/****** Object:  View [dbo].[VPartial_MemberType_WaterHeaterFlusingTemplate]    Script Date: 04/11/2012 17:21:28 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VPartial_MemberType_WaterHeaterFlusingTemplate]'))
DROP VIEW [dbo].[VPartial_MemberType_WaterHeaterFlusingTemplate]
GO
/****** Object:  View [dbo].[VPartial_MemberType_WaterHeaterFlusingTemplate]    Script Date: 04/11/2012 17:21:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VPartial_MemberType_WaterHeaterFlusingTemplate]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VPartial_MemberType_WaterHeaterFlusingTemplate]
AS
SELECT     dbo.tbl_MemberType_VisitsTypes.MemberTypeID, 12 / vistype1.VisitFrequency AS WaterHeaterFlushingPerAnnum
FROM         dbo.tbl_MemberType_VisitsTypes INNER JOIN
                      dbo.tbl_VisitType AS vistype1 ON dbo.tbl_MemberType_VisitsTypes.VisitTypeID = vistype1.VisitTypeID AND vistype1.VisitTypeID = 2
'
GO
/****** Object:  View [dbo].[VPartial_Membertype_PlumbingInspectionTemplate]    Script Date: 04/11/2012 17:21:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VPartial_Membertype_PlumbingInspectionTemplate]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VPartial_Membertype_PlumbingInspectionTemplate]
AS
SELECT     dbo.tbl_MemberType_VisitsTypes.MemberTypeID, 12 / vistype1.VisitFrequency AS PlumbingInspectionPerAnnum
FROM         dbo.tbl_MemberType_VisitsTypes INNER JOIN
                      dbo.tbl_VisitType AS vistype1 ON dbo.tbl_MemberType_VisitsTypes.VisitTypeID = vistype1.VisitTypeID AND vistype1.VisitTypeID = 1
'
GO
/****** Object:  View [dbo].[VPartial_Membertype_ACTuneUpTemplate]    Script Date: 04/11/2012 17:21:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VPartial_Membertype_ACTuneUpTemplate]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VPartial_Membertype_ACTuneUpTemplate]
AS
SELECT     dbo.tbl_MemberType_VisitsTypes.MemberTypeID, 12 / vistype1.VisitFrequency AS HeatingCoolingTuneUpPerAnnum
FROM         dbo.tbl_MemberType_VisitsTypes INNER JOIN
                      dbo.tbl_VisitType AS vistype1 ON dbo.tbl_MemberType_VisitsTypes.VisitTypeID = vistype1.VisitTypeID AND vistype1.VisitTypeID = 3
'
GO
/****** Object:  View [dbo].[vRpt_MembershipInspections]    Script Date: 04/11/2012 17:21:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_MembershipInspections]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[vRpt_MembershipInspections]
AS
SELECT     dbo.vRpt_JobDetail.JobID, dbo.vRpt_JobDetail.ClientID, dbo.vRpt_Job.CallCompleted, dbo.vRpt_Job.CustomerID
FROM         dbo.vRpt_JobDetail INNER JOIN
                      dbo.vRpt_Job ON dbo.vRpt_JobDetail.JobID = dbo.vRpt_Job.TicketNumber AND dbo.vRpt_JobDetail.ClientID = dbo.vRpt_Job.ClientID
WHERE     (dbo.vRpt_JobDetail.JobCode = ''A00100'') AND (dbo.vRpt_JobDetail.Amount = 0) OR
                      (dbo.vRpt_JobDetail.JobCode = ''A00200'') AND (dbo.vRpt_JobDetail.Amount = 0)
UNION 
SELECT  [TicketNumber] as Jobid
      ,[ClientID]
      ,[CallCompleted]
      ,[CustomerID]
  FROM [EightHundred].[dbo].[vRpt_Job]
  where [JobPriorityID] = 6 and (statusid = 6 or statusid = 7)
'
GO
/****** Object:  View [dbo].[vRpt_Membership]    Script Date: 04/11/2012 17:21:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_Membership]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[vRpt_Membership]
AS
SELECT     TOP (100) PERCENT cm.MemberID AS MembershipID, cm.CustomerID AS billtoCustomerID, cm.FranchiseID AS ClientID, 
                      dbo.vRpt_MembershipInspections.ClientID AS InspectionClientID, cm.MemberTypeID, dbo.tbl_Member_Type.MemberType, cm.StartDate AS MembershipStartDate, 
                      cm.EndDate AS MembershipEndDate, ISNULL(dbo.VPartial_Membertype_PlumbingInspectionTemplate.PlumbingInspectionPerAnnum, 0) 
                      AS req_PlumbingInspectionsPerYear, ISNULL(dbo.VPartial_MemberType_WaterHeaterFlusingTemplate.WaterHeaterFlushingPerAnnum, 0) 
                      AS req_WaterHeaterFlushingsPerYear, ISNULL(dbo.VPartial_Membertype_ACTuneUpTemplate.HeatingCoolingTuneUpPerAnnum, 0) AS req_ACTuneUpsPerYear, 
                      ISNULL(dbo.VPartial_Membertype_PlumbingInspectionTemplate.PlumbingInspectionPerAnnum, 0) 
                      + ISNULL(dbo.VPartial_MemberType_WaterHeaterFlusingTemplate.WaterHeaterFlushingPerAnnum, 0) 
                      + ISNULL(dbo.VPartial_Membertype_ACTuneUpTemplate.HeatingCoolingTuneUpPerAnnum, 0) AS req_TotalInspectionsPerYear, 
                      COUNT(dbo.vRpt_MembershipInspections.JobID) AS CountOfInspections, ISNULL(dbo.VPartial_Membertype_ACTuneUpTemplate.HeatingCoolingTuneUpPerAnnum, 0) 
                      AS bal_ACTuneUpsPerYear, MAX(dbo.vRpt_MembershipInspections.CallCompleted) AS LastDateInspected, ISNULL(dbo.vRpt_CustomerVisits.CountJobs, 0) 
                      AS CountCustomerVisits, dbo.vRpt_CustomerVisits.LastVisit AS LastCustomerVisit
FROM         dbo.tbl_Customer_Members AS cm LEFT OUTER JOIN
                      dbo.tbl_Member_Type ON cm.MemberTypeID = dbo.tbl_Member_Type.MemberTypeID LEFT OUTER JOIN
                      dbo.vRpt_CustomerVisits ON cm.CustomerID = dbo.vRpt_CustomerVisits.CustomerID LEFT OUTER JOIN
                      dbo.vRpt_MembershipInspections ON cm.CustomerID = dbo.vRpt_MembershipInspections.CustomerID LEFT OUTER JOIN
                      dbo.VPartial_Membertype_PlumbingInspectionTemplate ON 
                      cm.MemberTypeID = dbo.VPartial_Membertype_PlumbingInspectionTemplate.MemberTypeID LEFT OUTER JOIN
                      dbo.VPartial_MemberType_WaterHeaterFlusingTemplate ON 
                      cm.MemberTypeID = dbo.VPartial_MemberType_WaterHeaterFlusingTemplate.MemberTypeID LEFT OUTER JOIN
                      dbo.VPartial_Membertype_ACTuneUpTemplate ON cm.MemberTypeID = dbo.VPartial_Membertype_ACTuneUpTemplate.MemberTypeID
GROUP BY cm.MemberID, cm.CustomerID, cm.FranchiseID, dbo.vRpt_MembershipInspections.ClientID, cm.MemberTypeID, cm.StartDate, cm.EndDate, 
                      ISNULL(dbo.VPartial_Membertype_PlumbingInspectionTemplate.PlumbingInspectionPerAnnum, 0), 
                      ISNULL(dbo.VPartial_MemberType_WaterHeaterFlusingTemplate.WaterHeaterFlushingPerAnnum, 0), 
                      ISNULL(dbo.VPartial_Membertype_ACTuneUpTemplate.HeatingCoolingTuneUpPerAnnum, 0), 
                      ISNULL(dbo.VPartial_Membertype_PlumbingInspectionTemplate.PlumbingInspectionPerAnnum, 0) 
                      + ISNULL(dbo.VPartial_MemberType_WaterHeaterFlusingTemplate.WaterHeaterFlushingPerAnnum, 0) 
                      + ISNULL(dbo.VPartial_Membertype_ACTuneUpTemplate.HeatingCoolingTuneUpPerAnnum, 0), dbo.vRpt_CustomerVisits.CountJobs, 
                      dbo.vRpt_CustomerVisits.LastVisit, dbo.tbl_Member_Type.MemberType
ORDER BY billtoCustomerID
'
GO
/****** Object:  View [dbo].[vRPT_MembershipInfo]    Script Date: 04/11/2012 17:21:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRPT_MembershipInfo]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[vRPT_MembershipInfo]
AS
SELECT     dbo.vRpt_Membership.MembershipID, dbo.vRpt_Customer.CustomerID, dbo.vRpt_Customer.CustomerName, dbo.vRpt_Customer.Email, 
                      dbo.vRpt_Customer.PrimaryPhone, dbo.vRpt_Customer.CellPhone, dbo.vRpt_Customer.BillToAddress, dbo.vRpt_Customer.BillToCity, dbo.vRpt_Customer.BillToState, 
                      dbo.vRpt_Customer.BillToPostalCode, dbo.vRpt_Customer.BillToCountry, dbo.vRpt_Membership.MemberTypeID, dbo.vRpt_Membership.MemberType, 
                      dbo.vRpt_Membership.MembershipStartDate, dbo.vRpt_Membership.MembershipEndDate, dbo.vRpt_Membership.req_TotalInspectionsPerYear, 
                      dbo.vRpt_Membership.CountOfInspections, dbo.vRpt_Membership.LastDateInspected, dbo.vRpt_Membership.LastCustomerVisit, 
                      dbo.vRpt_Membership.CountCustomerVisits, dbo.vRpt_Customer.JobCount, dbo.vRpt_Customer.TotalSales, dbo.vRpt_Customer.AverageJob, 
                      dbo.vRpt_Customer.Payments, dbo.vRpt_Customer.Balance, dbo.vRpt_Customer.ClientID
FROM         dbo.vRpt_Customer INNER JOIN
                      dbo.vRpt_Membership ON dbo.vRpt_Customer.CustomerID = dbo.vRpt_Membership.billtoCustomerID
'
GO
