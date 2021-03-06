USE [eighthundred]
GO
/****** Object:  View [dbo].[View_CustomerID_CountJobs]    Script Date: 03/30/2012 11:38:54 ******/
DROP VIEW [dbo].[View_CustomerID_CountJobs]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_CustomerID_CountJobs]
AS
SELECT     dbo.tbl_Customer_Info.CustomerID, COUNT(dbo.tbl_Job.JobID) AS CountJobs
FROM         dbo.tbl_Customer_Info INNER JOIN
                      dbo.tbl_Job ON dbo.tbl_Customer_Info.CustomerID = dbo.tbl_Job.CustomerID
GROUP BY dbo.tbl_Customer_Info.CustomerID
GO
