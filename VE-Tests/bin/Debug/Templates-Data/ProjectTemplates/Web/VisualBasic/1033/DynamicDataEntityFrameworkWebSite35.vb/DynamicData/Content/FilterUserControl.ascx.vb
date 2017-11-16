Imports System.Web.DynamicData

Partial Class FilterUserControl
    Inherits System.Web.DynamicData.FilterUserControlBase

    Public Custom Event SelectedIndexChanged As EventHandler
        AddHandler(ByVal value As EventHandler)
            AddHandler DropDownList1.SelectedIndexChanged, value
        End AddHandler

        RemoveHandler(ByVal value As EventHandler)
            RemoveHandler DropDownList1.SelectedIndexChanged, value
        End RemoveHandler

        RaiseEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
        End RaiseEvent
    End Event

    Public Overrides ReadOnly Property SelectedValue() As String
        Get
            Return DropDownList1.SelectedValue
        End Get
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsPostBack Then
            PopulateListControl(DropDownList1)
            ' Set the initial value if there is one
            If Not String.IsNullOrEmpty(InitialValue) Then
                DropDownList1.SelectedValue = InitialValue
            End If
        End If
    End Sub


End Class
