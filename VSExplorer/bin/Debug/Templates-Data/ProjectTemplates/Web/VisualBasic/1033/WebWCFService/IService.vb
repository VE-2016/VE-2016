' NOTE: You can use the "Rename" command on the context menu to change the interface name "IService" in both code and config file together.
<ServiceContract()>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
Public Interface IService

    <OperationContract()>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
    Function GetData(ByVal value As Integer) As String

    <OperationContract()>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
    Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType

    ' TODO: Add your service operations here

End Interface

' Use a data contract as illustrated in the sample below to add composite types to service operations.
$if$ ($targetframeworkversion$ <= 3.5)
<DataContract()> _
Public Class CompositeType

    Private boolValueField As Boolean
    Private stringValueField As String

    <DataMember()> _
    Public Property BoolValue() As Boolean
        Get
            Return Me.boolValueField
        End Get
        Set(ByVal value As Boolean)
            Me.boolValueField = value
        End Set
    End Property

    <DataMember()> _
    Public Property StringValue() As String
        Get
            Return Me.stringValueField
        End Get
        Set(ByVal value As String)
            Me.stringValueField = value
        End Set
    End Property
$else$
<DataContract()>
Public Class CompositeType

    <DataMember()>
    Public Property BoolValue() As Boolean
    <DataMember()>
    Public Property StringValue() As String
$endif$
End Class
