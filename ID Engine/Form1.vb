Imports System.Net
Public Class mainForm
    Dim wheretosave As String
    Delegate Sub changetextsafe(ByVal length As Long, ByVal position As Long, ByVal percent As Integer, ByVal speed As Double)
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
    Public Sub changetexts(ByVal length As Long, ByVal position As Long, ByVal percent As Integer, ByVal speed As Double)
        Me.Label3.Text = If(length >= 0, "File Size : " & Math.Round(length / 1024) & "KB", "File Size : Unknown")
        Me.Label5.Text = "Downloading... : " & Me.txtFilename.Text
        Me.Label4.Text = "Downloaded : " & Math.Round(position / 1024, 2) & "KB"
        If length > 0 Then Me.Label4.Text &= " of " & Math.Round(length / 1024, 2) & "KB (" & percent & "%)"
        If speed = -1 Then
            Me.Label2.Text = "Download Speed : Speed Calculating..."
        Else
            Me.Label2.Text = "Download Speed : " & Math.Round(speed / 1024) & "KB/s"
        End If
        Me.ProgressBar1.Value = percent
    End Sub
    Private Sub btnDownload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDownload.Click
        Dim address As Uri = Nothing
        If Not Uri.TryCreate(Me.txtFilename.Text.Trim(), UriKind.Absolute, address) OrElse
            (address.Scheme <> Uri.UriSchemeHttp AndAlso address.Scheme <> Uri.UriSchemeHttps) Then
            MessageBox.Show("Please enter a valid HTTP or HTTPS URL.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Me.SaveFileDialog1.FileName = IO.Path.GetFileName(address.AbsolutePath)
        If Me.SaveFileDialog1.ShowDialog() <> DialogResult.OK Then Return

        Me.wheretosave = Me.SaveFileDialog1.FileName
        Me.SaveFileDialog1.FileName = ""
        Me.Label6.Text = "Save to : " & Me.wheretosave
        Me.txtFilename.Enabled = False
        Me.btnDownload.Enabled = False
        Me.btnCancel.Enabled = True
        Me.ProgressBar1.Value = 0
        Dialog1.ListBox1.Items.Add(Label8.Text & "  File : " & Me.wheretosave & "   URL : " & txtFilename.Text)
        Me.BackgroundWorker1.RunWorkerAsync(address)
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
        Dim destination As String = Me.wheretosave
        Dim partialPath As String = IO.Path.Combine(IO.Path.GetDirectoryName(destination), Guid.NewGuid().ToString("N") & ".part")
        Dim ownsPartial As Boolean = False
        Try
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(DirectCast(e.Argument, Uri)), HttpWebRequest)
            request.Timeout = 30000
            request.ReadWriteTimeout = 30000
            Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse),
                  input As IO.Stream = response.GetResponseStream(),
                  output As New IO.FileStream(partialPath, IO.FileMode.CreateNew, IO.FileAccess.Write)
                ownsPartial = True
                Dim length As Long = response.ContentLength
                Dim position As Long = 0
                Dim buffer(4095) As Byte
                Dim speedTimer As Stopwatch = Stopwatch.StartNew()
                Dim update As New changetextsafe(AddressOf changetexts)
                Dim lastUpdate As Long = -250
                Do
                    If Me.BackgroundWorker1.CancellationPending Then
                        e.Cancel = True
                        Return
                    End If

                    Dim count As Integer = input.Read(buffer, 0, buffer.Length)
                    If count = 0 Then Exit Do
                    output.Write(buffer, 0, count)
                    position += count

                    If speedTimer.ElapsedMilliseconds - lastUpdate >= 250 Then
                        Dim percent As Integer = If(length > 0, CInt(Math.Min(100.0, position * 100.0 / length)), 0)
                        Dim speed As Double = position / Math.Max(0.001, speedTimer.Elapsed.TotalSeconds)
                        Me.Invoke(update, length, position, percent, speed)
                        lastUpdate = speedTimer.ElapsedMilliseconds
                    End If
                Loop

                If length >= 0 AndAlso position <> length Then
                    Throw New IO.IOException("The download ended before the whole file arrived.")
                End If
            End Using

            If Me.BackgroundWorker1.CancellationPending Then
                e.Cancel = True
                Return
            End If
            If IO.File.Exists(destination) Then
                IO.File.Replace(partialPath, destination, Nothing)
            Else
                IO.File.Move(partialPath, destination)
            End If
            ownsPartial = False
        Finally
            If ownsPartial Then IO.File.Delete(partialPath)
        End Try
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If e.Error IsNot Nothing Then
            Me.txtFilename.Enabled = True
            Me.btnDownload.Enabled = True
            Me.btnCancel.Enabled = False
            Me.Label4.Text = "Download failed."
            MessageBox.Show(e.Error.Message, "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            If Not e.Cancelled Then Me.ProgressBar1.Value = 100
            downloadcomplete(e.Cancelled)
        End If
    End Sub

    Private Sub mainForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        If Me.BackgroundWorker1.IsBusy AndAlso e.CloseReason = CloseReason.UserClosing Then
            Me.BackgroundWorker1.CancelAsync()
            e.Cancel = True
        End If
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
