IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE name = 'JobCode' AND object_name(Object_id) = 'tbl_PB_JobCodes' AND is_nullable = 0)
	ALTER TABLE tbl_pb_Jobcodes ALTER COLUMN JobCode VARCHAR(10) NOT NULL
GO
sp_refreshview 'View_HVAC_App_Parts'