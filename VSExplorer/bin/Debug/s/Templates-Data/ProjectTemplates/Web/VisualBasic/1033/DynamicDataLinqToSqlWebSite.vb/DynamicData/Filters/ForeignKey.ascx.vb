Imports System.Linq.Expressions

Class ForeignKeyFilter
    Inherits QueryableFilterUserControl

    Private Const NullValueString As String = "[null]"
    
    Private Shadows ReadOnly Property Column As MetaForeignKeyColumn
        Get
            Return CType(MyBase.Column, MetaForeignKeyColumn)
        End Get
    End Property
    
    Public Overrides ReadOnly Property FilterControl As Control
        Get
            Return DropDownList1
        End Get
    End Property
    
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsPostBack Then
            If Not Column.IsRequired Then
                DropDownList1.Items.Add(New ListItem("[Not Set]", NullValueString))
            End If
            PopulateListControl(DropDownList1)
            ' Set the initial value if there is one
            Dim initialValue As String = DefaultValue
            If Not String.IsNullOrEmpty(initialValue) Then
                DropDownList1.SelectedValue = initialValue
            End If
        End If
    End Sub
    
    Protected Sub DropDownList1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        OnFilterChanged()
    End Sub
    
    Public Overrides Function GetQueryable(ByVal source As IQueryable) As IQueryable
        Dim selectedValue As String = DropDownList1.SelectedValue
        If String.IsNullOrEmpty(selectedValue) Then
            Return source
        End If
        If (selectedValue = NullValueString) Then
            Return ApplyEqualityFilter(source, Column.Name, Nothing)
        End If
        Dim dict As IDictionary = New Hashtable
        Column.ExtractForeignKey(dict, selectedValue)
        For Each entry As DictionaryEntry In dict
            Dim key As String = CType(entry.Key, String)
            If DefaultValues IsNot Nothing Then
                DefaultValues(key) = entry.Value
            End If
            source = ApplyEqualityFilter(source, Column.GetFilterExpression(key), entry.Value)
        Next
        Return source
    End Function
    
End Class
