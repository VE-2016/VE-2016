Imports Microsoft.AspNet.Membership.OpenAuth

Partial Class Account_OpenAuthProviders
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        $if$ ($targetframeworkversion$ == 4.0)
        AddHandler Page.PreRenderComplete, AddressOf Page_PreRenderComplete

        $endif$
        If IsPostBack Then
            Dim provider = Request.Form("provider")
            If provider Is Nothing Then
                Return
            End If

            Dim redirectUrl = "~/Account/RegisterExternalLogin"
            If Not String.IsNullOrEmpty(ReturnUrl) Then
                Dim resolvedReturnUrl = ResolveUrl(ReturnUrl)
                redirectUrl += "?ReturnUrl=" + HttpUtility.UrlEncode(resolvedReturnUrl)
            End If

            OpenAuth.RequestAuthentication(provider, redirectUrl)
        End If
    End Sub

    $if$ ($targetframeworkversion$ == 4.0)
    Protected Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs)
        providersList.DataSource = OpenAuth.AuthenticationClients.GetAll()
        providersList.DataBind()
    End Sub

    Protected Function Item(Of T As Class)() As T
        Return TryCast(Page.GetDataItem(), T)
    End Function
    $endif$

    Public Property ReturnUrl As String

    $if$ ($targetframeworkversion$ >= 4.5)
    Public Function GetProviderNames() As IEnumerable(Of ProviderDetails)
        Return OpenAuth.AuthenticationClients.GetAll()
    End Function
    $endif$
End Class
