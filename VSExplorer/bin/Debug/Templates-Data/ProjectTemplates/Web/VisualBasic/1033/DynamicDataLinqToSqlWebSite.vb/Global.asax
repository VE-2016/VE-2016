<%@ Application Language="VB" %>
<%@ Import Namespace="System.ComponentModel.DataAnnotations" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>

<script RunAt="server">
Private Shared s_defaultModel As New MetaModel
Public Shared ReadOnly Property DefaultModel() As MetaModel
    Get
        Return s_defaultModel
    End Get
End Property

Public Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
    '                     IMPORTANT: DATA MODEL REGISTRATION 
    ' Uncomment this line to register a LINQ to SQL model for ASP.NET Dynamic Data.
    ' Set ScaffoldAllTables = true only if you are sure that you want all tables in the
    ' data model to support a scaffold (i.e. templates) view. To control scaffolding for
    ' individual tables, create a partial class for the table and apply the
    ' <ScaffoldTable(true)> attribute to the partial class.
    ' Note: Make sure that you change "YourDataContextType" to the name of the data context
    ' class in your application.
    ' DefaultModel.RegisterContext(GetType(YourDataContextType), New ContextConfiguration() With {.ScaffoldAllTables = False})

    ' The following statement supports separate-page mode, where the List, Detail, Insert, and 
    ' Update tasks are performed by using separate pages. To enable this mode, uncomment the following 
    ' route definition, and comment out the route definitions in the combined-page mode section that follows.
    routes.Add(New DynamicDataRoute("{table}/{action}.aspx") With {
        .Constraints = New RouteValueDictionary(New With {.Action = "List|Details|Edit|Insert"}),
        .Model = DefaultModel})

    ' The following statements support combined-page mode, where the List, Detail, Insert, and
    ' Update tasks are performed by using the same page. To enable this mode, uncomment the
    ' following routes and comment out the route definition in the separate-page mode section above.
    'routes.Add(New DynamicDataRoute("{table}/ListDetails.aspx") With {
    '    .Action = PageAction.List,
    '    .ViewName = "ListDetails",
    '    .Model = DefaultModel})

    'routes.Add(New DynamicDataRoute("{table}/ListDetails.aspx") With {
    '    .Action = PageAction.Details,
    '    .ViewName = "ListDetails",
    '    .Model = DefaultModel})
End Sub

Private Shared Sub RegisterScripts()
    ScriptManager.ScriptResourceMapping.AddDefinition("jquery", New ScriptResourceDefinition With {
        .Path = "~/Scripts/jquery-1.7.1.min.js",
        .DebugPath = "~/Scripts/jquery-1.7.1.js",
        .CdnPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js",
        .CdnDebugPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.js",
        .CdnSupportsSecureConnection = True$if$ ($targetframeworkversion$ >= 4.5),
        .LoadSuccessExpression = "window.jQuery"$endif$
    })
End Sub

Private Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
    RegisterRoutes(RouteTable.Routes)
    RegisterScripts()
End Sub

</script>
