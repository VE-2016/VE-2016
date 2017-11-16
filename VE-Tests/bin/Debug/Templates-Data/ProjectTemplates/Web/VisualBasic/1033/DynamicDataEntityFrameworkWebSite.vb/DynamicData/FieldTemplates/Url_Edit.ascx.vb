Imports System
Imports System.Collections.Specialized
Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Partial Public Class Url_EditField
    Inherits System.Web.DynamicData.FieldTemplateUserControl
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Column.MaxLength < 20 Then
            TextBox1.Columns = Column.MaxLength
        End If
        TextBox1.ToolTip = Column.Description

        SetUpValidator(RequiredFieldValidator1)
        SetUpValidator(RegularExpressionValidator1)
        SetUpValidator(DynamicValidator1)
    End Sub

    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        MyBase.OnDataBinding(e)
        If Column.MaxLength > 0 Then
            TextBox1.MaxLength = Math.Max(FieldValueEditString.Length, Column.MaxLength)
        End If
    End Sub

    Protected Overrides Sub ExtractValues(ByVal dictionary As IOrderedDictionary)
        dictionary(Column.Name) = ConvertEditedValue(TextBox1.Text)
    End Sub

    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return TextBox1
        End Get
    End Property
End Class