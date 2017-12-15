<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ListViewContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenOriginalLocationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenDropboxLocationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.UnsyncToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RetrySyncToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.NotifyIconContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenCloudSyncToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.InfoButton = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.RefreshButton = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.SkyDriveButton = New System.Windows.Forms.Button()
        Me.GoogleDriveButton = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.DropboxButton = New System.Windows.Forms.Button()
        Me.ListViewContextMenuStrip.SuspendLayout()
        Me.NotifyIconContextMenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListView1
        '
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader4, Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3})
        Me.ListView1.ContextMenuStrip = Me.ListViewContextMenuStrip
        Me.ListView1.FullRowSelect = True
        Me.ListView1.Location = New System.Drawing.Point(12, 41)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.ShowItemToolTips = True
        Me.ListView1.Size = New System.Drawing.Size(632, 211)
        Me.ListView1.SmallImageList = Me.ImageList1
        Me.ListView1.TabIndex = 10
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = ""
        Me.ColumnHeader4.Width = 28
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Name"
        Me.ColumnHeader1.Width = 150
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Original  Location"
        Me.ColumnHeader2.Width = 225
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Cloud Location"
        Me.ColumnHeader3.Width = 225
        '
        'ListViewContextMenuStrip
        '
        Me.ListViewContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenFileToolStripMenuItem, Me.OpenOriginalLocationToolStripMenuItem, Me.OpenDropboxLocationToolStripMenuItem, Me.ToolStripSeparator1, Me.RefreshToolStripMenuItem, Me.ToolStripSeparator2, Me.UnsyncToolStripMenuItem, Me.RetrySyncToolStripMenuItem})
        Me.ListViewContextMenuStrip.Name = "ListViewContextMenuStrip"
        Me.ListViewContextMenuStrip.Size = New System.Drawing.Size(198, 148)
        '
        'OpenFileToolStripMenuItem
        '
        Me.OpenFileToolStripMenuItem.Name = "OpenFileToolStripMenuItem"
        Me.OpenFileToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.OpenFileToolStripMenuItem.Text = "Open File"
        '
        'OpenOriginalLocationToolStripMenuItem
        '
        Me.OpenOriginalLocationToolStripMenuItem.Name = "OpenOriginalLocationToolStripMenuItem"
        Me.OpenOriginalLocationToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.OpenOriginalLocationToolStripMenuItem.Text = "Open Original Location"
        '
        'OpenDropboxLocationToolStripMenuItem
        '
        Me.OpenDropboxLocationToolStripMenuItem.Name = "OpenDropboxLocationToolStripMenuItem"
        Me.OpenDropboxLocationToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.OpenDropboxLocationToolStripMenuItem.Text = "Open Cloud Location"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(194, 6)
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.RefreshToolStripMenuItem.Text = "Refresh"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(194, 6)
        '
        'UnsyncToolStripMenuItem
        '
        Me.UnsyncToolStripMenuItem.Name = "UnsyncToolStripMenuItem"
        Me.UnsyncToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.UnsyncToolStripMenuItem.Text = "Unsync"
        '
        'RetrySyncToolStripMenuItem
        '
        Me.RetrySyncToolStripMenuItem.Name = "RetrySyncToolStripMenuItem"
        Me.RetrySyncToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.RetrySyncToolStripMenuItem.Text = "Retry Sync"
        Me.RetrySyncToolStripMenuItem.Visible = False
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "Dropbox 48x48.ico")
        Me.ImageList1.Images.SetKeyName(1, "Dropbox Statusbar 16x16.ico")
        Me.ImageList1.Images.SetKeyName(2, "DropboxCheck.ico")
        Me.ImageList1.Images.SetKeyName(3, "DropboxError.ico")
        Me.ImageList1.Images.SetKeyName(4, "DropboxPending.ico")
        Me.ImageList1.Images.SetKeyName(5, "SkyDriveCheck.ico")
        Me.ImageList1.Images.SetKeyName(6, "SkyDriveError.ico")
        Me.ImageList1.Images.SetKeyName(7, "SkyDrivePending.ico")
        Me.ImageList1.Images.SetKeyName(8, "GoogleDriveCheck.ico")
        Me.ImageList1.Images.SetKeyName(9, "GoogleDriveError.ico")
        Me.ImageList1.Images.SetKeyName(10, "GoogleDrivePending.ico")
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.ContextMenuStrip = Me.NotifyIconContextMenuStrip
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "CloudSync"
        Me.NotifyIcon1.Visible = True
        '
        'NotifyIconContextMenuStrip
        '
        Me.NotifyIconContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenCloudSyncToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.NotifyIconContextMenuStrip.Name = "NotifyIconContextMenuStrip"
        Me.NotifyIconContextMenuStrip.Size = New System.Drawing.Size(164, 48)
        '
        'OpenCloudSyncToolStripMenuItem
        '
        Me.OpenCloudSyncToolStripMenuItem.DoubleClickEnabled = True
        Me.OpenCloudSyncToolStripMenuItem.Name = "OpenCloudSyncToolStripMenuItem"
        Me.OpenCloudSyncToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
        Me.OpenCloudSyncToolStripMenuItem.Text = "Open CloudSync"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'Panel1
        '
        Me.Panel1.Location = New System.Drawing.Point(99, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(4, 21)
        Me.Panel1.TabIndex = 18
        '
        'Panel2
        '
        Me.Panel2.Location = New System.Drawing.Point(196, 12)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(4, 21)
        Me.Panel2.TabIndex = 19
        '
        'InfoButton
        '
        Me.InfoButton.Image = Global.CloudSync.My.Resources.Resources.Information_Icon
        Me.InfoButton.Location = New System.Drawing.Point(167, 12)
        Me.InfoButton.Name = "InfoButton"
        Me.InfoButton.Size = New System.Drawing.Size(23, 23)
        Me.InfoButton.TabIndex = 6
        Me.InfoButton.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Image = Global.CloudSync.My.Resources.Resources.Add_Cloud__Corner_
        Me.Button4.Location = New System.Drawing.Point(70, 12)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(23, 23)
        Me.Button4.TabIndex = 3
        Me.Button4.UseVisualStyleBackColor = True
        '
        'RefreshButton
        '
        Me.RefreshButton.Image = Global.CloudSync.My.Resources.Resources.Refresh
        Me.RefreshButton.Location = New System.Drawing.Point(138, 12)
        Me.RefreshButton.Name = "RefreshButton"
        Me.RefreshButton.Size = New System.Drawing.Size(23, 23)
        Me.RefreshButton.TabIndex = 5
        Me.RefreshButton.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Image = Global.CloudSync.My.Resources.Resources.Add_File
        Me.Button1.Location = New System.Drawing.Point(12, 12)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(23, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Image = Global.CloudSync.My.Resources.Resources.Add_Folder
        Me.Button3.Location = New System.Drawing.Point(41, 12)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(23, 23)
        Me.Button3.TabIndex = 2
        Me.Button3.UseVisualStyleBackColor = True
        '
        'SkyDriveButton
        '
        Me.SkyDriveButton.Image = Global.CloudSync.My.Resources.Resources.SkyDrive_16x16
        Me.SkyDriveButton.Location = New System.Drawing.Point(235, 12)
        Me.SkyDriveButton.Name = "SkyDriveButton"
        Me.SkyDriveButton.Size = New System.Drawing.Size(23, 23)
        Me.SkyDriveButton.TabIndex = 8
        Me.SkyDriveButton.UseVisualStyleBackColor = True
        '
        'GoogleDriveButton
        '
        Me.GoogleDriveButton.Enabled = False
        Me.GoogleDriveButton.Image = Global.CloudSync.My.Resources.Resources.Google_Drive_16x16
        Me.GoogleDriveButton.Location = New System.Drawing.Point(264, 12)
        Me.GoogleDriveButton.Name = "GoogleDriveButton"
        Me.GoogleDriveButton.Size = New System.Drawing.Size(23, 23)
        Me.GoogleDriveButton.TabIndex = 9
        Me.GoogleDriveButton.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Image = Global.CloudSync.My.Resources.Resources.Settings
        Me.Button2.Location = New System.Drawing.Point(109, 12)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(23, 23)
        Me.Button2.TabIndex = 4
        Me.Button2.UseVisualStyleBackColor = True
        '
        'DropboxButton
        '
        Me.DropboxButton.Enabled = False
        Me.DropboxButton.Image = Global.CloudSync.My.Resources.Resources.Dropbox_16x16
        Me.DropboxButton.Location = New System.Drawing.Point(206, 12)
        Me.DropboxButton.Name = "DropboxButton"
        Me.DropboxButton.Size = New System.Drawing.Size(23, 23)
        Me.DropboxButton.TabIndex = 7
        Me.DropboxButton.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(656, 264)
        Me.Controls.Add(Me.InfoButton)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.RefreshButton)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.SkyDriveButton)
        Me.Controls.Add(Me.GoogleDriveButton)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.DropboxButton)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "CloudSync"
        Me.ListViewContextMenuStrip.ResumeLayout(False)
        Me.NotifyIconContextMenuStrip.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents NotifyIconContextMenuStrip As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenCloudSyncToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents ListViewContextMenuStrip As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenOriginalLocationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenDropboxLocationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents UnsyncToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RefreshToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents RetrySyncToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DropboxButton As System.Windows.Forms.Button
    Friend WithEvents GoogleDriveButton As System.Windows.Forms.Button
    Friend WithEvents SkyDriveButton As System.Windows.Forms.Button
    Friend WithEvents RefreshButton As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents InfoButton As System.Windows.Forms.Button

End Class
