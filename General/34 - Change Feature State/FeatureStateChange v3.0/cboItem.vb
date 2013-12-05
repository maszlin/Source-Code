Imports System.Collections.Generic
Imports System.Text

    Class cboItem
        Private _value As String

        Private _name As String
        Public Property Value() As String

            Get
                Return _value
            End Get
            Set(ByVal value As String)
                _value = value
            End Set
        End Property

        Public Property Name() As String


            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Sub New(ByVal name As String, ByVal value As String)


            _name = name


            _value = value
        End Sub
        Public Overrides Function ToString() As String

            Return _name

        End Function
    End Class