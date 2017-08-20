Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class UrlField
    Inherits FieldTemplateUserControl

    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return HyperLinkUrl
        End Get
    End Property
    
    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        HyperLinkUrl.NavigateUrl = ProcessUrl(FieldValueString)
    End Sub
    
    Private Function ProcessUrl(ByVal url As String) As String
        If (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) OrElse url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) Then
            Return url
        End If
        Return ("http://" & url)
    End Function
    
End Class
