' NOTE: You can use the "Rename" command on the context menu to change the class name "Service" in code, svc and config file together.
Public Class Service
    Implements IService

    Public Sub New()
    End Sub

    Public Function GetData(ByVal value As Integer) As String Implements IService.GetData
        Return String.Format("You entered: {0}", value)
    End Function

    Public Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType Implements IService.GetDataUsingDataContract
        If composite Is Nothing Then
            Throw New ArgumentNullException("composite")
        End If
        If composite.BoolValue Then
            composite.StringValue &= "Suffix"
        End If
        Return composite
    End Function

End Class
