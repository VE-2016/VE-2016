Imports Microsoft.AspNet.SignalR

Public Class $safeitemname$
    Inherits Hub

    Public Sub Hello()
        Clients.All.Hello()
    End Sub
End Class
