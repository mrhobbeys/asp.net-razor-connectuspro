Imports System.IO
Imports System.Xml
Imports System.Transactions
Imports SiteBlue.Business.Reporting
Imports SiteBlue.Business.Job
Imports SiteBlue.Business
Imports SiteBlue.Business.Invoice
Imports SiteBlue.Business.Alerts
Imports SiteBlue.Data.EightHundred

Public Class XMLProcessingFunctions

    Private Const XmlFolder As String = "XMLFolder"


    Private Shared tmpStatID As Integer

    Private Shared tmpJobStatus As String
    Private Shared tmpEstimateYN As String

    Private Shared CurrentFranchise As String

    Private Sub XMLProcessingFunctions()

    End Sub

    Private Shared Function ParseTaskParts(ByVal reader As XmlReader, jobNumber As Integer, ByVal tmpid As Integer, db As EightHundredEntities) As Boolean
        ''<Qty>1.0000</Qty>
        ''<PartCode>LA-101001                </PartCode>
        ''<PartDescription>Standard Labor</PartDescription>
        ''<UnitPrice>83.33</UnitPrice>
        ''<LinePrice>83.33</LinePrice>
        ''<HomeGuardPrice>83.33</HomeGuardPrice>


        reader.ReadToFollowing("Qty")
        Dim quantity = reader.ReadString()
        If quantity = "" Then
            'empty set, dont worry abnout it
        Else
            reader.ReadToFollowing("PartCode")
            Dim partCode = reader.ReadString().TruncateWithEllipsis(25)

            reader.ReadToFollowing("PartDescription")
            Dim partDescription = reader.ReadString().TruncateWithEllipsis(100)

            reader.ReadToFollowing("UnitPrice")
            Dim unitPrice = reader.ReadString()

            reader.ReadToFollowing("LinePrice")
            Dim linePrice = reader.ReadString()

            reader.ReadToFollowing("HomeGuardPrice")
            Dim homeGuardPrice = reader.ReadString()

            If partCode <> "" And unitPrice <> "" And quantity <> "" And linePrice <> "" And homeGuardPrice <> "" Then
                Try
                    Dim newpartrec As New tbl_Job_Task_Parts

                    newpartrec.JobTaskID = tmpid
                    ' newpartrec.HomeGuardPrice = HomeGuardPrice
                    newpartrec.PartCode = partCode
                    newpartrec.PartName = partDescription
                    newpartrec.Price = Convert.ToDecimal(unitPrice)
                    newpartrec.Quantity = Convert.ToDecimal(quantity)
                    newpartrec.PartsID = 0

                    db.tbl_Job_Task_Parts.AddObject(newpartrec)
                    db.SaveChanges()

                Catch ex As Exception
                    CommonFunctions.Log(CurrentFranchise, jobNumber, "Unable to insert part", ex, LogLevel.Error)
                    Return False
                End Try
            End If
        End If

        Return True
    End Function

    Private Shared Function GetJobTasksXml(job As tbl_Job, ByVal xmlBytes As Byte(), businessTypeId As Integer, db As EightHundredEntities) As JobTaskParsed
        Dim taskId = 0

        Dim result = New JobTaskParsed With {.Successful = True}
        Dim jobSvc = AbstractBusinessService.Create(Of JobService)(CommonFunctions.UserKey)

        Using scope As New TransactionScope()
            'first clear any existing tasks
            Try

                Dim oldTasks = db.tbl_Job_Tasks.Where(Function(p) p.JobID = job.JobID).ToList()
                For Each Oldtaskrec In oldTasks
                    Try
                        Dim tmppartid = Oldtaskrec.JobTaskID
                        db.tbl_Job_Task_Parts.Where(Function(p) p.JobTaskID = tmppartid).ToList().ForEach(Sub(p) db.DeleteObject(p))
                        db.SaveChanges()
                    Catch ex As Exception
                        CommonFunctions.Log(CurrentFranchise, job.JobID, "Unable to clear existing job tasks part", ex, LogLevel.Error)
                    End Try
                Next
                oldTasks.ForEach(Sub(t) db.DeleteObject(t))
                db.SaveChanges()
            Catch ex As Exception
                CommonFunctions.Log(CurrentFranchise, job.JobID, "Unable to clear existing job tasks", ex, LogLevel.Error)
                result.Successful = False
            End Try

            Using ms = New MemoryStream(xmlBytes)
                Using reader = XmlReader.Create(ms)
                    While reader.Read()

                        If reader.NodeType = XmlNodeType.Element And reader.Name = "TaxRate" Then
                            result.TaxRate = Convert.ToDecimal(reader.ReadString())
                        End If

                        If reader.NodeType = XmlNodeType.Element And reader.Name = "TaxName" Then
                            result.TaxName = reader.ReadString()
                        End If

                        If reader.NodeType = XmlNodeType.Element And reader.Name = "CouponCode" Then
                            result.CouponCode = reader.ReadString()
                        End If

                        If reader.NodeType = XmlNodeType.Element And reader.Name = "DiscountReason" Then
                            result.DiscountReason = reader.ReadString()
                        End If

                        If reader.NodeType = XmlNodeType.Element And reader.Name = "Part" Then
                            result.Successful = ParseTaskParts(reader, job.JobID, taskId, db) And result.Successful
                        End If

                        If reader.NodeType = XmlNodeType.Element And reader.Name = "Task" Then
                            Try
                                reader.ReadToFollowing("Qty")
                                Dim quantity = reader.ReadString()

                                If quantity = "" Then
                                    'empty set, dont worry about it
                                Else
                                    reader.ReadToFollowing("TaskCode")
                                    Dim taskCode = reader.ReadString()

                                    Dim addOnFlag = taskCode.EndsWith("A")

                                    'strip out the extra chaarcter if it is T or A  
                                    If taskCode.EndsWith("T") OrElse taskCode.EndsWith("A") Then
                                        taskCode = taskCode.Substring(0, taskCode.Length - 1)
                                    End If

                                    reader.ReadToFollowing("TaskDescription")
                                    Dim taskDescription = reader.ReadString().TruncateWithEllipsis(100)

                                    reader.ReadToFollowing("UnitPrice")
                                    Dim unitPrice = reader.ReadString()

                                    reader.ReadToFollowing("LinePrice")
                                    Dim linePrice = reader.ReadString()

                                    'HACK: Not sure why sometimes this column comes back from the iPad with commas in it.
                                    If Not String.IsNullOrWhiteSpace(linePrice) AndAlso linePrice.Contains(",") Then
                                        linePrice = linePrice.Replace(",", "")
                                    End If

                                    reader.ReadToFollowing("AdjustedPrice")
                                    Dim adjustedPrice = reader.ReadString()

                                    reader.ReadToFollowing("HomeGuardPrice")
                                    Dim homeGuardPrice = reader.ReadString()

                                    'MyReader.ReadToFollowing("StandardPrice")
                                    'Dim StandardPrice = MyReader.ReadString()

                                    'now update payment records
                                    'add this record
                                    Dim newTask As New tbl_Job_Tasks

                                    If String.IsNullOrEmpty(unitPrice) Then
                                        unitPrice = (Convert.ToDecimal(linePrice) / Convert.ToDecimal(quantity)).ToString()
                                    End If

                                    newTask.JobCode = taskCode
                                    newTask.JobID = job.JobID
                                    newTask.JobCodeDescription = taskDescription

                                    newTask.UnitPrice = Convert.ToDecimal(If(String.IsNullOrWhiteSpace(unitPrice), "0", unitPrice))
                                    newTask.LinePrice = Convert.ToDecimal(If(String.IsNullOrWhiteSpace(linePrice), "0", linePrice))
                                    newTask.AdjustedPrice = Convert.ToDecimal(If(String.IsNullOrWhiteSpace(adjustedPrice), "0", adjustedPrice))
                                    newTask.HomeGuardPrice = Convert.ToDecimal(If(String.IsNullOrWhiteSpace(homeGuardPrice), "0", homeGuardPrice))

                                    newTask.Price = Convert.ToDecimal(If(String.IsNullOrEmpty(adjustedPrice), unitPrice, adjustedPrice))

                                    If newTask.Price = 0 Then CommonFunctions.Log(CurrentFranchise, job.JobID, "Price is 0 on task.", Nothing, LogLevel.Warn)

                                    newTask.Quantity = Convert.ToDecimal(quantity)
                                    newTask.AddOnYN = addOnFlag
                                    newTask.MemberYN = (homeGuardPrice = adjustedPrice)
                                    newTask.AccountCode = If(CommonFunctions.Get_Account_Code_ByCode(job.FranchiseID, job.ServiceProID, taskCode, businessTypeId, db), String.Empty)
                                    newTask.ErrorFlag = String.IsNullOrEmpty(newTask.AccountCode)

                                    Dim jt = JobTask.MapFromModel(newTask)
                                    If jt.IsMemberPlan Then
                                        result.MemberPlanSold = True
                                        result.MemberPlanType = jobSvc.GetMemberType(newTask)
                                    End If

                                    db.tbl_Job_Tasks.AddObject(newTask)
                                    db.SaveChanges()
                                    taskId = newTask.JobTaskID
                                End If

                            Catch ex As Exception
                                CommonFunctions.Log(CurrentFranchise, job.JobID, "Unable to insert new task", ex, LogLevel.Error)
                                result.Successful = False
                            End Try
                        End If
                    End While
                End Using

                If result.Successful Then scope.Complete()
            End Using
        End Using

        Return result
    End Function

    Private Shared Function GetJobPaymentsXml(jobId As Integer, ByVal paymentFile As Byte(), franchiseId As Integer, ByRef warranty As String) As Boolean
        Dim success = True
        warranty = String.Empty

        Using ms = New MemoryStream(paymentFile)
            Using reader = XmlReader.Create(ms)
                While reader.Read()
                    If reader.NodeType = XmlNodeType.Element And reader.Name = "Warranty" Then
                        warranty = reader.ReadString()
                    End If

                    If reader.NodeType = XmlNodeType.Element And reader.Name = "Item" Then
                        reader.ReadToFollowing("PayType")
                        Dim paymentType = reader.ReadString()

                        reader.ReadToFollowing("PayAmount")
                        Dim paymentAmount = reader.ReadString()

                        reader.ReadToFollowing("PayAuth")
                        Dim paAuth = reader.ReadString().Truncate(20)

                        reader.ReadToFollowing("LockedYN")
                        Dim lockedFlag = reader.ReadString()
                        Using db = CommonFunctions.Create_SQL_Connection()

                            Dim paymentTypeId As Integer

                            'now update payment records
                            If lockedFlag <> "True" And paymentAmount <> "" Then
                                Try
                                    paymentTypeId = db.tbl_Payment_Types.Single(Function(s) s.PaymentType = paymentType).PaymentTypeId
                                Catch ex As Exception
                                    CommonFunctions.Log(CurrentFranchise, jobId, String.Format("Could not get payment type: '{0}'", paymentType), ex, LogLevel.Warn)
                                    paymentTypeId = 0
                                End Try

                                'add this record
                                Try
                                    Dim newPmt As New tbl_Payments

                                    newPmt.CheckNumber = paAuth
                                    newPmt.JobID = jobId
                                    newPmt.PaymentAmount = If(String.IsNullOrEmpty(paymentAmount), Nothing, Convert.ToDecimal(paymentAmount))
                                    newPmt.PaymentDate = DateTime.Today
                                    newPmt.FranchiseID = franchiseId
                                    newPmt.PaymentTypeID = paymentTypeId
                                    newPmt.ErrorFlag = paymentTypeId <> 0
                                    newPmt.CreateDate = DateTime.Now

                                    db.tbl_Payments.AddObject(newPmt)
                                    db.SaveChanges()

                                Catch ex As Exception
                                    CommonFunctions.Log(CurrentFranchise, jobId, "Unable to insert payment", ex, LogLevel.Error)

                                    success = False
                                End Try

                            End If
                        End Using
                    End If
                End While
            End Using
        End Using

        Return success
    End Function

    Private Shared Sub GetJobLocationXml(jobId As Integer, ByVal tmpOneFile As Byte())

        Using ms = New MemoryStream(tmpOneFile)
            Using reader = XmlReader.Create(ms)

                While reader.Read()
                    If reader.NodeType = XmlNodeType.Element And reader.Name = "Table" Then
                        reader.ReadToFollowing("BilltoCustomerID")
                        Dim strCustId = reader.ReadString()

                        reader.ReadToFollowing("EMail")
                        Dim email = reader.ReadString()

                        Using db = CommonFunctions.Create_SQL_Connection()
                            'now update email of job
                            If email <> "" Then
                                Try
                                    Dim cust As tbl_Customer
                                    Dim custId As Integer
                                    If String.IsNullOrEmpty(strCustId) OrElse strCustId = "0" Then

                                        Dim job = db.tbl_Job.Select(Function(j) New With {j.JobID, j.CustomerID}).Single(Function(j) j.JobID = jobId)
                                        custId = job.CustomerID
                                    Else
                                        custId = Convert.ToInt32(strCustId)
                                    End If

                                    cust = db.tbl_Customer.Single(Function(c) c.CustomerID = custId)

                                    If cust.EMail <> email Then
                                        cust.EMail = email
                                        db.SaveChanges()
                                    End If

                                Catch ex As Exception
                                    CommonFunctions.Log(CurrentFranchise, jobId, "Unable to insert email address", ex, LogLevel.Error)
                                End Try
                            End If
                        End Using
                    End If
                End While
            End Using
        End Using
    End Sub

    Private Shared Function GetJobRecommendationsXml(ByVal tmpOneFile As Byte()) As String
        Using ms = New MemoryStream(tmpOneFile)
            Using reader = XmlReader.Create(ms)
                While reader.Read()
                    If reader.NodeType = XmlNodeType.Element And reader.Name = "Job_Recommendations" Then
                        reader.ReadToFollowing("Recommendations")
                        Dim tmpStatus = reader.ReadString()
                        'now update recommendations of job
                        Return tmpStatus
                    End If
                End While
            End Using
        End Using
        Return String.Empty
    End Function

    Private Shared Function GetJobDiagsXml(ByVal tmpOneFile As Byte()) As String
        Using ms = New MemoryStream(tmpOneFile)
            Using reader = XmlReader.Create(ms)

                While reader.Read()
                    If reader.NodeType = XmlNodeType.Element And reader.Name = "Job_Diagnostics" Then
                        reader.ReadToFollowing("Diagnostics")
                        Return reader.ReadString()
                    End If
                End While
            End Using
        End Using

        Return String.Empty
    End Function

    Private Shared Sub GetJobStatusXml(jobId As Integer, ByVal tmpOneFile As Byte())
        ''STATUS: (EstimateYN now will have “No”, “Estimate”, “Parts”, or “People” in it… it used to be “Yes” or “No”).
        ''<!--8538_tbl_JobStatus.xml-->
        ''<tbl_JobStatus>
        ''    <FranchiseID>29</FranchiseID>
        ''    <LocationID>13087</LocationID>
        ''    <Status>Sent to Tablet</Status>
        ''    <SequenceID>1</SequenceID>
        ''    <JobType>Leak</JobType>
        ''    <JobDescription>test leak in yard</JobDescription>
        ''    <EstimateYN>No</EstimateYN>
        ''    <Message></Message>
        ''</tbl_JobStatus>

        tmpEstimateYN = "No"
        tmpJobStatus = "Completed"

        Using ms = New MemoryStream(tmpOneFile)
            Using reader = XmlReader.Create(ms)
                While reader.Read()
                    If reader.NodeType = XmlNodeType.Element And reader.Name = "Status" Then
                        tmpJobStatus = reader.ReadString()
                        If tmpJobStatus = "Arrived" Then
                            tmpJobStatus = "Active"
                        End If
                        If tmpJobStatus = "Wrapup" Then
                            tmpJobStatus = "Wrap Up"
                        End If
                        If tmpJobStatus = "Estimate Only" Then
                            tmpJobStatus = "Waiting Estimate"
                        End If
                    End If

                    If reader.NodeType = XmlNodeType.Element And reader.Name = "EstimateYN" Then
                        tmpEstimateYN = reader.ReadString()
                    End If

                    If reader.NodeType = XmlNodeType.Element And reader.Name = "Estimate" Then
                        tmpEstimateYN = reader.ReadString()
                    End If

                End While
            End Using
        End Using

        ''STATUS: (EstimateYN now will have “No”, “Estimate”, “Parts”, or “People” in it… it used to be “Yes” or “No”).
        If tmpEstimateYN = "Estimate" Then
            tmpJobStatus = "Waiting Estimate"
        ElseIf tmpEstimateYN = "Yes" Then
            tmpJobStatus = "Waiting Estimate"
        ElseIf tmpEstimateYN = "Parts" Then
            tmpJobStatus = "Waiting Parts"
        ElseIf tmpEstimateYN = "People" Then
            tmpJobStatus = "Waiting People"
        ElseIf tmpEstimateYN = "Recall" Then
            tmpJobStatus = "Recall"
        Else
            tmpJobStatus = "Completed"
        End If

        Dim status As tbl_Job_Status
        Using db = CommonFunctions.Create_SQL_Connection()
            status = db.tbl_Job_Status.SingleOrDefault(Function(s) s.Status = tmpJobStatus)
        End Using

        If status IsNot Nothing Then
            tmpStatID = status.StatusID
        Else
            CommonFunctions.Log(CurrentFranchise, jobId, "Could not get job status", Nothing, LogLevel.Warn)
            tmpStatID = 6           'must be completyed if we can't find a staus
        End If

    End Sub

    Private Shared Function DownloadJobFiles(ByVal jobNumber As Integer, ByVal sourceDir As String, archPath As String) As Dictionary(Of String, Byte())

        Dim files As Dictionary(Of String, Byte())
        Try
            files = Directory.GetFiles(sourceDir, jobNumber & "_*.*") _
                                 .Where(Function(f) Not f.Contains("_tbl_JobStatusChange.xml") AndAlso Not f.Contains("_tbl_JobStatusMessage.xml")) _
                                 .Select(Function(f) New FileInfo(f)) _
                                 .Where(Function(f) f.Extension = ".xml" OrElse f.Extension = ".jpg") _
                                 .ToDictionary(Function(f) f.FullName, Function(f) File.ReadAllBytes(f.FullName))

            If files.Count = 0 Then
                CommonFunctions.Log(CurrentFranchise, jobNumber, String.Format("No files for {0} in {1}", jobNumber, sourceDir), Nothing, LogLevel.Debug)
                CommonFunctions.TabletSuccess = False
                Return files
            End If

            'Archive the files into a ZIP file.
            CommonFunctions.ZipFiles(files.Select(Function(f) f.Key).ToArray(), archPath, 9)

            For Each pair In files
                Try
                    File.Delete(pair.Key)
                Catch ex As Exception
                    CommonFunctions.TabletSuccess = False
                    CommonFunctions.Log(CurrentFranchise, jobNumber, pair.Key & " could not be removed.", ex, LogLevel.Error)
                End Try
            Next

        Catch ex As Exception
            CommonFunctions.TabletSuccess = False
            CommonFunctions.Log(CurrentFranchise, jobNumber, "Could not load files for job '" & jobNumber & "' into memory.", ex, LogLevel.Error)
            files = Nothing
        End Try

        Return files

    End Function

    Private Shared Function GetJobsToProcess(ByVal sourceDir As String) As IEnumerable(Of Integer)

        Dim jobs As Integer() = Nothing

        Try
            jobs = Directory.GetFiles(sourceDir, "*" & CommonFunctions.OkFileExtension) _
                                   .Select(Function(f) Integer.Parse(Path.GetFileNameWithoutExtension(f))) _
                                   .ToArray()

            If jobs.Count = 0 Then CommonFunctions.TabletSuccess = False

            For Each j In jobs
                Try
                    File.Delete(Path.Combine(sourceDir, j & CommonFunctions.OkFileExtension))
                    CommonFunctions.TabletSuccess = True
                Catch ex As Exception
                    CommonFunctions.TabletSuccess = False
                    CommonFunctions.Log(CurrentFranchise, j, "Unable to remove OK file for job: " & j, ex, LogLevel.Error)
                End Try
            Next

        Catch ex As Exception
            CommonFunctions.TabletSuccess = False
            CommonFunctions.Log(CurrentFranchise, "OK file identification failed.", ex, LogLevel.Error)
        End Try

        Return jobs

    End Function

    Private Shared Function CheckForOkFiles(ByVal srcDir As String) As Boolean
        Try
            Dim any = Directory.Exists(srcDir) AndAlso Directory.GetFiles(srcDir, "*" & CommonFunctions.OkFileExtension).Length > 0

            If Not any Then CommonFunctions.TabletSuccess = False

            Return any
        Catch ex As Exception
            CommonFunctions.Log(CurrentFranchise, "Could not check for OK files to be processed", ex, LogLevel.Error)
            CommonFunctions.TabletSuccess = False
        End Try

        Return False
    End Function

    Private Shared Function GetTaxIdFromName(jobId As Integer, ByVal taxName As String, ByVal franchiseId As Integer) As Integer

        If String.IsNullOrWhiteSpace(taxName) Then Return 0

        Try
            Using db = CommonFunctions.Create_SQL_Connection()
                Return db.tbl_TaxRates.Single(Function(T) T.FranchiseId = franchiseId AndAlso T.TaxDescription = taxName).TaxRateID
            End Using
        Catch ex As Exception
            CommonFunctions.Log(CurrentFranchise, jobId, "Error getting tax id for authority " & taxName, ex, LogLevel.Error)
        End Try

        Return 0

    End Function

    Private Shared Function GetWarrantyLengthId(jobId As Integer, ByVal warrantyStr As String) As Integer
        If String.IsNullOrEmpty(warrantyStr) Then Return 0
        Try
            Using db = CommonFunctions.Create_SQL_Connection()
                Return db.tbl_Job_WarrantyLength.SingleOrDefault(Function(m) m.WarrantyLength = warrantyStr).WarrantyLengthID
            End Using
        Catch ex As Exception
            CommonFunctions.Log(CurrentFranchise, jobId, "Could not get warranty length....defaulting warranty to none.", ex, LogLevel.Warn)
            Return 0
        End Try
    End Function

    Private Shared Sub ProcessMessageFile(tabletCode As String, ByVal bytes As Byte())

        ''<?xml version="1.0" encoding="utf-8"?>
        ''<tbl_JobStatus>
        ''  <FranchiseID>41</FranchiseID>
        ''  <LocationID>58799</LocationID>
        ''  <Status>Sent to Tablet</Status>
        ''  <SequenceID>1</SequenceID>
        ''  <JobType>Kitchen</JobType>
        ''  <JobDescription>Sink Backed up</JobDescription>
        ''  <Estimate>No</Estimate>
        ''  <Message>Stuck in traffic</Message>
        ''</tbl_JobStatus>

        Dim franchiseId = 0
        Dim message = String.Empty

        Try
            tmpJobStatus = String.Empty
            Using ms = New MemoryStream(bytes)
                Using reader = XmlReader.Create(ms)

                    While reader.Read()

                        If reader.NodeType = XmlNodeType.Element And reader.Name = "FranchiseID" Then
                            franchiseId = Convert.ToInt32(reader.ReadString())
                        End If

                        If reader.NodeType = XmlNodeType.Element And reader.Name = "Message" Then
                            message = reader.ReadString().TruncateWithEllipsis(100)
                        End If

                    End While
                End Using
            End Using

        Catch ex As Exception
            CommonFunctions.Log(CurrentFranchise, "Unable to process the message status file.", ex, LogLevel.Error)
        End Try

        Dim msgFromTechId = 0
        Try
            Using db = CommonFunctions.Create_SQL_Connection()
                Dim tech = db.tbl_Franchise_Tablets.Where(Function(t) t.FranchiseID = franchiseId).ToArray() _
                                                    .Where(Function(t) If(t.TabletNumber, String.Empty).Split("-"c).Last() = tabletCode) _
                                                    .FirstOrDefault()
                If tech IsNot Nothing Then msgFromTechId = tech.EmployeeID
            End Using
        Catch ex As Exception
            CommonFunctions.Log(CurrentFranchise, "Unable to update read tablet record for:" & tabletCode, ex, LogLevel.Error)
        End Try

        If message <> String.Empty And tabletCode <> String.Empty Then

            Try
                'now update the message hisotry record.
                Dim techMsg As New tbl_Dispatch_Message_History
                Dim techName = CommonFunctions.Get_Employee_Name(msgFromTechId)
                techMsg.Message = message
                techMsg.FranchiseID = franchiseId
                techMsg.FranchiseName = CommonFunctions.Get_Franchise_Number(franchiseId)
                techMsg.MessageDate = DateTime.Now
                techMsg.ProcessedYN = False
                techMsg.TechID = msgFromTechId
                techMsg.TechName = techName

                Using db = CommonFunctions.Create_SQL_Connection()
                    db.tbl_Dispatch_Message_History.AddObject(techMsg)
                    db.SaveChanges()
                End Using

                CommonFunctions.Log(CurrentFranchise, String.Format("Message from {0} received", techName), Nothing, LogLevel.Debug)

            Catch ex As Exception
                CommonFunctions.Log(CurrentFranchise, "Unable to receive message from tech " & msgFromTechId.ToString(), ex, LogLevel.Error)
            End Try
        Else
            CommonFunctions.Log(CurrentFranchise, "Unable to process message from tech.  Tablet code or message content was empty", Nothing, LogLevel.Warn)
        End If

    End Sub

    Public Shared Function ProcessIPadStatusFile(jobId As Integer, ByVal bytes As Byte()) As Boolean

        Dim statusChangedTime = DateTime.Now
        Dim statusId = 0

        CommonFunctions.Log(CurrentFranchise, String.Format("Updating status for {0}", jobId), Nothing, LogLevel.Info)
        Using db = CommonFunctions.Create_SQL_Connection()
            Try
                tmpJobStatus = ""
                Using ms = New MemoryStream(bytes)
                    Using reader = XmlReader.Create(ms)

                        While reader.Read()
                            If reader.NodeType = XmlNodeType.Element And reader.Name = "Status" Then
                                tmpJobStatus = reader.ReadString()
                                If tmpJobStatus = "Arrived" Then
                                    tmpJobStatus = "Active"
                                End If
                                If tmpJobStatus = "Wrapup" Then
                                    tmpJobStatus = "Wrap Up"
                                End If
                                If tmpJobStatus = "Estimate Only" Then
                                    tmpJobStatus = "Waiting Estimate"
                                End If
                            End If

                            If reader.NodeType = XmlNodeType.Element And reader.Name = "EstimateYN" Then
                                tmpEstimateYN = reader.ReadString()
                            End If

                            If reader.NodeType = XmlNodeType.Element And reader.Name = "Estimate" Then
                                tmpEstimateYN = reader.ReadString()
                            End If
                        End While
                    End Using
                End Using
                'calc status id from status name
                Try
                    If tmpEstimateYN = "Estimate" Then
                        tmpJobStatus = "Waiting Estimate"
                    ElseIf tmpEstimateYN = "Yes" Then
                        tmpJobStatus = "Waiting Estimate"
                    ElseIf tmpEstimateYN = "Parts" Then
                        tmpJobStatus = "Waiting Parts"
                    ElseIf tmpEstimateYN = "People" Then
                        tmpJobStatus = "Waiting People"
                    ElseIf tmpEstimateYN = "Recall" Then
                        tmpJobStatus = "Recall"
                    End If

                    statusId = db.tbl_Job_Status.Single(Function(s) s.Status = tmpJobStatus).StatusID
                Catch ex As Exception
                    statusId = 0
                End Try

            Catch ex As Exception
                CommonFunctions.Log(CurrentFranchise, jobId, "Unable to process the status file", ex, LogLevel.Error)
            End Try

            If statusId <> 0 And jobId <> 0 Then

                Try
                    Dim job = db.tbl_Job.SingleOrDefault(Function(j) j.JobID = jobId)
                    job.StatusID = statusId
                    db.SaveChanges()
                    CommonFunctions.MyFranchiseID = job.FranchiseID

                    If tmpJobStatus = "Waiting Parts" Then _
                        AbstractBusinessService.Create(Of AlertEngine)(CommonFunctions.UserKey).SendAlert(AlertType.JobHaltedWaitingParts, job.FranchiseID)

                    CommonFunctions.Log(CurrentFranchise, job.JobID, "Job status updated.", Nothing, LogLevel.Info)

                Catch ex As Exception
                    CommonFunctions.Log(CurrentFranchise, jobId, "Unable to update job status.", ex, LogLevel.Error)
                End Try

                Try
                    'now update the status hisotry record.
                    Dim statuschange As New tbl_Job_Status_History

                    statuschange.JobID = jobId
                    Dim franchise = db.tbl_Franchise.SingleOrDefault(Function(f) f.FranchiseID = CommonFunctions.MyFranchiseID)
                    If Not franchise Is Nothing Then
                        statuschange.StatusDateChanged = statusChangedTime
                    End If
                    statuschange.StatusID = statusId
                    statuschange.ChangedOnTabletYN = True

                    db.tbl_Job_Status_History.AddObject(statuschange)
                    db.SaveChanges()

                    CommonFunctions.Log(CurrentFranchise, jobId, "Job status history logged.", Nothing, LogLevel.Debug)

                Catch ex As Exception
                    CommonFunctions.Log(CurrentFranchise, jobId, "Unable to update status history", ex, LogLevel.Error)
                End Try

                Try
                    'this is a tech status file to be updated
                    Dim assignedTechs = From j In db.tbl_Job_Technicians Where j.JobID = jobId Select j
                    For Each jobTech In assignedTechs
                        Dim serviceProId = jobTech.ServiceProID
                        Dim techjob = From t In db.tbl_Technician_Job Where t.FranchiseID = CommonFunctions.MyFranchiseID And t.ServiceProID = serviceProId And (t.CurrentJobID = jobId Or t.CurrentJobID = 0)
                        For Each tech In techjob
                            tech.CurrentStatusID = 0
                            tech.CurrentJobID = jobId
                            db.SaveChanges()
                        Next
                    Next
                    CommonFunctions.Log(CurrentFranchise, "Technician status updated.", Nothing, LogLevel.Debug)
                Catch ex As Exception
                    CommonFunctions.Log(CurrentFranchise, "Unable to update tech status history.", Nothing, LogLevel.Error)
                End Try
            End If
        End Using

        CommonFunctions.Log(CurrentFranchise, String.Format("Done processing status for {0}", jobId), Nothing, LogLevel.Info)

        Return True
    End Function

    Private Shared Function DownloadAndArchiveStatusFiles(ByVal sourceDir As String, archPath As String) As Dictionary(Of String, Byte())

        Dim files As Dictionary(Of String, Byte())

        Try
            files = Directory.GetFiles(sourceDir).Select(Function(f) New FileInfo(f)) _
                             .Where(Function(f) (f.Name.EndsWith("_tbl_JobStatusChange.xml") OrElse _
                                                 f.Name.EndsWith("_tbl_JobStatusMessage.xml") OrElse _
                                                 f.Name.EndsWith("_tbl_DispatchMessage.xml")) OrElse _
                                                 (f.Name.EndsWith("_tbl_JobStatus.xml") AndAlso
                                                  Not File.Exists(Path.Combine(f.DirectoryName, f.Name.Split("_"c).FirstOrDefault() & ".ok")))) _
                            .ToDictionary(Function(f) f.FullName, Function(f) File.ReadAllBytes(f.FullName))

            If files.Count = 0 Then
                CommonFunctions.Log(CurrentFranchise, "No status files found", Nothing, LogLevel.Debug)
                CommonFunctions.TabletSuccess = False
                Return files
            End If

            CommonFunctions.ZipFiles(files.Select(Function(f) f.Key).ToArray(), archPath, 9)

            For Each filePair In files
                Try
                    File.Delete(filePair.Key)
                Catch ex As Exception
                    CommonFunctions.Log(CurrentFranchise, "Could not delete file " & filePair.Key, ex, LogLevel.Error)
                End Try
            Next

        Catch ex As Exception
            CommonFunctions.TabletSuccess = False
            CommonFunctions.Log(CurrentFranchise, "Failed to download status files.", ex, LogLevel.Error)
            files = Nothing
        End Try

        Return files

    End Function

    Public Shared Sub ProcessStatusChanges(ByVal franchiseNumber As String)

        CurrentFranchise = franchiseNumber

        Dim inputPath = Path.Combine(CommonFunctions.InboundIPadPath, franchiseNumber)
        Dim archPath = Path.Combine(CommonFunctions.StatusArchiveFolder, franchiseNumber)

        If Not Directory.Exists(inputPath) Then Directory.CreateDirectory(inputPath)
        If Not Directory.Exists(archPath) Then Directory.CreateDirectory(archPath)

        Dim zipPath = Path.Combine(archPath, DateTime.Now.ToString(CommonFunctions.ArchiveTimeString) & ".zip")

        'download all xml files with status changes
        Dim statusFiles = DownloadAndArchiveStatusFiles(inputPath, zipPath)

        If statusFiles Is Nothing OrElse statusFiles.Count = 0 Then Return

        Try
            tmpJobStatus = String.Empty
            'completed if it came back fomr an ipad its completed or waiting estimate, if no status comes back it must be compeletd
            tmpEstimateYN = String.Empty

            For Each filePair In statusFiles

                Dim fileObj = New FileInfo(filePair.Key)
                Dim jobNumber = fileObj.Name.Split("_"c).First()
                Dim currFile = fileObj.Name.Substring(jobNumber.Length + 1)
                Select Case currFile
                    Case "tbl_JobStatusChange.xml", "tbl_JobStatus.xml"
                        ProcessIPadStatusFile(Integer.Parse(jobNumber), filePair.Value)
                    Case "tbl_JobStatusMessage.xml", "tbl_DispatchMessage.xml"
                        Dim tabletCode = New FileInfo(filePair.Key).Name.Split("_"c).First()
                        ProcessMessageFile(tabletCode, filePair.Value)
                End Select
            Next

        Catch ex As Exception
            CommonFunctions.Log(CurrentFranchise, "Could not process status changes", ex, LogLevel.Error)
        End Try

    End Sub

    Public Shared Sub ProcessJobFilesForFranchise(ByVal franchiseNumber As String)

        CurrentFranchise = franchiseNumber

        'define unique destination for inbound tablet data
        Dim inputPath = Path.Combine(CommonFunctions.InboundIPadPath, franchiseNumber)
        Dim archRootPath = Path.Combine(CommonFunctions.JobArchiveFolder, franchiseNumber)

        If Not Directory.Exists(inputPath) Then Directory.CreateDirectory(inputPath)
        If Not Directory.Exists(archRootPath) Then Directory.CreateDirectory(archRootPath)

        'If no OK files, bail now...nothing to do.
        If Not CheckForOkFiles(inputPath) Then
            CommonFunctions.Log(CurrentFranchise, CommonFunctions.InboundIPadPath & " had no files to process...", Nothing, LogLevel.Debug)
            Return
        End If

        CommonFunctions.TabletSuccess = True

        Dim jobsToProcess = GetJobsToProcess(inputPath)

        'If we could not download the OK files, bail now....error.
        If Not CommonFunctions.TabletSuccess Then
            CommonFunctions.Log(CurrentFranchise, "OK files were not downloaded.", New Exception("The download was not successful."), LogLevel.Error)
            Return
        End If

        If jobsToProcess.Count() = 0 Then
            CommonFunctions.Log(CurrentFranchise, "No files found to process.", Nothing, LogLevel.Debug)
            Return
        End If

        Dim invoiceSvc = AbstractBusinessService.Create(Of InvoiceService)(CommonFunctions.UserKey)
        Dim alertSvc = AbstractBusinessService.Create(Of AlertEngine)(CommonFunctions.UserKey)

        'loop through folder and process all OK files
        For Each jNum In jobsToProcess

            Dim jobNumber = jNum
            Dim jobProcessedWithoutError = True

            CommonFunctions.Log(CurrentFranchise, "Begin downloading files for job " & jobNumber, Nothing, LogLevel.Info)

            Dim archPath = Path.Combine(Path.Combine(CommonFunctions.JobArchiveFolder, CurrentFranchise), jobNumber.ToString())
            If Not Directory.Exists(archPath) Then Directory.CreateDirectory(archPath)
            Dim zipArchive = Path.Combine(archPath, DateTime.Now.ToString(CommonFunctions.ArchiveTimeString) & ".zip")
            Dim filesToProcess = DownloadJobFiles(jobNumber, inputPath, zipArchive)
            If filesToProcess IsNot Nothing AndAlso filesToProcess.Count() > 0 Then

                Dim isTaskFixMode = filesToProcess.Count = 1 AndAlso
                                    String.Compare(Path.GetFileName(filesToProcess.First().Key).ToLower(), jNum.ToString() & "_tbl_JobTasks.xml", True) = 0

                If isTaskFixMode Then _
                    CommonFunctions.Log(CurrentFranchise, jNum, "Entering FIX mode for job tasks", Nothing, LogLevel.Info)

                Using db = CommonFunctions.Create_SQL_Connection()

                    Dim job = db.tbl_Job.SingleOrDefault(Function(j) j.JobID = jobNumber)
                    Dim businessTypeId = job.BusinessTypeID
                    Dim franchiseId = job.FranchiseID
                    Dim jobSubTotal = 0D

                    'Skip completed jobs.
                    If job IsNot Nothing AndAlso job.StatusID <> 7 Then

                        Try

                            Dim recommendations = String.Empty
                            tmpJobStatus = ""
                            tmpStatID = 6    'completed if it came back fomr an ipad its completed or waiting estimate, if no status comes back it must be compeletd
                            Dim warranty = "1 Year"
                            tmpEstimateYN = String.Empty

                            Dim diags = String.Empty
                            Dim parsed = New JobTaskParsed

                            'loop thru all xml files in the list
                            For Each filePair In filesToProcess

                                Dim fileType = Path.GetFileName(filePair.Key).Substring(jobNumber.ToString().Length + 1)    'tbl_JobTasks.xml

                                Try

                                    Select Case fileType
                                        Case "tbl_JobTasks.xml"
                                            parsed = GetJobTasksXml(job, filePair.Value, businessTypeId, db)
                                            jobProcessedWithoutError = parsed.Successful AndAlso jobProcessedWithoutError
                                        Case "tbl_JobStatus.xml"
                                            GetJobStatusXml(jobNumber, filePair.Value)

                                        Case "tbl_Job_Diagnostics.xml"
                                            diags = GetJobDiagsXml(filePair.Value)

                                        Case "tbl_Job_Recommendations.xml"
                                            recommendations = GetJobRecommendationsXml(filePair.Value)

                                        Case "tbl_JobPayments.xml"
                                            jobProcessedWithoutError = GetJobPaymentsXml(jobNumber, filePair.Value, franchiseId, warranty)

                                        Case "tbl_Locations.xml"
                                            GetJobLocationXml(jobNumber, filePair.Value)

                                        Case "tbl_HomeGuard_Results.xml"

                                        Case "Accepted.jpg"
                                            job.AcceptedBy = filePair.Value

                                        Case "Signature.jpg"
                                            job.AuthorizationToStart = filePair.Value
                                    End Select

                                Catch ex As Exception
                                    CommonFunctions.Log(CurrentFranchise, jobNumber, "error processing file " & filePair.Key, ex, LogLevel.Error)
                                End Try
                            Next    ' xml file

                            If jobProcessedWithoutError Then
                                Try
                                    If Not isTaskFixMode Then
                                        job.WarrantyLen1 = GetWarrantyLengthId(jobNumber, warranty)
                                        job.Diagnostics = diags
                                        job.Recommendations = recommendations
                                        job.StatusID = tmpStatID
                                    End If

                                    'If a member plan was sold and this is not an estimate or a recall,
                                    'create the membership record.
                                    If parsed.MemberPlanSold AndAlso job.StatusID <> 13 AndAlso job.JobPriorityID <> 4 Then
                                        Dim maxEndDate = db.tbl_Customer_Members.Where(Function(m) m.CustomerID = job.CustomerID).ToArray().Max(Function(m) m.EndDate)
                                        Dim mi = tbl_Customer_Members.Createtbl_Customer_Members(0, job.FranchiseID, job.CustomerID, parsed.MemberPlanType)
                                        mi.StartDate = If(maxEndDate.GetValueOrDefault < DateTime.Today, DateTime.Today, maxEndDate.GetValueOrDefault.AddDays(1))
                                        mi.EndDate = mi.StartDate.GetValueOrDefault().AddYears(1)
                                        db.AddTotbl_Customer_Members(mi)
                                    End If

                                    If Not isTaskFixMode Then
                                        If job.StatusID = 6 Then job.CallCompleted = DateTime.Today
                                        job.TaxAuthorityID = GetTaxIdFromName(jobNumber, parsed.TaxName, franchiseId)
                                        job.TaxLaborPercentage = parsed.TaxRate
                                        job.TaxPartPercentage = job.TaxLaborPercentage
                                    End If

                                    Try
                                        Dim taskrecList = (From T In db.tbl_Job_Tasks Where T.JobID = jobNumber Select T).ToArray()
                                        For Each taskrec In taskrecList
                                            taskrec.AuthorizedYN = True
                                            jobSubTotal += (taskrec.Price * taskrec.Quantity)
                                            db.SaveChanges()
                                        Next
                                    Catch ex As Exception
                                        CommonFunctions.Log(CurrentFranchise, jobNumber, "Error loading job tasks for " & jobNumber, ex, LogLevel.Error)
                                        jobProcessedWithoutError = False
                                    End Try

                                    job.SubTotal = jobSubTotal
                                    job.TaxAmount = job.SubTotal * parsed.TaxRate
                                    job.TotalSales = job.SubTotal + job.TaxAmount
                                    job.Balance = job.TotalSales - db.tbl_Payments.Where(Function(p) p.JobID = job.JobID).ToArray().Sum(Function(p) p.PaymentAmount.GetValueOrDefault())

                                    If Not isTaskFixMode Then

                                        'If it's recall but has a total, treat it as a normal job.
                                        If job.JobPriorityID = 4 AndAlso job.SubTotal <> 0 Then job.JobPriorityID = 1
                                        If tmpJobStatus = "Waiting Estimate" Then
                                            job.JobPriorityID = 5
                                            job.CallCompleted = DateTime.Now
                                        End If

                                        If tmpJobStatus = "Waiting Parts" Then alertSvc.SendAlert(AlertType.JobHaltedWaitingParts, job.FranchiseID)
                                        If tmpJobStatus = "Recall" Then job.JobPriorityID = 4

                                    End If

                                    db.SaveChanges()
                                    CommonFunctions.Log(CurrentFranchise, jobNumber, "Job updated.", Nothing, LogLevel.Debug)

                                Catch ex As Exception
                                    CommonFunctions.Log(CurrentFranchise, jobNumber, "Unable to save job.", ex, LogLevel.Error)
                                    jobProcessedWithoutError = False
                                End Try
                            End If

                            If Not isTaskFixMode Then

                                'create estimate record if it doesn't exist.
                                If tmpJobStatus = "Waiting Estimate" AndAlso Not db.tbl_Job_Estimates.Any(Function(o) o.JobID = jobNumber) Then
                                    Dim newEstRec = New tbl_Job_Estimates
                                    newEstRec.JobID = jobNumber
                                    newEstRec.EstimateDate = DateTime.Today

                                    db.tbl_Job_Estimates.AddObject(newEstRec)
                                    db.SaveChanges()
                                End If

                                'coupon codes
                                If parsed.CouponCode <> "" Then
                                    Try
                                        Dim newCoderec = New tbl_Job_CouponCodes
                                        newCoderec.JobID = jobNumber
                                        newCoderec.CouponCode = parsed.CouponCode
                                        newCoderec.CouponReason = parsed.DiscountReason
                                        db.tbl_Job_CouponCodes.AddObject(newCoderec)
                                        db.SaveChanges()
                                    Catch ex As Exception
                                        CommonFunctions.Log(CurrentFranchise, job.JobID, "Could not save coupon code", ex, LogLevel.Error)
                                    End Try
                                End If

                                SendInvoice(db, invoiceSvc, job, franchiseNumber)

                            End If
                        Catch ex As Exception
                            CommonFunctions.Log(CurrentFranchise, job.JobID, "Unable to read/process job files.", ex, LogLevel.Error)
                            jobProcessedWithoutError = False
                        End Try
                    Else
                        Try
                            If Not Directory.Exists(CommonFunctions.SkipPath) Then Directory.CreateDirectory(CommonFunctions.SkipPath)
                            Dim zipArchiveObj = New FileInfo(zipArchive)
                            Dim skipFile = Path.Combine(CommonFunctions.SkipPath, franchiseId & "_" & jobNumber & "_" & zipArchiveObj.Name)
                            zipArchiveObj.MoveTo(skipFile)
                        Catch ex As Exception
                            CommonFunctions.Log(CurrentFranchise, "Unable to move to skipped directory: " & CommonFunctions.SkipPath, ex, LogLevel.Error)
                        End Try
                    End If
                End Using

                If Not jobProcessedWithoutError Then
                    Dim errFolder = Path.Combine(Path.Combine(CommonFunctions.ErrorPath, franchiseNumber), jobNumber.ToString())

                    Try
                        Dim zipArchiveObj = New FileInfo(zipArchive)
                        Dim errZipFile = Path.Combine(errFolder, zipArchiveObj.Name)
                        If Not Directory.Exists(errFolder) Then Directory.CreateDirectory(errFolder)
                        zipArchiveObj.MoveTo(errZipFile)
                    Catch ex As Exception
                        CommonFunctions.Log(CurrentFranchise, jobNumber, "Unable to migrate XML archive to ERROR path: " & errFolder, ex, LogLevel.Error)
                    End Try
                End If

            End If   'if xml files exist

            CommonFunctions.Log(CurrentFranchise, jobNumber, "Done processing job " & jobNumber, Nothing, LogLevel.Info)
        Next    'ok file

    End Sub

    Public Shared Sub Process()
        Dim franchiseList As String()

        Try

            Using db = CommonFunctions.Create_SQL_Connection()
                franchiseList = (From b In db.tbl_Franchise, C In db.tbl_Franchise_Contract _
                                   Where b.FranchiseID = C.FranchiseID _
                                   AndAlso C.TabletType = "iPad" _
                                   AndAlso b.FranchiseStatusID = 7 _
                                   AndAlso (b.FranchiseTypeID = 6 Or b.FranchiseTypeID = 5) Order By b.FranchiseNUmber Select b.FranchiseNUmber).ToArray()
            End Using
        Catch ex As Exception
            CommonFunctions.Log(CurrentFranchise, "Error loading franchises.", ex, LogLevel.Error)
            Return
        End Try

        For Each franchiseNumber In franchiseList

            CommonFunctions.Log(CurrentFranchise, String.Format("Begin processing franchise {0}", franchiseNumber), Nothing, LogLevel.Debug)

            Try
                'first check status chnages
                CommonFunctions.Log(CurrentFranchise, "Processing status changes...", Nothing, LogLevel.Debug)
                ProcessStatusChanges(franchiseNumber)
            Catch ex As Exception
                CommonFunctions.Log(CurrentFranchise, "Error was trying to read status changes", ex, LogLevel.Error)
            Finally
                CommonFunctions.Log(CurrentFranchise, "Done processing status changes...", Nothing, LogLevel.Debug)
            End Try

            Try
                'now check for complete jobs
                CommonFunctions.Log(CurrentFranchise, "Processing jobs...", Nothing, LogLevel.Debug)
                ProcessJobFilesForFranchise(franchiseNumber)
            Catch ex As Exception
                CommonFunctions.Log(CurrentFranchise, "Error trying to read job records.", ex, LogLevel.Error)
            Finally
                CommonFunctions.Log(CurrentFranchise, "Done processing jobs...", Nothing, LogLevel.Debug)
            End Try

            CommonFunctions.Log(CurrentFranchise, String.Format("Done processing franchise {0}", franchiseNumber), Nothing, LogLevel.Debug)
            frm_MainMenu.WriteToUI(String.Empty)
        Next

    End Sub

    Private Shared Sub SendInvoice(db As EightHundredEntities, invoiceSvc As InvoiceService, job As tbl_Job, franchiseNumber As String)

        Dim invoiceFile = Path.Combine(CommonFunctions.CommonEmailPath, franchiseNumber & "-" & job.JobID.ToString() & ".html")

        If File.Exists(invoiceFile) Then
            Dim sent = True
            Try

                Dim t = db.tbl_Customer.SingleOrDefault(Function(c) c.CustomerID = job.CustomerID)
                Dim email = "tobeinvoiced@1800plumber.com"

                If t IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(t.EMail) Then _
                    email = t.EMail

                Dim result = invoiceSvc.SendToCustomer(job.JobID, email)

                If Not result.Success Then
                    CommonFunctions.Log(CurrentFranchise, "Could not send invoice to customer: " & result.Message, Nothing, LogLevel.Error)
                    sent = False
                End If

            Catch ex As Exception
                CommonFunctions.Log(CurrentFranchise, "Error sending invoice to customer.", ex, LogLevel.Error)
                sent = False
            End Try

            If Not sent Then Return

            Try
                Dim fi = New FileInfo(invoiceFile)
                File.Move(fi.FullName, Path.Combine(fi.Directory.FullName, fi.Name & ".SENT"))
            Catch ex As Exception
                CommonFunctions.Log(CurrentFranchise, "Invoice sent to customer but the file could not be renamed. PLEASE REMOVE THIS FILE MANUALLY: " & invoiceFile, ex, LogLevel.Error)
            End Try
        End If
    End Sub

End Class
