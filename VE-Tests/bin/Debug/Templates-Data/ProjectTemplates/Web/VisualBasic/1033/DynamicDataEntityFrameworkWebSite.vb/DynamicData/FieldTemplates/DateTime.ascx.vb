Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class DateTimeField
    Inherits FieldTemplateUserControl

        
        Public Overrides ReadOnly Property DataControl As Control
            Get
                Return Literal1
            End Get
        End Property
    
    
End Class
