@If OAuthWebSecurity.RegisteredClientData.Count = 0 Then
    @<div class="message-info">
        <p>
            There are no external authentication services configured. See <a href="https://go.microsoft.com/fwlink/?LinkId=226949">this article</a>
            for details on setting up this ASP.NET application to support logging in via external services.
        </p>
    </div>
Else
    @<form method="post">
    @AntiForgery.GetHtml()
    <fieldset id="socialLoginList">
        <legend>Log in using another service</legend>
        <p>
            @For Each client As AuthenticationClientData In OAuthWebSecurity.RegisteredClientData
                @<button type="submit" name="provider" value="@client.AuthenticationClient.ProviderName"
                         title="Log in using your @client.DisplayName account">@client.DisplayName</button>
            Next
        </p>
    </fieldset>
    </form>
End If