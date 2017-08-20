Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class ChildrenField
    Inherits FieldTemplateUserControl

    Public Property NavigateUrl As String
    
    Public Property AllowNavigation As Boolean = True
    
    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return HyperLink1
        End Get
    End Property
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        HyperLink1.Text = ("View " & ChildrenColumn.ChildTable.DisplayName)
    End Sub
    
    Protected Function GetChildrenPath() As String
        If Not AllowNavigation Then
            Return Nothing
        End If
        If String.IsNullOrEmpty(NavigateUrl) Then
            Return ChildrenPath
        Else
            Return BuildChildrenPath(NavigateUrl)
        End If
    End Function
    
End Class
