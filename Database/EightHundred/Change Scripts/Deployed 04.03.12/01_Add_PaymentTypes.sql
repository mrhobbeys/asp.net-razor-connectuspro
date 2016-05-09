USE [EightHundred]
GO

BEGIN TRAN

IF NOT EXISTS(SELECT 1 FROM [tbl_Payment_Types] WHERE [PaymentType] = 'Write Off')
INSERT INTO [tbl_Payment_Types]([PaymentType],[DepositType],[SendToTabletYN]) VALUES('Write Off','GL Entry',0)

IF NOT EXISTS(SELECT 1 FROM [tbl_Payment_Types] WHERE [PaymentType] = 'Discount')
INSERT INTO [tbl_Payment_Types]([PaymentType],[DepositType],[SendToTabletYN]) VALUES('Discount','GL Entry',0)

IF NOT EXISTS(SELECT 1 FROM [tbl_Payment_Types] WHERE [PaymentType] = 'Balance Transfer')
INSERT INTO [tbl_Payment_Types]([PaymentType],[DepositType],[SendToTabletYN]) VALUES('Balance Transfer','GL Entry',0)

IF NOT EXISTS(SELECT 1 FROM [tbl_Payment_Types] WHERE [PaymentType] = 'Rebate')
INSERT INTO [tbl_Payment_Types]([PaymentType],[DepositType],[SendToTabletYN]) VALUES('Rebate','GL Entry',0)

IF NOT EXISTS(SELECT 1 FROM [tbl_Payment_Types] WHERE [PaymentType] = 'Refund')
INSERT INTO [tbl_Payment_Types]([PaymentType],[DepositType],[SendToTabletYN]) VALUES('Refund','GL Entry',0)
     
IF NOT EXISTS(SELECT 1 FROM [tbl_Payment_Types] WHERE [PaymentType] = 'NSF')
INSERT INTO [tbl_Payment_Types]([PaymentType],[DepositType],[SendToTabletYN]) VALUES('NSF','GL Entry',0)
GO

COMMIT TRAN