Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class MainForm
    Private themesGet As New themeapi
    Private curThemes
    Private curtheme
    Private popular = False

    Private Sub refreshThemes()
        ToolStripStatusLabel1.Text = "Loading theme list..."
        ListBox1.Items.Clear()
        If popular Then
            curThemes = themesGet.getPopular()
        Else
            themesGet.setSearch(TextBox1.Text)
            curThemes = themesGet.getSearch()
        End If
        For Each theme In curThemes
            ListBox1.Items.Add(theme("name"))
        Next
        ToolStripStatusLabel1.Text = "Waiting."
    End Sub
    Private Sub Search_Click(sender As Object, e As EventArgs) Handles Button1.Click
        popular = False
        themesGet.setPage(1)
        refreshThemes()
    End Sub

    Private Sub Popular_Click(sender As Object, e As EventArgs) Handles Button2.Click
        popular = True
        themesGet.setPage(1)
        refreshThemes()
    End Sub

    Private Sub back_click(sender As Object, e As EventArgs) Handles Button4.Click
        themesGet.setPage(themesGet.getPage - 1)
        refreshThemes()
    End Sub

    Private Sub forward_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If ListBox1.Items.Count = 15 Then
            themesGet.setPage(themesGet.getPage + 1)
            refreshThemes()
        End If

    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        ToolStripStatusLabel1.Text = "Setting up theme info..."
        curtheme = curThemes(ListBox1.SelectedIndex)

        themesGet.setID(curtheme("id"))
        curtheme = themesGet.getThemeJson()

        ToolStripStatusLabel1.Text = "Loading description..."
        RichTextBox1.Text = curtheme("desc")
        ToolStripStatusLabel1.Text = "Loading Title..."
        Label2.Text = curtheme("name")
        ToolStripStatusLabel1.Text = "Loading author..."
        Label3.Text = curtheme("by")
        ToolStripStatusLabel1.Text = "Loading tags..."
        RichTextBox2.Text = curtheme("tags")
        ToolStripStatusLabel1.Text = "Loading image..."
        PictureBox1.Image = themesGet.getImage()

        ToolStripStatusLabel1.Text = "Waiting."
    End Sub

    Private Sub sendtods_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ToolStripStatusLabel1.Text = "Attempting to send to 3ds!"

        Dim themeName As String = curtheme("name")
        For Each c In IO.Path.GetInvalidFileNameChars
            themeName = themeName.Replace(c, "")
        Next
        themeName += ".zip"
        Dim themePath = Application.StartupPath + "/" + themeName

        Try
            My.Computer.Network.DownloadFile("https://3dsthem.es/downloadTheme.php?i=" + curtheme("id"), themePath)

            ToolStripStatusLabel1.Text = "File download success! Attempting transfer..."
            Me.Refresh()
        Catch ex As Exception
            ToolStripStatusLabel1.Text = "File download error! Ending transfer..."
            Me.Refresh()
            MessageBox.Show("Error!", "3ds Theme Downloader")
            MessageBox.Show(ex.ToString, "3ds Theme Downloader")
            Exit Sub
        End Try
        Try
            Dim path As String = ""
            If RadioButton1.Checked Then path = RadioButton1.Text
            If RadioButton2.Checked Then path = RadioButton2.Text
            If RadioButton3.Checked Then path = RadioButton3.Text

            uploadFle("ftp://" + TextBox2.Text + path + themeName, themePath)

            ToolStripStatusLabel1.Text = "File transfer succcess!"
            Me.Refresh()
        Catch ex As Exception
            ToolStripStatusLabel1.Text = "File upload error! Ending transfer..."
            Me.Refresh()
            MessageBox.Show("Error!", "3ds Theme Downloader")
            MessageBox.Show(ex.ToString, "3ds Theme Downloader")
            Exit Sub
        End Try
        Try
            My.Computer.FileSystem.DeleteFile(themePath)
        Catch ex As Exception
            ToolStripStatusLabel1.Text = "Theme was not deleted, please delete manually..."
            Me.Refresh()
            MessageBox.Show("Error!", "3ds Theme Downloader")
            MessageBox.Show(ex.ToString, "3ds Theme Downloader")
            Exit Sub
        End Try
        MessageBox.Show("Successful transfer!", "3ds Theme Downloader")
    End Sub

    Private Sub uploadFle(ByVal destination, ByVal filePath)
        Dim ftpR As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(destination), System.Net.FtpWebRequest)
        ftpR.Method = System.Net.WebRequestMethods.Ftp.UploadFile
        ftpR.AuthenticationLevel = Security.AuthenticationLevel.None
        ftpR.UsePassive = False
        Dim file() As Byte = System.IO.File.ReadAllBytes(filePath)

        Dim strz As System.IO.Stream = ftpR.GetRequestStream()
        strz.Write(file, 0, file.Length)
        strz.Close()
        strz.Dispose()
    End Sub

End Class
