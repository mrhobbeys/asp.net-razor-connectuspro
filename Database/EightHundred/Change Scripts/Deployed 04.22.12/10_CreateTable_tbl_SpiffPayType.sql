USE [EightHundred]
GO

/*
	Description: Standard "picklist" for Spiff Pay Type
*/

/****** Object:  Table [dbo].[tbl_Spiff_PayType]    Script Date: 04/07/2012 09:20:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

/****** Object:  Table [dbo].[tbl_Spiff_PayType]    Script Date: 04/07/2012 09:20:03 ******/
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_Spiff_PayType]') AND type in (N'U'))
BEGIN
	PRINT('Creating Table: tbl_Spiff_PayType')

	CREATE TABLE [dbo].[tbl_Spiff_PayType](
		[SpiffPayTypeID]	[int]						  NOT NULL,
		[SpiffPayType]	[varchar](50)				  NOT NULL,
	 CONSTRAINT [PK_tbl_Spiff_PayType] PRIMARY KEY CLUSTERED 
	(
		[SpiffPayTypeID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	PRINT('Table Created: tbl_Spiff_PayType')
END
ELSE
BEGIN
	PRINT('Table Already Exists: tbl_Spiff_PayType')	
END

/*
	Add enum values
*/
IF NOT EXISTS (SELECT * FROM tbl_Spiff_PayType WHERE [SpiffPayType] = 'Flat Rate')
BEGIN
	PRINT('Inserting value: Flat Rate')
	INSERT INTO tbl_Spiff_PayType VALUES (0, 'Flat Rate')
END
ELSE
BEGIN
	PRINT('Value already exists: Flat Rate')	
END
IF NOT EXISTS (SELECT * FROM tbl_Spiff_PayType WHERE [SpiffPayType] = 'Commission')
BEGIN
	PRINT('Inserting value: Commission')
	INSERT INTO tbl_Spiff_PayType VALUES (1, 'Commission')
END
ELSE
BEGIN
	PRINT('Value already exists: Commission')	
END

SELECT * FROM tbl_Spiff_PayType

/*
	- Now add and chekc the constraint
*/

IF  NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_HR_PayrollSpiffs_tbl_Spiff_PayType]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_HR_PayrollSpiffs]'))
ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs]  WITH CHECK ADD  CONSTRAINT FK_tbl_HR_PayrollSpiffs_tbl_Spiff_PayType FOREIGN KEY([PayTypeID])
REFERENCES [dbo].[tbl_Spiff_PayType] ([SpiffPayTypeID])
GO

ALTER TABLE [dbo].[tbl_HR_PayrollSpiffs] CHECK CONSTRAINT FK_tbl_HR_PayrollSpiffs_tbl_Spiff_PayType
GO

SET ANSI_PADDING OFF
GO


