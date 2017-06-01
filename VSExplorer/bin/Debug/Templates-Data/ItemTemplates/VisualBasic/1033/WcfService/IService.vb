Imports System.ServiceModel

' NOTE: You can use the "Rename" command on the context menu to change the interface name "$safeitemrootname$" in both code and config file together.
<ServiceContract()>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
Public Interface $safeitemrootname$

    <OperationContract()>$if$ ($targetframeworkversion$ <= 3.5) _$endif$
    Sub DoWork()

End Interface
