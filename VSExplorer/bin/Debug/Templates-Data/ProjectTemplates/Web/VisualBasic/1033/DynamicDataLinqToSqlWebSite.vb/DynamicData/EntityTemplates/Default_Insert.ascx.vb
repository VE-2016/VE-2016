
Class Default_InsertEntityTemplate
    Inherits EntityTemplateUserControl

    Private currentColumn As MetaColumn
    
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        For Each column As MetaColumn In Table.GetScaffoldColumns(Mode, ContainerType)
            currentColumn = column
            Dim item As Control = New DefaultEntityTemplate._NamingContainer
            EntityTemplate1.ItemTemplate.InstantiateIn(item)
            EntityTemplate1.Controls.Add(item)
        Next
    End Sub
    
    Protected Sub Label_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim label = CType(sender, Label)
        label.Text = currentColumn.DisplayName
    End Sub
    
    Protected Sub Label_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        Dim label = CType(sender, Label)
        Dim dynamicControl As DynamicControl = CType(label.FindControl("DynamicControl"), DynamicControl)
        Dim ftuc = CType(dynamicControl.FieldTemplate, FieldTemplateUserControl)
        If ftuc IsNot Nothing AndAlso ftuc.DataControl IsNot Nothing Then
            label.AssociatedControlID = ftuc.DataControl.GetUniqueIDRelativeTo(label)
        End If
    End Sub
    
    Protected Sub DynamicControl_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim dynamicControl = CType(sender, DynamicControl)
        dynamicControl.DataField = currentColumn.Name
    End Sub
    
End Class
