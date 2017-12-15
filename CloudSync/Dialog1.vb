Imports System.Windows.Forms
Imports System.Xml
Imports CloudSync.Form1

Public Class Dialog1
    Dim FirstLoad As Boolean = True
    Dim CheckBox3_Changed_Null, UpdatingTab As Boolean
    'Dim SettingsDataPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\") + 1) + "SettingsData.xml"
    'Dim DropboxDirectory, DefaultDirectoryToSaveForFiles, DefaultDirectoryToSaveForFolders As String

    Public Function BrowseFolderForDirectory(Optional ByVal CloudService As Int16 = Nothing, Optional ByVal Message As String = "Select Directory", Optional ByVal CheckIfInCloud As Boolean = True) 'ONLY MADE OPTIONAL BECAUSE OF ADDRESSOF THE FIRST OPTION IS NOT REALLY OPTIONAL!
        Try
            If Me.TabControl1.InvokeRequired Then
                Me.TabControl1.Invoke(New MethodInvoker(AddressOf BrowseFolderForDirectory))
            Else
                With FolderBrowserDialog1
                    .RootFolder = System.Environment.SpecialFolder.Desktop
                    .SelectedPath = Settings(CloudService, CSDirectory) 'MAYBE NOT
                    .Description = Message
                    If .ShowDialog = DialogResult.OK Then
                        If CheckIfInCloud = True Then
                            '  dropbox2              dropbox
                            If .SelectedPath.IndexOf(Settings(CloudService, CSDirectory)) <> 0 Then 'CHECK IF REALLY DROPBOX
                                MsgBox("Alert 201: You must select a folder in your " + CloudServiceAsString(CloudService))
                                Return BrowseFolderForDirectory(CloudService, Message, CheckIfInCloud)
                                Exit Function
                            Else
                                If CheckIfCorrectFolder(.SelectedPath, Settings(CloudService, CSDirectory)) = False Then
                                    MsgBox("Alert 202: You must select a folder in your " + CloudServiceAsString(CloudService))
                                    Return BrowseFolderForDirectory(CloudService, Message, CheckIfInCloud)
                                    Exit Function
                                End If
                            End If
                        Else
                            Dim s = .SelectedPath
                            If .SelectedPath.IndexOf(Settings(CloudService, CSDirectory)) = 0 Then 'CHECK IF REALLY DROPBOX
                                If CheckIfCorrectFolder(.SelectedPath, Settings(CloudService, CSDirectory)) = False Then
                                    Return .SelectedPath
                                End If
                                MsgBox("Alert 203: You must select a folder outside your " + CloudServiceAsString(CloudService))
                                Return BrowseFolderForDirectory(CloudService, Message, CheckIfInCloud)
                                Exit Function
                            End If
                        End If
                        Return .SelectedPath
                    End If
                    '!if closed error is thrown!
                End With
            End If
        Catch ex As Exception
            MsgBox("Error 204: BrowseFolderForDirectory" + vbNewLine + ex.Message)
        End Try
    End Function

    Private Sub BrowseCloudFolder(ByVal CloudService As Int16)
        With FolderBrowserDialog1
            .RootFolder = Environment.SpecialFolder.Desktop
            .SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            .Description = "Select " + CloudServiceAsString(CloudService) + " Directory"
            If .ShowDialog = DialogResult.OK Then
                    Settings(CloudService, CSDirectory) = .SelectedPath
                    Settings(CloudService, CSDefaultDirectoryToSaveForFiles) = .SelectedPath
                    Settings(CloudService, CSDefaultDirectoryToSaveForFolders) = .SelectedPath
            End If
            '!if closed error is thrown!
        End With
    End Sub

    Private Sub UpdateTab(ByVal tab As Int16, Optional ByVal load As Boolean = False)
        Try
            UpdatingTab = True
            If load = True Then ReadFileSettingsAndSyncedFiles()

            If TabControl1.SelectedIndex = CloudServicesSyncedNumber Then
                Form2Browse = True
                Form2.ShowDialog()
                RefreshTabs(False)
                Return
            End If

            If CloudServicesSyncedNumber > 1 Then
                CheckBox3.Enabled = True
                If tab = DefaultCloudService Then
                    CheckBox3.Checked = True
                Else
                    CheckBox3.Checked = False
                End If
            Else
                CheckBox3.Enabled = False
                CheckBox3_Changed_Null = True
                CheckBox3.Checked = True
            End If
            If Settings(tab, CSFolderSyncOption) = 1 Then
                RadioButton1.Checked = True
            Else
                RadioButton1.Checked = False
            End If
            If Settings(tab, CSFolderSyncOption) = 2 Then
                RadioButton2.Checked = True
            Else
                RadioButton2.Checked = False
            End If
            CheckBox1.Checked = Settings(tab, CSPromptForDirectoryForFiles)
            CheckBox2.Checked = Settings(tab, CSPromptForDirectoryForFolders)
            TextBox1.Text = Settings(tab, CSDirectory)
            TextBox2.Text = Settings(tab, CSDefaultDirectoryToSaveForFiles)
            TextBox3.Text = Settings(tab, CSDefaultDirectoryToSaveForFolders)
            UpdatingTab = False
        Catch ex As Exception
            MsgBox("Error 205: UpdateTabs" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        '!!!!!!!!!!!!!!!!work()
        'Try
        If RadioButton1.Checked = True Then
            Settings(CloudServiceAsInt(TabControl1.SelectedIndex), CSFolderSyncOption) = 1
        Else
            Settings(TabControl1.SelectedIndex, CSFolderSyncOption) = 2
        End If
        If CheckBox1.Checked Then
            Settings(TabControl1.SelectedIndex, CSPromptForDirectoryForFiles) = True
        Else
            Settings(TabControl1.SelectedIndex, CSPromptForDirectoryForFiles) = False
        End If
        If CheckBox2.Checked Then
            Settings(TabControl1.SelectedIndex, CSPromptForDirectoryForFolders) = True
        Else
            Settings(TabControl1.SelectedIndex, CSPromptForDirectoryForFolders) = False
        End If

        WriteSettings()

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
        'Catch ex As Exception
        'MsgBox("Error 206: Ok.Click" + vbNewLine + ex.Message + vbNewLine + ex.InnerException.ToString)
        'End Try
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Try
            ReadFileSettingsAndSyncedFiles()
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        Catch ex As Exception
            MsgBox("Error 207: Cancel.Click" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub RadioButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButton2.Click
        Try
            If RadioButton1.Checked = True Then
                RadioButton1.Checked = False
                RadioButton2.Checked = True
            End If
            If RadioButton1.Checked = True Then
                Settings(TabControl1.SelectedIndex, CSFolderSyncOption) = 1
            Else
                Settings(TabControl1.SelectedIndex, CSFolderSyncOption) = 2
            End If
        Catch ex As Exception
            MsgBox("Error 208: Radio2.Click" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub RadioButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButton1.Click
        Try
            If RadioButton2.Checked = True Then
                RadioButton2.Checked = False
                RadioButton1.Checked = True
            End If
            If RadioButton1.Checked = True Then
                Settings(TabControl1.SelectedIndex, CSFolderSyncOption) = 1
            Else
                Settings(TabControl1.SelectedIndex, CSFolderSyncOption) = 2
            End If
        Catch ex As Exception
            MsgBox("Error 209: Radio2.Click" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub TextBox1_Clicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.Click
        BrowseCloudFolder(TabControl1.SelectedIndex)
        If TextBox1.Text <> Settings(TabControl1.SelectedIndex, CSDirectory) Then
            Settings(TabControl1.SelectedIndex, CSDefaultDirectoryToSaveForFiles) = Settings(TabControl1.SelectedIndex, CSDirectory)
            Settings(TabControl1.SelectedIndex, CSDefaultDirectoryToSaveForFolders) = Settings(TabControl1.SelectedIndex, CSDirectory)
            TextBox2.Text = Settings(TabControl1.SelectedIndex, CSDefaultDirectoryToSaveForFiles)
            TextBox3.Text = Settings(TabControl1.SelectedIndex, CSDefaultDirectoryToSaveForFolders)
        End If
        TextBox1.Text = Settings(TabControl1.SelectedIndex, CSDirectory)
    End Sub

    Private Sub TextBox2_Clicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.Click
        Dim tmp = BrowseFolderForDirectory(TabControl1.SelectedIndex, "Select Default Directory to Sync Files")
        If tmp <> "" Then Settings(TabControl1.SelectedIndex, CSDefaultDirectoryToSaveForFiles) = tmp
        TextBox2.Text = Settings(TabControl1.SelectedIndex, CSDefaultDirectoryToSaveForFiles)
    End Sub

    Private Sub TextBox3_TextClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.Click
        Dim tmp = BrowseFolderForDirectory(TabControl1.SelectedIndex, "Select Default Directory to Sync Folders")
        If tmp <> "" Then Settings(TabControl1.SelectedIndex, CSDefaultDirectoryToSaveForFolders) = tmp
        TextBox3.Text = Settings(TabControl1.SelectedIndex, CSDefaultDirectoryToSaveForFolders)
    End Sub

    Private Sub RefreshTabs(ByVal load As Boolean)
        Try
            'For i As Int16 = 0 To TabControl1.TabCount - 1
            '   TabControl1.TabPages.RemoveAt(i)
            'Next
            TabControl1.TabPages.Clear()
            TabControl1.Update()
            For i As Int16 = 0 To 2
                If Settings(i, CSDirectory) <> Nothing Then
                    TabControl1.TabPages.Add(CloudServiceAsString(i))
                End If
            Next
            TabControl1.TabPages.Add("    +")
            UpdateTab(CloudServicesSynced(0), load)
        Catch ex As Exception
            MsgBox("Error 210: RefreshTabs" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub Dialog1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RefreshTabs(True)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
            OK_Button_Click(sender, e)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
            Cancel_Button_Click(sender, e)
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        Try
            If TabControl1.SelectedIndex > -1 Then
                UpdateTab(TabControl1.SelectedIndex)
            End If
        Catch ex As Exception
            MsgBox("Error 213: TabChanged" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Settings(TabControl1.SelectedIndex, CSPromptForDirectoryForFiles) = True
        Else
            Settings(TabControl1.SelectedIndex, CSPromptForDirectoryForFiles) = False
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked Then
            Settings(TabControl1.SelectedIndex, CSPromptForDirectoryForFolders) = True
        Else
            Settings(TabControl1.SelectedIndex, CSPromptForDirectoryForFolders) = False
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox3.CheckedChanged
        'If CheckBox3_Changed_Null = False Then 'Checkbox3.Enabled = True
        'DefaultCloudService = CloudServiceAsInt(TabControl1.SelectedTab.Name)
        'Else
        'CheckBox3_Changed_Null = False
        'End If
        If UpdatingTab = False Then
            If CheckBox3.Checked = False Then
                DefaultCloudService = -1
            Else
                DefaultCloudService = CloudServiceAsInt(TabControl1.SelectedTab.Text)
            End If
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Settings(TabControl1.SelectedIndex, 0) = "1"
        Settings(TabControl1.SelectedIndex, 1) = ""
        Settings(TabControl1.SelectedIndex, 2) = ""
        Settings(TabControl1.SelectedIndex, 3) = ""
        Settings(TabControl1.SelectedIndex, 4) = "True"
        Settings(TabControl1.SelectedIndex, 5) = "False"
        CloudServicesSyncedNumber -= 1
        RefreshTabs(False)
    End Sub
End Class