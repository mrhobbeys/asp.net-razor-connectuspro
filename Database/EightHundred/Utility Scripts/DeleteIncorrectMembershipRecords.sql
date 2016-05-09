-- =============================================
-- Script Template
-- =============================================

--Select FranchiseID, count (memberid ) 
Delete  
from tbl_Customer_Members where 

CustomerID in (
SELECT  distinct     vRpt_Job.CustomerID  
FROM         vRpt_Job INNER JOIN
                      View_JobTaskPartDetails ON vRpt_Job.TicketNumber = View_JobTaskPartDetails.JobID 
WHERE     (View_JobTaskPartDetails.JobCode LIKE 'A%')   
)  and (not CustomerID in (SELECT  distinct     vRpt_Job.CustomerID  
FROM         vRpt_Job INNER JOIN
                      View_JobTaskPartDetails ON vRpt_Job.TicketNumber = View_JobTaskPartDetails.JobID 
WHERE      (View_JobTaskPartDetails.JobCode  LIKE 'A0%') )) and EndDate is null and StartDate is not null

