Imports CloudSync.Form1

Public Class Form2
    Public Sub BrowseCloudFolder(ByVal CloudService As Int16, ByVal tothrowerrors As String)
        With FolderBrowserDialog1
            .RootFolder = Environment.SpecialFolder.Desktop
            .SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            .Description = "Select " + CloudServiceAsString(CloudService) + " Directory"
            If .ShowDialog = DialogResult.OK Then
                WriteDefaultDirectory(CloudService, .SelectedPath)
            End If
            '!if closed error is thrown!
        End With
    End Sub

    Public Sub WriteDefaultDirectory(ByVal CloudService As Int16, ByVal path As String)
        Settings(CloudService, CSDirectory) = path
        Settings(CloudService, CSDefaultDirectoryToSaveForFiles) = path
        Settings(CloudService, CSDefaultDirectoryToSaveForFolders) = path
        WriteSettings()
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        If Form2Browse = True Then
            If Settings(0, CSDirectory) <> Nothing Then
                If MsgBoxResult.No = MsgBox("The current directory for Dropbox is " + Settings(0, CSDirectory) + vbNewLine + vbNewLine + "Would you like to select a new directory?", MsgBoxStyle.YesNo) Then
                    Return
                End If
            End If
            Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\Dropbox"
            If My.Computer.FileSystem.DirectoryExists(path) Then
                Dim result As MsgBoxResult = MsgBox(path + " has been detected." + vbNewLine + vbNewLine + "Would you like to use this as your default Dropbox folder?", MsgBoxStyle.YesNoCancel, "Default Dropbox Folder")
                If result = MsgBoxResult.Yes Then WriteDefaultDirectory(Dropbox, path)
                If result = MsgBoxResult.No Then BrowseCloudFolder(0, "")
            Else
                BrowseCloudFolder(Dropbox, "")
            End If
        Else
            SelectedCloudService = 0
            Me.Close()
        End If
    End Sub

    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
    End Sub

    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
    End Sub

    Private Sub PictureBox1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.MouseEnter
        PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Label1.Text = Settings(Dropbox, CSDirectory)
        Label1.BringToFront()
        Label1.BringToFront()
        Label1.BringToFront()
    End Sub

    Private Sub PictureBox1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.MouseLeave
        PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Label1.SendToBack()
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        If Form2Browse = True Then
            If Settings(1, CSDirectory) <> Nothing Then
                If MsgBoxResult.No = MsgBox("The current directory for SkyDrive is " + Settings(1, CSDirectory) + vbNewLine + vbNewLine + "Would you like to select a new directory?", MsgBoxStyle.YesNo) Then
                    Return
                End If
            End If
            Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\SkyDrive"
            If My.Computer.FileSystem.DirectoryExists(path) Then
                Dim result As MsgBoxResult = MsgBox(path + " has been detected." + vbNewLine + vbNewLine + "Would you like to use this as your default SkyDrive folder?", MsgBoxStyle.YesNoCancel, "Default SkyDrive Folder")
                If result = MsgBoxResult.Yes Then WriteDefaultDirectory(SkyDrive, path)
                If result = MsgBoxResult.No Then BrowseCloudFolder(SkyDrive, "")
            Else
                BrowseCloudFolder(SkyDrive, "")
            End If
        Else
            SelectedCloudService = 1
            Me.Close()
        End If
    End Sub

    Private Sub PictureBox2_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox2.MouseUp
        PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
    End Sub

    Private Sub PictureBox2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox2.MouseDown
        PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
    End Sub

    Private Sub PictureBox2_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox2.MouseEnter
        PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        'Label2.Text = Settings(SkyDrive, CSDirectory)
        'Label2.BringToFront()
    End Sub

    Private Sub PictureBox2_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox2.MouseLeave
        PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Label2.SendToBack()
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        If Form2Browse = True Then
            If Settings(2, CSDirectory) <> Nothing Then
                If MsgBoxResult.No = MsgBox("The current directory for Google Drive is " + Settings(2, CSDirectory) + vbNewLine + vbNewLine + "Would you like to select a new directory?", MsgBoxStyle.YesNo) Then
                    Return
                End If
            End If
            Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\Google Drive"
            If My.Computer.FileSystem.DirectoryExists(path) Then
                Dim result As MsgBoxResult = MsgBox(path + " has been detected." + vbNewLine + vbNewLine + "Would you like to use this as your default Google Drive folder?", MsgBoxStyle.YesNoCancel, "Default Google Drive Folder")
                If result = MsgBoxResult.Yes Then WriteDefaultDirectory(GoogleDrive, path)
                If result = MsgBoxResult.No Then BrowseCloudFolder(GoogleDrive, "")
            Else
                BrowseCloudFolder(GoogleDrive, "")
            End If
        Else
            SelectedCloudService = 2
            Me.Close()
        End If
    End Sub

    Private Sub PictureBox3_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox3.MouseUp
        PictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
    End Sub

    Private Sub PictureBox3_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox3.MouseDown
        PictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
    End Sub

    Private Sub PictureBox3_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox3.MouseEnter
        PictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        'Label3.Text = Settings(GoogleDrive, CSDirectory)
        'Label3.BringToFront()
    End Sub

    Private Sub PictureBox3_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox3.MouseLeave
        PictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Label3.SendToBack()
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Label1.Parent = PictureBox1
        'Label2.Parent = PictureBox2
        'Label3.Parent = PictureBox3

        PictureBox1.Enabled = True
        PictureBox1.Image = My.Resources.Dropbox_Banner
        PictureBox2.Enabled = True
        PictureBox2.Image = My.Resources.SkyDrive_Banner
        PictureBox3.Enabled = True
        PictureBox3.Image = My.Resources.GoogleDrive_Banner

        If Form2Browse = False Then
            Me.Text = "Cloud Service to Sync With"
            If Settings(0, CSDirectory) = Nothing Then
                PictureBox1.Enabled = False
                PictureBox1.Image = My.Resources.Dropbox_Banner__Disabled_
            End If
            If Settings(1, CSDirectory) = Nothing Then
                PictureBox2.Enabled = False
                PictureBox2.Image = My.Resources.SkyDrive_Banner__Disabled_
            End If
            If Settings(2, CSDirectory) = Nothing Then
                PictureBox3.Enabled = False
                PictureBox3.Image = My.Resources.GoogleDrive_Banner__Disabled_
            End If
        End If
    End Sub

    Private Sub Label1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Label1.MouseLeave
        PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Label1.SendToBack()
    End Sub

    Private Sub Label2_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Label2.MouseLeave
        PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Label2.SendToBack()
    End Sub

    Private Sub Label3_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Label3.MouseLeave
        PictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Label3.SendToBack()
    End Sub
End Class