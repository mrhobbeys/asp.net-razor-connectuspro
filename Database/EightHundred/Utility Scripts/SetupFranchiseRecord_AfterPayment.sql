use EightHundred 

/**** setup new franchise ****/
begin tran

Declare @FranchiseNUmber nvarchar(10)
Declare @WebSite varchar(150)
Declare @EMail varchar(100)
Declare @LegalName varchar(50)
Declare @Name varchar(50)
Declare @Company varchar(50)
Declare @LegalAddress varchar(50)
Declare @LegalAddress2 varchar(50)
Declare @LegalCity varchar(50)
Declare @LegalState varchar(50)
Declare @LegalPostal varchar(50)
Declare @startdate datetime
Declare @FranchiseID int
Declare @PhoneNumber nvarchar(11)
Declare @royaltyrate money
Declare @ownerid int

set @Website = 'www.westplumbinginc.com'
set @EMail = '@westplumbing.com'
set @LegalName = 'West Plumbing Sales and Service,Inc'
set @name = 'Merri West Babbit'
set @company = 'West Plumbing Sales and Service'
set @LegalAddress = '104 Constitution Drive'
set @LegalAddress2 = ''
set @LegalCity = 'Warner Robbins'
set @LegalState = 'GA'
set @LegalPostal  = '31088'
Set @startdate = '08/03/2012'
set @PhoneNumber = '4789539378'

set @royaltyrate = '0.0'
set @Franchisenumber = '1C' + @LegalState + '0'


INSERT INTO [eighthundred].[dbo].[tbl_Franchise]
           ([FranchiseNUmber]
           ,[ProspectID]
           ,[OwnerID]
           ,[ConceptID]
           ,[FranchiseTypeID]
           ,[FranchiseStatusID]
           ,[SalesRepID]
           ,[SupportRepID]
           ,[DivisionID]
           ,[StartDate]
           ,[RenewalDate]
           ,[WebSite]
           ,[EMail]
           ,[LegalName]
           ,[LegalAddress]
           ,[LegalAddress2]
           ,[LegalCity]
           ,[LegalState]
           ,[LegalPostal]
           ,[LegalCountryID]
           ,[ShipName]
           ,[ShipCompany]
           ,[ShipAddress]
           ,[ShipAddress2]
           ,[ShipCity]
           ,[ShipState]
           ,[ShipPostal]
           ,[ShipCountryID]
           ,[MailName]
           ,[MailCompany]
           ,[MailAddress]
           ,[MailAddress2]
           ,[MailCity]
           ,[MailState]
           ,[MailPostal]
           ,[MailCountryID]
           ,[OfficeName]
           ,[OfficeCompany]
           ,[OfficeAddress]
           ,[OfficeAddress2]
           ,[OfficeCity]
           ,[OfficeState]
           ,[OfficePostal]
           ,[OfficeCountryID]
           ,[GeneralNotes]
           ,[TimeOffset])
     VALUES
           (@FranchiseNUmber
           , 0 
           , 0 
           , 1 
           , 5
           , 10
           , 0
           , 0
           , 0
           , @startdate
           , '01/01/2112'
           , @WebSite
           , @EMail
           , @LegalName 
           , @LegalAddress 
           ,  @LegalAddress2 
, @LegalCity
, @LegalState 
, @LegalPostal 
           , 1
, @name
 , @Company 
           , @LegalAddress 
           ,  @LegalAddress2 
, @LegalCity
, @LegalState 
, @LegalPostal 
           , 1
, @name
 , @Company 
           , @LegalAddress 
           ,  @LegalAddress2 
, @LegalCity
, @LegalState 
, @LegalPostal 
           , 1
, @name
 , @Company 
           , @LegalAddress 
           ,  @LegalAddress2 
, @LegalCity
, @LegalState 
, @LegalPostal 
           , 1
           , ' '
           , 0 )

Set  @FranchiseID = ( Select  top 1 FranchiseID 
  from tbl_franchise
order by franchiseid desc )


INSERT INTO [EightHundred].[dbo].[tbl_Franchise_Contacts]
           ([FranchiseID]
           ,[ContactName]
           ,[PhoneTypeID]
           ,[PhoneNumber])
     VALUES
           (@FranchiseID 
           , @name
           , 2
           , @PhoneNumber)




INSERT INTO [EightHundred].[dbo].[tbl_Franchise_Contract]
           ([FranchiseID]
           ,[ContractDate]
           ,[ExpireDate]
           ,[RoyaltyRate]
           ,[ManagementRate]
           ,[TechnologyRate]
           ,[MarketingRate]
           ,[AccountingSystemType]
           ,[TabletType]
           ,[HandleOthersCalls]
           ,[LicenseInfo])
     VALUES
           (@FranchiseID 
           ,@startdate 
           ,@startdate 
           ,@royaltyrate 
           ,0
           ,0
           ,0
           ,'QB'
           ,'iPad'
           ,0
           ,' ')
           
INSERT INTO [EightHundred].[dbo].[tbl_Franchise_Owner]
           ([OwnerName]
           ,[LegalName]
           ,[OwnerAddress]
           ,[OwnerAddress2]
           ,[OwnerCity]
           ,[OwnerState]
           ,[OwnerPostal]
           ,[OwnerCountryID])
     VALUES
           ( @Name 
           , @LegalName 
           , @LegalAddress 
           , @LegalAddress2 
           , @LegalCity 
           , @LegalState 
           , @LegalPostal 
           , 1)


Set  @OwnerID = ( Select  top 1 Ownerid 
  from tbl_Franchise_Owner 
order by OwnerID  desc )

UPDATE Tbl_franchise
set franchisenumber =  tbl_franchise.franchisenumber + CAST(  @FranchiseID as nvarchar(3))
, OwnerID = @ownerid
where franchiseid = @FranchiseID 

commit tran
