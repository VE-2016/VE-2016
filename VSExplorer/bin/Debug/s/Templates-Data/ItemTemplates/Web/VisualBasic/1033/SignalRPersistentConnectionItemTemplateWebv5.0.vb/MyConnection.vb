Imports System.Threading.Tasks
Imports Microsoft.AspNet.SignalR

Public Class $safeitemname$
    Inherits PersistentConnection

    Protected Overrides Function OnConnected(ByVal request As IRequest, ByVal connectionId As String) As Task
        Return Connection.Send(connectionId, "Welcome!")
    End Function

    Protected Overrides Function OnReceived(ByVal request As IRequest, ByVal connectionId As String, ByVal data As String) As Task
        Return Connection.Broadcast(data)
    End Function
End Class
