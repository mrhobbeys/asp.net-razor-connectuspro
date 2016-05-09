Imports System.Configuration
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices
Imports SiteBlue.Core.Email
Imports ICSharpCode.SharpZipLib.Checksums
Imports SiteBlue.Business.Job
Imports SiteBlue.Data.EightHundred
Imports log4net
Imports ICSharpCode.SharpZipLib.Zip

Public Class CommonFunctions

    Private Shared ReadOnly BaseOutputFolder As String = ConfigurationManager.AppSettings("ProcessingRoot")
    Private Shared ReadOnly BaseInboundFilePath As String = ConfigurationManager.AppSettings("IPadRoot")

    Public Const OkFileExtension As String = ".ok"
    Public Shared ReadOnly UserKey As Guid = Guid.Empty
    Public Const ArchiveTimeString As String = "MMddyyyyHHmmss"
    Public Shared JobArchiveFolder As String = Path.Combine(Path.Combine(BaseOutputFolder, "Archive"), "Job")
    Public Shared StatusArchiveFolder As String = Path.Combine(Path.Combine(BaseOutputFolder, "Archive"), "Status")

    Public Shared ErrorPath As String = Path.Combine(BaseOutputFolder, "Error")
    Public Shared SkipPath As String = Path.Combine(BaseOutputFolder, "Skipped")

    Public Shared InboundIPadPath As String = Path.Combine(BaseInboundFilePath, "SentToOfficeIPad")
    Public Shared CommonEmailPath As String = Path.Combine(BaseInboundFilePath, "SentToOfficeEmail")
    Public Shared CommonPostOfficePath As String = Path.Combine(BaseInboundFilePath, "SentToOfficePostOffice")

    Public Shared sqlConString As String = My.MySettings.Default.EightHundredConnString
    Public Shared TabletSuccess As Boolean = True
    Public Shared MyFranchiseID As Integer

    Private Sub CommonFunctions()

    End Sub

    Public Shared Function Create_SQL_Connection() As AuditedJobContext

        Return New AuditedJobContext(UserKey, "Tablet Service", True)

    End Function

    Public Shared Function Get_Account_Code_ByCode(franchiseId As Integer, techId As Integer, ByVal jobCode As String, ByVal BusTypeId As Integer, db As EightHundredEntities) As String
        Dim accountCode = ""
        Try
            Dim pricebook = db.tbl_Franchise_Tablets.First(Function(ft) ft.FranchiseID = franchiseId AndAlso ft.EmployeeID = techId).PriceBookID
            Dim pb_jobcode = db.tbl_PB_JobCodes.First(Function(jc) jc.JobCode = jobCode AndAlso jc.tbl_PB_SubSection.tbl_PB_Section.PriceBookID = pricebook)
            If BusTypeId = 3 Then
                accountCode = pb_jobcode.ComAccountCode
            Else
                accountCode = pb_jobcode.ResAccountCode
            End If
            Return accountCode
        Catch ex As Exception
            Log(franchiseId.ToString(), "Could not find account code for job code " & jobCode, ex, LogLevel.Error)
            Return Nothing
        End Try
    End Function

    Public Shared Function Get_Franchise_Number(ByVal tmpFranchiseID As Integer) As String
        Try
            Using db = Create_SQL_Connection()
                Dim tmpFranchise = (From t In db.tbl_Franchise Where t.FranchiseID = tmpFranchiseID Select t.FranchiseNUmber).Single
                Get_Franchise_Number = tmpFranchise
            End Using
        Catch ex As Exception
            Get_Franchise_Number = "N/A"
        End Try

    End Function    'Get_Franchise_Number

    Public Shared Function Get_Employee_Name(ByVal tmpEmployeeID As Integer) As String
        Try
            Using db = Create_SQL_Connection()
                Dim tmpEmployee = (From t In db.tbl_Employee Where t.EmployeeID = tmpEmployeeID Select t.Employee).Single
                Get_Employee_Name = tmpEmployee
            End Using
        Catch ex As Exception
            Get_Employee_Name = "N/A"
        End Try

    End Function    'Get_Employee_Name

    Public Shared Function GetTimeSpecificArchiveFolder() As String
        Return DateTime.Now.ToString(ArchiveTimeString)
    End Function

    Private Shared ReadOnly jobLogger As ILog = LogManager.GetLogger("JobLogger")

    Public Shared Sub Log(franchise As String, msg As String, e As Exception, level As LogLevel)
        LogInternal(jobLogger, franchise, Nothing, msg, e, level)
    End Sub

    Public Shared Sub Log(franchiseId As String, jobId As Integer, msg As String, e As Exception, level As LogLevel)
        LogInternal(jobLogger, franchiseId, jobId, msg, e, level)
    End Sub

    Private Shared Sub LogInternal(logger As ILog, franchise As String, jobId As Integer?, msg As String, e As Exception, level As LogLevel)

        Dim uiMsg = msg & ": " & If(e Is Nothing, String.Empty, e.ToString())

        If e IsNot Nothing AndAlso Convert.ToString(ConfigurationManager.AppSettings("DebugMode")).ToLower() = "true" Then _
            MessageBox.Show(uiMsg)

        Select Case level
            Case LogLevel.Info
                If logger.IsInfoEnabled Then logger.Info(msg, e)
            Case LogLevel.Error
                If logger.IsErrorEnabled Then logger.Error(msg, e)

                Dim email = New EmailEngine()
                Dim m = New MailMessage()
                m.To.Add(ConfigurationManager.AppSettings("ErrorRecipients"))
                Dim env = ConfigurationManager.AppSettings("Environment")
                m.Subject = env & " - Error in Connectus Systems Application Server"
                m.Body = "Franchise ID: " & If(String.IsNullOrWhiteSpace(franchise), "UNKNOWN", franchise) & "<br/>JobID: " &
                    If(Not jobId.HasValue, "UNKNOWN", jobId.ToString()) & "<br /><br />" &
                        msg & "<br /><br />" & e.ToString()
                m.IsBodyHtml = True
                m.Priority = MailPriority.High
                email.Send(m)
            Case (LogLevel.Warn)
                If logger.IsWarnEnabled Then logger.Warn(msg, e)
            Case (LogLevel.Debug)
                If logger.IsDebugEnabled Then logger.Debug(msg, e)

        End Select

        frm_MainMenu.WriteToUI(String.Format("{0}: {1}", DateTime.Now, uiMsg))

    End Sub

    ''' <summary> 
    ''' Load an image from a file, and create a byte array using the image format type 
    ''' (i.e. Bmp, Png, Jpeg, etc.) 
    ''' </summary> 
    ''' <param name="fileName">Full path file name for the image file.</param> 
    ''' <returns>A byte array representing the image.</returns> 
    Public Shared Function ImageFromFile(ByVal fileName As String) As Byte()

        Using img = Image.FromFile(fileName)
            Using ms = New MemoryStream()
                img.Save(ms, ImageFormat.Bmp)
                Return ms.GetBuffer()
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Writes the zip file.
    ''' </summary>
    ''' <param name="filesToZip">The files to zip.</param>
    ''' <param name="zipFile">The destination zip file.</param>
    ''' <param name="compression">The compression level.</param>
    Public Shared Sub ZipFiles(filesToZip As String(), zipFile As String, compression As Integer)

        If compression < 0 OrElse compression > 9 Then _
            Throw New ArgumentException("Invalid compression rate.")

        If Not Directory.Exists(New FileInfo(zipFile).Directory.ToString()) Then _
            Throw New ArgumentException("The Path does not exist.")

        If filesToZip.Any(Function(c1) Not File.Exists(c1)) Then _
            Throw New ArgumentException(String.Format("The File{0}does not exist!", filesToZip.First(Function(f) Not File.Exists(f))))

        Dim crc32 As New Crc32()
        Using stream As New ZipOutputStream(File.Create(zipFile))
            stream.SetLevel(compression)

            For i = 0 To filesToZip.Count - 1
                Dim entry = New ZipEntry(Path.GetFileName(filesToZip(i)))
                entry.DateTime = DateTime.Now

                Using fs = File.OpenRead(filesToZip(i))
                    Dim buffer = New Byte(Convert.ToInt32(fs.Length) - 1) {}
                    fs.Read(buffer, 0, buffer.Length)
                    entry.Size = fs.Length
                    fs.Close()
                    crc32.Reset()
                    crc32.Update(buffer)
                    entry.Crc = crc32.Value
                    stream.PutNextEntry(entry)
                    stream.Write(buffer, 0, buffer.Length)
                End Using
            Next
            stream.Finish()
            stream.Close()
        End Using
    End Sub

End Class

Public Enum LogLevel
    [Error] = 0
    Warn = 1
    Info = 2
    Debug = 3
End Enum

Public Module Extensions
    <Extension()>
    Public Function TruncateWithEllipsis(val As String, length As Integer) As String

        If length <= 0 Then Throw New ArgumentException("Must be a value greater than 0", "length")

        Dim ellipsis = " ..."
        If val Is Nothing Then Return Nothing
        If String.IsNullOrWhiteSpace(val) Then Return String.Empty

        val = val.Trim()

        If length <= ellipsis.Length Then Return ellipsis.Truncate(length)
        If val.Length <= length Then Return val

        Return val.Truncate(length - ellipsis.Length) + If(val.Length > length, ellipsis, String.Empty)
    End Function

    <Extension()>
    Public Function Truncate(val As String, length As Integer) As String

        If length <= 0 Then Throw New ArgumentException("Must be a value greater than 0", "length")

        If val Is Nothing Then Return Nothing
        If String.IsNullOrWhiteSpace(val) Then Return String.Empty

        val = val.Trim()

        Return val.Substring(0, Math.Min(val.Length, length))
    End Function
End Module