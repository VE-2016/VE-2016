<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="RegisterExternalLogin.aspx.cs" Inherits="Account_RegisterExternalLogin" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h1>Register with your <%: ProviderDisplayName %> account</h1>
        <h2><%: ProviderUserName %>.</h2>
    </hgroup>

    $if$ ($targetframeworkversion$ >= 4.5)
    <asp:ModelErrorMessage runat="server" ModelStateKey="Provider" CssClass="field-validation-error" />
    $else$
    <asp:Label runat="server" ID="providerMessage" CssClass="field-validation-error" />
    $endif$

    <asp:PlaceHolder runat="server" ID="userNameForm">
        <fieldset>
            <legend>Association Form</legend>
            <p>
                You've authenticated with <strong><%: ProviderDisplayName %></strong> as
                <strong><%: ProviderUserName %></strong>. Please enter a user name below for the current site
                and click the Log in button.
            </p>
            <ol>
                <li class="email">
                    <asp:Label runat="server" AssociatedControlID="userName">User name</asp:Label>
                    <asp:TextBox runat="server" ID="userName" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="userName"
                        Display="Dynamic" ErrorMessage="User name is required" ValidationGroup="NewUser" />
                    $if$ ($targetframeworkversion$ >= 4.5)
                    <asp:ModelErrorMessage runat="server" ModelStateKey="UserName" CssClass="field-validation-error" />
                    $else$
                    <asp:Label runat="server" ID="userNameMessage" CssClass="field-validation-error" />
                    $endif$
                </li>
            </ol>
            <asp:Button runat="server" Text="Log in" ValidationGroup="NewUser" OnClick="logIn_Click" />
            <asp:Button runat="server" Text="Cancel" CausesValidation="false" OnClick="cancel_Click" />
        </fieldset>
    </asp:PlaceHolder>
</asp:Content>
