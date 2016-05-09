
-- Add technician's Image Url to Technicians table
-- Length: 255
-- Allow Null: Yes

ALTER TABLE Technicians ADD ImageUrl NVARCHAR(255) NULL;
GO

-- Add columns to Technicians table:
--				QuestionnairePassed: Bit, accepts NULL
--				PlumbingPassed: Bit, accepts NULL
--				HvacPassed: Bit, accepts NULL
ALTER TABLE Technicians ADD QuestionnairePassed BIT NULL;
ALTER TABLE Technicians ADD PlumbingPassed BIT NULL;
ALTER TABLE Technicians ADD HvacPassed BIT NULL;
GO