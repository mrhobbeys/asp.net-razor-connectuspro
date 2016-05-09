USE [EightHundred]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Audit_FranchiseZip_AuditLog]') AND parent_object_id = OBJECT_ID(N'[dbo].[Audit_FranchiseZip]'))
ALTER TABLE [dbo].[Audit_FranchiseZip] DROP CONSTRAINT [FK_Audit_FranchiseZip_AuditLog]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Audit_FranchiseZip_AuditEntryID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Audit_FranchiseZip] DROP CONSTRAINT [DF__Audit_FranchiseZip_AuditEntryID]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Audit_FranchiseZip]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[Audit_FranchiseZip](
	[AuditEntryID] [uniqueidentifier] NOT NULL,
	[AuditID] [uniqueidentifier] NOT NULL,
	[Attribute] [varchar](255) NOT NULL,
	[NewValue] [nvarchar](max) NULL,
	[OldValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_Audit_FranchiseZip] PRIMARY KEY CLUSTERED 
(
	[AuditEntryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GO

ALTER TABLE [dbo].[Audit_FranchiseZip]  WITH CHECK ADD  CONSTRAINT [FK_Audit_FranchiseZip_AuditLog] FOREIGN KEY([AuditID])
REFERENCES [dbo].[AuditLog] ([AuditID])
GO

ALTER TABLE [dbo].[Audit_FranchiseZip] CHECK CONSTRAINT [FK_Audit_FranchiseZip_AuditLog]
GO

ALTER TABLE [dbo].[Audit_FranchiseZip] ADD  CONSTRAINT [DF__Audit_FranchiseZip_AuditEntryID]  DEFAULT (newsequentialid()) FOR [AuditEntryID]
GO


