Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web

<ServiceContract(Namespace:="")>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
Public Class $safeitemrootname$

    ' To use HTTP GET, add <WebGet()> attribute. (Default ResponseFormat is WebMessageFormat.Json)
    ' To create an operation that returns XML,
    '     add <WebGet(ResponseFormat:=WebMessageFormat.Xml)>,
    '     and include the following line in the operation body:
    '         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml"
    <OperationContract()>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
    Public Sub DoWork()
        ' Add your operation implementation here
    End Sub

    ' Add more operations here and mark them with <OperationContract()>

End Class
