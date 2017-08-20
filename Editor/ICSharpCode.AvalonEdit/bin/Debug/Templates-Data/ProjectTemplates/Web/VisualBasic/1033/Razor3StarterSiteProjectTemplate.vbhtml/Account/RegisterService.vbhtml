@* Remove this section if you are using bundling *@
@Section Scripts
    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
End Section

@Code
    Layout = "~/_SiteLayout.vbhtml"
    PageData("Title") = "Register"

    Dim email As String = ""
    Dim loginData As String = ""
    Dim providerDisplayName As String = ""

    Dim returnUrl As String = Request.QueryString("ReturnUrl")
    If returnUrl.IsEmpty() Then
        ' Some external login providers always require a return URL value
        returnUrl = Href("~/")
    End If

    ' Setup validation
    Validation.RequireField("email", "The user name field is required.")

    If IsPost AndAlso Request.Form("newAccount").AsBool() Then
        ' Handle new account registration form
        AntiForgery.Validate()

        email = Request.Form("email")
        loginData = Request.Form("loginData")

        Dim provider As String = ""
        Dim providerUserId As String = ""
        If WebSecurity.IsAuthenticated OrElse Not OAuthWebSecurity.TryDeserializeProviderUserId(loginData, provider, providerUserId) Then
            Response.Redirect("~/Account/Manage")
            Return
        End If

        providerDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName
        If Validation.IsValid() Then
            ' Insert a new user into the database
            Dim db As Database = Database.Open("StarterSite")

            ' Check if user already exists
            Dim user As Object = db.QuerySingle("SELECT Email FROM UserProfile WHERE LOWER(Email) = LOWER(@0)", email)
            If user Is Nothing Then
                ' Insert email into the profile table
                db.Execute("INSERT INTO UserProfile (Email) VALUES (@0)", email)
                OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, email)

                OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie:= False)

                Context.RedirectLocal(returnUrl)
                Return
            Else
                ModelState.AddError("email", "User name already exists. Please enter a different user name.")
            End If
        End If
    Else
        ' Handle callbacks from the external login provider

        Dim result As AuthenticationResult = OAuthWebSecurity.VerifyAuthentication(Href("~/Account/RegisterService", New With { .ReturnUrl = returnUrl }))
        If result.IsSuccessful Then
            Dim registered As Boolean = OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, False)
            If registered Then
                Context.RedirectLocal(returnUrl)
                Return
            End If

            If WebSecurity.IsAuthenticated Then
                ' If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, WebSecurity.CurrentUserName)
                Context.RedirectLocal(returnUrl)
                Return
            Else
                ' User is new, set default user name to the value obtained from external login provider
                email = result.UserName
                loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId)
                providerDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName
            End If
        Else
            Response.Redirect("~/Account/ExternalLoginFailure")
            Return
        End If
    End If
End Code

<hgroup class="title">
    <h1>@PageData("Title").</h1>
    <h2>Associate your @providerDisplayName account.</h2>
</hgroup>

<form method="post">
    @AntiForgery.GetHtml()
    <input type="hidden" name="loginData" value="@loginData" />

    @* If at least one validation error exists, notify the user *@
    @Html.ValidationSummary(excludeFieldErrors:=True)

    <fieldset>
        <legend>Registration Form</legend>
        <p>
            You've successfully authenticated with <strong>@providerDisplayName</strong>. Please
            enter a user name for this site below and click the Confirm button to finish logging
            in.
        </p>
        <ol>
            <li class="email">
                <label for="email" @If Not ModelState.IsValidField("email") Then@<text>class="error-label"</text>End If>Email address</label>
                <input type="text" id="email" name="email" value="@email" @Validation.For("email") />
                @* Write any email validation errors to the page *@
                @Html.ValidationMessage("email")
            </li>
        </ol>
        <button type="submit" name="newAccount" value="true">Register</button>
    </fieldset>
</form>