Imports System.Windows.Forms

Public Class Dialog1
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.Hide()
    End Sub

    Private Sub Dialog1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Width = 892
        Me.Height = 344
    End Sub
End Class
