<%@ WebService Language="VB" Class="$safeitemrootname$" %>

Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

$if$ ($targetframeworkversion$ >= 3.5)' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
$endif$<WebService(Namespace := "http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _  
Public Class $safeitemrootname$
    Inherits System.Web.Services.WebService 
    
	<WebMethod()> _
	Public Function HelloWorld() As String
		Return "Hello World"
	End Function

End Class
