Imports Microsoft.Azure.WebJobs

' To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
Module $safeitemname$

    ' Please set the following connection strings in app.config for this WebJob to run:
    ' AzureWebJobsDashboard and AzureWebJobsStorage
    Sub Main()
        Dim config As New JobHostConfiguration()

        If (config.IsDevelopment) Then
            config.UseDevelopmentSettings()
        End If

        Dim host As New JobHost()
        ' The following code ensures that the WebJob will be running continuously            
        host.RunAndBlock()
    End Sub

End Module
