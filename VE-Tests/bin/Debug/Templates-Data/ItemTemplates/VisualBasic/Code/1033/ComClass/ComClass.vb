<ComClass($safeitemname$.ClassId, $safeitemname$.InterfaceId, $safeitemname$.EventsId)> _
Public Class $safeitemname$

#Region "COM GUIDs"
    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.
    Public Const ClassId As String = "$guid1$"
    Public Const InterfaceId As String = "$guid2$"
    Public Const EventsId As String = "$guid3$"
#End Region

    ' A creatable COM class must have a Public Sub New() 
    ' with no parameters, otherwise, the class will not be 
    ' registered in the COM registry and cannot be created 
    ' via CreateObject.
    Public Sub New()
        MyBase.New()
    End Sub

End Class


