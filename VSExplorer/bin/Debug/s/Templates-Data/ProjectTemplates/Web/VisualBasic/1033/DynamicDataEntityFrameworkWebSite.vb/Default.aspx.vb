Imports System.Web.DynamicData

Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim visibleTables As IList = ASP.global_asax.DefaultModel.VisibleTables
        If visibleTables.Count = 0 Then
            Throw New InvalidOperationException("There are no accessible tables. " &
                "Make sure that at least one data model is registered in Global.asax " &
                "and scaffolding is enabled or implement custom pages.")
        End If
        Menu1.DataSource = visibleTables
        Menu1.DataBind()
    End Sub
    
End Class
