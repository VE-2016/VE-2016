
Class DefaultEntityTemplate
    Inherits EntityTemplateUserControl

    Private currentColumn As MetaColumn
    
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        For Each column As MetaColumn In Table.GetScaffoldColumns(Mode, ContainerType)
            currentColumn = column
            Dim item As Control = New _NamingContainer
            EntityTemplate1.ItemTemplate.InstantiateIn(item)
            EntityTemplate1.Controls.Add(item)
        Next
    End Sub
    
    Protected Sub Label_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim label = CType(sender, Label)
        label.Text = currentColumn.DisplayName
    End Sub
    
    Protected Sub DynamicControl_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim dynamicControl = CType(sender, DynamicControl)
        dynamicControl.DataField = currentColumn.Name
    End Sub
    
    Public Class _NamingContainer
        Inherits Control
        Implements INamingContainer
    End Class
    
End Class
