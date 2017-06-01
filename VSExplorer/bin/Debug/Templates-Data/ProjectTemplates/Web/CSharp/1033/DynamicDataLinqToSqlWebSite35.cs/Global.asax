<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.DynamicData" %>

<script RunAt="server">
    public static void RegisterRoutes(RouteCollection routes) {
        MetaModel model = new MetaModel();
        
        //                    IMPORTANT: DATA MODEL REGISTRATION 
        // Uncomment this line to register LINQ to SQL classes
        // model for ASP.NET Dynamic Data. Set ScaffoldAllTables = true only if you are sure 
        // that you want all tables in the data model to support a scaffold (i.e. templates) 
        // view. To control scaffolding for individual tables, create a partial class for 
        // the table and apply the [Scaffold(true)] attribute to the partial class.
        // Note: Make sure that you change "YourDataContextType" to the name of the data context
        // class in your application.
        //model.RegisterContext(typeof(YourDataContextType), new ContextConfiguration() { ScaffoldAllTables = false });

        // The following statement supports separate-page mode, where the List, Detail, Insert, and 
        // Update tasks are performed by using separate pages. To enable this mode, uncomment the following 
        // route definition, and comment out the route definitions in the combined-page mode section that follows.
        routes.Add(new DynamicDataRoute("{table}/{action}.aspx") {
            Constraints = new RouteValueDictionary(new { action = "List|Details|Edit|Insert" }),
            Model = model
        });

        // The following statements support combined-page mode, where the List, Detail, Insert, and
        // Update tasks are performed by using the same page. To enable this mode, uncomment the
        // following routes and comment out the route definition in the separate-page mode section above.
        //routes.Add(new DynamicDataRoute("{table}/ListDetails.aspx") {
        //    Action = PageAction.List,
        //    ViewName = "ListDetails",
        //    Model = model
        //});

        //routes.Add(new DynamicDataRoute("{table}/ListDetails.aspx") {
        //    Action = PageAction.Details,
        //    ViewName = "ListDetails",
        //    Model = model
        //});
    }

    void Application_Start(object sender, EventArgs e) {
        RegisterRoutes(RouteTable.Routes);
    }

</script>
