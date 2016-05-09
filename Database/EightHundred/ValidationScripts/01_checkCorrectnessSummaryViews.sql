-- =============================================
-- Run this validation script after deploying views.  The scripts are written in a way that no records should be returned. 
--  If the results show records - this shows the error that needs to be corrected
-- =============================================
/****** Validation script => should return no records ******/
SELECT COUNT([TicketNumber]), ticketnumber
  FROM [EightHundred].[dbo].[vRpt_Job]
  group by TicketNumber 
  having  COUNT([TicketNumber]) > 1
  
/****** Validation script => should return no records ******/
Select COUNT(customerid), customerid
from EightHundred.dbo.vRpt_Customer
group by customerid 
having COUNT(customerid) > 1


/****** Validation script => should return no records ******/
Select COUNT(membershipid), membershipid
from eighthundred.dbo.vRpt_MembershipInfo 
group by MembershipID 
having COUNT (membershipid) > 1

/****** Validation script => should return no records ******/
Select COUNT(uid), accountcode, clientid, wsrcompleteddate, ServiceName, BusinessType 
from eighthundred.dbo.vRpt_AccountingSummary 
group by  accountcode, clientid, wsrcompleteddate, ServiceName, BusinessType 
having COUNT(uid) > 1
