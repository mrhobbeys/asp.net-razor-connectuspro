BEGIN TRAN
;WITH [LastChanges] AS
(
	SELECT JobID, MAX(StatusDateChanged) AS LastChange FROM tbl_Job_Status_History h
	GROUP BY JobID, StatusDateChanged
)
, [LastEstimates] AS
(
	SELECT JobID, MAX(EstimateDate) AS [EstimateDate] FROM tbl_Job_Estimates GROUP BY JobID
)
, [Audits] AS
(
	SELECT 
			 al.AuditDate
			, j.*
			, lc.LastChange
			, le.EstimateDate
			, ROW_NUMBER() OVER(PARTITION BY j.JobId ORDER BY al.AuditDate DESC) AS [Sequence]
			, CASE WHEN le.JobID IS NOT NULL THEN le.EstimateDate 
				   ELSE COALESCE(al.AuditDate, lc.LastChange, j.InvoicedDate, j.ScheduleEnd) END AS [ProposedCompletion]
			, cs.Status AS [CurrentStatus]
			, sp.Status AS [Previous Status]
			, sn.Status AS [LastAuditedStatus]
	FROM tbl_Job j
    LEFT JOIN (AuditLog al
				INNER JOIN Audit_Job aj
				ON al.AuditID = aj.AuditID
				INNER JOIN tbl_Job_Status sp
				ON CONVERT(INT, aj.OldValue) = sp.StatusID
				INNER JOIN tbl_Job_Status sn
				ON CONVERT(INT, aj.NewValue) = sn.StatusID)
	ON al.EntityID = j.JobID AND aj.Attribute = 'StatusID' 
	INNER JOIN tbl_Job_Status cs
	ON cs.StatusID = j.StatusID
	LEFT JOIN [LastChanges] lc
	ON lc.JobID = j.JobID
	LEFT JOIN [LastEstimates] le
	ON le.JobID = j.JobID
	WHERE CallCompleted IS NULL 
		AND j.StatusId IN (16,7,6, 12) 
)
--select 'UPDATE [tbl_Jobs] SET CallCompleted = NULL WHERE JobID = ' + CONVERT(VARCHAR(10), JobID) + ' AND CallCompleted IS NOT NULL' from [audits]

UPDATE j
SET
	 CallCompleted = a.ProposedCompletion
FROM tbl_Job j
INNER JOIN [audits] a 
ON j.JobID = a.JobID
WHERE Sequence = 1

COMMIT TRAN
