USE [EightHundred]
/*
	Author: Bryan Panjavan
	Description: Since Rate can be a financial numerical value, OR a multiplied rate, this column has to accomodate both
*/
ALTER TABLE [tbl_HR_PayrollSpiffs] ALTER COLUMN Rate decimal(18,8)




