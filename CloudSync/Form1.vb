Imports System.Security.Permissions
Imports System.IO
Imports System.Security.Principal
Imports System.ComponentModel
Imports System.Xml
Imports System.Runtime.InteropServices
Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

'Thanks for using CloudSync. You can show your appreciation and support for future development by donating!     

Public Class Form1
    Public Shared SyncedFiles(0, 4), FilesToBeSynced(0, 4), Settings(3, 5), SettingsDataPath, LogDataPath, LastUpdatedFile As String
    Dim FileWatchers() As IO.FileSystemWatcher
    Public Shared isRunningAsAdmin, Monitoring, TrulyClose, Startup, Form2Browse As Boolean
    Public Shared CloudServicesSynced(3), isAdminUser, RestartingAsAdmin, DefaultCloudService, CloudServicesSyncedNumber, SelectedCloudService As Int16
    Dim LastWrite As Date
    Dim CommandLineArgs As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs
    Public Event StartupNextInstance As ApplicationServices.StartupNextInstanceEventHandler

    Public Const SFCloudService As Int16 = 0
    Public Const SFStatus As Int16 = 1
    Public Const SFOriginalFilePath As Int16 = 2
    Public Const SFCloudFilePath As Int16 = 3
    Public Const SFFolderSyncOptionOption As Int16 = 4
    Public Const CSFolderSyncOption As Int16 = 0
    Public Const CSDirectory As Int16 = 1
    Public Const CSDefaultDirectoryToSaveForFiles As Int16 = 2
    Public Const CSDefaultDirectoryToSaveForFolders As Int16 = 3
    Public Const CSPromptForDirectoryForFiles As Int16 = 4
    Public Const CSPromptForDirectoryForFolders As Int16 = 5
    Public Const Dropbox As Int16 = 0
    Public Const SkyDrive As Int16 = 1
    Public Const GoogleDrive As Int16 = 2
    Public Const StatusSynced As String = "Synced"
    Public Const StatusFailed As String = "Error: Failed to sync"
    Public Const StatusFailedMovingDirectory As String = "Error: Failed moving directory"
    Public Const StatusToBeSynced As String = "To Be Synced"
    Public Const StatusCancelled As String = "Cancelled"
    Public Const NotAdminUser As Int16 = 2

    Dim LogStream As FileStream
    Dim LogWriter As StreamWriter

    '<DllImport("user32.dll", CallingConvention:=CallingConvention.Cdecl, SetLastError:=True, CharSet:=CharSet.Auto)> Private Shared Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
    'End Function

    '<DllImport("user32.dll", CallingConvention:=CallingConvention.Cdecl)> Private Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    'End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function FindWindow( _
     ByVal lpClassName As String, _
     ByVal lpWindowName As String) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Private Declare Function BringWindowToTop Lib "user32" (ByVal _
    hwnd As Integer) As Integer

    Private Declare Function SetFocus Lib "user32.dll" (ByVal hWnd As IntPtr) As Integer

    'Private Declare Function FindWindow Lib "user32" Alias _
    '   "FindWindowA" (ByVal lpClassName As Object, ByVal lpWindowName _
    '   As Object) As Integer

    Public Function FileName(ByRef FilePath As String) As String
        Dim s = FilePath.Substring(FilePath.LastIndexOf("\") + 1)
        If FilePath.Substring(FilePath.LastIndexOf("\") + 1) <> "" Then
            Return FilePath.Substring(FilePath.LastIndexOf("\") + 1)
        Else
            'second to last
            Dim q = FilePath.Substring(FilePath.Substring(0, FilePath.Length - 1).LastIndexOf("\") + 1)
            Return FilePath.Substring(FilePath.Substring(0, FilePath.Length - 1).LastIndexOf("\") + 1)
        End If
    End Function

    Public Function ToOneDimArray(ByVal Array As String(,), ByVal i As Integer)
        Dim TempArray(0, 4) As String
        For x As Integer = 0 To 4
            TempArray(0, x) = Array(i, x)
        Next
        Return TempArray
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
                WriteSettings()
            End If
            '!if closed error is thrown!
        End With
    End Sub

    Public Shared Function CloudServiceAsString(ByVal CloudService As Int16)
        If CloudService = Dropbox Then Return "Dropbox"
        If CloudService = SkyDrive Then Return "SkyDrive"
        If CloudService = GoogleDrive Then Return "Google Drive"
    End Function

    Public Shared Function CloudServiceAsInt(ByVal CloudService As String)
        If CloudService = "Dropbox" Then Return Dropbox
        If CloudService = "SkyDrive" Then Return SkyDrive
        If CloudService = "GoogleDrive" Or CloudService = "Google Drive" Then Return GoogleDrive
    End Function

    Dim argsCloudService As Int16 = -1 'USING BECAUSE WHEN INVOKING CLOUDSERVICE DOES NOT GET PASSED
    Dim SelectedLocation As String
    Public Function BrowseFolderForDirectory(Optional ByVal CloudService As Int16 = 0, Optional ByVal Message As String = "Select Directory", Optional ByVal CheckIfInCloud As Boolean = True) 'ONLY MADE OPTIONAL BECAUSE OF ADDRESSOF THE FIRST OPTION IS NOT REALLY OPTIONAL!
        Try
            If Me.ListView1.InvokeRequired Then
                'Dim parameters As Object() = New Object() {CObj("Hello World")}
                argsCloudService = CloudService
                Me.ListView1.Invoke(New MethodInvoker(AddressOf BrowseFolderForDirectory))
            Else
                If argsCloudService <> -1 Then
                    CloudService = argsCloudService
                    argsCloudService = -1
                End If

                With FolderBrowserDialog1
                    .RootFolder = System.Environment.SpecialFolder.Desktop
                    .SelectedPath = Settings(CloudService, CSDirectory) 'MAYBE NOT
                    .Description = Message
                    If .ShowDialog = DialogResult.OK Then
                        If CheckIfInCloud = True Then
                            '  dropbox2              dropbox
                            If .SelectedPath.IndexOf(Settings(CloudService, CSDirectory)) <> 0 Then 'CHECK IF REALLY DROPBOX
                                MsgBox("You must select a folder in your " + CloudServiceAsString(CloudService))
                                Return BrowseFolderForDirectory(CloudService, Message, CheckIfInCloud)
                                Exit Function
                            Else
                                If CheckIfCorrectFolder(.SelectedPath, Settings(CloudService, CSDirectory)) = False Then
                                    MsgBox("You must select a folder in your " + CloudServiceAsString(CloudService))
                                    Return BrowseFolderForDirectory(CloudService, Message, CheckIfInCloud)
                                    Exit Function
                                End If
                            End If
                        Else
                            SelectedLocation = .SelectedPath
                            If .SelectedPath.IndexOf(Settings(CloudService, CSDirectory)) = 0 Then 'CHECK IF REALLY DROPBOX
                                If CheckIfCorrectFolder(.SelectedPath, Settings(CloudService, CSDirectory)) = False Then
                                    Return .SelectedPath
                                End If
                                MsgBox("You must select a folder outside your " + CloudServiceAsString(CloudService))
                                Return BrowseFolderForDirectory(CloudService, Message, CheckIfInCloud)
                                Exit Function
                            End If
                        End If
                        SelectedLocation = .SelectedPath
                        Return .SelectedPath
                    End If
                    '!if closed error is thrown!
                End With
            End If
        Catch ex As Exception
            MsgBox("Error 101: BrowseFolderForDirectory" + vbNewLine + ex.Message)
        End Try
    End Function

    Public Function AddFilesToCloud(ByVal FilesArray As String(,)) '!MAKE SURE ALL CALLS TO ADDFILESTODROPBOX DEFINE SDCloudService!!!!!
        Dim isDirectory As Boolean
        Dim CloudService As Int16
        If CloudServicesSyncedNumber > 1 And DefaultCloudService = -1 Then
            Form2Browse = False
            Form2.ShowDialog()
            CloudService = SelectedCloudService
        End If
        If CloudServicesSyncedNumber = 1 Then
            CloudService = CloudServicesSynced(0)
        End If
        If DefaultCloudService > -1 Then
            CloudService = DefaultCloudService
            'CloudService = CloudServicesSynced(0) why was it this?.......
        End If
        For i As Integer = 0 To FilesToBeSynced.Length / 5 - 1
            Dim SyncingFromDropbox As Boolean
            FilesToBeSynced(i, SFCloudService) = CloudService

            If My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFOriginalFilePath)) Then isDirectory = True

            'check if syncing from inside cloud folder, because then you can  sync that cloud folder to somewhere on your HD! :D
            If FilesToBeSynced(i, SFOriginalFilePath).IndexOf(Settings(CloudService, CSDirectory)) = 0 Then '!CHECK IF REALLY DIRECTORY!
                If CheckIfCorrectFolder(FilesToBeSynced(i, SFOriginalFilePath), Settings(CloudService, CSDirectory)) = False Then GoTo NotDropboxFolder
                SyncingFromDropbox = True
                FilesToBeSynced(i, SFCloudFilePath) = FilesToBeSynced(i, SFOriginalFilePath)
                BrowseFolderForDirectory(CloudService, "Select directory where you would like to sync to", False)
                If SelectedLocation = Nothing Then
                    FilesToBeSynced(i, SFStatus) = StatusCancelled
                    GoTo GotoNext
                End If
                FilesToBeSynced(i, SFOriginalFilePath) = SelectedLocation + "\" + FileName(FilesToBeSynced(i, SFCloudFilePath))
                If isDirectory = True Then
                    If My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFOriginalFilePath)) = True Then '!IF EXISTS DONT CREATE A NEW FILE IN SETTINGS!!!
                        If MsgBoxResult.Yes = MsgBox(FilesToBeSynced(i, SFOriginalFilePath) + " " + "already exists." + vbNewLine + "Would you like to overwrite it?", MsgBoxStyle.YesNoCancel) Then 'ask to merge or overwrite or what
                            My.Computer.FileSystem.DeleteDirectory(FilesToBeSynced(i, SFOriginalFilePath), FileIO.DeleteDirectoryOption.DeleteAllContents)
                        Else
                            FilesToBeSynced(i, SFStatus) = StatusCancelled
                            GoTo GotoNext
                        End If
                    End If
                    Shell("cmd.exe /c mklink /d """ + FilesToBeSynced(i, SFOriginalFilePath) + """" + " " + """" + FilesToBeSynced(i, SFCloudFilePath) + """", Microsoft.VisualBasic.AppWinStyle.Hide, True, 60000 * 20)
                    Threading.Thread.Sleep(2000)
                    If My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFCloudFilePath)) = True And My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFOriginalFilePath)) Then
                        FilesToBeSynced(i, SFStatus) = StatusSynced
                        MsgBox("Synced Successfully.") 'change status and update
                    Else
                        FilesToBeSynced(i, SFStatus) = StatusFailed
                        MsgBox("Error 142: Failed Syncing.") 'change status and update 'ADD TO LOG
                    End If
                Else
                    Shell("cmd.exe /c mklink /h """ + FilesToBeSynced(i, SFOriginalFilePath) + """" + " " + """" + FilesToBeSynced(i, SFCloudFilePath) + """", Microsoft.VisualBasic.AppWinStyle.Hide, True, 60000 * 20)
                    If My.Computer.FileSystem.FileExists(FilesToBeSynced(i, SFCloudFilePath)) = False Then
                        FilesToBeSynced(i, SFStatus) = StatusFailed
                        MsgBox("Error 143: Failed Syncing.") 'change status and update 'ADD TO LOG
                    Else
                        FilesToBeSynced(i, SFStatus) = StatusSynced
                        MsgBox("Synced Successfully.") 'change status and update
                    End If
                End If
                GoTo GotoNext
                'Exit For
            End If

NotDropboxFolder:

            If isDirectory = True Then
                If SyncingFromDropbox = False Then
                    If Settings(CloudService, CSPromptForDirectoryForFolders) = True Then
                        BrowseFolderForDirectory(CloudService) '!!! added cloudservice here... idk why it was here before...
                        If SelectedLocation = Nothing Then
                            FilesToBeSynced(i, SFStatus) = StatusCancelled
                            GoTo GotoNext
                        End If
                        FilesToBeSynced(i, SFCloudFilePath) = SelectedLocation + "\" + FileName(FilesToBeSynced(i, SFOriginalFilePath))
                    Else
                        FilesToBeSynced(i, SFCloudFilePath) = Settings(CloudService, CSDirectory) + "\" + FileName(FilesToBeSynced(i, SFOriginalFilePath))
                    End If
                End If
                If Settings(CloudService, CSFolderSyncOption) = "1" Then 'moves and delete files and creates link: 
                    If My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFCloudFilePath)) = True Then
                        If MsgBoxResult.Yes = MsgBox(Settings(CloudService, CSDirectory) + "\" + FileName(FilesToBeSynced(i, SFOriginalFilePath)) + " " + "already exists." + vbNewLine + "Would you like to overwrite it?", MsgBoxStyle.YesNoCancel) Then 'ask to merge or overwrite or what
                            My.Computer.FileSystem.DeleteDirectory(FilesToBeSynced(i, SFCloudFilePath), FileIO.DeleteDirectoryOption.DeleteAllContents)
                        Else
                            FilesToBeSynced(i, SFStatus) = StatusCancelled
                            GoTo GotoNext
                        End If
                    End If
                    FilesToBeSynced(i, SFFolderSyncOptionOption) = "1"
                    '!!!DONT SHOW UI, ONLY SHOWING FOR DEBUG PURPOSES
                    My.Computer.FileSystem.MoveDirectory(FilesToBeSynced(i, SFOriginalFilePath), FilesToBeSynced(i, SFCloudFilePath), FileIO.UIOption.AllDialogs)
                    If My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFCloudFilePath)) Then
                        'LogWriter.Write(vbNewLine + "cmd.exe /c mklink /d """ + FilesToBeSynced(i, SFOriginalFilePath) + """" + " " + """" + FilesToBeSynced(i, SFCloudFilePath) + """")
                        Shell("cmd.exe /c mklink /d """ + FilesToBeSynced(i, SFOriginalFilePath) + """" + " " + """" + FilesToBeSynced(i, SFCloudFilePath) + """", Microsoft.VisualBasic.AppWinStyle.Hide, True, 60000 * 20)
                    Else
                        FilesToBeSynced(i, SFStatus) = StatusFailedMovingDirectory
                        MsgBox("Error 144: Failed moving directory.") 'change status and update
                        GoTo GotoNext
                    End If
                Else
                    If My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFCloudFilePath)) = True Then
                        If MsgBoxResult.Yes = MsgBox(FilesToBeSynced(i, SFCloudFilePath) + " " + "already exists." + vbNewLine + "Would you like to overwrite it?", MsgBoxStyle.YesNoCancel) Then 'ask to merge or overwrite or what
                            My.Computer.FileSystem.DeleteDirectory(FilesToBeSynced(i, SFCloudFilePath), FileIO.DeleteDirectoryOption.DeleteAllContents)
                        Else
                            FilesToBeSynced(i, SFStatus) = StatusCancelled
                            GoTo GotoNext
                        End If
                    End If
                    FilesToBeSynced(i, SFFolderSyncOptionOption) = "2"
                    'IF EXISTS THROWS ERROR:
                    My.Computer.FileSystem.CopyDirectory(FilesToBeSynced(i, SFOriginalFilePath), FilesToBeSynced(i, SFCloudFilePath))
                    AddFilesToMonitor(FilesToBeSynced) '!!!!what if more than one file?!!!!'
                End If
                If My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFCloudFilePath)) = True And My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFOriginalFilePath)) Then
                    FilesToBeSynced(i, SFStatus) = StatusSynced
                    MsgBox("Synced Successfully.") 'change status and update
                Else
                    Threading.Thread.Sleep(1000)
                    If My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFCloudFilePath)) = True And My.Computer.FileSystem.DirectoryExists(FilesToBeSynced(i, SFOriginalFilePath)) = False Then
                        If isDirectory = True Then
                            '!!!undo!
                        End If
                        FilesToBeSynced(i, SFStatus) = StatusFailed
                        MsgBox("Error 145: Failed Syncing.") 'change status and update
                    End If
                End If
            Else 'FILES
                MsgBox("Syncing files may not always be very reliable such as syncing Word files. In such cases it is recommended to sync directories.")
                If My.Computer.FileSystem.FileExists(FilesToBeSynced(i, SFOriginalFilePath)) Then
                    If Settings(CloudService, CSPromptForDirectoryForFiles) = True Then
                        BrowseFolderForDirectory(CloudService)
                        If SelectedLocation = Nothing Then
                            FilesToBeSynced(i, SFStatus) = StatusCancelled
                            GoTo GotoNext
                        End If
                        FilesToBeSynced(i, SFCloudFilePath) = SelectedLocation + "\" + FileName(FilesToBeSynced(i, SFOriginalFilePath))
                    Else
                        FilesToBeSynced(i, SFCloudFilePath) = Settings(CloudService, CSDirectory) + "\" + FileName(FilesToBeSynced(i, SFOriginalFilePath))
                    End If
                    If My.Computer.FileSystem.FileExists(FilesToBeSynced(i, SFCloudFilePath)) Then
                        If MsgBoxResult.Yes = MsgBox(FilesToBeSynced(i, SFCloudFilePath) + " " + "already exists." + vbNewLine + "Would you like to overwrite it?", MsgBoxStyle.YesNoCancel) Then 'ask to merge or overwrite or what
                            My.Computer.FileSystem.DeleteFile(FilesToBeSynced(i, SFCloudFilePath))
                        Else
                            FilesToBeSynced(i, SFStatus) = StatusCancelled
                            GoTo GotoNext
                        End If
                    End If
                    FilesToBeSynced(i, SFFolderSyncOptionOption) = "0"
                    Shell("cmd.exe /c mklink /h """ + FilesToBeSynced(i, SFCloudFilePath) + """" + " " + """" + FilesToBeSynced(i, SFOriginalFilePath) + """", Microsoft.VisualBasic.AppWinStyle.Hide, True, 5000)
                    'moves and delete files: Shell("cmd.exe /c mklink """ + FilesToBeSynced(i) + """" + " " + """" + DropboxDirectory + "\" + FileName(FilesToBeSynced(i)) + """")
                    Threading.Thread.Sleep(2000)
                    If My.Computer.FileSystem.FileExists(FilesToBeSynced(i, SFCloudFilePath)) = False Then
                        FilesToBeSynced(i, SFStatus) = StatusFailed
                        MsgBox("Error 146: Failed Syncing.") 'change status and update
                    Else
                        FilesToBeSynced(i, SFStatus) = StatusSynced
                        MsgBox("Synced Successfully.") 'change status and update
                    End If
                Else
                    FilesToBeSynced(i, SFStatus) = StatusFailed
                End If
            End If
GotoNext:
        Next

        WriteSyncedFilesInSettings()

        'PopulateListView()
        'IF EXISTS DONT WRITE!
        Return FilesArray
    End Function
    'mklink "C:\Users\VirusZer0\Desktop\Code.txt" "C:\Users\VirusZer0\Dropbox\Code.txt"
    'cmd.exe /c mklink /h "C:\Users\VirusZer0\Dropbox\TestDirectory.rar" "C:\Users\VirusZer0\Desktop\TestDirectory.rar"

    Public Sub Unsync(Optional ByVal FileIndex As Int16 = Nothing, Optional ByVal ShowMessage As Boolean = True) '!CHANGE TO -1!!!!!!
        Try
            Dim CountEnd As Int16
            If FileIndex = Nothing Then
                CountEnd = ListView1.SelectedIndices.Count - 1
            Else
                CountEnd = 0
            End If
            For i = 0 To CountEnd
                Dim index As Int16
                If FileIndex = Nothing Then
                    index = ListView1.SelectedIndices(i) 'IF YOU DO USE FOR ELSEWHERE, REPLACE THAT WITH INDEX ALL OVER THIS FUNCTION!
                Else
                    index = FileIndex
                End If

                Dim xmldoc As New XmlDataDocument
                xmldoc.Load(SettingsDataPath)
                Dim xmlnode_SyncedFiles As XmlNodeList = xmldoc.GetElementsByTagName("SyncedFiles")
                Dim root As XmlNodeList = xmldoc.GetElementsByTagName("root")
                If SyncedFiles(index, SFFolderSyncOptionOption) = "0" Then
                    Try
                        If My.Computer.FileSystem.FileExists(SyncedFiles(index, SFCloudFilePath)) Then
                            My.Computer.FileSystem.DeleteFile(SyncedFiles(index, SFCloudFilePath))
                        End If
                        If ShowMessage = True Then MsgBox("Succesfully unsynced """ + SyncedFiles(index, SFOriginalFilePath) + """.")
                    Catch ex As Exception
                        MsgBox("Error 103: Failed to unsync """ + SyncedFiles(index, SFOriginalFilePath) + """" + ". A file in either the directory or the cloud  is in use by another application. Close the file and try again.")
                        GoTo LabelNextLoop1
                    End Try
                ElseIf SyncedFiles(index, SFFolderSyncOptionOption) = "1" Then
                    Try
                        If My.Computer.FileSystem.DirectoryExists(SyncedFiles(index, SFOriginalFilePath)) And My.Computer.FileSystem.FileExists(SyncedFiles(index, SFCloudFilePath)) Then
                            My.Computer.FileSystem.DeleteDirectory(SyncedFiles(index, SFOriginalFilePath), Microsoft.VisualBasic.FileIO.DeleteDirectoryOption.DeleteAllContents)
                        End If
                        If My.Computer.FileSystem.DirectoryExists(SyncedFiles(index, SFCloudFilePath)) Then
                            My.Computer.FileSystem.MoveDirectory(SyncedFiles(index, SFCloudFilePath), SyncedFiles(index, SFOriginalFilePath))
                        Else
                            If My.Computer.FileSystem.DirectoryExists(SyncedFiles(index, SFOriginalFilePath)) = False Then
                                MsgBox("Error 102: Failed to unsync """ + SyncedFiles(index, SFOriginalFilePath) + """" + ". The directory does not appear to exist. If this was unintentional you can restore using the Cloud's restore feature.")
                            End If
                        End If
                        If ShowMessage = True Then MsgBox("Succesfully unsynced """ + SyncedFiles(index, SFOriginalFilePath) + """.")
                    Catch ex As Exception
                        MsgBox("Error 103B: Failed to unsync """ + SyncedFiles(index, SFOriginalFilePath) + """" + ". A file in either the directory or the cloud  is in use by another application. Close the file and try again.")
                        GoTo LabelNextLoop1
                    End Try
                ElseIf SyncedFiles(index, SFFolderSyncOptionOption) = "2" Then
                    'stop watching, refresh
                    Dim Path As String = SyncedFiles(index, SFOriginalFilePath)
                    For i2 = 1 To FileWatchers.Length - 1
                        Dim s = FileWatchers
                        If Path.IndexOf(FileWatchers(i2).Path) = 0 Then
                            If CheckIfCorrectFolder(Path, FileWatchers(i2).Path) = False Then
                                GoTo LabelNext
                            End If
                            FileWatchers(i2).EnableRaisingEvents = False
                            FileWatchers(i2 + 1).EnableRaisingEvents = False
                            Exit For
                        End If
LabelNext:
                    Next
                    Try
                        If My.Computer.FileSystem.DirectoryExists(SyncedFiles(index, SFCloudFilePath)) Then
                            My.Computer.FileSystem.DeleteDirectory(SyncedFiles(index, SFCloudFilePath), Microsoft.VisualBasic.FileIO.DeleteDirectoryOption.DeleteAllContents)
                        End If
                        If ShowMessage = True Then MsgBox("Succesfully unsynced """ + SyncedFiles(index, SFOriginalFilePath) + """.")
                    Catch ex As Exception
                        MsgBox("Error 104: Failed to unsync """ + SyncedFiles(index, SFOriginalFilePath) + """" + ". A file in either the directory or the cloud  is in use by another application. Close the file and try again.")
                    End Try
                End If
                root(0).RemoveChild(xmlnode_SyncedFiles(index))
                xmldoc.Save(SettingsDataPath)
LabelNextLoop1:
            Next

            'uncessary to call because SettingsFileUpdated calls PopulateListView()
        Catch ex As Exception
            MsgBox("Error 105: Unsync" + vbNewLine + ex.Message)
        End Try
    End Sub

    Public Shared Sub ReadFileSettingsAndSyncedFiles()
        Try
            Dim retryCount As Integer
            Dim xmldoc As New XmlDataDocument
            If My.Computer.FileSystem.FileExists(SettingsDataPath) = False Then
                Dim xmldoc1 As XDocument =
                    <?xml version="1.0" encoding="utf-8"?>
                    <root>
                        <Build BuildNumber="1000"/>
                        <Settings>
                            <Dropbox>
                                <FolderSyncOption>1</FolderSyncOption>
                                <Directory></Directory>
                                <DefaultDirectoryToSaveForFiles></DefaultDirectoryToSaveForFiles>
                                <DefaultDirectoryToSaveForFolders></DefaultDirectoryToSaveForFolders>
                                <PromptForDirectoryForFiles>True</PromptForDirectoryForFiles>
                                <PromptForDirectoryForFolders>False</PromptForDirectoryForFolders>
                            </Dropbox>
                            <SkyDrive>
                                <FolderSyncOption>1</FolderSyncOption>
                                <Directory></Directory>
                                <DefaultDirectoryToSaveForFiles></DefaultDirectoryToSaveForFiles>
                                <DefaultDirectoryToSaveForFolders></DefaultDirectoryToSaveForFolders>
                                <PromptForDirectoryForFiles>True</PromptForDirectoryForFiles>
                                <PromptForDirectoryForFolders>False</PromptForDirectoryForFolders>
                            </SkyDrive>
                            <GoogleDrive>
                                <FolderSyncOption>1</FolderSyncOption>
                                <Directory></Directory>
                                <DefaultDirectoryToSaveForFiles></DefaultDirectoryToSaveForFiles>
                                <DefaultDirectoryToSaveForFolders></DefaultDirectoryToSaveForFolders>
                                <PromptForDirectoryForFiles>True</PromptForDirectoryForFiles>
                                <PromptForDirectoryForFolders>False</PromptForDirectoryForFolders>
                            </GoogleDrive>
                            <Other>
                                <FolderSyncOption>1</FolderSyncOption>
                                <Directory></Directory>
                                <DefaultDirectoryToSaveForFiles></DefaultDirectoryToSaveForFiles>
                                <DefaultDirectoryToSaveForFolders></DefaultDirectoryToSaveForFolders>
                                <PromptForDirectoryForFiles>True</PromptForDirectoryForFiles>
                                <PromptForDirectoryForFolders>False</PromptForDirectoryForFolders>
                            </Other>
                            <Global>
                                <DefaultCloudService>-1</DefaultCloudService>
                            </Global>
                        </Settings>
                    </root>
                xmldoc1.Save(SettingsDataPath)
            End If
            'If My.Computer.FileSystem.fil Then
retryLoad:
            Try
                xmldoc.Load(SettingsDataPath)
            Catch ex As Exception
                retryCount += 1
                If retryCount = 10 Then
                    MsgBox("Error 106: Couldn't read settings file." + vbNewLine + ex.Message)
                    Return
                Else
                    GoTo retryLoad
                End If
            End Try

            Dim xmlnode_Settings As XmlNodeList = xmldoc.GetElementsByTagName("Settings")
            CloudServicesSyncedNumber = 0
            For i = 0 To 3
                Settings(i, CSFolderSyncOption) = xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(0).InnerText.Trim()
                Settings(i, CSDirectory) = xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(1).InnerText.Trim()
                Settings(i, CSDefaultDirectoryToSaveForFiles) = xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(2).InnerText.Trim()
                Settings(i, CSDefaultDirectoryToSaveForFolders) = xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(3).InnerText.Trim()
                Settings(i, CSPromptForDirectoryForFiles) = Boolean.Parse(xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(4).InnerText.Trim())
                Settings(i, CSPromptForDirectoryForFolders) = Boolean.Parse(xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(5).InnerText.Trim())
                If Settings(i, CSDirectory) <> Nothing Then
                    CloudServicesSyncedNumber += 1
                    CloudServicesSynced(CloudServicesSyncedNumber - 1) = i
                End If
            Next
            DefaultCloudService = Int16.Parse(xmlnode_Settings(0).ChildNodes.Item(4).ChildNodes.Item(0).InnerText.Trim())

            Dim xmlnode_SyncedFiles As XmlNodeList = xmldoc.GetElementsByTagName("SyncedFiles")
            ReDim SyncedFiles(xmlnode_SyncedFiles.Count - 1, 4)
            For i = 0 To xmlnode_SyncedFiles.Count - 1
                SyncedFiles(i, SFCloudService) = xmlnode_SyncedFiles(i).ChildNodes.Item(0).InnerText.Trim()
                SyncedFiles(i, SFStatus) = xmlnode_SyncedFiles(i).ChildNodes.Item(1).InnerText.Trim()
                SyncedFiles(i, SFOriginalFilePath) = xmlnode_SyncedFiles(i).ChildNodes.Item(2).InnerText.Trim()
                SyncedFiles(i, SFCloudFilePath) = xmlnode_SyncedFiles(i).ChildNodes.Item(3).InnerText.Trim()
                SyncedFiles(i, SFFolderSyncOptionOption) = xmlnode_SyncedFiles(i).ChildNodes.Item(4).InnerText.Trim()
            Next
            ToggleCloudButtons()
        Catch ex As Exception
            MsgBox("Error 107: ReadFileSettingsAndSyncedFiles" + vbNewLine + ex.Message)
        End Try
    End Sub 'AFTER THIS IT CRASHES!

    Public Shared Sub WriteSettings()
        Try
            Dim xmldoc As New XmlDataDocument
            xmldoc.Load(SettingsDataPath)
            Dim xmlnode_Settings As XmlNodeList = xmldoc.GetElementsByTagName("Settings")

            For i As Integer = 0 To 3
                xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(0).InnerText = Settings(i, CSFolderSyncOption)
                xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(1).InnerText = Settings(i, CSDirectory)
                xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(2).InnerText = Settings(i, CSDefaultDirectoryToSaveForFiles)
                xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(3).InnerText = Settings(i, CSDefaultDirectoryToSaveForFolders)
                xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(4).InnerText = Settings(i, CSPromptForDirectoryForFiles)
                xmlnode_Settings(0).ChildNodes.Item(i).ChildNodes.Item(5).InnerText = Settings(i, CSPromptForDirectoryForFolders)
            Next
            xmlnode_Settings(0).ChildNodes.Item(4).ChildNodes.Item(0).InnerText = DefaultCloudService.ToString

            xmldoc.Save(SettingsDataPath)
        Catch ex As Exception
            MsgBox("Error 140: WriteSettings" + vbNewLine + ex.Message)
        End Try
    End Sub

    Public Sub WriteSyncedFilesInSettings()
        Try
            Dim xmldoc As New XmlDataDocument
            xmldoc.Load(SettingsDataPath)

            For i = 0 To FilesToBeSynced.Length / 5 - 1
                Dim xmlElement_SyncedFiles As XmlElement = xmldoc.CreateElement("SyncedFiles")
                If FilesToBeSynced(i, SFStatus) <> StatusCancelled Then
                    Dim xmlElement_CloudService As XmlElement = xmldoc.CreateElement("CloudService")
                    xmlElement_CloudService.InnerText = FilesToBeSynced(i, SFCloudService)
                    xmlElement_SyncedFiles.AppendChild(xmlElement_CloudService)

                    Dim xmlElement_Status As XmlElement = xmldoc.CreateElement("Status")
                    xmlElement_Status.InnerText = FilesToBeSynced(i, SFStatus)
                    xmlElement_SyncedFiles.AppendChild(xmlElement_Status)

                    Dim xmlElement_OriginalFilePath As XmlElement = xmldoc.CreateElement("OriginalFilePath")
                    xmlElement_OriginalFilePath.InnerText = FilesToBeSynced(i, SFOriginalFilePath)
                    xmlElement_SyncedFiles.AppendChild(xmlElement_OriginalFilePath)

                    Dim xmlElement_DropboxFilePath As XmlElement = xmldoc.CreateElement("CloudFilePath")
                    xmlElement_DropboxFilePath.InnerText = FilesToBeSynced(i, SFCloudFilePath)
                    xmlElement_SyncedFiles.AppendChild(xmlElement_DropboxFilePath)

                    Dim xmlElement_FolderSyncOption As XmlElement = xmldoc.CreateElement("FolderSyncOption")
                    xmlElement_FolderSyncOption.InnerText = FilesToBeSynced(i, SFFolderSyncOptionOption)
                    xmlElement_SyncedFiles.AppendChild(xmlElement_FolderSyncOption)

                    xmldoc.DocumentElement.AppendChild(xmlElement_SyncedFiles)
                End If
            Next
            ReDim FilesToBeSynced(0, 4)

            xmldoc.Save(SettingsDataPath)
            'unnecesarry to call because change of file will call. ReadFileSettingsAndSyncedFiles() 'appends to synced files array too
        Catch ex As Exception
            MsgBox("Error 108: WriteSyncedFilesInSettings" + vbNewLine + ex.Message)
        End Try
    End Sub

    Public Sub EditSyncedFilesInSettings(ByVal i As Int16)
        Dim xmldoc As New XmlDataDocument
        Try
            xmldoc.Load(SettingsDataPath)
        Catch ex As Exception
            MsgBox("Error 109: Couldn't read settings file." + vbNewLine + ex.Message)
        End Try

        Dim xmlnode_SyncedFiles As XmlNodeList = xmldoc.GetElementsByTagName("SyncedFiles")
        If SyncedFiles(i, SFOriginalFilePath) = xmlnode_SyncedFiles(i).ChildNodes.Item(2).InnerText.Trim() And SyncedFiles(i, SFCloudFilePath) = xmlnode_SyncedFiles(i).ChildNodes.Item(3).InnerText.Trim() Then
            xmlnode_SyncedFiles(i).ChildNodes.Item(SFStatus).InnerText = SyncedFiles(i, SFStatus)
            xmlnode_SyncedFiles(i).ChildNodes.Item(SFFolderSyncOptionOption).InnerText = SyncedFiles(i, SFFolderSyncOptionOption)
        Else
            MsgBox("Error 110")
        End If
        'SyncedFiles(i, SFCloudService) = xmlnode_SyncedFiles(i).ChildNodes.Item(0).InnerText.Trim()
        'SyncedFiles(i, SFStatus) = xmlnode_SyncedFiles(i).ChildNodes.Item(1).InnerText.Trim()
        'SyncedFiles(i, SFOriginalFilePath) = xmlnode_SyncedFiles(i).ChildNodes.Item(2).InnerText.Trim()
        'SyncedFiles(i, SFDropboxFilePath) = xmlnode_SyncedFiles(i).ChildNodes.Item(3).InnerText.Trim()
        'SyncedFiles(i, SFFolderSyncOptionOption) = xmlnode_SyncedFiles(i).ChildNodes.Item(4).InnerText.Trim()
        xmldoc.Save(SettingsDataPath)
    End Sub

    Public Sub PopulateListView()
        If Me.ListView1.InvokeRequired Then
            Me.ListView1.Invoke(New MethodInvoker(AddressOf PopulateListView))
        Else
            Try
                ListView1.View = System.Windows.Forms.View.Details
                ListView1.Items.Clear()
                For i As Integer = 0 To SyncedFiles.Length / 5 - 1
                    Dim Row(3) As String
                    Row(1) = FileName(SyncedFiles(i, SFOriginalFilePath))
                    Row(2) = SyncedFiles(i, SFOriginalFilePath)
                    Row(3) = SyncedFiles(i, SFCloudFilePath)
                    Dim Item As ListViewItem = New ListViewItem(Row)
                    If SyncedFiles(i, SFStatus) = StatusSynced Then '!!!IF SFCLOUDSERVICE IS NOTHING IN THE XML, IT WILL THROW AN ERROR HERE!
                        Item.ImageIndex = SyncedFiles(i, SFCloudService) * 3 + 2 '2, 5, 8 ;; c3 + 2
                    ElseIf SyncedFiles(i, SFStatus) = StatusToBeSynced Then
                        Item.ImageIndex = Integer.Parse(SyncedFiles(i, SFCloudService)) * 3 + 4 '4, 7, 10;; c3 + 4
                    Else
                        Item.ImageIndex = Integer.Parse(SyncedFiles(i, SFCloudService)) * 3 + 3 '3, 6, 9 ;; c3 + 3
                    End If
                    ListView1.Items.Add(Item)
                Next
            Catch ex As Exception
                'MsgBox("Error 111: PopulateListView" + vbNewLine + ex.Message)
            End Try
        End If
    End Sub

    Private Sub AddFilesToMonitor(Optional ByVal FilesArray As String(,) = Nothing, Optional ByVal File As String = Nothing, Optional ByVal y As Integer = 0)
        Try
            Dim OriginalLength = 0
            If File <> Nothing Then 'if file is something
                ReDim FilesArray(0, 4)
                FilesArray(0, SFOriginalFilePath) = File.Substring(0, File.LastIndexOf("\"))
            End If
            If FileWatchers Is Nothing Then
                ReDim FileWatchers(0)
            Else
                OriginalLength = FileWatchers.Length
                Dim q = FileWatchers.Length - 1 + 2 * FilesArray.Length / 5
                ReDim Preserve FileWatchers(FileWatchers.Length - 1 + 2 * FilesArray.Length / 5)
            End If
            For i As Integer = OriginalLength To FilesArray.Length / 5 - 1 + OriginalLength Step 2
                Dim x As Int16
                'check if exists
                If My.Computer.FileSystem.DirectoryExists(FilesArray(x, SFOriginalFilePath)) = False Then
                    SyncedFiles(y, SFStatus) = StatusFailed
                    EditSyncedFilesInSettings(y)
                Else
                    FileWatchers(i) = New FileSystemWatcher
                    Try
                        FileWatchers(i).Path = FilesArray(x, SFOriginalFilePath)
                    Catch ex As Exception
                        SyncedFiles(y, SFStatus) = StatusFailed
                        MsgBox("Error 112: Folders using RealSync no longer exist. They will be unsynced.")
                        'Unsync(x)
                    End Try
                    If File = Nothing Then FileWatchers(i).IncludeSubdirectories = True
                    FileWatchers(i).EnableRaisingEvents = True
                    If File <> Nothing Then
                        FileWatchers(i).NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.FileName
                    End If
                    AddHandler FileWatchers(i).Changed, AddressOf ChangeOfFile
                    AddHandler FileWatchers(i).Created, AddressOf ChangeOfFile
                    AddHandler FileWatchers(i).Deleted, AddressOf ChangeOfFile
                    AddHandler FileWatchers(i).Renamed, AddressOf RenameOfFile
                End If
                If File = Nothing Then
                    Monitoring = True
                    'check if App is in startup, if not add
                    If My.Computer.FileSystem.FileExists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup) + "\" + My.Application.Info.AssemblyName + ".lnk") = False Then
                        MsgBox("CloudSync will now start on system start up to make sure files are synced properly using RealSync.")
                        CreateShortCut(Application.ExecutablePath, System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup), My.Application.Info.AssemblyName, Application.ExecutablePath, 7)
                    End If

                    If My.Computer.FileSystem.DirectoryExists(FilesArray(x, SFCloudFilePath)) = False Then
                        SyncedFiles(y, SFStatus) = StatusFailed
                        EditSyncedFilesInSettings(y)
                    Else
                        FileWatchers(i + 1) = New FileSystemWatcher
                        Try
                            FileWatchers(i + 1).Path = FilesArray(x, SFCloudFilePath)
                        Catch ex As Exception
                            SyncedFiles(y, SFStatus) = StatusFailed
                            MsgBox("Error 113: Folders using RealSync no longer exist. They will be unsynced.")
                            'Unsync(x)
                        End Try
                        FileWatchers(i + 1).IncludeSubdirectories = True
                        FileWatchers(i + 1).EnableRaisingEvents = True
                        AddHandler FileWatchers(i + 1).Changed, AddressOf ChangeOfFile
                        AddHandler FileWatchers(i + 1).Created, AddressOf ChangeOfFile
                        AddHandler FileWatchers(i + 1).Deleted, AddressOf ChangeOfFile
                        AddHandler FileWatchers(i + 1).Renamed, AddressOf RenameOfFile
                    End If
                End If
                x += 1
            Next
        Catch ex As Exception
            MsgBox("Error: AddFilesToMonitor")
        End Try
    End Sub

    Sub CreateShortCut(ByVal TargetPath As String, ByVal ShortCutPath As String, ByVal ShortCutname As String, Optional ByVal WorkPath As String = "", Optional ByVal Window_Style As Integer = 0, Optional ByVal IconNum As Integer = 0)
        Dim VbsObj As Object
        VbsObj = CreateObject("WScript.Shell")
        Dim MyShortcut As Object
        MyShortcut = VbsObj.CreateShortcut(ShortCutPath & "\" & ShortCutname & ".lnk")
        MyShortcut.TargetPath = TargetPath
        MyShortcut.Arguments = "/Startup"
        Dim s = WorkPath.Substring(0, WorkPath.LastIndexOf("\"))
        MyShortcut.WorkingDirectory = WorkPath.Substring(0, WorkPath.LastIndexOf("\"))
        MyShortcut.WindowStyle = Window_Style
        MyShortcut.IconLocation = TargetPath & "," & IconNum
        MyShortcut.Save()
    End Sub

    Public Shared Function CheckIfCorrectFolder(ByVal FolderPath As String, ByVal Path As String) As Boolean
        Try
            Dim o = FolderPath.Length
            If FolderPath.IndexOf("\", Path.Length) <> -1 Then
                'Watcher path/edited file thrown path
                If FolderPath.IndexOf("\", Path.Length) <> Path.Length Then ''if it does have a subfolder in name then check if the folder names are equal by coparing their sizes
                    Return False
                End If
            Else
                If FolderPath.Substring(Path.Length) <> "" Then
                    Return False
                End If
            End If
            Return True
        Catch ex As Exception
            MsgBox("Error 114: CheckIfCorrectFolder" + vbNewLine + ex.Message)
        End Try
    End Function

    Public Sub UpdateFile(ByVal FilePath As String, ByVal Change As System.IO.WatcherChangeTypes, Optional ByVal e As RenamedEventArgs = Nothing)
        Try
            Dim Location, Location2, i As Int16
            Dim Message As String
            Dim FolderPath As String = FilePath.Substring(0, FilePath.LastIndexOf("\"))
            For i = 0 To SyncedFiles.Length / 5 - 1
                If FolderPath.IndexOf(SyncedFiles(i, SFOriginalFilePath)) = 0 Then
                    If CheckIfCorrectFolder(FolderPath, SyncedFiles(i, SFOriginalFilePath)) = False Then
                        GoTo GotoNext
                    End If
                    Location = SFOriginalFilePath
                    Location2 = SFCloudFilePath
                    GoTo Found
                End If
                If FolderPath.IndexOf(SyncedFiles(i, SFCloudFilePath)) = 0 Then
                    If CheckIfCorrectFolder(FolderPath, SyncedFiles(i, SFOriginalFilePath)) = False Then
                        GoTo GotoNext
                    End If
                    Location = SFCloudFilePath
                    Location2 = SFOriginalFilePath
                    GoTo Found
                End If
GotoNext:
            Next
            MsgBox("Error 115: Folders not properly synced. This might be caused by exiting Cloud Sync. Do not exit it if any folders are synced via Real Sync.")
            Exit Sub
Found:

            'If SyncedFiles(i, SFStatus) = "DropboxPathUpdated" Then
            'SyncedFiles(i, SFStatus) = "Synced"
            'Exit For
            'End If
            'If SyncedFiles(i, SFStatus) = "Synced" Then
            Dim FileName As String = FilePath.Substring(SyncedFiles(i, Location).Length + 1)
            If Change = System.IO.WatcherChangeTypes.Renamed Then
                Dim RenamedName As String
                If e.Name.IndexOf("\") <> -1 Then
                    RenamedName = e.Name.Substring(e.Name.LastIndexOf("\") + 1)
                Else
                    RenamedName = e.Name
                End If
                Dim iOfToDisable, x As Int16
                Dim a = DisableWatcher(i, Location2)
                iOfToDisable = a(0)
                x = a(1)
                Try
                    If My.Computer.FileSystem.DirectoryExists(SyncedFiles(i, Location2) + "\" + FileName) Then 'COULD REALLY BE A FOLDER BUT IT IS NOT SYNCED! CHECK!
                        My.Computer.FileSystem.RenameDirectory(SyncedFiles(i, Location2) + "\" + FileName, RenamedName)
                    Else
                        My.Computer.FileSystem.RenameFile(SyncedFiles(i, Location2) + "\" + FileName, RenamedName)
                    End If
                Catch
                    MsgBox("Error 116: Folders not properly synced. This might be caused by exiting Cloud Sync. Do not exit it if any folders are synced via Real Sync.")
                End Try
                FileWatchers(iOfToDisable + x).EnableRaisingEvents = True
                Message = """" + e.OldName + """ has been renamed to """ + e.Name + """" + "in " + FilePath
            ElseIf Change = System.IO.WatcherChangeTypes.Changed Or Change = System.IO.WatcherChangeTypes.Created Then
                Dim OriginalFileDate As Date = My.Computer.FileSystem.GetFileInfo(SyncedFiles(i, Location) + "\" + FileName).LastWriteTime
                Dim DropboxFileDate As Date = My.Computer.FileSystem.GetFileInfo(SyncedFiles(i, Location2) + "\" + FileName).LastWriteTime
                If My.Computer.FileSystem.GetFileInfo(SyncedFiles(i, Location) + "\" + FileName).LastWriteTime = My.Computer.FileSystem.GetFileInfo(SyncedFiles(i, Location2) + "\" + FileName).LastWriteTime Then
                    Exit Sub
                End If
                If My.Computer.FileSystem.DirectoryExists(FilePath) Then 'THIS INCLUDES IF ANYTHING IN THAT FOLDER (SUBFOLDER) IS RENAMED OR MODIFIED)
                    If Change = System.IO.WatcherChangeTypes.Changed Then
                        'Do nothing if changed
                        Exit Sub
                    Else
                        Dim iOfToDisable, x As Int16
                        Dim a = DisableWatcher(i, Location2)
                        iOfToDisable = a(0)
                        x = a(1)
                        My.Computer.FileSystem.CopyDirectory(FilePath, SyncedFiles(i, Location2) + "\" + FileName, True)
                        FileWatchers(iOfToDisable + x).EnableRaisingEvents = True
                    End If
                Else
                    Dim iOfToDisable, x As Int16
                    Dim a = DisableWatcher(i, Location2)
                    iOfToDisable = a(0)
                    x = a(1)
                    My.Computer.FileSystem.CopyFile(FilePath, SyncedFiles(i, Location2) + "\" + FileName, True)
                    FileWatchers(iOfToDisable + x).EnableRaisingEvents = True
                End If
                Message = """" + FileName + """ has been " + Change.ToString + " in " + FilePath
            ElseIf Change = System.IO.WatcherChangeTypes.Deleted Then
                Dim iOfToDisable, x As Int16
                Dim a = DisableWatcher(i, Location2)
                iOfToDisable = a(0)
                x = a(1)
                Try
                    If My.Computer.FileSystem.DirectoryExists(SyncedFiles(i, Location2) + "\" + FileName) Then
                        My.Computer.FileSystem.DeleteDirectory(SyncedFiles(i, Location2) + "\" + FileName, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Else
                        My.Computer.FileSystem.DeleteFile(SyncedFiles(i, Location2) + "\" + FileName)
                    End If
                Catch
                    MsgBox("Error 117: Folders not properly synced. This might be caused by exiting Cloud Sync. Do not exit it if any folders are synced via Real Sync.")
                End Try
                FileWatchers(iOfToDisable + x).EnableRaisingEvents = True
                Message = """" + FileName + """ has been updated in " + FilePath
            End If
            LastUpdatedFile = FolderPath

            NotifyIcon1.ShowBalloonTip(3000, """" + FileName + """ " + Change.ToString, Message, System.Windows.Forms.ToolTipIcon.Info)
        Catch ex As Exception
            MsgBox("Error 118: UpdateFile")
        End Try
    End Sub

    Public Sub SettingsDataUpdated()
        Try
            Dim x As Integer
            ReadFileSettingsAndSyncedFiles()
            If SyncedFiles.Length <> 0 Then
                ReDim FilesToBeSynced(SyncedFiles.Length / 5 - 1, 4)
            Else
                ReDim FilesToBeSynced(0, 4)
            End If

            Dim FilesAdded(SyncedFiles.Length / 5 - 1) As Integer
            For i As Integer = 0 To SyncedFiles.Length / 5 - 1
                If SyncedFiles(i, SFStatus) = StatusToBeSynced Then
                    FilesAdded(i) = 1
                    For i2 As Integer = 0 To 4
                        FilesToBeSynced(x, i2) = SyncedFiles(i, i2)
                    Next
                    x += 1
                End If
            Next
            If x > 0 Then
                Dim temp(,) = FilesToBeSynced
                ReDim FilesToBeSynced(x - 1, 4)
                For i As Integer = 0 To x - 1
                    FilesToBeSynced(i, 0) = temp(i, 0)
                    FilesToBeSynced(i, 1) = temp(i, 1)
                    FilesToBeSynced(i, 2) = temp(i, 2)
                    FilesToBeSynced(i, 3) = temp(i, 3)
                    FilesToBeSynced(i, 4) = temp(i, 4)
                Next
                AddFilesToCloud(FilesToBeSynced)
                For i = 0 To FilesAdded.Length - 1
                    If FilesAdded(i) = 1 Then
                        Unsync(i, False) '!!!WHY?......... optional ShowMessage = (default) true. hereUnsync(i, false). unsyncing because we need to remove the old "to be synced" item in SyncedFiles xml. a new one gets added when you sync it for real.
                    End If
                Next
            End If
            Monitoring = False
            For i = 0 To SyncedFiles.Length / 5 - 1
                If SyncedFiles(i, SFFolderSyncOptionOption) = "2" Then
                    Monitoring = True
                End If
            Next
            If Monitoring = False And My.Computer.FileSystem.FileExists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup) + "\" + My.Application.Info.AssemblyName + ".lnk") = True Then
                Try
                    My.Computer.FileSystem.DeleteFile(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup) + "\" + My.Application.Info.AssemblyName + ".lnk")
                Catch ex As Exception
                    MsgBox("Error: Couldn't remove CloudSync from Startup")
                End Try
            End If
            ReDim FilesToBeSynced(0, 4)
            PopulateListView() '!NECEARRY OR NOT?
        Catch ex As Exception
            MsgBox("Error 119: Updated" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Function DisableWatcher(ByVal a As Int16, ByVal b As Int16)
        Try
            Dim i, x
            x = -1
            Dim Path As String = SyncedFiles(a, b)
            For i = 0 To FileWatchers.Length - 1 Step 2
                If Path.IndexOf(FileWatchers(i).Path) = 0 Then
                    If CheckIfCorrectFolder(Path, FileWatchers(i).Path) = False Then
                        GoTo GotoNext
                    End If
                    FileWatchers(i).EnableRaisingEvents = False
                    x = 0
                    Exit For
                End If
                If Path.IndexOf(FileWatchers(i + 1).Path) = 0 Then
                    If CheckIfCorrectFolder(Path, FileWatchers(i + 1).Path) = False Then
                        GoTo GotoNext
                    End If
                    FileWatchers(i + 1).EnableRaisingEvents = False
                    x = 1
                    Exit For
                End If
GotoNext:
            Next 'CRASHES!
            Return {i, x}

            'Dim iOfToDisable, x As Int16
            'Dim a = DisableWatcher(SyncedFiles(i, SFDropboxFilePath))
            'iOfToDisable = a(0)
            'x = a(1) 'CRASHESSS AFTER!
            'Do stuff
            'FileWatchers(iOfToDisable + x).EnableRaisingEvents = True
        Catch ex As Exception
            MsgBox("Error 120: DisableWatcher")
        End Try
    End Function

    Private Sub ChangeOfFile(ByVal source As Object, ByVal e As System.IO.FileSystemEventArgs)
        If e.ChangeType = IO.WatcherChangeTypes.Changed And e.FullPath = SettingsDataPath Then
            SettingsDataUpdated() '!!!!! FOR NOW
            Exit Sub
        End If
        'ERROR IF SETTINGSDATA IS TAMPERED WITH! TAKE ADVANGTAGE!
        UpdateFile(e.FullPath, e.ChangeType)
    End Sub

    Private Sub RenameOfFile(ByVal source As Object, ByVal e As RenamedEventArgs)
        UpdateFile(e.OldFullPath, WatcherChangeTypes.Renamed, e)
    End Sub

    Private Sub MyApplication_StartupNextInstance(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
        Dim inputArgument As String = "/input="
        Dim inputName As String = ""

        For Each s As String In e.CommandLine
            If s.ToLower.StartsWith(inputArgument) Then
                inputName = s.Remove(0, inputArgument.Length)
            End If
        Next

        If inputName = "" Then
            MsgBox("No input name")
        Else
            MsgBox("Input name: " & inputName)
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Form2.ShowDialog()
        'Exit Sub
        Try
            If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                Dim FilesToBeSyncedTemp = OpenFileDialog1.FileNames
                ReDim FilesToBeSynced(FilesToBeSyncedTemp.Length - 1, 4)
                For i = 0 To FilesToBeSyncedTemp.Length - 1
                    FilesToBeSynced(i, SFOriginalFilePath) = FilesToBeSyncedTemp(i)
                    'LogWriter.Write(vbNewLine + FilesToBeSyncedTemp(i).ToString())
                Next
                AddFilesToCloud(FilesToBeSynced)
                'AddFilesToMonitor(Files)
            End If
        Catch ex As Exception
            MsgBox("Error 121: Button1.Click")
        End Try
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try
            Dialog1.ShowDialog()
            ReadFileSettingsAndSyncedFiles()
        Catch ex As Exception
            MsgBox("Error 122: Button2.CLick " + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Try
            With FolderBrowserDialog1
                .RootFolder = Environment.SpecialFolder.Desktop
                .SelectedPath = "c:\"
                .Description = "Select Folder to Sync"
                If .ShowDialog = DialogResult.OK Then
                    ReDim FilesToBeSynced(0, 4)
                    FilesToBeSynced(0, SFOriginalFilePath) = .SelectedPath ' + "\"
                    'LogWriter.Write(vbNewLine + FilesToBeSynced(0, SFOriginalFilePath).ToString())
                    AddFilesToCloud(FilesToBeSynced)
                End If
                '!if closed error is thrown!
            End With
        Catch ex As Exception
            MsgBox("Error 123: Button3.Click ")
        End Try
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'MsgBox("CLOSING!")
        Try
            'LogWriter.Write(vbNewLine + "isAdminUser, RestartingAsAdmin: " + isAdminUser.ToString + RestartingAsAdmin.ToString)
            If TrulyClose = False Then
                If isAdminUser = NotAdminUser And RestartingAsAdmin = 0 Or Monitoring = True Then
                    'LogWriter.Write(vbNewLine + "Not Closing")
                    e.Cancel = True
                    Me.Hide()
                    NotifyIcon1.Visible = True
                End If
            End If
        Catch ex As Exception
            MsgBox("Error 124 ")
        End Try
    End Sub


    Private Sub Form1_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.GotFocus
        If Startup = True Then
            Startup = False
            Me.Hide()
        End If
    End Sub

    Public Shared Sub ToggleCloudButtons()
        If Settings(0, CSDirectory) = Nothing Then
            Form1.DropboxButton.Enabled = False
        Else
            Form1.DropboxButton.Enabled = True
        End If
        If Settings(1, CSDirectory) = Nothing Then
            Form1.SkyDriveButton.Enabled = False
        Else
            Form1.SkyDriveButton.Enabled = True
        End If
        If Settings(2, CSDirectory) = Nothing Then
            Form1.GoogleDriveButton.Enabled = False
        Else
            Form1.GoogleDriveButton.Enabled = True
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Dim Running As Boolean = False
            Dim CommandLineArgsAsString As String

            Dim identity = WindowsIdentity.GetCurrent()
            Dim principal = New WindowsPrincipal(identity)
            isRunningAsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator)

            SettingsDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\CloudSync\SyncedFiles.xml" 'Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\") + 1) + "SettingsData.xml"
            LogDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\CloudSync\Log.log" 'Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\") + 1) + "SettingsData.xml"

            'If My.Computer.FileSystem.FileExists(LogDataPath) = False Then
            'System.IO.File.Create(LogDataPath)
            'End If

            'LogStream = New FileStream(LogDataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite)
            'Dim LogWriter = New StreamWriter(LogStream)

            If My.Computer.FileSystem.DirectoryExists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\CloudSync") = False Then
                My.Computer.FileSystem.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\CloudSync")
            End If

            'My.Settings.DropboxPath = "that"
            'My.Settings.Save()

            'LogWriter.Write("Ignore this. If you get any errors, copy paste this to me." + vbNewLine + vbNewLine + "Debug code:" + vbNewLine)

            Dim proc() As Process
            Dim ModuleName, ProcName As String
            ModuleName = System.Diagnostics.Process.GetCurrentProcess.MainModule.ModuleName
            ProcName = System.IO.Path.GetFileNameWithoutExtension(ModuleName)
            proc = System.Diagnostics.Process.GetProcessesByName(ProcName)
            If proc.Length > 1 Then
                'LogWriter.Write(vbNewLine + "Application is already running") 'You can replace this with Reporter.ReportEvent
                Running = True
            End If
            If Running = False Then
                'LogWriter.Write(vbNewLine + "Not Running")
            End If

            'LogWriter.Write(vbNewLine + "isAdmin: " + isRunningAsAdmin.ToString)

            If CommandLineArgs.Count <> 0 Then
                CommandLineArgsAsString = CommandLineArgs(0)
                'MsgBox(CommandLineArgsAsString)
            End If

            If isRunningAsAdmin = False And Running = False Then
                Dim p As New Process
                p.StartInfo.FileName = Application.ExecutablePath
                p.StartInfo.Verb = "runas"
                'p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                If CommandLineArgsAsString <> Nothing Then
                    'LogWriter.Write(vbNewLine + "before launching: " + CommandLineArgs(0) + vbNewLine + "string to be used: " + CommandLineArgsAsString)
                    p.StartInfo.Arguments = """NotAdminUser" + CommandLineArgsAsString + """"
                Else
                    p.StartInfo.Arguments = """NotAdminUser"""
                End If
                Try
                    p.Start()
                Catch ex As Exception
                    If ex.Message = "The operation was canceled by the user" Then
                        Me.Close()
                    End If
                End Try
                TrulyClose = True
                RestartingAsAdmin = 1 '1 = yes/true
                Me.Close()
                If RestartingAsAdmin = 1 Then Exit Sub
            End If

            'START REAL CODE

            'welcome screen asking which service to use IF NOT right clicked. Service will be determined by settings as default service (option to ask each time too) and ... as right click (in future)
            'CloudService = "Dropbox"

            ReadFileSettingsAndSyncedFiles()
            AddFilesToMonitor(Nothing, SettingsDataPath)

            ToggleCloudButtons()

            For i As Integer = 0 To SyncedFiles.Length / 5 - 1
                If SyncedFiles(i, SFFolderSyncOptionOption) = "2" Then
                    AddFilesToMonitor(ToOneDimArray(SyncedFiles, i), , i) 'ERROR
                End If
            Next

            For i As Int16 = 0 To CloudServicesSyncedNumber - 1
                If My.Computer.FileSystem.DirectoryExists(Settings(CloudServicesSynced(i), CSDirectory)) = False Then
                    Settings(i, 0) = "1"
                    Settings(i, 1) = ""
                    Settings(i, 2) = ""
                    Settings(i, 3) = ""
                    Settings(i, 4) = "True"
                    Settings(i, 5) = "False"
                    WriteSettings()
                    MsgBox("Alert 141: The directory for " + CloudServiceAsString(i) + " no longer seems to exist.")
                End If
            Next

            If Settings(Dropbox, CSDirectory) = Nothing And Settings(SkyDrive, CSDirectory) = Nothing And Settings(GoogleDrive, CSDirectory) = Nothing Then '!!!!
                Form2Browse = True
                Form2.ShowDialog()
            End If

            'SettingsDataUpdated()

            'LogWriter.Write(vbNewLine + "String: " + CommandLineArgsAsString)

            If CommandLineArgsAsString <> Nothing Then 'if it contains something
                If CommandLineArgsAsString.IndexOf("NotAdminUser") = 0 Then
                    'LogWriter.Write(vbNewLine + "Arguments: " + CommandLineArgs(0))
                    'MsgBox("Arguments: " + CommandLineArgs(0))
                    CommandLineArgsAsString = CommandLineArgsAsString.Substring(12) 'update the string to delete the NotAdminUser argument and only keep
                    'the rest of the - ie, argument of the file
                    isAdminUser = NotAdminUser
                    FilesToBeSynced(0, SFOriginalFilePath) = CommandLineArgsAsString
                End If
                If CommandLineArgsAsString.IndexOf("/Startup") = 0 Then
                    Startup = True
                End If

                'if the remaining argument does have additional arguments -ie, argument of file then update the settings file so the admin program can
                'do its thing and then close the non-admin program.
                If CommandLineArgsAsString <> Nothing Then
                    FilesToBeSynced(0, SFOriginalFilePath) = CommandLineArgsAsString
                    If isRunningAsAdmin = False And Running = True Then 'that means that program opened by right clicking while program is already open
                        FilesToBeSynced(0, SFStatus) = StatusToBeSynced
                        WriteSyncedFilesInSettings()
                        TrulyClose = True
                        'MsgBox("CLOSING!")
                        Me.Close() '!!!ERROR
                        If TrulyClose = True Then Exit Sub
                    End If

                    For i = 0 To FilesToBeSynced.Length / 5 - 1 'WHY IS IT 30?....
                        'LogWriter.Write(vbNewLine + "Files: " + FilesToBeSynced(i, SFOriginalFilePath).ToString())
                        'MsgBox("Files: " + Files(i).ToString())
                    Next
                    AddFilesToCloud(FilesToBeSynced)
                End If
                'LogWriter.Write(vbNewLine + "Commands: " + CommandLineArgs(0))
            End If
            TrulyClose = False
            'If CommandLineArgsAsString <> Nothing Then Me.Close()
            If Running = True Then
                Try
                    MsgBox("CloudSync is already running.") '
                    NotifyIcon1.ShowBalloonTip(3000, "CloudSync is already running.", "", System.Windows.Forms.ToolTipIcon.Info)
                    Dim w = FindWindow(Nothing, "CloudSync")
                    SetForegroundWindow(w)
                    SetFocus(w)
                    BringWindowToTop(w)
                Catch ex As Exception

                End Try
                TrulyClose = True
                Me.Close()
            End If
            PopulateListView()
        Catch ex As Exception
            MsgBox("Error 125: Load" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub OpenCloudSyncToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenCloudSyncToolStripMenuItem.Click
        Me.Show()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        TrulyClose = True
        Me.Close()
    End Sub

    Private Sub NotifyIcon1_BalloonTipClicked(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.BalloonTipClicked
        Try
            Shell("explorer.exe " + LastUpdatedFile, Microsoft.VisualBasic.AppWinStyle.NormalFocus)
        Catch ex As Exception
            MsgBox("Error 126: Directory doesn't exist. Resyncing will fix this issue.")
        End Try
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
    End Sub

    Private Sub ListView1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Try
            OpenFileDirectory()
        Catch ex As Exception
            MsgBox("Error 150: File could not be opened. Resyncing might fix this issue.")
        End Try
    End Sub

    Private Sub ListViewContextMenuStrip_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ListViewContextMenuStrip.Opening
        Try '!!!fix me!!! RETRY SYNC ISNT SHOWING UP
            'RetrySyncToolStripMenuItem.HideDropDown()
            'ListViewContextMenuStrip.Items.Remove(RetrySyncToolStripMenuItem)
            If ListView1.SelectedIndices.Count < 2 And ListView1.SelectedIndices.Count <> 0 Then
                'ListViewContextMenuStrip.Items("Open Cloud Location").Name = "Open " + SyncedFiles(ListView1.SelectedIndices(0), SFCloudService) + " Location"
            End If
            If ListView1.SelectedIndices.Count > 0 Then
                For i = 0 To ListView1.SelectedIndices.Count - 1
                    If SyncedFiles(ListView1.SelectedIndices(i), SFStatus) = StatusFailed Or SyncedFiles(ListView1.SelectedIndices(i), SFStatus) = StatusFailedMovingDirectory Or SyncedFiles(ListView1.SelectedIndices(i), SFStatus) = StatusCancelled Then
                        ListViewContextMenuStrip.Items.Add(RetrySyncToolStripMenuItem)
                        'RetrySyncToolStripMenuItem.ShowDropDown()
                        'AddFilesToCloud()
                    End If
                    If My.Computer.FileSystem.DirectoryExists(SyncedFiles(ListView1.SelectedIndices(i), SFOriginalFilePath)) Then
                        ListViewContextMenuStrip.Items.Remove(Me.OpenFileToolStripMenuItem)
                        Exit Sub
                    End If
                Next
                ListViewContextMenuStrip.Items.Insert(0, Me.OpenFileToolStripMenuItem)
            Else
                e.Cancel = True
            End If
        Catch ex As Exception
            MsgBox("Error 127" + vbNewLine + ex.Message)
        End Try
    End Sub

    Private Sub OpenFileDirectory()
        Try
            For i = 0 To ListView1.SelectedIndices.Count - 1
                If SyncedFiles(ListView1.SelectedIndices(i), SFFolderSyncOptionOption) = "0" Then
                    If My.Computer.FileSystem.FileExists(SyncedFiles(ListView1.SelectedIndices(i), SFOriginalFilePath)) Then
                        Try
                            Shell("explorer /select, " + SyncedFiles(ListView1.SelectedIndices(i), SFOriginalFilePath), Microsoft.VisualBasic.AppWinStyle.NormalFocus)
                        Catch ex As Exception
                            MsgBox("Error 128: Directory doesn't exist. Resyncing will fix this issue.")
                        End Try
                    Else
                        MsgBox("Error 129: Directory doesn't exist. Resyncing will fix this issue.")
                    End If
                Else
                    If My.Computer.FileSystem.DirectoryExists(SyncedFiles(ListView1.SelectedIndices(i), SFOriginalFilePath)) Then
                        Try
                            Shell("explorer " + SyncedFiles(ListView1.SelectedIndices(i), SFOriginalFilePath), Microsoft.VisualBasic.AppWinStyle.NormalFocus)
                        Catch ex As Exception
                            MsgBox("Error 130: Directory doesn't exist. Resyncing will fix this issue.")
                        End Try
                    Else
                        MsgBox("Error 131: Folder doesn't exist. Resyncing will fix this issue.")
                    End If
                End If
            Next
        Catch ex As Exception
            MsgBox("Error 132 ")
        End Try
    End Sub

    Private Sub OpenOriginalLocationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenOriginalLocationToolStripMenuItem.Click
        OpenFileDirectory()
    End Sub

    Private Sub OpenFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenFileToolStripMenuItem.Click
        Try
            For i = 0 To ListView1.SelectedIndices.Count - 1
                If My.Computer.FileSystem.FileExists(SyncedFiles(ListView1.SelectedIndices(i), SFCloudFilePath)) Then
                    Try
                        Shell(SyncedFiles(ListView1.SelectedIndices(i), SFCloudFilePath), Microsoft.VisualBasic.AppWinStyle.NormalFocus)
                    Catch ex As Exception
                        MsgBox("Error 133: File could not be opened. Resyncing might fix this issue.")
                    End Try
                Else
                    MsgBox("Error 133B: File could not be found. Resyncing might fix this issue.")
                End If
            Next
        Catch ex As Exception
            MsgBox("Error 134 ")
        End Try
    End Sub

    Private Sub OpenDropboxLocationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenDropboxLocationToolStripMenuItem.Click
        Try
            For i = 0 To ListView1.SelectedIndices.Count - 1
                If SyncedFiles(ListView1.SelectedIndices(i), SFFolderSyncOptionOption) = "0" Then
                    If My.Computer.FileSystem.FileExists(SyncedFiles(ListView1.SelectedIndices(i), SFCloudFilePath)) Then
                        Try
                            Shell("explorer /select, " + SyncedFiles(ListView1.SelectedIndices(i), SFCloudFilePath), Microsoft.VisualBasic.AppWinStyle.NormalFocus)
                        Catch ex As Exception
                            MsgBox("Error 135: File doesn't exist. Resyncing will fix this issue.")
                        End Try
                    Else
                        MsgBox("Error 136: File doesn't exist. Resyncing will fix this issue.")
                    End If
                Else
                    If My.Computer.FileSystem.DirectoryExists(SyncedFiles(ListView1.SelectedIndices(i), SFCloudFilePath)) Then
                        Try
                            Shell("explorer " + SyncedFiles(ListView1.SelectedIndices(i), SFCloudFilePath), Microsoft.VisualBasic.AppWinStyle.NormalFocus)
                        Catch ex As Exception
                            MsgBox("Error 137: Directory doesn't exist. Resyncing will fix this issue.")
                        End Try
                    Else
                        MsgBox("Error 138: Directory doesn't exist. Resyncing will fix this issue.")
                    End If
                End If
            Next
        Catch ex As Exception
            MsgBox("Error 139")
        End Try
    End Sub

    Private Sub UnsyncToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnsyncToolStripMenuItem.Click
        Unsync()
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshToolStripMenuItem.Click
        PopulateListView()
    End Sub

    Private Sub RefreshButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
        PopulateListView()
    End Sub

    Private Sub DropboxButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DropboxButton.Click
        Try
            Shell("explorer " + Settings(0, CSDirectory), Microsoft.VisualBasic.AppWinStyle.NormalFocus)
        Catch ex As Exception
            MsgBox("Error 147: Dropbox directory not found")
        End Try
    End Sub

    Private Sub SkyDriveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SkyDriveButton.Click
        Try
            Shell("explorer " + Settings(1, CSDirectory), Microsoft.VisualBasic.AppWinStyle.NormalFocus)
        Catch ex As Exception
            MsgBox("Error 148: SkyDrive directory not found")
        End Try
    End Sub

    Private Sub GoogleDriveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleDriveButton.Click
        Try
            Shell("explorer " + Settings(2, CSDirectory), Microsoft.VisualBasic.AppWinStyle.NormalFocus)
        Catch ex As Exception
            MsgBox("Error 149: GoogleDrive directory not found")
        End Try
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        'w 40
        ListView1.Width = Me.Width - 40
        'h 89
        ListView1.Height = Me.Height - 89
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Form2Browse = True
        Form2.ShowDialog()
    End Sub

    Private Sub InfoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InfoButton.Click
        AboutBox1.ShowDialog()
    End Sub
End Class


'upto 150. next is going to be 151.

'http://www.iconarchive.com/show/oxygen-icons-by-oxygen-icons.org/Actions-view-refresh-icon.html
'http://www.iconarchive.com/show/koloria-icons-by-graphicrating/Folder-Add-icon.html
'main icon: http://www.iconarchive.com/show/cloud-icons-by-jackietran/cloud-reload-icon.html