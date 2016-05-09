USE [EightHundred]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TIMESHEET_EMPLOYEE]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_HR_TimeSheet]'))
ALTER TABLE [dbo].[tbl_HR_TimeSheet] DROP CONSTRAINT [FK_TIMESHEET_EMPLOYEE]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TIMESHEET_PAYROLLDETAILS]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_HR_TimeSheet]'))
ALTER TABLE [dbo].[tbl_HR_TimeSheet] DROP CONSTRAINT [FK_TIMESHEET_PAYROLLDETAILS]
GO

/****** Object:  Table [dbo].[tbl_HR_TimeSheet]    Script Date: 03/30/2012 10:41:55 ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_HR_TimeSheet]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[tbl_HR_TimeSheet](
	[TimeSheetID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [int] NOT NULL,
	[PayrollDetailID] [int] NULL,
	[WeekEndingDateOn] [datetime] NOT NULL,
	[SundayHours] [decimal](8, 4) NOT NULL,
	[MondayHours] [decimal](8, 4) NOT NULL,
	[TuesdayHours] [decimal](8, 4) NOT NULL,
	[WednesdayHours] [decimal](8, 4) NOT NULL,
	[ThursdayHours] [decimal](8, 4) NOT NULL,
	[FridayHours] [decimal](8, 4) NOT NULL,
	[SaturdayHours] [decimal](8, 4) NOT NULL,
	[TimeStamp] [timestamp] NULL,
 CONSTRAINT [PK_tbl_HR_TimeSheet] PRIMARY KEY CLUSTERED 
(
	[TimeSheetID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO

ALTER TABLE [dbo].[tbl_HR_TimeSheet]  WITH CHECK ADD  CONSTRAINT [FK_TIMESHEET_EMPLOYEE] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[tbl_Employee] ([EmployeeID])
GO

ALTER TABLE [dbo].[tbl_HR_TimeSheet] CHECK CONSTRAINT [FK_TIMESHEET_EMPLOYEE]
GO

ALTER TABLE [dbo].[tbl_HR_TimeSheet]  WITH CHECK ADD  CONSTRAINT [FK_TIMESHEET_PAYROLLDETAILS] FOREIGN KEY([PayrollDetailID])
REFERENCES [dbo].[tbl_PayrollDetails] ([PayrollDetailID])
GO

ALTER TABLE [dbo].[tbl_HR_TimeSheet] CHECK CONSTRAINT [FK_TIMESHEET_PAYROLLDETAILS]
GO

