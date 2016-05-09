USE [EightHundred]
GO

/****** Object:  Table [dbo].[tbl_HR_PayrollSpiffs]    Script Date: 04/07/2012 13:54:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_HR_PayrollSpiffs]') AND type in (N'U'))
DROP TABLE [dbo].[tbl_HR_PayrollSpiffs]
GO

USE [EightHundred]
GO

/****** Object:  Table [dbo].[tbl_HR_PayrollSpiffs]    Script Date: 04/07/2012 13:54:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_HR_PayrollSpiffs](
	[PayrollSpiffID]	[int]	IDENTITY(1,1) NOT NULL,
	[PayrollSetupID]	[int]	NOT NULL,
	[JobCodeID]			[int]	NOT NULL,	-- tied to tbl_PB_JobCodes
	[ServiceProID] [int] NOT NULL,			-- tbl_Employee
	[PayTypeID] [int] NOT NULL,
	[Rate] decimal(10,6) NOT NULL,
	[DateExpires] [datetime] NULL,
	[Comments] [text] NULL,
	[AddonYN] [bit] NOT NULL,
	[timestamp] [timestamp] NULL,
	[ActiveYN] [bit] NOT NULL
 CONSTRAINT [PK_tbl_HR_PayrollSpiffs] PRIMARY KEY CLUSTERED 
(
	[PayrollSpiffID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



GO

IF  NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_HR_PayrollSpiffs_tbl_HR_PayrollSetup]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_HR_PayrollSpiffs]'))
ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs]  WITH CHECK ADD  CONSTRAINT FK_tbl_HR_PayrollSpiffs_tbl_HR_PayrollSetup FOREIGN KEY([PayrollSetupID])
REFERENCES [dbo].tbl_HR_PayrollSetup ([PayrollSetupID])
GO

ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs] CHECK CONSTRAINT FK_tbl_HR_PayrollSpiffs_tbl_HR_PayrollSetup
GO


IF  NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_HR_PayrollSpiffs_tbl_PB_JobCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_HR_PayrollSpiffs]'))
ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs]  WITH CHECK ADD  CONSTRAINT [FK_tbl_HR_PayrollSpiffs_tbl_PB_JobCodes] FOREIGN KEY([JobCodeID])
REFERENCES [dbo].tbl_PB_JobCodes ([JobCodeID])
GO

ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs] CHECK CONSTRAINT [FK_tbl_HR_PayrollSpiffs_tbl_PB_JobCodes]
GO

IF  NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_HR_PayrollSpiffs_tbl_Employee]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_HR_PayrollSpiffs]'))
ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs]  WITH CHECK ADD  CONSTRAINT [FK_tbl_HR_PayrollSpiffs_tbl_Employee] FOREIGN KEY([ServiceProID])
REFERENCES [dbo].tbl_Employee (EmployeeID)
GO

ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs] CHECK CONSTRAINT [FK_tbl_HR_PayrollSpiffs_tbl_Employee]
GO

IF  NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_HR_PayrollSpiffs_tbl_Spiff_PayType]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_HR_PayrollSpiffs]'))
ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs]  WITH CHECK ADD  CONSTRAINT [FK_tbl_HR_PayrollSpiffs_tbl_Spiff_PayType] FOREIGN KEY([PayTypeID])
REFERENCES [dbo].[tbl_Spiff_PayType] ([SpiffPayTypeID])
GO

ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs] CHECK CONSTRAINT [FK_tbl_HR_PayrollSpiffs_tbl_Spiff_PayType]
GO

