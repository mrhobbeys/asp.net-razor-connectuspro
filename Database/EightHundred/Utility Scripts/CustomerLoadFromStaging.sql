USE [EightHundred]
GO

BEGIN TRAN

BEGIN TRY
DECLARE @LastId INT
SELECT @LastId = ISNULL(MAX(CustomerID), 0) FROM ZU_CustomerUpload

DECLARE @MaxCustomerId INT
SELECT @MaxCustomerId = MAX(CustomerID) FROM tbl_Customer

DECLARE @InsertedRows TABLE (CustomerID INT, RecordId INT )

PRINT 'Cleaning data...'
UPDATE ZU_CustomerUpload
SET FirstAndLastName = SUBSTRING(FirstAndLastName,1,50)
, CompanyName = SUBSTRING(CompanyName, 1,50)
, Address = SUBSTRING(Address,1,75)
WHERE LEN(FirstAndLastName) > 50 OR LEN(COmpanyName) > 50 OR LEN(Address) > 75

UPDATE ZU_CustomerUpload
SET State = LTRIM(RTRIM(state))
WHERE State <> LTRIM(RTRIM(state))

UPDATE ZU_CustomerUpload
SET State = 'ON'
WHERE State = 'Ontario'

UPDATE ZU_CustomerUpload
SET State = 'QB'
WHERE State = 'Quebec'

UPDATE ZU_CustomerUpload
SET State = 'AB'
WHERE State = 'Alberta'

UPDATE ZU_CustomerUpload
SET PrimaryPhone = REPLACE(REPLACE(REPLACE(REPLACE(PrimaryPhone, ')',''),'(',''),'-',''),' ','')
WHERE PrimaryPhone IS NOT NULL

UPDATE ZU_CustomerUpload
SET PrimaryPhone = '(' + SUBSTRING(PrimaryPhone, 1,3) + ') ' + SUBSTRING(PrimaryPhone, 4,3) + '-' + SUBSTRING(PrimaryPhone, 7,4)
WHERE LEN(PrimaryPhone) >= 10

UPDATE ZU_CustomerUpload
SET CellPhone = REPLACE(REPLACE(REPLACE(REPLACE(CellPhone, ')',''),'(',''),'-',''),' ','')
WHERE CellPhone IS NOT NULL

UPDATE ZU_CustomerUpload
SET CellPhone = '(' + SUBSTRING(CellPhone, 1,3) + ') ' + SUBSTRING(CellPhone, 4,3) + '-' + SUBSTRING(CellPhone, 7,4)
WHERE LEN(CellPhone) >= 10

PRINT 'Adding Customers'
INSERT INTO @InsertedRows(CustomerID, RecordId)
SELECT ROW_NUMBER() OVER (ORDER BY FirstAndLastName) + @MaxCustomerId AS CustomerId,
CustUploadId
FROM ZU_CustomerUpload
WHERE CustomerId IS NULL

SET IDENTITY_INSERT [tbl_customer] OFF
SET IDENTITY_INSERT [tbl_customer] ON
INSERT INTO [tbl_Customer]
           ([CustomerId], 
            [FindByName]
           ,[CustomerName]
           ,[CompanyName]
           ,[EMail])
SELECT i.CustomerID, RTRIM(LTRIM(FirstAndLastName + ' ' + CompanyName)),
SUBSTRING(FirstAndLastName, 1, 50),
SUBSTRING(CompanyName, 1, 50),
Email
FROM ZU_CustomerUpload c
INNER JOIN @InsertedRows i
ON c.CustUploadId = i.RecordId
WHERE c.CustomerId IS NULL
SET IDENTITY_INSERT [tbl_customer] OFF

PRINT 'Mapping new customers to staging row.'
UPDATE cu
SET cu.CustomerId = i.CustomerId
FROM ZU_CustomerUpload cu
INNER JOIN @InsertedRows i
ON cu.CustUploadId = i.RecordId

PRINT 'Adding Customer -> Franchise Mappings'
INSERT INTO [tbl_Customer_Info]
           ([FranchiseID]
           ,[CustomerID]
           ,[BusinessTypeID]
           ,[CustomerTypeID]
           ,[StatusID]
           ,[CreditTermsID]
           ,[CreditLimit]
           ,[CustomerNotes])
SELECT FranchiseID
	, CustomerId
	, CASE WHEN LEN(LTRIM(ISNULL(CommercialorResidential, '2'))) = 0 THEN 2 ELSE CONVERT(INT,ISNULL(CommercialorResidential, '2')) END
	, 0
	, 2
	, 0
	, 0.00
	, NULL
