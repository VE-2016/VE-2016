Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class Enumeration_EditField
    Inherits FieldTemplateUserControl

    Private _enumType As Type
    
    Private ReadOnly Property EnumType As Type
        Get
            If (_enumType Is Nothing) Then
                _enumType = Column.GetEnumType
            End If
            Return _enumType
        End Get
    End Property
    
    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return DropDownList1
        End Get
    End Property
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        DropDownList1.ToolTip = Column.Description
        If (DropDownList1.Items.Count = 0) Then
            If Mode = DataBoundControlMode.Insert OrElse Not Column.IsRequired Then
                DropDownList1.Items.Add(New ListItem("[Not Set]", String.Empty))
            End If
            PopulateListControl(DropDownList1)
        End If
        SetUpValidator(RequiredFieldValidator1)
        SetUpValidator(DynamicValidator1)
    End Sub
    
    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        MyBase.OnDataBinding(e)
        If Mode = DataBoundControlMode.Edit AndAlso FieldValue IsNot Nothing Then
            Dim selectedValueString As String = GetSelectedValueString()
            Dim item As ListItem = DropDownList1.Items.FindByValue(selectedValueString)
            If item IsNot Nothing Then
                DropDownList1.SelectedValue = selectedValueString
            End If
        End If
    End Sub
    
    Protected Overrides Sub ExtractValues(ByVal dictionary As IOrderedDictionary)
        Dim value As String = DropDownList1.SelectedValue
        If (value = String.Empty) Then
            value = Nothing
        End If
        dictionary(Column.Name) = ConvertEditedValue(value)
    End Sub
    
End Class
