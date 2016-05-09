Imports System.Configuration
Imports log4net.Config

Public Class frm_MainMenu
    Private _engine As Engine

    Delegate Sub SetTextCallback(text As String)

    Private Sub FrmMainMenuLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        lbl_ConnectionString.Text = My.Settings.EightHundredConnString
        _engine = Engine.Instance

    End Sub

    Private Sub FrmMainMenuClose(sender As Object, e As EventArgs) Handles MyBase.FormClosing
        _engine.Dispose()
    End Sub

    Private Sub LblExitLinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles lbl_exit.LinkClicked
        _engine.Dispose()
    End Sub    'lbl_exit_LinkClicked

    Private Shared inst As frm_MainMenu

    Public Shared Sub WriteToUI(msg As String)
        If inst IsNot Nothing Then inst.WriteInternal(msg)
    End Sub

    Public Sub WriteInternal(msg As String)
        msg = msg.Trim(Environment.NewLine.ToCharArray())
        msg &= Environment.NewLine
        If txt_MessageBox.InvokeRequired Then
            Dim d = New SetTextCallback(AddressOf WriteInternal)
            Me.Invoke(d, New Object() {msg})
        Else
            If txt_MessageBox.Text.Length > 50000 Then txt_MessageBox.Text = String.Empty
            txt_MessageBox.Text &= msg
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        _engine.Pause(Nothing)
        LinkLabel1.Text = If(_engine.IsEnabled, "Stop", "Start") & " Processing"
        MessageBox.Show("Processing is now: " & If(_engine.IsEnabled, "ON", "OFF"))

    End Sub



    Public Sub New()

        InitializeComponent()
        inst = Me

    End Sub
End Class