Imports System.Configuration
Imports System.Threading

Public Class Engine
    Implements IDisposable

    'Private ReadOnly jobFileWatcher As New FileSystemWatcher()
    Private ReadOnly _timer As New System.Timers.Timer()
    Private _isRunning As Boolean

    Public Property IsRunning() As Boolean
        Get
            Return _isRunning
        End Get
        Private Set(ByVal value As Boolean)
            _isRunning = value
        End Set
    End Property

    Private Shared m_instance As Engine
    Private Shared syncRoot As New Object()


    Public ReadOnly Property IsEnabled() As Boolean
        Get
            'Return jobFileWatcher.EnableRaisingEvents
            Return _timer.Enabled
        End Get
    End Property

    Public Shared ReadOnly Property Instance() As Engine
        Get
            If m_instance IsNot Nothing Then
                Return m_instance
            End If

            SyncLock syncRoot
                Return If(m_instance, New Engine())
            End SyncLock
        End Get
    End Property

    Private Sub New()
        'jobFileWatcher.Path = CommonFunctions.InboundIPadPath
        'jobFileWatcher.IncludeSubdirectories = True

        'If Not jobFileWatcher.EnableRaisingEvents Then
        '    jobFileWatcher.EnableRaisingEvents = True
        'End If
        'AddHandler jobFileWatcher.Created, AddressOf EventTarget

        Dim interval As Integer
        If Not Integer.TryParse(ConfigurationManager.AppSettings("Interval"), interval) Then _
            interval = 60000

        _timer.Interval = interval
        _timer.Enabled = True
        AddHandler _timer.Elapsed, AddressOf EventTarget
        _timer.Start()
    End Sub

    'Private Shared Sub EventTarget(sender As Object, e As FileSystemEventArgs)
    '    Dim fi = New FileInfo(e.FullPath)
    '    If fi.Extension <> CommonFunctions.OkFileExtension Then
    '        Return
    '    End If

    '    'Dim franchise = fi.Directory.Name
    '    'XMLProcessingFunctions.ProcessJobFilesForFranchise(franchise)
    'End Sub

    Private Sub EventTarget(sender As Object, e As EventArgs)
        Try
            IsRunning = True
            CommonFunctions.Log(String.Empty, "Begin processing files", Nothing, LogLevel.Info)
            _timer.Stop()
            XMLProcessingFunctions.Process()
        Catch ex As Exception
            CommonFunctions.Log(String.Empty, "An unhandled exception occurred.", ex, LogLevel.Error)
        Finally
            _timer.Start()
            CommonFunctions.Log(String.Empty, "End processing files", Nothing, LogLevel.Info)
            IsRunning = False
        End Try

    End Sub

    Public Sub Pause(pause As Boolean?)
        'jobFileWatcher.EnableRaisingEvents = If(pause, Not pause.Value, Not jobFileWatcher.EnableRaisingEvents)
        _timer.Enabled = If(pause, Not pause.Value, Not _timer.Enabled)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        'If jobFileWatcher IsNot Nothing Then
        '    jobFileWatcher.Dispose()
        'End If

        _timer.Stop()

        If _timer IsNot Nothing Then
            _timer.Dispose()
        End If
    End Sub
End Class
