Imports System.Web
Public Class $safeitemrootname$ 
    Implements IHttpModule

    Private WithEvents _context As HttpApplication

    ''' <summary>
    '''  You will need to configure this module in the Web.config file of your
    '''  web and register it with IIS before being able to use it. For more information
    '''  see the following link: https://go.microsoft.com/?linkid=8101007
    ''' </summary>
#Region "IHttpModule Members"

    Public Sub Dispose() Implements IHttpModule.Dispose

        ' Clean-up code here

    End Sub

    Public Sub Init(ByVal context As HttpApplication) Implements IHttpModule.Init
        _context = context
    End Sub

#End Region

    Public Sub OnLogRequest(ByVal source As Object, ByVal e As EventArgs) Handles _context.LogRequest

        ' Handles the LogRequest event to provide a custom logging 
        ' implementation for it

    End Sub
End Class
