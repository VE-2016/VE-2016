<%@ Control Language="VB" ClassName="$safeitemrootname$" Inherits="System.Web.DynamicData.FieldTemplateUserControl" %>

<asp:Literal runat="server" ID="Literal1" Text="<%# FieldValueString %>" />

<script runat="server">
    Public Overrides ReadOnly Property DataControl() As Control
        Get
            Return Literal1
        End Get
    End Property
</script>
