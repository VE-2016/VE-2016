@Code
    Layout = "~/_SiteLayout.vbhtml"
    PageData("Title") = "Registration Confirmation Page"

    Dim message As String = ""
    Dim confirmationToken As String = Request("confirmationCode")

    WebSecurity.Logout()
    If Not confirmationToken.IsEmpty() Then
        If WebSecurity.ConfirmAccount(confirmationToken) Then
            message = "Registration Confirmed! Click on the log in tab to log in to the site."
        Else
            message = "Could not confirm your registration info."
        End If
    End If
End Code

<hgroup class="title">
    <h1>@PageData("Title").</h1>
    <h2>Use the form below to confirm your account.</h2>
</hgroup>

@If Not message.IsEmpty() Then
    @<p>@message</p>
Else
    @<form method="post">
        <fieldset>
            <legend>Confirmation Code</legend>
            <ol>
                <li>
                    <label for="confirmationCode">Confirmation code</label>
                    <input type="text" id="confirmationCode" name="confirmationCode" />
                </li>
            </ol>
            <input type="submit" value="Confirm" />
        </fieldset>
    </form>
End If