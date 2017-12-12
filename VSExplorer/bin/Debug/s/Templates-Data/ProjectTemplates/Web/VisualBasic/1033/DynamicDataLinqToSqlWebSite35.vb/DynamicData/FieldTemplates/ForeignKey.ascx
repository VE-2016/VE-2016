<%@ Control Language="VB" CodeFile="ForeignKey.ascx.vb" Inherits="ForeignKeyField" %>

<asp:HyperLink ID="HyperLink1" runat="server"
    Text="<%# GetDisplayString() %>"
    NavigateUrl="<%# GetNavigateUrl() %>"  />