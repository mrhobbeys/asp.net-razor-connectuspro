--THIS SCRIPT SHOULD REMAIN REPEATABLE AND NOT DESTROY DATA THAT ALREADY EXISTS.
USE [EightHundred]
GO

DECLARE @Services TABLE(ServiceID INT)
DECLARE @Zips TABLE(Zip VARCHAR(12), City VARCHAR(50), State VARCHAR(2))
DECLARE @FranchiseId INT, @RoyaltyRate DECIMAL, @MgmtRate DECIMAL, @TechRate DECIMAL, @MktgRate DECIMAL, @ContractStart DATETIME, @ContractEnd DATETIME
DECLARE @DBA TABLE(DBAName VARCHAR(50))
DECLARE @Taxes TABLE([Description] VARCHAR(50), LaborAmount REAL, PartsAmount REAL, AccountCode VARCHAR(5))
DECLARE @FranchisePrimaryContact VARCHAR(50), @FranchisePrimaryPhone VARCHAR(15)

SET @FranchiseId = 66
SET @RoyaltyRate = .0925
SET @MgmtRate = 0
SET @TechRate = 0
SET @MktgRate = 0
SET @ContractStart = '4/15/2012'
SET @ContractEnd = '12/31/2020'
SET @FranchisePrimaryContact = 'Jay North'
SET @FranchisePrimaryPhone = '(626)252-1679'

SET NOCOUNT ON

--Add all services here.
INSERT INTO @Services(ServiceID) VALUES(1) --Plumbing

INSERT INTO @Taxes(Description, LaborAmount, PartsAmount, AccountCode) VALUES('CA State Tax',0.0875,0.0875,'20601')

--Add all zip codes, city and state here.
INSERT INTO @Zips(Zip,City,State) VALUES('91801','Alhambra','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91803','Alhambra','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91001','Altadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91003','Altadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91007','Arcadia','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91006','Arcadia','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91702','Azusa','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91501','Burbank','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('93012','Camarillo','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91722','Covina','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91724','Covina','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91010','Duarte','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91732','El Monte','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91731','El Monte','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('90032','El Sereno','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91207','Glendale','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91208','Glendale','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91206','Glendale','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91740','Glendora','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91741','Glendora','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91011','La Canada','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91214','La Cresenta','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91011','LaCanada Flintridge','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('90042','Los Angeles','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('90031','Los Angeles','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('90065','Los Angeles','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('90026','Los Angeles','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('90016','Los Angeles','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('90039','Los Angeles','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91016','Monrovia','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91010','Monrovia','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91755','Monterey Park','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91754','Monterey Park','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91020','Montrose ','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('19073','Newton Square','PA')
INSERT INTO @Zips(Zip,City,State) VALUES('94301','Palo Alto','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('33067','Parkland','FL')
INSERT INTO @Zips(Zip,City,State) VALUES('91107','Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91106','Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91103','Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91104','Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91105','Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91101','Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91001','Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91737','Rancho Cucamonga','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91770','Rosemead','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91030','Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('92109','San Diego','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91775','San Gabriel','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91776','San Gabriel','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91108','San Marino','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91204','Sierra Madre','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91030','So Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91030','South Pasadena','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91604','Studio City','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91780','Temple City','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91042','Tujunga','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91786','Upland','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91789','Walnut','CA')
INSERT INTO @Zips(Zip,City,State) VALUES('91024','Sierra Madre','CA')

INSERT INTO @DBA(DBAName) VALUES('Northland Plumbing')

SET NOCOUNT OFF

/*********************DO NOT MODIFY BELOW THIS LINE*********************************/

BEGIN TRAN

PRINT 'Ignoring services that already exist.'
DELETE s
FROM @Services s
INNER JOIN tbl_Franchise_Services fs
ON s.ServiceID = fs.ServiceID
WHERE fs.FranchiseID = @FranchiseId

PRINT 'Adding missing services.'
INSERT INTO tbl_Franchise_Services(FranchiseID, ServiceID)
SELECT @FranchiseId, ServiceID FROM @Services

IF NOT EXISTS(SELECT 1 FROM tbl_Franchise_Contract WHERE FranchiseID = @FranchiseId)
BEGIN
	PRINT 'Adding franchise contract.'	
	INSERT INTO [EightHundred].[dbo].[tbl_Franchise_Contract]
			   ([FranchiseID],[ContractDate],[ExpireDate]
			   ,[RoyaltyRate],[ManagementRate],[TechnologyRate]
			   ,[MarketingRate],[AccountingSystemType],[TabletType]
			   ,[HandleOthersCalls],[LicenseInfo])
		 VALUES
			   (@FranchiseId, @ContractStart, @ContractEnd
			   ,@RoyaltyRate,@MgmtRate,@TechRate
			   ,@MktgRate,'QB','iPad'
			   ,0,'')
			   END
ELSE
BEGIN
	PRINT 'Franchise contract already exists...skipping.'
END

PRINT 'Ignoring zip codes already mapped to this franchise.'
DELETE z
FROM @Zips z
INNER JOIN tbl_Franchise_ZipList zl
ON zl.FranchiseZipID = z.Zip
WHERE zl.FranchiseID = @FranchiseId

PRINT 'Adding unmapped zip codes to franchise'
INSERT INTO [EightHundred].[dbo].[tbl_Franchise_ZipList]([FranchiseZipID],[FranchiseID],[ActiveYN],[DateAdded],[DateRemoved],[OwnedYN],[ServicesYN],[City],[State],[Country],[CallTakerMessage]) 
SELECT Zip, @FranchiseID, 1,GETDATE(), NULL, 0, 1, City, State, 'USA', NULL  FROM @Zips

PRINT 'Ignoring DBA(s) already mapped to this franchise.'
DELETE z
FROM @DBA z
INNER JOIN tbl_Dispatch_DBA d
ON d.DBAName = z.DBAName
WHERE d.FranchiseID = @FranchiseId

PRINT 'Adding DBA record(s)'
INSERT INTO [tbl_Dispatch_DBA]([FranchiseID],[DBAName],[DBAIMage])
SELECT @FranchiseId, DBAName, NULL FROM @DBA

PRINT 'Ignoring tax rates already mapped to this franchise'
DELETE t
FROM @Taxes t
INNER JOIN tbl_TaxRates tr
ON tr.TaxDescription = t.[Description]
WHERE tr.FranchiseId = @FranchiseId

PRINT 'Adding new tax rates'
INSERT INTO tbl_taxRates(TaxDescription, LaborAmount, PartsAmount, FranchiseId,AccountCode,ActiveYN)
SELECT Description, LaborAmount, PartsAmount, @FranchiseId, AccountCode, 1 FROM @Taxes

PRINT 'Creating Franchise Contact'
IF NOT EXISTS (SELECT 1 FROM tbl_Franchise_Contacts WHERE FranchiseID = @FranchiseId AND PhoneTypeID = 2)
BEGIN
	INSERT INTO tbl_Franchise_Contacts(FranchiseID, PhoneNumber, ContactName, PhoneTypeID) VALUES (@FranchiseId, @FranchisePrimaryPhone, @FranchisePrimaryContact, 2)
	PRINT 'Created!'
END
ELSE
BEGIN
	PRINT 'Contact already exists'
END

COMMIT TRAN