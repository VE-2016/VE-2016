Imports System.ComponentModel.DataAnnotations
Imports System.Web.DynamicData
Imports System.Web

Class EnumerationField
    Inherits FieldTemplateUserControl

    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return Literal1
        End Get
    End Property
    
    Public ReadOnly Property EnumFieldValueString As String
        Get
            If (FieldValue Is Nothing) Then
                Return FieldValueString
            End If
            Dim enumType As Type = Column.GetEnumType
            If enumType IsNot Nothing Then
                Dim enumValue As Object = System.Enum.ToObject(enumType, FieldValue)
                Return FormatFieldValue(enumValue)
            End If
            Return FieldValueString
        End Get
    End Property
    
End Class
