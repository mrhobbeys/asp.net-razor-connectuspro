USE [EightHundred]
GO

/*
	Description: Standard "picklist" for Overtime Method, currently used on Payroll Setup
*/

/****** Object:  Table [dbo].[tbl_OvertimeMethod]    Script Date: 04/07/2012 09:20:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

/****** Object:  Table [dbo].[tbl_OvertimeMethod]    Script Date: 04/07/2012 09:20:03 ******/
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_OvertimeMethod]') AND type in (N'U'))
BEGIN
	PRINT('Creating Table: tbl_OvertimeMethod')

CREATE TABLE [dbo].[tbl_OvertimeMethod](
	[OvertimeMethodID] [int] NOT NULL,
	[OvertimeMethod] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbl_OvertimeMethod] PRIMARY KEY CLUSTERED 
(
	[OvertimeMethodID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


	PRINT('Table Created: tbl_OvertimeMethod')
END
ELSE
BEGIN
	PRINT('Table Already Exists: tbl_OvertimeMethod')	
END

/*
	Add enum values
*/
IF NOT EXISTS (SELECT * FROM tbl_OvertimeMethod WHERE OvertimeMethod = 'Weekly')
BEGIN
	PRINT('Inserting value: Weekly')
	INSERT INTO tbl_OvertimeMethod VALUES (1, 'Weekly')
END
ELSE
BEGIN
	PRINT('Value already exists: Weekly')	
END
IF NOT EXISTS (SELECT * FROM tbl_OvertimeMethod WHERE OvertimeMethod = 'Daily')
BEGIN
	PRINT('Inserting value: Daily')
	INSERT INTO tbl_OvertimeMethod VALUES (2, 'Daily')
END
ELSE
BEGIN
	PRINT('Value already exists: Daily')	
END

SELECT * FROM tbl_OvertimeMethod

/*
	- Now add and chekc the constraint
*/

IF  NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_HR_PayrollSetup_tbl_OvertimeMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_HR_PayrollSetup]'))
ALTER TABLE [dbo].[tbl_HR_PayrollSetup]  WITH CHECK ADD  CONSTRAINT [FK_tbl_HR_PayrollSetup_tbl_OvertimeMethod] FOREIGN KEY([OvertimeMethod])
REFERENCES [dbo].[tbl_OvertimeMethod] ([OvertimeMethodID])
GO

ALTER TABLE [dbo].[tbl_HR_PayrollSetup] CHECK CONSTRAINT [FK_tbl_HR_PayrollSetup_tbl_OvertimeMethod]
GO

SET ANSI_PADDING OFF
GO


