<%@ Control Language="VB" AutoEventWireup="true" CodeFile="ViewSwitcher.ascx.vb" Inherits="ViewSwitcher" %>
<div id="viewSwitcher">
    <%: CurrentView %> view | <a href="<%: SwitchUrl %>" data-ajax="false">Switch to <%: AlternateView %></a>
</div>
