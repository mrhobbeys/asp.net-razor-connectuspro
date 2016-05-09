-- =============================================
-- performs the weekly lock for the WSR Report + creates batch report + creates audit entries in the weekly lock table
-- =============================================

Declare @WSRDate as date
Declare @WSRSelectDate as date
Declare @LiveClients as Table(Franchiseid int, FranchiseName nvarchar(100)) 
Declare @userid as nvarchar(100)
declare @lockcomment as nvarchar(100)
Declare @Tmp_WeeklyLockTable as table(Franchiseid int,WeeklylockDate date, PerformedBy nvarchar(100), PerformedByComment nvarchar(100))

-- input variables
Set @WSRDate = '05/05/2012'
Set @WSRSelectDate = DATEADD(Day, 1, @WSRDate)
set @userid = 'rdelange'
set @lockcomment = 'System Generated Automatic Lock Process'
 
-- live clients - add clients once they are live
Insert into @LiveClients values (24, 'Greenville')
Insert into @LiveClients values (28, 'Augusta West')
Insert into @LiveClients values (29, 'Pearland')
Insert into @LiveClients values (32, 'Stamford')
Insert into @LiveClients values (33, 'Danbury CT')
Insert into @LiveClients values (34, 'Mesa')
Insert into @LiveClients values (38, 'Amarillo')
Insert into @LiveClients values (39, 'Dallas')
Insert into @LiveClients values (45, 'VIP Plumbing')
Insert into @LiveClients values (47, 'Expert Plumbing')
Insert into @LiveClients values (50, 'Bryant')
Insert into @LiveClients values (51, 'Schaal')
Insert into @LiveClients values (59, 'Upstate - rivera')
Insert into @LiveClients values (65, 'Jim the Plumber')

--Insert into @LiveClients values (52, 'Sunkel')
--Insert into @LiveClients values (66, 'North')


/****** Display records that will be locked******/
select j.jobid, j.franchiseid, j.calltaken,  j.callcompleted, j.subtotal,  j.wsrcompleteddate from tbl_job j
where j.franchiseid in ( select Franchiseid  from @LiveClients  ) 
and statusid = 7 and lockedyn = 0 and callcompleted < @WSRSelectDate and wsrcompleteddate is null



/****** Perform weekly lock ******/
begin tran
update tbl_job 
set lockedYN = 1,
wsrcompleteddate = @WSRDate
where franchiseid in ( select Franchiseid  from @LiveClients  ) 
and statusid = 7 and lockedyn = 0 and callcompleted < @WSRSelectDate and wsrcompleteddate is null
commit tran

/****** Perform Summary report after lock ******/
select j.franchiseid, COUNT( j.jobid) as Jobcount, sum(j.subtotal) as Jobtotal, min(j.callcompleted) as firtCallCompleted,  max(j.callcompleted) as lastCallCompleted  from tbl_job j
where j.franchiseid in ( select Franchiseid  from @LiveClients  ) 
and statusid = 7 and lockedyn = 1  and wsrcompleteddate = @WSRDate
group by j.franchiseid

/****** Batch Report after weekly lock ******/
SELECT    v.TicketNumber, v.ClientID, f.legalname , v.CallCompleted , v.StatusID , v.Status, v.ServiceID , v.ServiceName, v.TotalSales , v.TaxAmount ,
                       v.SubTotal , v.TaxDescription, v.BusinessTypeID , v.BusinessType, v.CustomerName, v.JobAddress, v.JobCity, v.JobState, v.JobPostalCode, 
                       v.JobPriority, v.ServiceProID , v.Balance , j.LockedYN
FROM         View_JobInfo AS v INNER JOIN
                      tbl_Job AS j ON v.TicketNumber = j.JobID
                      inner join tbl_franchise f on j.franchiseid = f.franchiseid
WHERE     (j.FranchiseID IN ( select Franchiseid  from @LiveClients  )) AND (v.StatusID = 7) AND (j.LockedYN = 1) 
AND (j.WSRCompletedDate = @WSRDate )



/****** Fill Temp table with records that should be inserted ******/
insert into @Tmp_WeeklyLockTable(Franchiseid,WeeklylockDate, PerformedBy, PerformedByComment)
SELECT     Distinct FranchiseID, WSRCompletedDate, @userid , @lockcomment 
FROM         tbl_Job AS j
WHERE     (FranchiseID IN ( select franchiseid from @LiveClients )) AND (StatusID = 7) AND (LockedYN = 1) AND (WSRCompletedDate = @WSRDate)

/****** delete records that are already there ******/
delete from @Tmp_WeeklyLockTable where Franchiseid in (
Select t.franchiseid from  @Tmp_WeeklyLockTable t
inner join Tbl_Accounting_WeeklyLockHistory r on t.franchiseid = r.FranchiseID and t.PerformedBy = r.PerformedBy and t.PerformedByComment = r.PerformedByComment and t.WeeklylockDate = r.WeeklyLockDate )

/****** Batch Report after weekly lock ******/
select * from @Tmp_WeeklyLockTable

/****** Batch Report after weekly lock ******/
insert into Tbl_Accounting_WeeklyLockHistory(Franchiseid,WeeklylockDate, PerformedBy, PerformedByComment)
select * from @Tmp_WeeklyLockTable 

/****** Batch Report after weekly lock ******/
select * from Tbl_Accounting_WeeklyLockHistory
order by weeklylockid desc
