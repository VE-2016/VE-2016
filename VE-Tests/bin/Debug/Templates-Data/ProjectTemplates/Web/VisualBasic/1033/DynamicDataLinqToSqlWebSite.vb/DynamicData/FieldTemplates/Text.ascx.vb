Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class TextField
    Inherits FieldTemplateUserControl

    Private Const MAX_DISPLAYLENGTH_IN_LIST As Integer = 25
    
    Public Overrides ReadOnly Property FieldValueString As String
        Get
            Dim value As String = MyBase.FieldValueString
            If ContainerType = ContainerType.List Then
                If value IsNot Nothing AndAlso value.Length > MAX_DISPLAYLENGTH_IN_LIST Then
                    value = (value.Substring(0, (MAX_DISPLAYLENGTH_IN_LIST - 3)) & "...")
                End If
            End If
            Return value
        End Get
    End Property
    
    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return Literal1
        End Get
    End Property
    
End Class
