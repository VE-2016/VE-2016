Imports System.Web.DynamicData
Imports System.Web.Routing
Imports System.Web.UI.WebControls.Expressions

Class Details
    Inherits Page

    Protected table As MetaTable
    
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        table = DynamicDataRouteHandler.GetRequestMetaTable(Context)
        FormView1.SetMetaTable(table)
        DetailsDataSource.EntityTypeFilter = table.EntityType.Name
    End Sub
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Title = table.DisplayName
        DetailsDataSource.Include = table.ForeignKeyColumnsNames
    End Sub
    
    Protected Sub FormView1_ItemDeleted(ByVal sender As Object, ByVal e As FormViewDeletedEventArgs)
        If e.Exception Is Nothing OrElse e.ExceptionHandled Then
            Response.Redirect(table.ListActionPath)
        End If
    End Sub
    
End Class
