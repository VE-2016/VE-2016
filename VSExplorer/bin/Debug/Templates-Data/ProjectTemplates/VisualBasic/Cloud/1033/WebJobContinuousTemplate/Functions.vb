Imports System.IO
Imports Microsoft.Azure.WebJobs

Public Module Functions
    ' This function will get triggered/executed when a new message is written 
    ' on an Azure Queue called queue.
    Public Sub ProcessQueueMessage(<QueueTrigger("queue")> message As String, log As TextWriter)
        log.WriteLine(message)
    End Sub
End Module
