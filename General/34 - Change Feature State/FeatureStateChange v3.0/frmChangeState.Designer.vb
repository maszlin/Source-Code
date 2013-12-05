<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmStateChange
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblJobState = New System.Windows.Forms.Label
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblPPWO = New System.Windows.Forms.Label
        Me.dgvFeature = New System.Windows.Forms.DataGridView
        Me.btnSelectAll = New System.Windows.Forms.Button
        Me.btnUnselectAll = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.chk_highlight = New System.Windows.Forms.CheckBox
        Me.cb_JobID = New System.Windows.Forms.CheckBox
        Me.cmbJob = New System.Windows.Forms.ComboBox
        Me.btnJob = New System.Windows.Forms.Button
        CType(Me.dgvFeature, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(55, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Job State:"
        '
        'lblJobState
        '
        Me.lblJobState.AutoSize = True
        Me.lblJobState.Location = New System.Drawing.Point(370, 36)
        Me.lblJobState.Name = "lblJobState"
        Me.lblJobState.Size = New System.Drawing.Size(59, 13)
        Me.lblJobState.TabIndex = 2
        Me.lblJobState.Text = "lblJobState"
        Me.lblJobState.Visible = False
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(522, 16)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "Update"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(605, 16)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Close"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(19, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Scheme Name:"
        '
        'lblPPWO
        '
        Me.lblPPWO.AutoSize = True
        Me.lblPPWO.Location = New System.Drawing.Point(114, 9)
        Me.lblPPWO.Name = "lblPPWO"
        Me.lblPPWO.Size = New System.Drawing.Size(50, 13)
        Me.lblPPWO.TabIndex = 6
        Me.lblPPWO.Text = "lblPPWO"
        '
        'dgvFeature
        '
        Me.dgvFeature.AllowUserToAddRows = False
        Me.dgvFeature.AllowUserToDeleteRows = False
        Me.dgvFeature.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvFeature.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.dgvFeature.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.dgvFeature.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgvFeature.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvFeature.Location = New System.Drawing.Point(12, 57)
        Me.dgvFeature.Name = "dgvFeature"
        Me.dgvFeature.Size = New System.Drawing.Size(693, 251)
        Me.dgvFeature.TabIndex = 0
        '
        'btnSelectAll
        '
        Me.btnSelectAll.Location = New System.Drawing.Point(6, 16)
        Me.btnSelectAll.Name = "btnSelectAll"
        Me.btnSelectAll.Size = New System.Drawing.Size(63, 23)
        Me.btnSelectAll.TabIndex = 7
        Me.btnSelectAll.Text = "Select All"
        Me.btnSelectAll.UseVisualStyleBackColor = True
        '
        'btnUnselectAll
        '
        Me.btnUnselectAll.Location = New System.Drawing.Point(89, 16)
        Me.btnUnselectAll.Name = "btnUnselectAll"
        Me.btnUnselectAll.Size = New System.Drawing.Size(72, 23)
        Me.btnUnselectAll.TabIndex = 8
        Me.btnUnselectAll.Text = "Unselect All"
        Me.btnUnselectAll.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chk_highlight)
        Me.GroupBox1.Controls.Add(Me.btnOK)
        Me.GroupBox1.Controls.Add(Me.btnUnselectAll)
        Me.GroupBox1.Controls.Add(Me.btnSelectAll)
        Me.GroupBox1.Controls.Add(Me.btnCancel)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 314)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(693, 47)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        '
        'chk_highlight
        '
        Me.chk_highlight.AutoSize = True
        Me.chk_highlight.Checked = True
        Me.chk_highlight.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chk_highlight.Location = New System.Drawing.Point(204, 20)
        Me.chk_highlight.Name = "chk_highlight"
        Me.chk_highlight.Size = New System.Drawing.Size(106, 17)
        Me.chk_highlight.TabIndex = 12
        Me.chk_highlight.Text = "Highlight on Map"
        Me.chk_highlight.UseVisualStyleBackColor = True
        '
        'cb_JobID
        '
        Me.cb_JobID.AutoSize = True
        Me.cb_JobID.Location = New System.Drawing.Point(633, 28)
        Me.cb_JobID.Name = "cb_JobID"
        Me.cb_JobID.Size = New System.Drawing.Size(72, 17)
        Me.cb_JobID.TabIndex = 16
        Me.cb_JobID.Text = "By Job ID"
        Me.cb_JobID.UseVisualStyleBackColor = True
        '
        'cmbJob
        '
        Me.cmbJob.FormattingEnabled = True
        Me.cmbJob.Location = New System.Drawing.Point(117, 26)
        Me.cmbJob.Name = "cmbJob"
        Me.cmbJob.Size = New System.Drawing.Size(141, 21)
        Me.cmbJob.TabIndex = 17
        '
        'btnJob
        '
        Me.btnJob.Location = New System.Drawing.Point(268, 26)
        Me.btnJob.Name = "btnJob"
        Me.btnJob.Size = New System.Drawing.Size(100, 23)
        Me.btnJob.TabIndex = 18
        Me.btnJob.Text = "Update Job State"
        Me.btnJob.UseVisualStyleBackColor = True
        '
        'frmStateChange
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(730, 372)
        Me.Controls.Add(Me.btnJob)
        Me.Controls.Add(Me.cmbJob)
        Me.Controls.Add(Me.cb_JobID)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.dgvFeature)
        Me.Controls.Add(Me.lblPPWO)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblJobState)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmStateChange"
        Me.Text = "Feature State Change v3.0"
        CType(Me.dgvFeature, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblJobState As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblPPWO As System.Windows.Forms.Label
    Friend WithEvents dgvFeature As System.Windows.Forms.DataGridView
    Friend WithEvents btnSelectAll As System.Windows.Forms.Button
    Friend WithEvents btnUnselectAll As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents chk_highlight As System.Windows.Forms.CheckBox
    Private WithEvents cb_JobID As System.Windows.Forms.CheckBox
    Friend WithEvents cmbJob As System.Windows.Forms.ComboBox
    Friend WithEvents btnJob As System.Windows.Forms.Button
End Class
