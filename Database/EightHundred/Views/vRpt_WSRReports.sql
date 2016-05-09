-- =============================================
-- Script to create vRpt_WSR and vRpt_WSR_Dates
-- =============================================
USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_WSR]    Script Date: 04/22/2012 15:30:24 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_WSR]'))
DROP VIEW [dbo].[vRpt_WSR]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_WSR]    Script Date: 04/22/2012 15:30:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vRpt_WSR]
AS
SELECT     j.WSRCompletedDate, j.JobID AS TicketNumber, j.CallCompleted AS CallCompleted, s.StatusID, s.Status, svc.ServiceID, svc.ServiceName, j.TotalSales, j.TaxAmount, 
                      j.SubTotal, CASE WHEN tr.TaxDescription IS NULL THEN (CASE WHEN j.taxamount = 0 THEN 'No Tax' ELSE 'Unspecified Tax' END) 
                      ELSE tr.TaxDescription END AS TaxDescription, bt.BusinessTypeID, bt.BusinessType, CASE WHEN c.CustomerName IS NULL AND c.CompanyName IS NULL 
                      THEN 'UNKNOWN CUSTOMER' WHEN c.CustomerName IS NULL OR
                      LTRIM(c.CustomerName) = '' THEN c.companyname WHEN c.companyname IS NULL OR
                      LTRIM(c.companyname) = '' THEN c.customername ELSE c.companyname + ' - ' + c.customername END AS CustomerName, l.Address AS JobAddress, 
                      l.City AS JobCity, l.State AS JobState, l.PostalCode AS JobPostalCode, j.FranchiseID AS ClientID, j.JobPriorityID, pr.JobPriority, j.ServiceProID, j.Balance
FROM         dbo.tbl_Job AS j INNER JOIN
                      dbo.tbl_Job_Status AS s ON j.StatusID = s.StatusID INNER JOIN
                      dbo.tbl_Customer AS c ON j.CustomerID = c.CustomerID INNER JOIN
                      dbo.tbl_Locations AS l ON j.LocationID = l.LocationID INNER JOIN
                      dbo.tbl_Customer_BusinessType AS bt ON j.BusinessTypeID = bt.BusinessTypeID INNER JOIN
                      dbo.tbl_Job_Priority AS pr ON pr.JobPriorityID = j.JobPriorityID LEFT OUTER JOIN
                      dbo.tbl_Services AS svc ON j.ServiceID = svc.ServiceID LEFT OUTER JOIN
                      dbo.tbl_TaxRates AS tr ON j.TaxAuthorityID = tr.TaxRateID

GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_WSR_Dates]    Script Date: 04/22/2012 15:31:24 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vRpt_WSR_Dates]'))
DROP VIEW [dbo].[vRpt_WSR_Dates]
GO

USE [EightHundred]
GO

/****** Object:  View [dbo].[vRpt_WSR_Dates]    Script Date: 04/22/2012 15:31:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[vRpt_WSR_Dates]
AS
SELECT DISTINCT WSRCompletedDate, ClientID
FROM         dbo.vRpt_WSR


GO



