using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.Membership.OpenAuth;

public partial class Account_OpenAuthProviders : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
        {
            $if$ ($targetframeworkversion$ == 4.0)
            Page.PreRenderComplete += Page_PreRenderComplete;

            $endif$
            if (IsPostBack)
            {
                var provider = Request.Form["provider"];
                if (provider == null)
                {
                    return;
                }
                
                var redirectUrl = "~/Account/RegisterExternalLogin";
                if (!String.IsNullOrEmpty(ReturnUrl))
                {
                    var resolvedReturnUrl = ResolveUrl(ReturnUrl);
                    redirectUrl += "?ReturnUrl=" + HttpUtility.UrlEncode(resolvedReturnUrl);
                }
                
                OpenAuth.RequestAuthentication(provider, redirectUrl);
            }
        }

        $if$ ($targetframeworkversion$ == 4.0)
        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            providersList.DataSource = OpenAuth.AuthenticationClients.GetAll();
            providersList.DataBind();
        }

        protected T Item<T>() where T : class
        {
            return Page.GetDataItem() as T ?? default(T);
        }
        $endif$

        public string ReturnUrl { get; set; }

        $if$ ($targetframeworkversion$ >= 4.5)
        public IEnumerable<ProviderDetails> GetProviderNames()
        {
            return OpenAuth.AuthenticationClients.GetAll();
        }
        $endif$
}