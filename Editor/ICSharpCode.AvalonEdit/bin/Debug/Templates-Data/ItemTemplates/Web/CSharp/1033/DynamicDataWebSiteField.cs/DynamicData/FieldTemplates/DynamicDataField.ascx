<%@ Control Language="C#" ClassName="$safeitemrootname$" Inherits="System.Web.DynamicData.FieldTemplateUserControl" %>

<asp:Literal runat="server" ID="Literal1" Text="<%# FieldValueString %>" />

<script runat="server">
    public override Control DataControl {
        get {
            return Literal1;
        }
    }
</script>
