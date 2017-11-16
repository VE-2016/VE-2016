<%@ Application Language="VB" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.DynamicData" %>

<script RunAt="server">
Public Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
    Dim model As New MetaModel

    '                     IMPORTANT: DATA MODEL REGISTRATION 
    ' Uncomment this line to register LINQ to SQL classes
    ' model for ASP.NET Dynamic Data. Set ScaffoldAllTables = true only if you are sure 
    ' that you want all tables in the data model to support a scaffold (i.e. templates) 
    ' view. To control scaffolding for individual tables, create a partial class for 
    ' the table and apply the [Scaffold(true)] attribute to the partial class.
    ' Note: Make sure that you change "YourDataContextType" to the name of the data context
    ' class in your application.
    ' model.RegisterContext(GetType(YourDataContextType), New ContextConfiguration() With {.ScaffoldAllTables = False})

    ' The following statement supports separate-page mode, where the List, Detail, Insert, and 
    ' Update tasks are performed by using separate pages. To enable this mode, uncomment the following 
    ' route definition, and comment out the route definitions in the combined-page mode section that follows.
    routes.Add(New DynamicDataRoute("{table}/{action}.aspx") With { _
        .Constraints = New RouteValueDictionary(New With {.Action = "List|Details|Edit|Insert"}), _
        .Model = model})

    ' The following statements support combined-page mode, where the List, Detail, Insert, and
    ' Update tasks are performed by using the same page. To enable this mode, uncomment the
    ' following routes and comment out the route definition in the separate-page mode section above.
    'routes.Add(New DynamicDataRoute("{table}/ListDetails.aspx") With { _
    '    .Action = PageAction.List, _
    '    .ViewName = "ListDetails", _
    '    .Model = model})

    'routes.Add(New DynamicDataRoute("{table}/ListDetails.aspx") With { _
    '    .Action = PageAction.Details, _
    '    .ViewName = "ListDetails", _
    '    .Model = model})
End Sub

Private Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
    RegisterRoutes(RouteTable.Routes)
End Sub

</script>
