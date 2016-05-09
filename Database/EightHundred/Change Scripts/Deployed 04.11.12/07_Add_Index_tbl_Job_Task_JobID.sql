USE [EightHundred]
GO

/****** Object:  Index [IX_tbl_Job_Tasks_JobID]    Script Date: 04/10/2012 10:21:09 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tbl_Job_Tasks]') AND name = N'IX_tbl_Job_Tasks_JobID')
DROP INDEX [IX_tbl_Job_Tasks_JobID] ON [dbo].[tbl_Job_Tasks] WITH ( ONLINE = OFF )
GO

USE [EightHundred]
GO

/****** Object:  Index [IX_tbl_Job_Tasks_JobID]    Script Date: 04/10/2012 10:21:11 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Job_Tasks_JobID] ON [dbo].[tbl_Job_Tasks] 
(
	[JobID] ASC
)
INCLUDE ( [AddOnYN],
[Price],
[JobCode],
[JobCodeDescription],
[Quantity],
[AccountCode]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

