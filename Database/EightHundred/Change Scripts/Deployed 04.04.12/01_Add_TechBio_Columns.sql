USE [EightHundred]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Dispatch_TechBios' AND name = 'LastDrugTest')
	ALTER TABLE tbl_Dispatch_TechBios ADD LastDrugTest DATETIME

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Dispatch_TechBios' AND name = 'BackgroundCheckCompleted')
	ALTER TABLE tbl_Dispatch_TechBios ADD BackgroundCheckCompleted DATETIME

IF EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Dispatch_TechBios' AND name = 'timestamp')
	ALTER TABLE tbl_Dispatch_TechBios DROP COLUMN [timestamp]

IF EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Dispatch_TechBios' AND name = 'BioImage')
	ALTER TABLE tbl_Dispatch_TechBios DROP COLUMN BioImage

IF EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Dispatch_TechBios' AND name = 'BioText')
	ALTER TABLE tbl_Dispatch_TechBios DROP COLUMN BioText

GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Dispatch_TechBios' AND name = 'BioText')
	ALTER TABLE tbl_Dispatch_TechBios ADD BioText NVARCHAR(MAX)

GO

BEGIN TRAN
DELETE FROM tbl_Dispatch_TechBios


INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (86,'1/11/2012','1/23/2012','Your 1‐800‐PLUMBER® CareTech Certified technician today will be David. David has been in the plumbing industry for nearly 2 years. He holds an Associates Degree in Industrial Maintenance with a major in Heating, Ventilation and Air Conditioning and is a registered Air Conditioning Technician.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (90,'1/11/2012','12/1/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Enes. Enes has been in the plumbing industry for nearly 22 years. He holds a Tradesman Plumbers license and has taken several various technical classes for plumbing, along with on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (92,'12/1/2011','12/1/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Jeremy. Jeremy has been in the plumbing industry for over 20 years. He holds a Tradesman Plumbers license and has taken several various technical classes for plumbing, along with on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (93,'1/11/2012','12/1/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be JR. JR has been in the plumbing industry for over 15 years. He holds a Master Plumbers license and has taken several various technical classes for plumbing, along with on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (94,'1/11/2012','12/6/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Andrew. Andrew has been in the plumbing industry for nearly 4 years. He holds a Tradesman Plumbers license and has taken several various technical classes for plumbing, along with on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (96,'1/11/2012','12/2/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Daniel. Daniel has been in the plumbing industry for over 9 years. He holds a Journeyman Plumbers license and has taken several various technical classes for plumbing, along with on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (97,'1/11/2012','12/1/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Fabian. Fabian has been in the plumbing industry for nearly 5 years. He holds a Plumbers license and has taken several various technical classes for plumbing, along with on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (98,'1/11/2012','12/2/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Curt. Curt has been in the plumbing industry for nearly 14 years. He holds a Journeyman Plumbers license and has taken several various technical classes for plumbing, along with on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (99,'1/11/2012','12/6/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Jasen. Jasen has been in the plumbing industry for over 10 years. He holds a Masters Plumbers license and has taken several various technical classes for plumbing, along with on the job training. He also holds an Associates Degree in Industrial Maintenance.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (271,'1/23/2012','1/25/2012','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Jason. Jason has been in the plumbing industry for nearly 10 years. He holds Tradesman Plumbers license and has taken several various technical classes for plumbing, along with on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (34,'12/15/2011','4/2/2011','Your 1‐800‐PLUMBER CareTech Certified technician today will be Billy. Billy has been in the plumbing industry for over 16 years. He holds Master Plumbers licenses in both Georgia and South Carolina. Billy is a specialist at sewer line installation and replacement and has many years experience in residential construction as well.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (35,'2/1/2011','2/1/2011','Andy has helped hundreds of our friends and neighbors since joining our team in 2007. With over 4,000 hours of on-the-job training, Andy is in the process of earning his Georgia State Journeyman’s License.         With his training and experience we are confident that Andy will be able to help resolve today’s problem.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (129,'7/15/2011','7/15/2011','Your 1‐800‐PLUMBER CareTech Certified technician today will be John. John has been in the plumbing industry for nearly 40 years. He holds a plumbing license and several years of on the job training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (172,'12/15/2011','12/15/2011','Your 1‐800‐PLUMBER CareTech Certified technician is Cliff. Cliff has over 20 years of plumbing experience and holds a both a Journeyman and a Master Plumbing license. Cliff is also a Water Supply Protection Specialist Endorsement, and is certified with the U.S. Labor Board of Plumbing.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (191,'','12/15/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be David. David is fully licensed and insured to perform plumbing, heating, air conditioning, installation and repair. David works on a philosophy of quality and craftsmanship. He views his trade as a craft and so values and insists upon neat, well thought out, quality work from each and every employee. David is a 1992 graduate of Henry Abbott Technical School’s Plumbing and HVAC Program.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (20,'12/14/2011','10/5/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Tim. Tim has been in the plumbing and electrical industry for over 11 years. He holds Master Electrician’s and Master Plumber’s licenses in South Carolina, as well as an Electrical and Plumbing card from the Labor Licensing and Regulation of South Carolina Residential Builder’s Commission. He is a certified Rheem, Rannai and Noritz installer and certified for TracPipe and Wardflex flexible gas piping systems.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (73,'7/21/2011','7/6/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Jonathan. Jonathan has nearly a decade of experience in residential and commercial plumbing repair and replacement. He currently holds his Arizona Journeyman’s Plumbing License and is certified in backflow testing, Wirsbo Pex and Gastite. Jonathan completed plumbing courses with the City of Phoenix as well as an extensive on‐the‐job apprenticeship. Jonathan is a firm believer in continuing education and enjoys the regular weekly training sessions provided by 1‐800‐PLUMBER®. All of Jonathan’s plumbing experience has been gained in the Phoenix area, so he is an expert in the local systems and specific needs of its residents.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (100,'7/21/2011','7/6/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Rob. Rob has over 6 years experience in residential and commercial plumbing repair and replacement. He currently holds his Arizona Journeyman’s Plumbing License and is certified in Wirsbo Pex and Gastite. Rob completed an extensive program with the PHCC Plumbing Apprenticeship Program. Rob joined his family in the plumbing business, so he knows how to handle any plumbing repair or replacement task.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (132,'9/6/2011','9/1/2011','Your 1‐800‐PLUMBER® CareTech Certified Technician today will be Chris Lauderdale. Chris has been in the plumbing industry for 8 years. He holds plumbing, heating and cooling certificates along with a Certified Apartment Maintenance Technician certificate. Chris received his certification at Amico Inc and Fair Housing Training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (133,'9/6/2011','9/1/2011','Your 1‐800‐PLUMBER® CareTech Certified Technician today will be Chris Lauderdale. Chris has been in the plumbing industry for 8 years. He holds plumbing, heating and cooling certificates along with a Certified Apartment Maintenance Technician certificate. Chris received his certification at Amico Inc and Fair Housing Training.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (193,'12/7/2011','11/22/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Jeremy. Jeremy has over 8 years experience in residential and commercial heating and air conditioning repair and replacement as well as residential plumbing. Training first in Washington state, Jeremy has held both electrical and gas & fuel/oil mechanics licenses in Spokane County. Jeremy completed HVAC courses from RSI in Phoenix, AZ as well as Island NW Training Center in Spokane, WA. With his background, Jeremy can handle any heating, air conditioning or plumbing problem he encounters.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (268,'1/26/2012','1/3/2012','Your 1‐800‐PLUMBER® CareTech Certified Technician today will be Otis. Otis has been in the plumbing industry for 3 years. He holds a plumbing certificate and also a certificate in heating and cooling which he received from Randolph Technical School in Detroit, Michigan.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (270,'1/26/2012','1/24/2012','Your 1‐800‐PLUMBER® CareTech Certified Technician today will be Bill. Bill has been in the plumbing industry for 18 years. He holds a Journeyman’s Card, which he received from GSA Water Heater Service in Michigan.')
INSERT INTO [EightHundred].[dbo].[tbl_Dispatch_TechBios]
           ([ServiceProID]
           ,[LastDrugTest]
           ,[BackgroundCheckCompleted]
           ,[BioText])
     VALUES
           (65,'12/11/2011','12/11/2011','Your 1‐800‐PLUMBER® CareTech Certified technician today will be Chris. Chris has over 27 years experience in the plumbing and heating industry. He currently holds a P‐1 Master Plumbing license and an S‐1 Master Heating/Cooling license in the state of Connecticut. Chris received extensive training at the J.M. Wright Technical School and with nearly three decades of experience, there is no problem too big for him to handle.')
COMMIT TRAN