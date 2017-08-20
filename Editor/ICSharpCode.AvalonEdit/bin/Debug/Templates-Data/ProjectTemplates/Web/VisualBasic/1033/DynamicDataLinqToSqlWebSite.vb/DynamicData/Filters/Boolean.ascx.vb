Imports System.Linq.Expressions

Class BooleanFilter
    Inherits QueryableFilterUserControl

    Private Const NullValueString As String = "[null]"
    
    Public Overrides ReadOnly Property FilterControl As Control
        Get
            Return DropDownList1
        End Get
    End Property
    
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        If Not Column.ColumnType.Equals(GetType(System.Boolean)) Then
            Throw New InvalidOperationException(String.Format("A boolean filter was loaded for column '{0}' but the column has an incompatible type '{1}'.", Column.Name, Column.ColumnType))
        End If
        If Not Page.IsPostBack Then
            DropDownList1.Items.Add(New ListItem("All", String.Empty))
            If Not Column.IsRequired Then
                DropDownList1.Items.Add(New ListItem("[Not Set]", NullValueString))
            End If
            DropDownList1.Items.Add(New ListItem("True", Boolean.TrueString))
            DropDownList1.Items.Add(New ListItem("False", Boolean.FalseString))
            ' Set the initial value if there is one
            Dim initialValue As String = DefaultValue
            If Not String.IsNullOrEmpty(initialValue) Then
                DropDownList1.SelectedValue = initialValue
            End If
        End If
    End Sub
    
    Public Overrides Function GetQueryable(ByVal source As IQueryable) As IQueryable
        Dim selectedValue As String = DropDownList1.SelectedValue
        If String.IsNullOrEmpty(selectedValue) Then
            Return source
        End If
        Dim value As Object = selectedValue
        If (selectedValue = NullValueString) Then
            value = Nothing
        End If
        If DefaultValues IsNot Nothing Then
            DefaultValues(Column.Name) = value
        End If
        Return ApplyEqualityFilter(source, Column.Name, value)
    End Function
    
    Protected Sub DropDownList1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        OnFilterChanged()
    End Sub
    
End Class
