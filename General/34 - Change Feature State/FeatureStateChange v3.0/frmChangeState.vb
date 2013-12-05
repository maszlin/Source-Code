
Imports System.Data.OleDB
Imports System
Imports System.Collections.Generic
Imports Intergraph.GTechnology.API
Imports Intergraph.GTechnology.Interfaces
Imports System.ComponentModel
Imports System.Data


Public Class frmStateChange
    Public DataCon As IGTDataContext
    Public rsAttributeQuery As ADODB.Recordset
    Public iCNOFeatState = 51
    Public iCNOFibreCable = 7201
    Public bValid = False 'boolean variable to determine if record is a valid one to change feature state
    Public bFinalPost As Boolean 'Boolean variable to indicate that the job is in Final Post stage
    Public bFeatureSelected As Boolean 'boolean variable to indicate that there are features selected.
    Public bDistribution As Boolean 'boolean variable to determine if job is part of a distribution scheme
    Public sASB = "ASB"
    Public sJobID As String
    Public m_oCustomCommandHelper As IGTCustomCommandHelper
    Public iSourceIDCol = 4
    Public iDestIDCol = 8
    Public iFeatureIDCol = 4
    Public iWorkOrder = 4
    Public iSlashCNO = 80
    Public iSlashCNODet = 79
    Public iZoomFactor = 1.1
    Public sPostedID As String = ""
    Public bPPX = False
    Public WithEvents oFeatPlaceService As Intergraph.GTechnology.API.IGTFeaturePlacementService
    Public aPPXFeatFNO As String()
    Public aPPXFeatFID As String()
    Public oFeat As IGTKeyObject
    Public oDDCKeyObjects As IGTDDCKeyObjects
    Public iFeatCount As Integer
    Public iPlacedCount As Integer
    Public bPlaced2ndSlash As Boolean
    Public sMasterPPWO As String = ""


    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        EndProgram()
    End Sub

    Private Sub frmStateChange_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim sSQL As String
        Dim oDA As New OleDbDataAdapter
        Dim oDT As New DataTable
        Dim oDGVR As New System.Windows.Forms.DataGridViewRow
        Dim oDGVC As New System.Windows.Forms.DataGridViewColumn
       
        Try
            GTClassFactory.Create(Of IGTApplication)().BeginWaitCursor()
            DataCon = GTClassFactory.Create(Of IGTApplication)().DataContext

            sJobID = DataCon.ActiveJob
            sSQL = "SELECT * FROM G3E_JOB WHERE G3E_IDENTIFIER = '" & sJobID & "'"
            rsAttributeQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            If rsAttributeQuery.RecordCount > 0 Then
                If Not IsDBNull(rsAttributeQuery.Fields.Item("JOB_STATE").Value) Then
                    lblJobState.Text = rsAttributeQuery.Fields.Item("JOB_STATE").Value
                End If
                If Not IsDBNull(rsAttributeQuery.Fields.Item("SCHEME_NAME").Value) Then
                    lblPPWO.Text = rsAttributeQuery.Fields.Item("SCHEME_NAME").Value
                End If
                sMasterPPWO = lblPPWO.Text
            End If

            bFinalPost = False
            bDistribution = False
            PopulateJobState(bFinalPost, bDistribution)
            'Vinod 01-Nov-2012
            If (GTClassFactory.Create(Of IGTApplication)().SelectedObjects.FeatureCount > 0) Then
                PopulateDataGrid(bFinalPost, bDistribution)
            End If

            ''Vinod 17-Jan-2011
            'If UCase(lblJobState.Text) = "DEMOLISHED" Or UCase(lblJobState.Text) = "ABANDONED" Or UCase(lblJobState.Text) = "AMENDMENT" Or UCase(lblJobState.Text) = "RECOVERED" Then
            '    PopulateDataGrid(bFinalPost, bDistribution)
            'Else
            '    PopulateGridJob(lblPPWO.Text, 0)
            'End If

            GTClassFactory.Create(Of IGTApplication)().EndWaitCursor()
        Catch ex As Exception
            MsgBox("frmStateChange_Load:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try

    End Sub

    Private Sub PopulateDataGrid(ByVal bFinalPost As Boolean, ByVal bDistribution As Boolean)
        Dim oDDCKeyObjects As IGTDDCKeyObjects

        Try
            oDDCKeyObjects = GTClassFactory.Create(Of IGTApplication)().SelectedObjects.GetObjects()
            If (GTClassFactory.Create(Of IGTApplication)().SelectedObjects.FeatureCount > 0) Then
                PopulateSelected(oDDCKeyObjects)
            Else 'Vinod 01-Nov-2012
                dgvFeature.Rows.Clear()
                dgvFeature.Columns.Clear()
            End If

        Catch ex As Exception
            MsgBox("PopulateDataGrid:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try

    End Sub

    Private Sub PopulateJobState(ByVal bFinalPost As Boolean, ByVal bDistribution As Boolean)
       
        Dim jobState As String

        Try
            cmbJob.Items.Clear()
            jobState = Get_Value("SELECT JOB_STATE FROM G3E_JOB WHERE G3E_IDENTIFIER = '" & sJobID & "'")
            cmbJob.Text = jobState
            If jobState = "PROPOSED" Then
                cmbJob.Items.Add(New cboItem("PROPOSED", "PROPOSED"))
                cmbJob.Items.Add(New cboItem("UN_CONSTRUCT", "UN_CONSTRUCT"))
            ElseIf jobState = "UN_CONSTRUCT" Then
                cmbJob.Items.Add(New cboItem("UN_CONSTRUCT", "UN_CONSTRUCT"))
                cmbJob.Items.Add(New cboItem("COMPLETED", "COMPLETED"))
            ElseIf jobState = "COMPLETED" Then
                cmbJob.Enabled = False
            End If

            'sSQL = "SELECT FEATURE_STATE FROM GC_NETELEM WHERE SCHEME_NAME =" + sJobID
            'rs = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)

            'If Not (rs.EOF) Then
            '    rs.MoveFirst()
            '    Do While Not rs.EOF

            '        'Child_fid = rs.Fields.Item("G3E_FID").Value
            '        'Child_fno = rs.Fields.Item("G3E_FNO").Value
            '        'If sNewFID <> sOldFID Then
            '        'PopulateGrid(Child_fid.ToString, Child_fno.ToString, iRow)
            '        'bValid = False
            '        'End If
            '        rs.MoveNext()
            '    Loop
            'End If

        Catch ex As Exception
            MsgBox("Populate Job State:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try

    End Sub

    Private Sub PopulateGridJob(ByVal sPPWO As String, ByRef iRow As Integer)
        Dim iCol As Integer
        Dim i As Integer
        Dim sSQL As String
        Dim sFNO As String
        Dim dgvCbCol As New DataGridViewCheckBoxColumn()
        Try
            'Vinod 01-Nov-2012
            dgvFeature.Rows.Clear()
            dgvFeature.Columns.Clear()

            sSQL = "SELECT A.G3E_FNO, A.G3E_FID, B.G3E_USERNAME ""Feature_Type"", A.G3E_FID ""Feature_ID"", A.FEATURE_STATE ""Current_Feature_State"", A.SAP_WRK_ID, A.OWNERSHIP FROM GC_NETELEM A, G3E_FEATURE B WHERE A.JOB_ID = '" & sJobID & "' AND A.G3E_FNO = B.G3E_FNO"
            rsAttributeQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            iCol = rsAttributeQuery.Fields.Count - 2
            If Not (rsAttributeQuery.EOF And rsAttributeQuery.BOF) Then

                If sMasterPPWO.Length > 0 Then
                    bValid = True
                End If
                Do While Not rsAttributeQuery.EOF

                    sFNO = rsAttributeQuery.Fields.Item(0).Value
                    If iRow = 0 Then
                        With dgvCbCol
                            .HeaderText = ""
                            .Name = "CBColFibre"
                            .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                            .FlatStyle = FlatStyle.Standard
                            .CellTemplate = New DataGridViewCheckBoxCell()
                        End With

                        dgvFeature.Columns.Add(dgvCbCol)

                        For i = 1 To iCol ' Display Grid Header
                            dgvFeature.Columns.Add(rsAttributeQuery.Fields.Item(i - 1).Name, _
                            Replace(rsAttributeQuery.Fields.Item(i - 1).Name, "_", " "))

                            If i < iFeatureIDCol - 1 Then
                                dgvFeature.Columns(i).Visible = False
                            End If
                        Next

                        dgvFeature.Rows.Add()
                        dgvFeature.Columns.Add("NewFeatState", "New Feature State")
                        For i = 1 To iCol

                            dgvFeature.Item(i, iRow).Value = rsAttributeQuery.Fields.Item(i - 1).Value

                            dgvFeature.Item(i, iRow).ReadOnly = True
                            If Not bValid Then
                                dgvFeature.Item(i, iRow).Style.ForeColor = Drawing.Color.Gray
                                dgvFeature.Item(0, iRow).ReadOnly = True
                            End If
                        Next
                        If bValid Then
                            PopulateCombo(lblJobState.Text, dgvFeature.Item("Current_Feature_State", iRow).Value.ToString, dgvFeature, iRow, iCol + 1, sFNO)
                        Else
                            dgvFeature.Item(iCol + 1, iRow).ReadOnly = True
                        End If
                    Else
                        dgvFeature.Rows.Insert(iRow, 1)
                        For i = 1 To iCol

                            dgvFeature.Item(i, iRow).Value = rsAttributeQuery.Fields.Item(i - 1).Value

                            dgvFeature.Item(i, iRow).ReadOnly = True
                            If Not bValid Then
                                dgvFeature.Item(i, iRow).Style.ForeColor = Drawing.Color.Gray
                            End If
                        Next
                        If bValid Then
                            PopulateCombo(lblJobState.Text, dgvFeature.Item("Current_Feature_State", iRow).Value.ToString, dgvFeature, iRow, iCol + 1, sFNO)
                        Else
                            dgvFeature.Item(iCol + 1, iRow).ReadOnly = True
                            dgvFeature.Item(0, iRow).ReadOnly = True
                        End If
                    End If
                    iRow = iRow + 1
                    rsAttributeQuery.MoveNext()
                Loop
            End If


        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub

    Private Sub PopulateSelected(ByRef oDDCKeyObjects As IGTDDCKeyObjects)
        Dim sTemp As String
        Dim sOldFID As Integer
        Dim sNewFID As Integer
        Dim oDDCKeyObject As IGTDDCKeyObject
        Dim iRow As Integer
        Dim Owner_fid As String
        Dim sSQL As String
        Dim rs As ADODB.Recordset
        Dim Child_fid As Integer
        Dim Child_fno As Integer

        sTemp = ""
        sOldFID = 0
        iRow = 0
        bFeatureSelected = True

        'Vinod 01-Nov-2012
        dgvFeature.Rows.Clear()
        dgvFeature.Columns.Clear()

        For Each oDDCKeyObject In oDDCKeyObjects
            sNewFID = oDDCKeyObject.FID

            If sNewFID <> sOldFID Then
                PopulateGrid(sNewFID, oDDCKeyObject.FNO.ToString, iRow)
                'atiqah's addition---------------
                If oDDCKeyObject.FNO = 5100 Or oDDCKeyObject.FNO = 5600 Then
                    Owner_fid = Get_Value("select g3e_id from gc_ownership where g3e_fno = " + oDDCKeyObject.FNO.ToString + " and g3e_fid = " + oDDCKeyObject.FID.ToString).ToString
                    sSQL = "SELECT DISTINCT G3E_FID, G3E_FNO FROM gc_ownership where g3e_fno = 12300 and owner1_id = " + Owner_fid
                    rs = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
                    If Not (rs.EOF) Then
                        rs.MoveFirst()
                        Do While Not rs.EOF

                            Child_fid = rs.Fields.Item("G3E_FID").Value
                            Child_fno = rs.Fields.Item("G3E_FNO").Value
                            If sNewFID <> sOldFID Then
                                PopulateGrid(Child_fid.ToString, Child_fno.ToString, iRow)
                                bValid = False
                            End If
                            rs.MoveNext()
                        Loop
                    End If
                End If
                '--------------------
                sOldFID = sNewFID
                bValid = False
            End If

        Next

        If dgvFeature.Rows.Count = 0 Then
            MsgBox("Selected Features are not valid for update of Posting Status Attribute", vbOKOnly + vbExclamation, "NEPS")
            EndProgram()
        End If
    End Sub

    Private Sub PopulateGrid(ByVal sFID As String, ByVal sFNO As String, ByRef iRow As Integer)
        Dim iCol As Integer
        Dim i As Integer
        Dim sSQL As String
        Dim dgvCbCol As New DataGridViewCheckBoxColumn()
        Try

            sSQL = "SELECT A.G3E_FNO, A.G3E_FID, B.G3E_USERNAME ""Feature_Type"", A.G3E_FID ""Feature_ID"", A.FEATURE_STATE ""Current_Feature_State"", A.SAP_WRK_ID, A.OWNERSHIP FROM GC_NETELEM A, G3E_FEATURE B WHERE A.G3E_FID = " & sFID & " AND A.G3E_FNO = B.G3E_FNO"
            rsAttributeQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            iCol = rsAttributeQuery.Fields.Count - 2

            If Not (rsAttributeQuery.EOF And rsAttributeQuery.BOF) Then

                If sMasterPPWO.Length > 0 Then
                    bValid = True
                End If

                If iRow = 0 Then
                    With dgvCbCol
                        .HeaderText = ""
                        .Name = "CBColFibre"
                        .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                        .FlatStyle = FlatStyle.Standard
                        .CellTemplate = New DataGridViewCheckBoxCell()
                    End With

                    dgvFeature.Columns.Add(dgvCbCol)

                    For i = 1 To iCol ' Display Grid Header
                        dgvFeature.Columns.Add(rsAttributeQuery.Fields.Item(i - 1).Name, _
                        Replace(rsAttributeQuery.Fields.Item(i - 1).Name, "_", " "))

                        If i < iFeatureIDCol - 1 Then
                            dgvFeature.Columns(i).Visible = False
                        End If
                    Next

                    dgvFeature.Rows.Add()
                    dgvFeature.Columns.Add("NewFeatState", "New Feature State")
                    For i = 1 To iCol

                        dgvFeature.Item(i, iRow).Value = rsAttributeQuery.Fields.Item(i - 1).Value

                        dgvFeature.Item(i, iRow).ReadOnly = True
                        If Not bValid Then
                            dgvFeature.Item(i, iRow).Style.ForeColor = Drawing.Color.Gray
                            dgvFeature.Item(0, iRow).ReadOnly = True
                        End If
                    Next

                    If bValid Then
                       PopulateCombo(lblJobState.Text, dgvFeature.Item("Current_Feature_State", iRow).Value.ToString, dgvFeature, iRow, iCol + 1, sFNO)
                    Else
                        dgvFeature.Item(iCol + 1, iRow).ReadOnly = True
                    End If

                Else
                    dgvFeature.Rows.Insert(iRow, 1)
                    For i = 1 To iCol

                        dgvFeature.Item(i, iRow).Value = rsAttributeQuery.Fields.Item(i - 1).Value

                        dgvFeature.Item(i, iRow).ReadOnly = True
                        If Not bValid Then
                            dgvFeature.Item(i, iRow).Style.ForeColor = Drawing.Color.Gray
                        End If
                    Next
                    If bValid Then
                        PopulateCombo(lblJobState.Text, dgvFeature.Item("Current_Feature_State", iRow).Value.ToString, dgvFeature, iRow, iCol + 1, sFNO)
                    Else
                        dgvFeature.Item(iCol + 1, iRow).ReadOnly = True
                        dgvFeature.Item(0, iRow).ReadOnly = True
                    End If
                End If

                iRow = iRow + 1
            End If
        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub

    Private Sub PopulateCombo(ByVal sJobState As String, ByVal sCurFeatState As String, ByRef dgvFeature As System.Windows.Forms.DataGridView, ByVal iRow As Integer, ByVal iCol As Integer, ByVal sFNO As String)
        Dim sSQL As String
        Dim rsAttributeQuery As ADODB.Recordset
        Dim oDGVcmb As New System.Windows.Forms.DataGridViewComboBoxCell()
        Dim oDGVText As New System.Windows.Forms.DataGridViewTextBoxCell()
        Dim JOB_ID As String
        Try

            'Vinod 06-Nov-2012 If Mig_Job and PPF display the new state as PPF
            JOB_ID = ""
            sSQL = "SELECT JOB_ID from GC_NETELEM where G3E_FID = " & dgvFeature.Rows(iRow).Cells(2).Value.ToString()
            rsAttributeQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            If Not (rsAttributeQuery.EOF And rsAttributeQuery.BOF) Then

                JOB_ID = rsAttributeQuery.Fields.Item("JOB_ID").Value.ToString
            End If

            If JOB_ID = "Mig_Job" And dgvFeature.Rows(iRow).Cells(5).Value.ToString() = "PPF" Then
                oDGVcmb.Items.Add("PPF")
            End If

            sSQL = "SELECT NEPS_NEW_FEATURESTATE, NEPS_FILTER FROM NEPS_STATECHANGE WHERE NEPS_JOBSTATE = '" & sJobState & "' AND NEPS_CUR_FEATURESTATE = '" & sCurFeatState & "'"
            rsAttributeQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            If Not (rsAttributeQuery.EOF And rsAttributeQuery.BOF) Then
                rsAttributeQuery.MoveFirst()

                Do While Not rsAttributeQuery.EOF
                    If InStr(rsAttributeQuery.Fields("NEPS_FILTER").Value.ToString, sFNO, CompareMethod.Text) > 0 Then
                        oDGVcmb.Items.Add(rsAttributeQuery.Fields.Item("NEPS_NEW_FEATURESTATE").Value)
                    End If
                    rsAttributeQuery.MoveNext()
                Loop

                If oDGVcmb.Items.Count > 0 Then
                    If oDGVcmb.Items.Count = 1 Then
                        oDGVText.Value = oDGVcmb.Items.Item(0)
                        dgvFeature.Rows(iRow).Cells(iCol) = oDGVText
                        dgvFeature.Rows(iRow).Cells(iCol).ReadOnly = True
                    Else
                        oDGVcmb.Value = oDGVcmb.Items.Item(0)
                        dgvFeature.Rows(iRow).Cells(iCol) = oDGVcmb
                    End If
                End If

            Else 'Vinod 02-Nov-2012
                dgvFeature.Rows(iRow).Cells(iCol).ReadOnly = True
                dgvFeature.Rows(iRow).Cells(iCol).Value = ""
            End If
        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub

    Private Sub PopulateMINCombo(ByVal sMinTable As String, ByVal sMinColumn As String, ByVal iRow As Integer, ByVal iCol As Integer, ByVal sCurrentItem As String, Optional ByVal sFilterVal As String = "", Optional ByVal sPLFilter As String = "")
        Dim sSQL As String
        Dim rsAttributeQuery As ADODB.Recordset
        Dim oDGVcmb As New System.Windows.Forms.DataGridViewComboBoxCell()
        Try
            If sPLFilter.Length > 0 And sFilterVal.Length > 0 Then
                sSQL = "SELECT " & sMinColumn & " FROM " & sMinTable & " WHERE " & sPLFilter & " = '" & sFilterVal & "'"
            Else
                sSQL = "SELECT " & sMinColumn & " FROM " & sMinTable
            End If

            oDGVcmb.Items.Clear()
            oDGVcmb.DropDownWidth = 100
            rsAttributeQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            If Not (rsAttributeQuery.EOF And rsAttributeQuery.BOF) Then
                rsAttributeQuery.MoveFirst()
                Do While Not rsAttributeQuery.EOF
                    oDGVcmb.Items.Add(rsAttributeQuery.Fields.Item(sMinColumn).Value.ToString)
                    rsAttributeQuery.MoveNext()
                Loop
            End If

            sSQL = "SELECT " & sMinColumn & " FROM " & sMinTable & " WHERE MIN_MATERIAL = '" & sCurrentItem & "'"
            rsAttributeQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            If Not (rsAttributeQuery.EOF And rsAttributeQuery.BOF) Then
                rsAttributeQuery.MoveFirst()
                oDGVcmb.Value = rsAttributeQuery.Fields.Item(sMinColumn).Value.ToString
            End If
            dgvFeature.Rows(iRow).Cells(iCol) = oDGVcmb
        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Dim i As Integer
        Dim bUpdate As Boolean
        Dim sPPXFeatFNO As String
        Dim sPPXFeatFID As String
        Dim sCheck As Boolean
        Dim sCheckID As String
        Dim FNO As String
        Dim FID As String
        Dim iSelected As Boolean
        'Anna's addition
        Dim CivilUpdate As New NEPS.GTechnology.NEPSCivilChangeFeatureState.GTChangeCivilFeatureState
        '----------------

        Try
            'Vinod06-Nov-2012
            sCheck = False
            sCheckID = ""
            iSelected = False
            For i = 0 To dgvFeature.Rows.Count - 1
                If dgvFeature.Item(0, i).Value = True Then
                    If dgvFeature.Item("NewFeatState", i).Value.ToString = "" Then
                        If sCheckID = "" Then
                            sCheckID = dgvFeature.Item("G3E_FID", i).Value.ToString
                        Else
                            sCheckID = "," + dgvFeature.Item("G3E_FID", i).Value.ToString
                        End If
                        sCheck = True
                    End If
                    iSelected = True
                End If
            Next

            If (iSelected = False And dgvFeature.Rows.Count > 0) Then
                MsgBox("Please select the rows to Update", vbOKOnly + vbInformation, "NEPS")
                Return
            End If

            If (sCheck = True) Then
                MsgBox("There is No State to Transition To." + vbNewLine + "FID=" + sCheckID, vbOKOnly + vbInformation, "NEPS")
                Return
            End If

            GTClassFactory.Create(Of IGTApplication)().BeginWaitCursor()
            bUpdate = False
            m_oTransactionManager.Begin("UpdateFeature")
            sPPXFeatFNO = ""
            sPPXFeatFID = ""
            iFeatCount = 0
            For i = 0 To dgvFeature.Rows.Count - 1
                If dgvFeature.Item(0, i).Value = True Then
                    FNO = dgvFeature.Item("G3E_FNO", i).Value.ToString
                    FID = dgvFeature.Item("G3E_FID", i).Value.ToString
                    'Anna's addition
                    If Not CivilUpdate.CivilToBeUpdate(FNO, dgvFeature.Item("Current_Feature_State", i).Value.ToString) Then
                        '----------------
                        If Not dgvFeature.Item("NewFeatState", i).Value Is Nothing Then
                            UpdateRecord(dgvFeature.Item("G3E_FNO", i).Value.ToString, dgvFeature.Item("G3E_FID", i).Value.ToString, i, "", False)
                            bUpdate = True
                            If bPPX Then
                                sPPXFeatFNO = sPPXFeatFNO & dgvFeature.Item("G3E_FNO", i).Value.ToString & "|"
                                sPPXFeatFID = sPPXFeatFID & dgvFeature.Item("G3E_FID", i).Value.ToString & "|"
                                iFeatCount = iFeatCount + 1
                            End If
                        End If
                        'Anna's addition
                    Else
                        If Not CivilUpdate.ChangeFeatureState(FNO, FID, dgvFeature.Item("Current_Feature_State", i).Value.ToString, GTClassFactory.Create(Of IGTApplication)().DataContext) Then
                            MsgBox("Error during updating Feature with ID=" + FID, vbOKOnly + vbCritical, "NEPS")
                            m_oTransactionManager.Rollback()
                            EndProgram()
                        Else
                            bUpdate = True
                            If bPPX Then
                                sPPXFeatFNO = sPPXFeatFNO & dgvFeature.Item("G3E_FNO", i).Value.ToString & "|"
                                sPPXFeatFID = sPPXFeatFID & dgvFeature.Item("G3E_FID", i).Value.ToString & "|"
                                iFeatCount = iFeatCount + 1
                            End If
                        End If
                    End If
                    '---------------------

                End If
            Next
            m_oTransactionManager.Commit()

            GTClassFactory.Create(Of IGTApplication)().EndWaitCursor()

            If bUpdate Then
                MsgBox("Records are updated successfully", vbOKOnly + vbExclamation, "NEPS")

                If lblPPWO.Text <> "" And cb_JobID.Checked = True Then
                    PopulateGridJob(lblPPWO.Text, 0)
                    PopulateJobState(bFinalPost, bDistribution)
                Else
                    PopulateDataGrid(bFinalPost, bDistribution)
                    PopulateJobState(bFinalPost, bDistribution)
                End If

            End If


        Catch ex As Exception
            MsgBox("btnOK_Click:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            m_oTransactionManager.Rollback()
            EndProgram()
        End Try
    End Sub

    Private Sub PlaceSlash(ByVal sFNO As String, ByVal sFID As String)
        Dim bDetail As Boolean
        Dim iDetailID As Integer
        Dim iObjectDetID As Integer
        Dim oMapWindows As IGTMapWindows
        Dim oMapWindow As IGTMapWindow
        Dim rsDetLeg As ADODB.Recordset
        Dim bOpen As Boolean
        Try
            bPlaced2ndSlash = False
            If Not oDDCKeyObjects Is Nothing Then
                oDDCKeyObjects.Remove(oDDCKeyObjects.Item(0))
            End If
            bDetail = False
            bOpen = False
            oDDCKeyObjects = GTClassFactory.Create(Of IGTApplication)().DataContext.GetDDCKeyObjects(sFNO, sFID, GTComponentGeometryConstants.gtddcgPrimaryGeographic)
            If oDDCKeyObjects.Count = 0 Then 'Should be detail feature
                oDDCKeyObjects = GTClassFactory.Create(Of IGTApplication)().DataContext.GetDDCKeyObjects(sFNO, sFID, GTComponentGeometryConstants.gtddcgPrimaryDetail)
                bDetail = True
            End If

            If bDetail Then
                iDetailID = GTClassFactory.Create(Of IGTApplication)().ActiveMapWindow.DetailID

                iObjectDetID = oDDCKeyObjects.Item(0).Recordset.Fields("G3E_DETAILID").Value

                If Not iDetailID = iObjectDetID Then
                    oMapWindows = GTClassFactory.Create(Of IGTApplication)().GetMapWindows(GTMapWindowTypeConstants.gtapmtDetail)
                    For Each oMapWindow In oMapWindows
                        If oMapWindow.DetailID = iObjectDetID Then
                            oMapWindow.Activate()
                            bOpen = True
                        End If
                    Next
                    If bOpen = False Then
                        GTClassFactory.Create(Of IGTApplication)().OpenDetailWindow("Detail Fibre", iObjectDetID)
                    End If
                End If
            Else
                oMapWindows = GTClassFactory.Create(Of IGTApplication)().GetMapWindows(GTMapWindowTypeConstants.gtapmtGeographic)
                If oMapWindows.Count > 0 Then
                    oMapWindows.Item(0).Activate()
                Else
                    GTClassFactory.Create(Of IGTApplication)().NewMapWindow("Geobase Fibre")
                End If
            End If

            GTClassFactory.Create(Of IGTApplication)().SelectedObjects.Clear()
            GTClassFactory.Create(Of IGTApplication)().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObjects.Item(0))
            GTClassFactory.Create(Of IGTApplication)().ActiveMapWindow.FitSelectedObjects(iZoomFactor)
            m_oTransactionManager.Begin("Add Slash")
            oFeat = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenFeature(sFNO, sFID)
            If bDetail Then
                oFeatPlaceService.StartComponent(oFeat, iSlashCNODet)
            Else
                oFeatPlaceService.StartComponent(oFeat, iSlashCNO)
            End If
            GTClassFactory.Create(Of IGTApplication)().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to place the slash marks for the selected PPX feature or press Esc to cancel placement")
        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub

    Private Sub oFeatPlaceService_Finished(ByVal sender As Object, ByVal e As Intergraph.GTechnology.API.GTFinishedEventArgs) Handles oFeatPlaceService.Finished
        Dim sGeometry As String
        Try
            sGeometry = oDDCKeyObjects.Item(0).Geometry.Type
            If iFeatCount = 1 Then
                If sGeometry = "LineGeometry" Or sGeometry = "PolylineGeometry" Or sGeometry = "CompositePolylineGeometry" Then
                    If Not bPlaced2ndSlash Then
                        Place2ndSlash()
                    Else
                        SaveAddSlash()
                    End If
                ElseIf sGeometry = "OrientedPointGeometry" Or sGeometry = "PointGeometry" Then
                    SaveAddSlash()
                End If
            ElseIf iFeatCount > 1 Then
                If sGeometry = "LineGeometry" Or sGeometry = "PolylineGeometry" Or sGeometry = "CompositePolylineGeometry" Then
                    If Not bPlaced2ndSlash Then
                        Place2ndSlash()
                    Else
                        iPlacedCount = iPlacedCount + 1
                        If iFeatCount >= iPlacedCount Then
                            m_oTransactionManager.Commit()
                            PlaceSlash(aPPXFeatFNO(iPlacedCount - 1), aPPXFeatFID(iPlacedCount - 1))
                        Else
                            SaveAddSlash()
                        End If
                    End If
                ElseIf sGeometry = "OrientedPointGeometry" Or sGeometry = "PointGeometry" Then
                    iPlacedCount = iPlacedCount + 1
                    If iFeatCount >= iPlacedCount Then
                        m_oTransactionManager.Commit()
                        PlaceSlash(aPPXFeatFNO(iPlacedCount - 1), aPPXFeatFID(iPlacedCount - 1))
                    Else
                        SaveAddSlash()
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub

    Private Sub Place2ndSlash()
        bPlaced2ndSlash = True
        oFeatPlaceService.StartComponent(oFeat, iSlashCNO)
        GTClassFactory.Create(Of IGTApplication)().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to place the slash marks for the selected PPX feature or press Esc to cancel placement")
    End Sub

    Public Sub SaveAddSlash()
        m_oTransactionManager.Commit()
        oFeatPlaceService.Dispose()
        oFeatPlaceService = Nothing
        GTClassFactory.Create(Of IGTApplication)().SelectedObjects.Clear()
        oDDCKeyObjects.Remove(oDDCKeyObjects.Item(0))
        oDDCKeyObjects = Nothing
        EndProgram()
    End Sub

    Private Sub UpdateRecord(ByVal sFNO As String, ByVal sFID As String, ByVal iRow As Integer, ByVal sMinValue As String, ByVal bCable As Boolean)
        Dim sCNO() As String
        Dim iCNO() As Short
        Dim sColumn() As String
        Dim sValue() As Object
        Dim j As Integer


        GetUpdateValues(sFNO, sCNO, sColumn, sValue, bFinalPost, iRow, bCable)
        ReDim iCNO(sCNO.GetLength(0) - 1)
        For j = 0 To sCNO.GetLength(0) - 1
            iCNO(j) = System.Convert.ToInt16(sCNO(j))
        Next

        UpdateRecord(sFNO, sFID, iCNO, sColumn, sValue)
    End Sub

    Private Sub GetUpdateValues(ByVal sFNO As String, ByRef sCNO() As String, ByRef sColumn() As String, ByRef sValue() As Object, ByVal bFinalPost As Boolean, ByVal iRow As Integer, ByVal bCable As Boolean, Optional ByVal sMinMaterialDesc As String = "")
        Dim sSQL As String
        Dim rsAttributeQuery As ADODB.Recordset
        Dim oField As ADODB.Field
        Dim i As Integer
        Dim currentTime As System.DateTime
        Dim sCurrentTime As String
        Try
            
            sCNO = Split(51.ToString, "|", , CompareMethod.Text)
            sColumn = Split("FEATURE_STATE", "|", , CompareMethod.Text)

            currentTime = System.DateTime.Now
            sCurrentTime = Mid(currentTime.Year.ToString, 3)
            If currentTime.Month.ToString().Length = 1 Then
                sCurrentTime = sCurrentTime & "0" & currentTime.Month.ToString()
            Else
                sCurrentTime = sCurrentTime & currentTime.Month.ToString()
            End If

            ReDim sValue(sCNO.GetLength(0) - 1)
            sValue(0) = dgvFeature.Item("NewFeatState", iRow).Value
           
        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub


    Private Sub UpdateRecord(ByVal sFNO As Short, ByVal sFID As Integer, ByRef iCNO As Short(), ByRef sColumn As String(), ByRef sValue As Object())
        Dim oFeat1 As IGTKeyObject
        Dim i As Integer

        Try

            oFeat1 = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenFeature(sFNO, sFID)
            For i = 0 To iCNO.Length - 1
                oFeat1.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", sValue(i))
                'Vinod 02-Nov-2012
                'oFeat1.Components.GetComponent(51).Recordset.Update("JOB_ID", GTClassFactory.Create(Of IGTApplication)().DataContext.ActiveJob())
                'Vinod 12-Nov-2012
                oFeat1.Components.GetComponent(51).Recordset.Update("JOB_STATE", lblJobState.Text)
            Next

            If sFNO = 2200 Then
                Call ChangeFormationState(sFID, sValue(0))
            End If

        Catch ex As Exception
            MsgBox("UpdateRecord:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try

    End Sub

    Private Sub ChangeFormationState(ByVal Pipe_FID As Integer, ByVal State As String)
        Dim sSQL As String
        Dim rs As ADODB.Recordset
        Dim oFeatForm As IGTKeyObject
        Dim oFeatDuct As IGTKeyObject

        sSQL = "SELECT G3E_FID,G3E_FNO FROM GC_CONTAIN WHERE G3E_OWNERFID =" & Pipe_FID
        rs = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
        If Not (rs.EOF And rs.BOF) Then
            rs.MoveFirst()
            Do While Not rs.EOF
                oFeatForm = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenFeature(rs.Fields.Item("G3E_FNO").Value, rs.Fields.Item("G3E_FID").Value)
                oFeatForm.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", State)
                rs.MoveNext()

            Loop
        End If

        sSQL = "SELECT G3E_FID, G3E_FNO FROM GC_CONTAIN WHERE G3E_OWNERFID IN ( SELECT G3E_FID FROM GC_CONTAIN WHERE G3E_OWNERFID =" & Pipe_FID & ")"
        rs = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
        If Not (rs.EOF And rs.BOF) Then
            rs.MoveFirst()
            Do While Not rs.EOF
                oFeatDuct = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenFeature(rs.Fields.Item("G3E_FNO").Value, rs.Fields.Item("G3E_FID").Value)
                oFeatDuct.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", State)
                rs.MoveNext()

            Loop
        End If


    End Sub

    Public Sub UpdateAllOthers() 'method to update all other features which are placed in this ppwo but doesn't appear in the dialog
        Dim sSQL As String
        Dim rsQuery As ADODB.Recordset
        Dim sCNO() As String
        Dim iCNO() As Short
        Dim sColumn() As String
        Dim sValue() As Object
        Try
            sPostedID = Mid(sPostedID, 1, sPostedID.Length - 1)
            sSQL = "SELECT G3E_FNO, G3E_FID FROM GC_NETELEM WHERE JOB_ID = '" & sJobID & _
                "' AND G3E_FID NOT IN (" & sPostedID & ")"

            rsQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            If Not (rsQuery.EOF And rsQuery.BOF) Then
                rsQuery.MoveFirst()
                Do While Not rsQuery.EOF
                    GetUpdateValues("0", sCNO, sColumn, sValue, True, 0, False)
                    rsQuery.MoveNext()
                Loop
            End If
        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub

    Public Sub EndProgram()
        Try
            GTClassFactory.Create(Of IGTApplication)().EndWaitCursor()
            Me.Close()
            Me.Dispose()
            If Not (m_oCustomCommandHelper Is Nothing) Then
                m_oCustomCommandHelper.Complete()
            End If
        Catch ex As Exception
            MsgBox("EndProgram:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
        End Try
    End Sub


    Private Sub PopulateID(ByVal sFNO As String, ByVal sFID As String, ByVal iRow As Integer, ByVal iCol As Integer, Optional ByVal sAttrFilter As String = "", Optional ByRef sAttrVal As String = "")
        Dim sAttrTable As String
        Dim sAttrCol As String
        Dim sSQL As String
        Dim rsQuery As ADODB.Recordset
        Dim rsQuery2 As ADODB.Recordset
        Try
            'sSQL = "SELECT DISTINCT NEPS_TABLE, NEPS_ATTRIBUTE FROM NEPS_FEATURE_ATTRIBUTE WHERE NEPS_FNO = " & sFNO
            'rsQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            'If Not (rsQuery.EOF And rsQuery.BOF) Then
            '    rsQuery.MoveFirst()
            '    sAttrTable = rsQuery.Fields.Item("NEPS_TABLE").Value.ToString
            '    sAttrCol = rsQuery.Fields.Item("NEPS_ATTRIBUTE").Value.ToString

            '    If sAttrFilter.Length > 0 Then
            '        sSQL = "SELECT DISTINCT " & sAttrCol & ", " & sAttrFilter & " FROM " & sAttrTable & " WHERE G3E_FID = " & sFID
            '    Else
            '        sSQL = "SELECT DISTINCT " & sAttrCol & " FROM " & sAttrTable & " WHERE G3E_FID = " & sFID
            '    End If
            '    rsQuery2 = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            '    If Not (rsQuery2.EOF And rsQuery2.BOF) Then
            '        rsQuery2.MoveFirst()
            '        dgvFeature.Item(iCol, iRow).Value = rsQuery2.Fields.Item(sAttrCol).Value.ToString
            '        If sAttrFilter.Length > 0 Then
            '            sAttrVal = rsQuery2.Fields.Item(sAttrFilter).Value.ToString
            '        End If
            '    End If
            'End If

            'rsQuery = Nothing
            'rsQuery2 = Nothing
        Catch ex As Exception
            MsgBox("Error:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            EndProgram()
        End Try
    End Sub

    Private Sub FocusSelected(ByVal iFNO As Short, ByVal iFID As Integer)
        Dim oDDCKeyObjects As IGTDDCKeyObjects
        Dim i As Integer
        Try
            'GTClassFactory.Create(Of IGTApplication)().SelectedObjects.Clear()
            'oDDCKeyObjects = GTClassFactory.Create(Of IGTApplication)().DataContext.GetDDCKeyObjects(iFNO, iFID, GTComponentGeometryConstants.gtddcgPrimaryGeographic)
            'If oDDCKeyObjects.Count = 0 Then 'Should be detail feature or No Graphics
            '    'oDDCKeyObjects = GTClassFactory.Create(Of IGTApplication)().DataContext.GetDDCKeyObjects(iFNO, iFID, GTComponentGeometryConstants.gtddcgPrimaryDetail)
            '    MsgBox("Selected feature is in a Detail Window. Please open the Detail Window to view it Or No Graphics Feature.", vbOKOnly + vbExclamation, "NEPS")
            'Else
            '    GTClassFactory.Create(Of IGTApplication)().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObjects(0))
            '    GTClassFactory.Create(Of IGTApplication)().ActiveMapWindow.FitSelectedObjects(iZoomFactor)
            '    GTClassFactory.Create(Of IGTApplication)().RefreshWindows()
            'End If

            'oDDCKeyObjects = Nothing

            GTClassFactory.Create(Of IGTApplication)().SelectedObjects.Clear()
            oDDCKeyObjects = GTClassFactory.Create(Of IGTApplication)().DataContext.GetDDCKeyObjects(iFNO, iFID, GTComponentGeometryConstants.gtddcgAllGeographic)

            If oDDCKeyObjects.Count = 0 Then 'Should be detail feature or No Graphics
                MsgBox("Non Graphic Feature or Detail Window Feature. Please open the Detail Window to view it.", vbOKOnly + vbExclamation, "NEPS")
                Return
            End If

            For i = 0 To oDDCKeyObjects.Count - 1
                GTClassFactory.Create(Of IGTApplication)().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObjects(i))
            Next

            GTClassFactory.Create(Of IGTApplication)().ActiveMapWindow.FitSelectedObjects(iZoomFactor)
            'GTClassFactory.Create(Of IGTApplication)().ActiveMapWindow.DisplayScale = 500
            GTClassFactory.Create(Of IGTApplication)().RefreshWindows()
            oDDCKeyObjects = Nothing

        Catch ex As Exception
            'MsgBox("FocusSelected:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            'EndProgram()
        End Try

    End Sub

    Private Sub dgvFeature_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvFeature.CellClick
        Dim iRow As Integer
        Dim iCol As Integer

        If chk_highlight.Checked = True Then
            iRow = dgvFeature.SelectedCells(0).RowIndex
            iCol = iFeatureIDCol 'dgvFeature.SelectedCells(0).ColumnIndex
            'If iCol = iFeatureIDCol Then
            If iRow > 0 Then
                FocusSelected(dgvFeature.Item(iCol - 3, iRow).Value, dgvFeature.Item(iCol - 2, iRow).Value)
            End If
        End If

    End Sub

    Private Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectAll.Click
        Dim i As Integer

        For i = 0 To dgvFeature.Rows.Count - 1
            If Not dgvFeature.Item(0, i).ReadOnly Then
                dgvFeature.Item(0, i).Value = True
            End If
        Next

    End Sub

    Private Sub btnUnselectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnselectAll.Click
        Dim i As Integer

        For i = 0 To dgvFeature.Rows.Count - 1
            If Not dgvFeature.Item(0, i).ReadOnly Then
                dgvFeature.Item(0, i).Value = False
            End If
        Next

    End Sub

    'Private Sub cmdSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim sSQL As String
    '    Dim rsQuery As ADODB.Recordset
    '    Dim iRow As Integer

    '    Try
    '        If txtPPWO.Text.Length > 0 Then
    '            If dgvFeature.RowCount = 0 Then
    '                GTClassFactory.Create(Of IGTApplication)().BeginWaitCursor()
    '                sSQL = "SELECT G3E_FNO, G3E_FID FROM GC_NETELEM WHERE SCHEME_NAME = '" & txtPPWO.Text & "'"
    '                rsQuery = DataCon.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
    '                If Not (rsQuery.EOF And rsQuery.BOF) Then
    '                    rsQuery.MoveFirst()
    '                    iRow = 0
    '                    Do While Not rsQuery.EOF
    '                        PopulateGrid(rsQuery.Fields.Item("G3E_FID").Value.ToString, rsQuery.Fields.Item("G3E_FNO").Value.ToString, iRow)
    '                        rsQuery.MoveNext()
    '                    Loop
    '                    GTClassFactory.Create(Of IGTApplication)().EndWaitCursor()
    '                Else
    '                    MsgBox("Scheme Name entered did not contain any features", vbOKOnly + vbExclamation, "NEPS")
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        GTClassFactory.Create(Of IGTApplication)().EndWaitCursor()
    '        MsgBox("cmdSearch_Click:" & vbCrLf & Err.Description & vbCrLf & "FNO: " & rsQuery.Fields.Item("G3E_FNO").Value.ToString & "FID: " & rsQuery.Fields.Item("G3E_FID").Value.ToString, vbOKOnly + vbExclamation, Err.Source)
    '    End Try
    'End Sub

    Private Sub dgvFeature_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvFeature.CellContentClick

    End Sub

    'Vinod 01-Nov-2012
    Private Sub cb_JobID_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_JobID.CheckedChanged
        If lblPPWO.Text <> "" And cb_JobID.Checked = True Then
            PopulateGridJob(lblPPWO.Text, 0)
        Else
            PopulateDataGrid(bFinalPost, bDistribution)
        End If
    End Sub

    Private Function Get_Value(ByVal sSql As String) As String
        Try
            Dim rsPP As New ADODB.Recordset()
            rsPP = GTClassFactory.Create(Of IGTApplication)().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1)
            If rsPP.RecordCount > 0 Then
                rsPP.MoveFirst()
                Return (rsPP.Fields(0).Value.ToString())
            End If
            Return Nothing
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Running Edit Common Attributes...", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Return Nothing
        End Try

    End Function

    Private Sub btnJob_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnJob.Click
        Dim EditJob As IGTApplication
        Dim ssql As String
        Dim rs As ADODB.Recordset
        Dim count As Int16

        count = 0
        ssql = "Update G3E_JOB SET JOB_STATE = '" + cmbJob.SelectedItem.ToString + "' WHERE G3E_IDENTIFIER = '" & sJobID & "'"
        EditJob = GTClassFactory.Create(Of IGTApplication)()
        rs = EditJob.DataContext.Execute(ssql, count, 0, Nothing)

        ssql = "SELECT * FROM G3E_JOB WHERE G3E_IDENTIFIER = '" & sJobID & "'"
        rsAttributeQuery = DataCon.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
        lblJobState.Text = rsAttributeQuery.Fields.Item("JOB_STATE").Value

        If lblPPWO.Text <> "" And cb_JobID.Checked = True Then
            PopulateGridJob(lblPPWO.Text, 0)
            PopulateJobState(bFinalPost, bDistribution)
        Else
            PopulateDataGrid(bFinalPost, bDistribution)
            PopulateJobState(bFinalPost, bDistribution)
        End If
        MsgBox("Job State Updated!")
    End Sub

   
End Class