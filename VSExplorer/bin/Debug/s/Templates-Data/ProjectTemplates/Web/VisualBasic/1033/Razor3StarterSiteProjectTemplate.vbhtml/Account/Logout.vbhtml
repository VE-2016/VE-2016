@Code
    WebSecurity.RequireAuthenticatedUser()

    If IsPost Then
        ' Verify the request was submitted by the user
        AntiForgery.Validate()

        ' Log out of the current user context
        WebSecurity.Logout()

        ' Redirect back to the return URL or homepage
        Dim returnUrl As String = Request.QueryString("ReturnUrl")
        Context.RedirectLocal(returnUrl)
    Else
        Response.Redirect("~/")
    End If
End Code