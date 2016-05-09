USE [EightHundred]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Audit_Membership_AuditLog]') AND parent_object_id = OBJECT_ID(N'[dbo].[Audit_Membership]'))
ALTER TABLE [dbo].[Audit_Membership] DROP CONSTRAINT [FK_Audit_Membership_AuditLog]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Audit_Membership_AuditEntryID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Audit_Membership] DROP CONSTRAINT [DF__Audit_Membership_AuditEntryID]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Audit_Membership]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[Audit_Membership](
	[AuditEntryID] [uniqueidentifier] NOT NULL,
	[AuditID] [uniqueidentifier] NOT NULL,
	[Attribute] [varchar](255) NOT NULL,
	[NewValue] [nvarchar](max) NULL,
	[OldValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_Audit_Membership] PRIMARY KEY CLUSTERED 
(
	[AuditEntryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GO

ALTER TABLE [dbo].[Audit_Membership]  WITH CHECK ADD  CONSTRAINT [FK_Audit_Membership_AuditLog] FOREIGN KEY([AuditID])
REFERENCES [dbo].[AuditLog] ([AuditID])
GO

ALTER TABLE [dbo].[Audit_Membership] CHECK CONSTRAINT [FK_Audit_Membership_AuditLog]
GO

ALTER TABLE [dbo].[Audit_Membership] ADD  CONSTRAINT [DF__Audit_Membership_AuditEntryID]  DEFAULT (newsequentialid()) FOR [AuditEntryID]
GO


