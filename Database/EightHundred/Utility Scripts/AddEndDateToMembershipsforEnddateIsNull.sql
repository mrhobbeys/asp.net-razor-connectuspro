-- =============================================
-- Script Template
-- =============================================
begin tran
update [EightHundred].[dbo].[tbl_Customer_Members] 
set EndDate = DATEADD(year, CONVERT(INT, 1), StartDate )

  where EndDate  is null and StartDate  is not null
  commit tran