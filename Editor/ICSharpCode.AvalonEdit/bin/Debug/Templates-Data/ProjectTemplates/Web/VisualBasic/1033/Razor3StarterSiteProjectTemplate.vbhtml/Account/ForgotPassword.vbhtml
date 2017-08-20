@* Remove this section if you are using bundling *@
@Section Scripts
    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
End Section

@Code
    Layout = "~/_SiteLayout.vbhtml"
    PageData("Title") = "Forget Your Password"

    Dim passwordSent As Boolean = False
    Dim resetToken As String = ""
    Dim email As String = Request.Form("email")

    If email Is Nothing Then
        email = Request.QueryString("email")
    End If

    ' Setup validation
    Validation.RequireField("email", "The email address field is required.")

    If IsPost Then
        AntiForgery.Validate()
        ' validate email
        Dim isValid As Boolean = True
        If Validation.IsValid() Then
            If WebSecurity.GetUserId(email) > -1 AndAlso WebSecurity.IsConfirmed(email) Then
                resetToken = WebSecurity.GeneratePasswordResetToken(email) ' Optionally specify an expiration date for the token
            Else
                passwordSent = True ' We don't want to disclose that the user does not exist.
                isValid = False '
            End If
        End If
        If isValid Then
            Dim hostUrl As String = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped)
            Dim resetUrl As String = hostUrl + VirtualPathUtility.ToAbsolute("~/Account/PasswordReset?resetToken=" + HttpUtility.UrlEncode(resetToken))
            WebMail.Send(
                to:=email,
                subject:="Please reset your password",
                body:="Use this password reset token to reset your password. The token is: " + resetToken + ". Visit <a href=""" + HttpUtility.HtmlAttributeEncode(resetUrl) + """>" + resetUrl + "</a> to reset your password."
            )
            passwordSent = True
        End If
    End If
End Code

<hgroup class="title">
    <h1>@PageData("Title").</h1>
    <h2>Use the form below to reset your password.</h2>
</hgroup>

@If Not WebMail.SmtpServer.IsEmpty() Then
    @<p>
        We will send password reset instructions to the email address associated with your account.
    </p>

    If passwordSent Then
        @<p class="message-success">
            Instructions to reset your password have been sent to the specified email address.
        </p>
    End If

    @<form method="post">
        @AntiForgery.GetHtml()
        @Html.ValidationSummary(excludeFieldErrors:=True)

        <fieldset>
            <legend>Password Reset Instructions Form</legend>
            <ol>
                <li class="email">
                    <label for="email" @If Not ModelState.IsValidField("email") Then@<text>class="error-label"</text> End If>Email address</label>
                    <input type="text" id="email" name="email" value="@email" disabled="@passwordSent" @Validation.For("email") />
                    @Html.ValidationMessage("email")
                </li>
            </ol>
            <p class="form-actions">
                <input type="submit" value="Send instructions" disabled="@passwordSent" />
            </p>
        </fieldset>
    </form>
Else
   @<p class="message-info">
       Password recovery is disabled for this website because the SMTP server is 
       not configured correctly. Please contact the owner of this site to reset 
       your password.
   </p>
End If