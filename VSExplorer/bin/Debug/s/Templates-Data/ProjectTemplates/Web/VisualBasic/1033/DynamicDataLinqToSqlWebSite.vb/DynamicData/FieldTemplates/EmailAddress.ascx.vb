Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class EmailAddressField
    Inherits FieldTemplateUserControl

    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return HyperLink1
        End Get
    End Property
    
    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        Dim url As String = FieldValueString
        If Not url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) Then
            url = "mailto:" & url
        End If
        HyperLink1.NavigateUrl = url
    End Sub
    
End Class
