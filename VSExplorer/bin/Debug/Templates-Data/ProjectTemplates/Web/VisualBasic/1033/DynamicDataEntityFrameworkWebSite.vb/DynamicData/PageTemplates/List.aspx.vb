Imports System.Web.DynamicData
Imports System.Web.Routing
Imports System.Web.UI.WebControls.Expressions

Class List
    Inherits Page

    Protected table As MetaTable
    
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        table = DynamicDataRouteHandler.GetRequestMetaTable(Context)
        GridView1.SetMetaTable(table, table.GetColumnValuesFromRoute(Context))
        GridDataSource.EntityTypeFilter = table.EntityType.Name
        
    End Sub
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Title = table.DisplayName
        GridDataSource.Include = table.ForeignKeyColumnsNames
    
        ' Disable various options if the table is readonly
        If table.IsReadOnly Then
            GridView1.Columns(0).Visible = False
            InsertHyperLink.Visible = False
            GridView1.EnablePersistedSelection = False
        End If
    End Sub
    
    Protected Sub Label_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        Dim label = CType(sender, Label)
        Dim dynamicFilter = CType(label.FindControl("DynamicFilter"), DynamicFilter)
        Dim fuc = CType(dynamicFilter.FilterTemplate, QueryableFilterUserControl)
        If fuc IsNot Nothing AndAlso fuc.FilterControl IsNot Nothing Then
            label.AssociatedControlID = fuc.FilterControl.GetUniqueIDRelativeTo(label)
        End If
    End Sub
    
    Protected Overrides Sub OnPreRenderComplete(ByVal e As EventArgs)
        Dim routeValues As New RouteValueDictionary(GridView1.GetDefaultValues)
        InsertHyperLink.NavigateUrl = table.GetActionPath(PageAction.Insert, routeValues)
        MyBase.OnPreRenderComplete(e)
    End Sub
    
    Protected Sub DynamicFilter_FilterChanged(ByVal sender As Object, ByVal e As EventArgs)
        GridView1.PageIndex = 0
    End Sub
    
End Class
