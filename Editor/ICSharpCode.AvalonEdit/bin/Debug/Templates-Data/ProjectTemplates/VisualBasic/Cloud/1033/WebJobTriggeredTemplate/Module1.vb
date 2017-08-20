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
        ' The following code will invoke a function called ManualTrigger and 
        ' pass in data (value in this case) to the function
        host.Call(GetType(Functions).GetMethod("ManualTrigger"), New With {
            .value = 20
        })
    End Sub

End Module
