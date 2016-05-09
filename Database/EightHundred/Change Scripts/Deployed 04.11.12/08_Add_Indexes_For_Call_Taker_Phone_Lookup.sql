USE [EightHundred]
GO

/****** Object:  Index [IX_tbl_Contacts_PhoneTypeID]    Script Date: 04/12/2012 09:31:32 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tbl_Contacts]') AND name = N'IX_tbl_Contacts_PhoneTypeID')
DROP INDEX [IX_tbl_Contacts_PhoneTypeID] ON [dbo].[tbl_Contacts] WITH ( ONLINE = OFF )
GO

/****** Object:  Index [IX_tbl_Contacts_PhoneTypeID]    Script Date: 04/12/2012 09:31:34 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Contacts_PhoneTypeID] ON [dbo].[tbl_Contacts] 
(
	[PhoneTypeID] ASC
)
INCLUDE ( [ContactID],
[CustomerID],
[LocationID],
[ContactName],
[PhoneNumber]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_tbl_Customer_Info_FranchiseID]    Script Date: 04/12/2012 09:32:09 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tbl_Customer_Info]') AND name = N'IX_tbl_Customer_Info_FranchiseID')
DROP INDEX [IX_tbl_Customer_Info_FranchiseID] ON [dbo].[tbl_Customer_Info] WITH ( ONLINE = OFF )
GO

/****** Object:  Index [IX_tbl_Customer_Info_FranchiseID]    Script Date: 04/12/2012 09:32:12 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Customer_Info_FranchiseID] ON [dbo].[tbl_Customer_Info] 
(
	[FranchiseID] ASC
)
INCLUDE ( [CustomerID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

