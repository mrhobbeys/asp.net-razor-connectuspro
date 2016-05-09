USE [EightHundred]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Audit_Deposit_AuditLog]') AND parent_object_id = OBJECT_ID(N'[dbo].[Audit_Deposit]'))
ALTER TABLE [dbo].[Audit_Deposit] DROP CONSTRAINT [FK_Audit_Deposit_AuditLog]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Audit_Deposit_AuditEntryID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Audit_Deposit] DROP CONSTRAINT [DF__Audit_Deposit_AuditEntryID]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Audit_Deposit]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[Audit_Deposit](
	[AuditEntryID] [uniqueidentifier] NOT NULL,
	[AuditID] [uniqueidentifier] NOT NULL,
	[Attribute] [varchar](255) NOT NULL,
	[NewValue] [nvarchar](max) NULL,
	[OldValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_Audit_Deposit] PRIMARY KEY CLUSTERED 
(
	[AuditEntryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GO

ALTER TABLE [dbo].[Audit_Deposit]  WITH CHECK ADD  CONSTRAINT [FK_Audit_Deposit_AuditLog] FOREIGN KEY([AuditID])
REFERENCES [dbo].[AuditLog] ([AuditID])
GO

ALTER TABLE [dbo].[Audit_Deposit] CHECK CONSTRAINT [FK_Audit_Deposit_AuditLog]
GO

ALTER TABLE [dbo].[Audit_Deposit] ADD  CONSTRAINT [DF__Audit_Deposit_AuditEntryID]  DEFAULT (newsequentialid()) FOR [AuditEntryID]
GO


