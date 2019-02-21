Public Class frmabt

    Private Sub Form3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.about
        Me.ShowInTaskbar = False
        Me.BackgroundImage = My.Resources._150_x_150_logo_transparent
        Me.Button1.Focus()

        Me.Label4.Text = "Share this freeware with your families and friends." & vbNewLine & "They will be happy to use ID Engine. Thank you!"

        On Error Resume Next
        Const file As String = "about.std"
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
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)

        Me.LabelProductName.Text = "Product     :   " & My.Application.Info.ProductName
        Me.LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString)
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = "Company     :   " & My.Application.Info.CompanyName
        Me.TextBoxDescription.Text = "" & My.Application.Info.Description
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub frmabt_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Width = 600
        Me.Height = 400
    End Sub
End Class