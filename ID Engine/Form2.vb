Public Class frmhlp
    Private Sub frmhlp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.asd
        Me.ShowInTaskbar = False
        Me.BackgroundImage = My.Resources.back___Copy
        Me.Button1.Focus()
        Dim file As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "help.std")
        If IO.File.Exists(file) Then
            Label1.Text = IO.File.ReadAllText(file, System.Text.Encoding.Default)
        End If
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
    Private Sub frmhlp_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Width = 770
        Me.Height = 522
    End Sub
End Class