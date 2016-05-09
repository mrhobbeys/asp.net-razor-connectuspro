USE [EightHundred]
GO

/****** Object:  StoredProcedure [dbo].[GoLiveCheck]    Script Date: 04/27/2012 14:31:35 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GoLiveCheck]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GoLiveCheck]
GO

USE [EightHundred]
GO

/****** Object:  StoredProcedure [dbo].[GoLiveCheck]    Script Date: 04/27/2012 14:31:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Changelog
-- Author:		RJ Delange
-- Date: 04/27/2012
-- Comments:	Initial creation
-- =============================================
CREATE PROCEDURE [dbo].[GoLiveCheck](@ClientID int)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Declare @GoLiveResults  table(CheckNr int, [TestDescription] nvarchar(250), PassFail bit, Comments nvarchar(4000), resolution nvarchar(4000))
	Declare @CheckNr int
	Declare @TmpDescription nvarchar(250)
	Declare @TmpPassFail bit
	Declare @TmpComments nvarchar(4000)
	Declare @TmpResolution nvarchar(4000)
	declare @tmpchar nvarchar(300)
	declare @tmpint int
	
	-- Check 1 - initial check
	Set @Checknr = 1
	Set @TmpDescription = 'Initial Check'
	set @TmpPassFail  = 1
	set @TmpComments  = 'Test to see if Database communication is correct for clientid: ' + cast(@ClientID as nvarchar(20)) 
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )

	-- Check 2 - LegalName
	Set @Checknr = 2
	Set @TmpDescription = 'Client Legal Name Check'
	
	select @tmpchar = LegalName from tbl_Franchise where FranchiseID = @ClientID  
	if @tmpchar is null
	begin
	set @TmpPassFail  = 0
	set @TmpComments  = 'Please define clientid in tbl_franchise: ' + cast(@ClientID as nvarchar(20))
	end
	else
	begin 
	set @TmpPassFail  = 1
	set @TmpComments  = 'Legal Name defined: ' + @tmpchar 
	end
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )

    --  check 3 - make sure at least one phone number is setup

	Set @Checknr = 3
	Set @TmpDescription = 'Phone number setup Check'
	
	select @tmpint = count(l.LookupId ) from [DB_10668_Calls].dbo.LookupScript l inner join [DB_10668_Calls].dbo.Tbl_scriptToFranchiseID s on l.LookupId  = s.ScriptID  where s.FranchiseID  = @ClientID  
	if @tmpint = 0
	begin
	set @TmpPassFail  = 0
	set @TmpComments  = 'Please define phonenumber in phone number manager for clientid: ' + cast(@ClientID as nvarchar(20))
	end
	else
	begin 
	set @TmpPassFail  = 1
	set @TmpComments  = 'Number of phone numbers defined: ' + CAST(@tmpint  as nvarchar(3)) 
	end
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )

-- check 4 - make sure there is a contract entry
	Set @Checknr = 4
	Set @TmpDescription = 'Client Contract setup Check'
	
	select @tmpchar = 'Management Rate: ' + CAST(l.managementrate as nvarchar(5)) + ' Royalty Rate: ' + CAST(l.RoyaltyRate as nvarchar(5))  + ' Marketing Rate: ' + CAST(l.MarketingRate as nvarchar(5)) + ' Tecnhnology Rate: ' + CAST(l.TechnologyRate as nvarchar(5)) + ' Expiration Date: ' + CONVERT(VARCHAR(10), l.expiredate , 101)  from [EightHundred].dbo.tbl_Franchise_Contract  l  where l.FranchiseID  = @ClientID  
	if @tmpchar is null
	begin
	set @TmpPassFail  = 0
	set @TmpComments  = 'Please define contract in contract table for clientid: ' + cast(@ClientID as nvarchar(20))
	end
	else
	begin 
	set @TmpPassFail  = 1
	set @TmpComments  = 'Contract defined - ' + @tmpchar 
	end
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )

-- check 5 - make sure there is a franchise contact defined
	Set @Checknr = 5
	Set @TmpDescription = 'Client Contact setup Check'
	
	select @tmpchar = 'Contactname: ' + CAST(l.ContactName  as nvarchar(30)) + ' ContactPhone Number: ' + CAST(l.PhoneNumber  as nvarchar(20))    from [EightHundred].dbo.tbl_Franchise_Contacts  l  where l.FranchiseID  = @ClientID  
	if @tmpchar is null
	begin
	set @TmpPassFail  = 0
	set @TmpComments  = 'Please define contract in contract table for clientid: ' + cast(@ClientID as nvarchar(20))
	end
	else
	begin 
	set @TmpPassFail  = 1
	set @TmpComments  = 'Contact defined - ' + @tmpchar 
	end
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )

-- check 6 - make sure there is at least one customer master
	Set @Checknr = 6
	Set @TmpDescription = 'Customer Master setup Check'
	
	select @tmpint = count(c.CustomerID  ) from [EightHundred].dbo.tbl_Customer c inner join [EightHundred].dbo.tbl_Customer_Info ci  on c.CustomerID   = ci.CustomerID   where ci.FranchiseID   = @ClientID  
	if @tmpint = 0
	begin
	set @TmpPassFail  = 0
	set @TmpComments  = 'Please upload customers for clientid: ' + cast(@ClientID as nvarchar(20))
	end
	else
	begin 
	set @TmpPassFail  = 1
	set @TmpComments  = 'Number of customer defined: ' + CAST(@tmpint  as nvarchar(10)) 
	end
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )

-- check 7 - make sure there is at least one location 
	Set @Checknr = 7
	Set @TmpDescription = 'Location Master setup Check'
	
	select @tmpint = count(l.ActvieCustomerID   ) from [EightHundred].dbo.tbl_Locations  l inner join [EightHundred].dbo.tbl_Customer c on l.ActvieCustomerID = c.CustomerID  inner join [EightHundred].dbo.tbl_Customer_Info ci  on c.CustomerID   = ci.CustomerID   where ci.FranchiseID   = @ClientID  
	if @tmpint = 0
	begin
	set @TmpPassFail  = 0
	set @TmpComments  = 'Please upload locations for clientid: ' + cast(@ClientID as nvarchar(20))
	end
	else
	begin 
	set @TmpPassFail  = 1
	set @TmpComments  = 'Number of locations defined: ' + CAST(@tmpint  as nvarchar(10)) 
	end
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )

-- check 8 - make sure there is at least one zipcode defined
	Set @Checknr = 8
	Set @TmpDescription = 'Zipcode setup Check'
	
	select @tmpint = count(z.ZipID   ) from [EightHundred].dbo.tbl_Franchise_ZipList   z   where z.FranchiseID   = @ClientID  
	if @tmpint = 0
	begin
	set @TmpPassFail  = 0
	set @TmpComments  = 'Please upload ziplist for clientid: ' + cast(@ClientID as nvarchar(20))
	end
	else
	begin 
	set @TmpPassFail  = 1
	set @TmpComments  = 'Number of zipcodes defined: ' + CAST(@tmpint  as nvarchar(10)) 
	end
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )

-- check 9 - make sure there is at least one tax authority
	Set @Checknr = 9
	Set @TmpDescription = 'Tax Authority setup Check'
	
	select @tmpint = count(x.TaxRateID    ) from [EightHundred].dbo.tbl_TaxRates    x   where x.FranchiseID   = @ClientID  
	if @tmpint = 0
	begin
	set @TmpPassFail  = 0
	set @TmpComments  = 'Please upload Tax Authority List for clientid: ' + cast(@ClientID as nvarchar(20))
	end
	else
	begin 
	set @TmpPassFail  = 1
	set @TmpComments  = 'Number of tax Authorities defined: ' + CAST(@tmpint  as nvarchar(10)) 
	end
    if @TmpPassFail <> 1 begin 	set @TmpResolution   = 'Email ConnectusPro Helpdesk at helpdesk@connectuspro.com' end else 	set @TmpResolution   = ' ' 
	Insert @GoLiveResults values (@CheckNr , @TmpDescription , @TmpPassFail , @TmpComments, @TmpResolution  )





	-- return results
    Select * from @GoLiveResults 
    order by CheckNr 
	
END

GO


