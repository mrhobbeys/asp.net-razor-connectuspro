USE [EightHundred]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Audit_Franchise_AuditLog]') AND parent_object_id = OBJECT_ID(N'[dbo].[Audit_Franchise]'))
ALTER TABLE [dbo].[Audit_Franchise] DROP CONSTRAINT [FK_Audit_Franchise_AuditLog]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Audit_Franchise__Audit__648DC6E3]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Audit_Franchise] DROP CONSTRAINT [DF__Audit_Franchise__Audit__648DC6E3]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Audit_Franchise]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[Audit_Franchise](
	[AuditEntryID] [uniqueidentifier] NOT NULL,
	[AuditID] [uniqueidentifier] NOT NULL,
	[Attribute] [varchar](255) NOT NULL,
	[NewValue] [nvarchar](max) NULL,
	[OldValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_Audit_Franchise] PRIMARY KEY CLUSTERED 
(
	[AuditEntryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GO

ALTER TABLE [dbo].[Audit_Franchise]  WITH CHECK ADD  CONSTRAINT [FK_Audit_Franchise_AuditLog] FOREIGN KEY([AuditID])
REFERENCES [dbo].[AuditLog] ([AuditID])
GO

ALTER TABLE [dbo].[Audit_Franchise] CHECK CONSTRAINT [FK_Audit_Franchise_AuditLog]
GO

ALTER TABLE [dbo].[Audit_Franchise] ADD  CONSTRAINT [DF__Audit_Franchise__Audit__648DC6E3]  DEFAULT (newsequentialid()) FOR [AuditEntryID]
GO


