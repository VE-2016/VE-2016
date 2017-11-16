Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Data.Objects
Imports System.Data.Objects.DataClasses
Imports System.ComponentModel

Class ManyToManyField
    Inherits System.Web.DynamicData.FieldTemplateUserControl
    Protected Overrides Sub OnDataBinding(e As EventArgs)
        MyBase.OnDataBinding(e)

        Dim entity As Object
        Dim rowDescriptor As ICustomTypeDescriptor = TryCast(Row, ICustomTypeDescriptor)
        If rowDescriptor IsNot Nothing Then
            entity = rowDescriptor.GetPropertyOwner(Nothing)
        Else
            entity = Row
        End If

        Dim entityCollection = Column.EntityTypeProperty.GetValue(entity, Nothing)
        Dim realEntityCollection = TryCast(entityCollection, RelatedEnd)
        If realEntityCollection IsNot Nothing AndAlso Not realEntityCollection.IsLoaded Then
            realEntityCollection.Load()
        End If

        Repeater1.DataSource = entityCollection
        Repeater1.DataBind()
    End Sub

    Public Overrides ReadOnly Property DataControl() As Control
        Get
            Return Repeater1
        End Get
    End Property

End Class
