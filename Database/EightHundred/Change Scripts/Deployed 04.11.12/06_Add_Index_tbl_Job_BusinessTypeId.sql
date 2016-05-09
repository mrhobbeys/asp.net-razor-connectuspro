USE [EightHundred]
GO

/****** Object:  Index [IX_tbl_Job_BusinessTypeID]    Script Date: 04/10/2012 10:20:07 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tbl_Job]') AND name = N'IX_tbl_Job_BusinessTypeID')
DROP INDEX [IX_tbl_Job_BusinessTypeID] ON [dbo].[tbl_Job] WITH ( ONLINE = OFF )
GO

USE [EightHundred]
GO

/****** Object:  Index [IX_tbl_Job_BusinessTypeID]    Script Date: 04/10/2012 10:20:10 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Job_BusinessTypeID] ON [dbo].[tbl_Job] 
(
	[BusinessTypeID] ASC
)
INCLUDE ( [JobID],
[FranchiseID],
[CustomerID],
[LocationID],
[ServiceProID],
[CallSourceID],
[CallCompleted],
[Balance],
[StatusID],
[TotalSales],
[InvoicedDate],
[JobPriorityID],
[ScheduleEnd],
[SubTotal],
[TaxAmount],
[TaxAuthorityID],
[ServiceID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

