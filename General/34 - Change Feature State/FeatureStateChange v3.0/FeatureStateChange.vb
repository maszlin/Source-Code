Imports System.Data.OleDB
Imports System
Imports System.Collections.Generic
Imports Intergraph.GTechnology.API
Imports Intergraph.GTechnology.Interfaces
Imports System.ComponentModel
Imports System.Data

Public Class WinWrapper
    Implements System.Windows.Forms.IWin32Window
    ''Overridable ReadOnly Property Handle() As System.IntPtr Implements System.Windows.Forms.IWin32Window.Handle
    ''    Get           
    ''        Dim iptr As New Object                 
    ''        iptr = CType(GTApplication.Application.hWnd, System.IntPtr)
    ''        Return iptr
    ''    End Get
    ''End Property


    Overridable ReadOnly Property Handle() As IntPtr Implements IWin32Window.Handle
        Get
            Dim iptr As New System.IntPtr()
            iptr = DirectCast(GTClassFactory.Create(Of IGTApplication)().hWnd, System.IntPtr)
            Return iptr
        End Get
    End Property

End Class

Public Class FeatureStateChange
    Implements IGTCustomCommandModeless
    Public WithEvents m_oCustomCommandHelper As IGTCustomCommandHelper
    Public frmChangeState As frmStateChange

    Public Sub Activate(ByVal CustomCommandHelper As IGTCustomCommandHelper) Implements IGTCustomCommandModeless.Activate
        Dim objWinWrapper As New WinWrapper

        Try
            m_oCustomCommandHelper = CustomCommandHelper
            frmChangeState = New frmStateChange
            frmChangeState.m_oCustomCommandHelper = CustomCommandHelper
            frmChangeState.Show(objWinWrapper)

        Catch ex As Exception
            MsgBox("IGTCustomCommandModal_Activate:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
            m_oCustomCommandHelper.Complete()
            m_oTransactionManager = Nothing
        End Try
    End Sub

    Public ReadOnly Property CanTerminate() As Boolean Implements IGTCustomCommandModeless.CanTerminate
        Get
            Return True
        End Get
    End Property

    Public Sub Pause() Implements IGTCustomCommandModeless.Pause

    End Sub

    Public Sub [Resume]() Implements IGTCustomCommandModeless.Resume

    End Sub

    Public Sub Terminate() Implements IGTCustomCommandModeless.Terminate
        Try
            m_oTransactionManager = Nothing
            m_oCustomCommandHelper = Nothing

        Catch ex As Exception
            MsgBox("IGTCustomCommandModal_Terminate:" & vbCrLf & Err.Description, vbOKOnly + vbExclamation, Err.Source)
        End Try

    End Sub

    Public Sub m_oCustomCommandHelper_Click(ByVal MapWindow As IGTMapWindow, ByVal Button As Long, ByVal Key As Long, ByVal WindowPoint As IGTPoint, ByVal WorldPoint As IGTPoint)
        MsgBox("Entered Click Event")
    End Sub

    Public WriteOnly Property TransactionManager() As IGTTransactionManager Implements IGTCustomCommandModeless.TransactionManager
        Set(ByVal value As IGTTransactionManager)
            m_oTransactionManager = value
        End Set
    End Property

    Private Sub m_oCustomCommandHelper_KeyUp(ByVal sender As Object, ByVal e As Intergraph.GTechnology.API.GTKeyEventArgs) Handles m_oCustomCommandHelper.KeyUp
        If e.KeyCode = System.Windows.Forms.Keys.Escape Then
            'frmChangeState.oFeatPlaceService.CancelPlacement()
            'frmChangeState.SaveAddSlash()
        End If
    End Sub

End Class
