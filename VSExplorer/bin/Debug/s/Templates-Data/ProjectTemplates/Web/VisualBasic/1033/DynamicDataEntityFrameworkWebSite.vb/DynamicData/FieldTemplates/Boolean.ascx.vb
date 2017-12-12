Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class BooleanField
    Inherits FieldTemplateUserControl

    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return CheckBox1
        End Get
    End Property
    
    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        MyBase.OnDataBinding(e)
        Dim val As Object = FieldValue
        If val IsNot Nothing Then
            CheckBox1.Checked = CType(val, Boolean)
        End If
    End Sub
    
End Class
