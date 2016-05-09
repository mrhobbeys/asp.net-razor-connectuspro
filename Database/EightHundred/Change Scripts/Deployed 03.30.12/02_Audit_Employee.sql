USE [EightHundred]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Audit_Employee_AuditLog]') AND parent_object_id = OBJECT_ID(N'[dbo].[Audit_Employee]'))
ALTER TABLE [dbo].[Audit_Employee] DROP CONSTRAINT [FK_Audit_Employee_AuditLog]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Audit_Employee__Audit__648DC6E3]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Audit_Employee] DROP CONSTRAINT [DF__Audit_Employee__Audit__648DC6E3]
END

/****** Object:  Table [dbo].[Audit_Employee]    Script Date: 03/28/2012 08:50:29 ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Audit_Employee]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[Audit_Employee](
	[AuditEntryID] [uniqueidentifier] NOT NULL,
	[AuditID] [uniqueidentifier] NOT NULL,
	[Attribute] [varchar](255) NOT NULL,
	[NewValue] [nvarchar](max) NULL,
	[OldValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_Audit_Employee] PRIMARY KEY CLUSTERED 
(
	[AuditEntryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GO

ALTER TABLE [dbo].[Audit_Employee]  WITH CHECK ADD  CONSTRAINT [FK_Audit_Employee_AuditLog] FOREIGN KEY([AuditID])
REFERENCES [dbo].[AuditLog] ([AuditID])
GO

ALTER TABLE [dbo].[Audit_Employee] CHECK CONSTRAINT [FK_Audit_Employee_AuditLog]
GO

ALTER TABLE [dbo].[Audit_Employee] ADD  CONSTRAINT [DF__Audit_Employee__Audit__648DC6E3]  DEFAULT (newsequentialid()) FOR [AuditEntryID]
GO


