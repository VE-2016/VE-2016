Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Net
Imports System.Threading
Imports Microsoft.ServiceBus
Imports Microsoft.ServiceBus.Messaging
Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.ServiceRuntime

Public Class WorkerRole
    Inherits RoleEntryPoint

    ' The name of your queue
    Const QueueName As String = "ProcessingQueue"
	
    ' QueueClient is Thread-safe. Recommended that you cache 
    ' rather than recreating it on every request
    Dim Client As QueueClient
    Dim CompletedEvent = New ManualResetEvent(False)
	
    Public Overrides Sub Run()

        Trace.WriteLine("Starting processing of messages")

        ' Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
        Client.OnMessage(Sub(receivedMessage As BrokeredMessage)
                             Try
                                 ' Process the message
                                 Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString())
                             Catch
                                 ' Handle any message processing specific exceptions here
                             End Try
                         End Sub)

        CompletedEvent.WaitOne()
        
    End Sub

    Public Overrides Function OnStart() As Boolean

        ' Set the maximum number of concurrent connections 
        ServicePointManager.DefaultConnectionLimit = 12

        ' Create the queue if it does not exist already
        Dim connectionString As String = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString")
        Dim namespaceManager As NamespaceManager = namespaceManager.CreateFromConnectionString(connectionString)
        If (Not namespaceManager.QueueExists(QueueName)) Then
            namespaceManager.CreateQueue(QueueName)
        End If
		
        ' Get a client to use the queue
        Client = QueueClient.CreateFromConnectionString(connectionString, QueueName)
        Return MyBase.OnStart()

    End Function

    Public Overrides Sub OnStop()
	
        ' Close the connection to Service Bus Queue
        Client.Close()
        CompletedEvent.Set()
        MyBase.OnStop()
		
    End Sub

End Class
