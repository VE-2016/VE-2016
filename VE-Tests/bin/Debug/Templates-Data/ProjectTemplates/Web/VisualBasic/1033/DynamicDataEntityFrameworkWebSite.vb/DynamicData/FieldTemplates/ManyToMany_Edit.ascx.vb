Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Data.Objects
Imports System.Data.Objects.DataClasses
Imports System.Collections.Generic

Class ManyToMany_EditField
    Inherits System.Web.DynamicData.FieldTemplateUserControl
    Protected Property ObjectContext() As ObjectContext
        Get
            Return m_ObjectContext
        End Get
        Set(value As ObjectContext)
            m_ObjectContext = value
        End Set
    End Property
    Private m_ObjectContext As ObjectContext

    Public Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim ds As EntityDataSource = CType(Me.FindDataSourceControl, EntityDataSource)

        AddHandler ds.ContextCreated, AddressOf Me.ds_ContextCreated

        AddHandler ds.Updating, AddressOf Me.DataSource_UpdatingOrInserting
        AddHandler ds.Inserting, AddressOf Me.DataSource_UpdatingOrInserting
    End Sub
    Private Sub ds_ContextCreated(ByVal sender As Object, ByVal e As EntityDataSourceContextCreatedEventArgs)
        ObjectContext = e.Context
    End Sub

    Private Sub DataSource_UpdatingOrInserting(ByVal sender As Object, ByVal e As EntityDataSourceChangingEventArgs)
        Dim childTable As MetaTable = ChildrenColumn.ChildTable
        If (Mode = DataBoundControlMode.Edit) Then
            ObjectContext.LoadProperty(e.Entity, Column.Name)
        End If

        Dim entityCollection As Object = Column.EntityTypeProperty.GetValue(e.Entity, Nothing)
        For Each childEntity As Object In childTable.GetQuery(e.Context)
            Dim isCurrentlyInList As Object = ListContainsEntity(childTable, entityCollection, childEntity)
            Dim pkString As String = childTable.GetPrimaryKeyString(childEntity)
            Dim listItem As ListItem = CheckBoxList1.Items.FindByValue(pkString)
            If (listItem Is Nothing) Then
                Continue For
            End If
            If listItem.Selected Then
                If Not isCurrentlyInList Then
                    entityCollection.Add(childEntity)
                End If
            ElseIf isCurrentlyInList Then
                entityCollection.Remove(childEntity)
            End If
        Next
    End Sub

    Private Shared Function ListContainsEntity(table As MetaTable, list As IEnumerable(Of Object), entity As Object) As Boolean
        Return list.Any(Function(e) AreEntitiesEqual(table, e, entity))
    End Function

    Private Shared Function AreEntitiesEqual(table As MetaTable, entity1 As Object, entity2 As Object) As Boolean
        Return Enumerable.SequenceEqual(table.GetPrimaryKeyValues(entity1), table.GetPrimaryKeyValues(entity2))
    End Function

    Protected Sub CheckBoxList1_DataBound(sender As Object, e As EventArgs)
        Dim childTable As MetaTable = ChildrenColumn.ChildTable

        Dim entityCollection As IEnumerable(Of Object) = Nothing

        If Mode = DataBoundControlMode.Edit Then
            Dim entity As Object
            Dim rowDescriptor As ICustomTypeDescriptor = TryCast(Row, ICustomTypeDescriptor)
            If rowDescriptor IsNot Nothing Then
                entity = rowDescriptor.GetPropertyOwner(Nothing)
            Else
                entity = Row
            End If

            entityCollection = DirectCast(Column.EntityTypeProperty.GetValue(entity, Nothing), IEnumerable(Of Object))
            Dim realEntityCollection = TryCast(entityCollection, RelatedEnd)
            If realEntityCollection IsNot Nothing AndAlso Not realEntityCollection.IsLoaded Then
                realEntityCollection.Load()
            End If
        End If

        For Each childEntity As Object In childTable.GetQuery(ObjectContext)
            Dim listItem As New ListItem(childTable.GetDisplayString(childEntity), childTable.GetPrimaryKeyString(childEntity))

            If Mode = DataBoundControlMode.Edit Then
                listItem.Selected = ListContainsEntity(childTable, entityCollection, childEntity)
            End If
            CheckBoxList1.Items.Add(listItem)
        Next
    End Sub

    Public Overrides ReadOnly Property DataControl() As Control
        Get
            Return CheckBoxList1
        End Get
    End Property
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function


End Class
