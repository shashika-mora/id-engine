Imports System.Net
Public Class mainForm
    Dim wheretosave As String
    Delegate Sub changetextsafe(ByVal length As Long, ByVal position As Integer, ByVal percent As Integer, ByVal speed As Double)
    Delegate Sub downloadcompletesafe(ByVal cancelled As Boolean)
    Public Sub downloadcomplete(ByVal cancelled As Boolean)
        Me.txtFilename.Enabled = True
        Me.btnDownload.Enabled = True
        Me.btnCancel.Enabled = False
        If cancelled Then
            Me.Label4.Text = "Cancelled !"
            MessageBox.Show("Download Cancelled !", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Me.Label4.Text = "Successfully Downloaded !"
            MessageBox.Show("Successfully Downloaded !", "Downloaded", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Timer1.Enabled = True
        End If
    End Sub
    Public Sub changetexts(ByVal length As Long, ByVal position As Integer, ByVal percent As Integer, ByVal speed As Double)
        Me.Label3.Text = "File Size : " & Math.Round(length / 1024) & "KB"
        Me.Label5.Text = "Downloading... : " & Me.txtFilename.Text
        Me.Label4.Text = "Downloaded : " & Math.Round((position / 1024), 2) & "KB of " & Math.Round((length / 1024), 2) & "KB (" & Me.ProgressBar1.Value & " %)"
        If speed = -1 Then
            Me.Label2.Text = "Download Speed : Speed Calculating..."
        Else
            Me.Label2.Text = "Download Speed : " & Math.Round(speed / 1024) & "KB/s"
        End If
        Me.ProgressBar1.Value = percent
    End Sub
    Private Sub btnDownload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDownload.Click
        If Me.txtFilename.Text <> "" AndAlso (Me.txtFilename.Text.StartsWith("http://") Or Me.txtFilename.Text.StartsWith("HTTP://") Or (Me.txtFilename.Text.StartsWith("https://"))) Then
            Me.SaveFileDialog1.FileName = Me.txtFilename.Text.Split("/"c)(Me.txtFilename.Text.Split("/"c).Length - 1)
            If Me.SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Me.wheretosave = Me.SaveFileDialog1.FileName
                Me.SaveFileDialog1.FileName = ""
                Me.Label6.Text = "Save to : " & Me.wheretosave
                Me.txtFilename.Enabled = False
                Me.btnDownload.Enabled = False
                Me.btnCancel.Enabled = True
                Me.BackgroundWorker1.RunWorkerAsync()
            End If
        Else
            MessageBox.Show("Please Input a valid URL for Download.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        Dialog1.ListBox1.Items.Add(Label8.Text & "  File : " & Me.wheretosave & "   URL : " & txtFilename.Text)
    End Sub
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.cantrim.Enabled = True
        Me.BackgroundWorker1.CancelAsync()
    End Sub
    Private Sub mainForm_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Leave
        Me.Close()
        Dialog1.Close()
        frmabt.Close()
        frmhlp.Close()
    End Sub
    Private Sub mainForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Label8.Visible = False
        Me.logontrim.Enabled = True
        Me.Label4.Text = ""
        Me.Timer1.Enabled = False
        Me.txtFilename.Text = Clipboard.GetText
        Me.BackgroundImage = My.Resources.id_background
        Me.Icon = My.Resources.asd
        Me.txtFilename.Focus()
    End Sub
    Private Sub BackgroundWorker1_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim theResponse As HttpWebResponse
        Dim theRequest As HttpWebRequest
        Try
            theRequest = WebRequest.Create(Me.txtFilename.Text)
            theResponse = theRequest.GetResponse
        Catch ex As Exception
            MessageBox.Show("An Error occurred while downloading file : " & vbNewLine & ControlChars.CrLf & "1) File doesn't exist." & ControlChars.CrLf & "2) Remote server Error.", "Error Founded !", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Dim canceldelegate As New downloadcompletesafe(AddressOf downloadcomplete)
            Me.Invoke(canceldelegate, True)
            Exit Sub
        End Try
        Dim length As Long = theResponse.ContentLength
        Dim safedelegate As New changetextsafe(AddressOf changetexts)
        Me.Invoke(safedelegate, length, 0, 0, 0)
        Dim writestream As New IO.FileStream(Me.wheretosave, IO.FileMode.Create)
        Dim nread As Integer
        Dim speedtimer As New Stopwatch
        Dim currentspeed As Double = -1
        Dim readings As Integer = 0
        Do
            If BackgroundWorker1.CancellationPending Then
                Exit Do
            End If
            speedtimer.Start()
            Dim readbytes(4095) As Byte
            Dim bytesread As Integer = theResponse.GetResponseStream.Read(readbytes, 0, 4096)
            nread += bytesread
            Dim percent As Short = (nread * 100) / length
            Me.Invoke(safedelegate, length, nread, percent, currentspeed)
            If bytesread = 0 Then Exit Do
            writestream.Write(readbytes, 0, bytesread)
            speedtimer.Stop()
            readings += 1
            If readings >= 5 Then
                currentspeed = 20408 / (speedtimer.ElapsedMilliseconds / 1000)
                speedtimer.Reset()
                readings = 0
            End If
        Loop
        writestream.Close()
        If Me.BackgroundWorker1.CancellationPending Then
            IO.File.Delete(Me.wheretosave)
            Dim canceldelegate As New downloadcompletesafe(AddressOf downloadcomplete)
            Me.Invoke(canceldelegate, True)
            Exit Sub
        End If
        Dim completedelegates As New downloadcompletesafe(AddressOf downloadcomplete)
        Me.Invoke(completedelegates, False)
    End Sub

    Private Sub logontrim_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles logontrim.Tick
        Label8.Text = Now
        If ProgressBar1.Value >= 1 Then
            ProgressBar1.Cursor = Cursors.WaitCursor
        End If
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dialog1.Show()
        Dialog1.ShowInTaskbar = False
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        frmhlp.Show()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        frmabt.Show()
    End Sub
    Private Sub mainForm_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Width = 900
        Me.Height = 500
    End Sub
End Class