FROM ZU_CustomerUpload
WHERE CustomerId > @LastId

PRINT 'Adding Bill To Locations'
INSERT INTO [tbl_Locations]
           ([BilltoCustomerID]
           ,[ActvieCustomerID]
           ,[Address]
           ,[City]
           ,[State]
           ,[PostalCode]
           ,[Country]
           ,[GPS]
           ,[Directions]
           ,[LocationNotes]
           ,[LocationName]
           ,[LocationCompany])
SELECT CustomerId, NULL, Address, City, State, Zip, Country, NULL, NULL, NULL, FirstAndLastName, CompanyName
FROM ZU_CustomerUpload
WHERE CustomerId > @LastId

PRINT 'Adding Job Locations'
INSERT INTO [tbl_Locations]
           ([BilltoCustomerID]
           ,[ActvieCustomerID]
           ,[Address]
           ,[City]
           ,[State]
           ,[PostalCode]
           ,[Country]
           ,[GPS]
           ,[Directions]
           ,[LocationNotes]
           ,[LocationName]
           ,[LocationCompany])
SELECT NULL, CustomerId, Address, City, State, Zip, Country, NULL, NULL, NULL, FirstAndLastName, CompanyName
FROM ZU_CustomerUpload
WHERE CustomerId > @LastId

PRINT 'Adding Job Location Primary Contacts'
INSERT INTO [tbl_Contacts]
           ([CustomerID]
           ,[LocationID]
           ,[ContactName]
           ,[PhoneTypeID]
           ,[PhoneNumber])
SELECT c.CustomerId, l.LocationID, c.FirstAndLastName, 2, c.PrimaryPhone
FROM ZU_CustomerUpload c
INNER JOIN tbl_Locations l
ON c.CustomerId = l.ActvieCustomerID
WHERE c.CustomerId > @LastId

PRINT 'Adding Bill To Primary Contacts'
INSERT INTO [tbl_Contacts]
           ([CustomerID]
           ,[LocationID]
           ,[ContactName]
           ,[PhoneTypeID]
           ,[PhoneNumber])
SELECT c.CustomerId, l.LocationID, c.FirstAndLastName, 2, c.PrimaryPhone
FROM ZU_CustomerUpload c
INNER JOIN tbl_Locations l
ON c.CustomerId = l.BilltoCustomerID
WHERE c.CustomerId > @LastId

PRINT 'Adding Job Location Secondary Contacts'
INSERT INTO [tbl_Contacts]
           ([CustomerID]
           ,[LocationID]
           ,[ContactName]
           ,[PhoneTypeID]
           ,[PhoneNumber])
SELECT c.CustomerId, l.LocationID, c.FirstAndLastName, 4, c.CellPhone
FROM ZU_CustomerUpload c
INNER JOIN tbl_Locations l
ON c.CustomerId = l.ActvieCustomerID
WHERE c.CustomerId > @LastId

PRINT 'Adding Bill To Secondary Contacts'
INSERT INTO [tbl_Contacts]
           ([CustomerID]
           ,[LocationID]
           ,[ContactName]
           ,[PhoneTypeID]
           ,[PhoneNumber])
SELECT c.CustomerId, l.LocationID, c.FirstAndLastName, 4, c.CellPhone
FROM ZU_CustomerUpload c
INNER JOIN tbl_Locations l
ON c.CustomerId = l.BilltoCustomerID
WHERE c.CustomerId > @LastId

PRINT 'Adding Members'
INSERT INTO [tbl_Customer_Members]
           ([FranchiseID]
           ,[CustomerID]
           ,[MemberTypeID]
           ,[StartDate]
           ,[EndDate])
SELECT FranchiseID, CustomerId,1, NULL, CASE WHEN LEN(ISNULL(MembershipExpirationDate,'')) = 0 THEN NULL ELSE CONVERT(DATETIME, MembershipExpirationDate) END
FROM ZU_CustomerUpload 
WHERE CustomerId > @LastId
AND MembershiptPlanName IS NOT NULL AND LEN(LTRIM(MembershiptPlanName)) > 0

COMMIT TRAN
PRINT 'Done'
END TRY
BEGIN CATCH
	PRINT 'Error: ' + ERROR_MESSAGE()
	ROLLBACK TRAN
	PRINT 'Transaction rolled back'
END CATCH