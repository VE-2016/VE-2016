@Code
    Layout = "~/_SiteLayout.vbhtml"
    PageData("Title") = "Account Locked"
End Code

<hgroup class="title">
    <h1 class="error">@PageData("Title").</h1>
    <h2 class="error">Your account was locked out due to too many invalid log in attempts.</h2>
</hgroup>

<p>
    Don't worry, the account will automatically be unlocked in 60 seconds. 
    Please try again after that time has passed.
</p>