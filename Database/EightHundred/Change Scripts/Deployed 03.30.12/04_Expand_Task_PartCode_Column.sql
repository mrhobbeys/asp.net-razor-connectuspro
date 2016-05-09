USE [EightHundred]
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Job_Task_Parts' AND name = 'PartCode' AND max_length = 10)
	ALTER TABLE tbl_Job_Task_Parts ALTER COLUMN PartCode VARCHAR(25)

SELECT * FROM sys.columns WHERE OBJECT_NAME(OBJECT_ID) = 'tbl_Job_Task_Parts' AND name = 'PartCode'