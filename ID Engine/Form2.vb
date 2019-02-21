Public Class frmhlp
    Private Sub frmhlp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.help
        Me.ShowInTaskbar = False
        Me.BackgroundImage = My.Resources.back___Copy
        Me.Button1.Focus()
        On Error Resume Next
        Const file As String = "help.std"
        Dim tmp As String
        tmp = ""
        FileSystem.FileOpen(1, file, OpenMode.Input)
        Do While Not FileSystem.EOF(1)
            tmp = tmp & FileSystem.LineInput(1)
            If Not FileSystem.EOF(1) Then
                tmp = tmp & Chr(13) & Chr(10)
            End If
        Loop
        FileSystem.FileClose(1)
        Label1.Text = tmp
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
    Private Sub frmhlp_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Width = 770
        Me.Height = 522
    End Sub
End Class