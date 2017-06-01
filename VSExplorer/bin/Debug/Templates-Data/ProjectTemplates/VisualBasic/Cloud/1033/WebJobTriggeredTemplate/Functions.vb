Imports System.IO
Imports System.Runtime.InteropServices
Imports Microsoft.Azure.WebJobs

Public Module Functions
    ' This function will be triggered based on the schedule you have set for this WebJob
    ' This function will enqueue a message on an Azure Queue called queue
    <NoAutomaticTrigger>
    Public Sub ManualTrigger(log As TextWriter, value As Integer, <Queue("queue"), Out()> ByRef message As String)
        log.WriteLine("Function is invoked with value={0}", value)
        message = value.ToString()
        log.WriteLine("Following message will be written on the Queue={0}", message)
    End Sub
End Module