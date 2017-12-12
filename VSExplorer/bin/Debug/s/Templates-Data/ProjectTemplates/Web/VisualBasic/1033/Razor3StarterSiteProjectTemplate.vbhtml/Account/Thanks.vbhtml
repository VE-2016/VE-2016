@Code
    ' Set the layout page and page title
    Layout = "~/_SiteLayout.vbhtml"
    PageData("Title") = "Thanks for registering"
End Code

@If Not WebSecurity.IsAuthenticated Then
    @<hgroup class="title">
        <h1>@PageData("Title").</h1>
        <h2>But you're not done yet!</h2>
    </hgroup>

    @<p>
       An email with instructions on how to activate your account is on its way to you.
    </p>
Else
    @<hgroup class="title">
        <h1>@PageData("Title").</h1>
        <h2>You are all set.</h2>
    </hgroup>

    @<p>
        It looks like you've already confirmed your account and are good to go.
    </p>
End If