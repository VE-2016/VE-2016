Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class ForeignKeyField
    Inherits FieldTemplateUserControl

    Public Property NavigateUrl As String
    
    Public Property AllowNavigation As Boolean = True
    
    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return HyperLink1
        End Get
    End Property
    
    Protected Function GetDisplayString() As String
        Dim value As Object = FieldValue
        If (value Is Nothing) Then
            Return FormatFieldValue(ForeignKeyColumn.GetForeignKeyString(Row))
        Else
            Return FormatFieldValue(ForeignKeyColumn.ParentTable.GetDisplayString(value))
        End If
    End Function
    
    Protected Function GetNavigateUrl() As String
        If Not AllowNavigation Then
            Return Nothing
        End If
        If String.IsNullOrEmpty(NavigateUrl) Then
            Return ForeignKeyPath
        Else
            Return BuildForeignKeyPath(NavigateUrl)
        End If
    End Function
    
End Class
