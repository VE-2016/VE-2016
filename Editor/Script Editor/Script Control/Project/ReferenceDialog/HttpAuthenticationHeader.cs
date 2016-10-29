
using System;
using System.Net;
using System.Text;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    /// <summary>
    /// Represents the WWW-Authenticate HTTP response header.
    /// </summary>
    public class HttpAuthenticationHeader
    {
        private string[] _authenticationSchemes;

        public HttpAuthenticationHeader(WebHeaderCollection headers)
        {
            _authenticationSchemes = headers.GetValues("WWW-Authenticate");
        }

        public override string ToString()
        {
            if (HasAuthenticationSchemes)
            {
                StringBuilder schemes = new StringBuilder();
                foreach (string scheme in _authenticationSchemes)
                {
                    schemes.Append("WWW-Authenticate: ");
                    schemes.Append(scheme);
                    schemes.Append("\r\n");
                }
                return schemes.ToString();
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets a comma separated list of authentication types.
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                if (HasAuthenticationSchemes)
                {
                    int schemesAdded = 0;
                    StringBuilder authenticationType = new StringBuilder();
                    for (int i = 0; i < _authenticationSchemes.Length; ++i)
                    {
                        string scheme = _authenticationSchemes[i];
                        int index = scheme.IndexOf(' ');
                        if (index > 0)
                        {
                            scheme = scheme.Substring(0, index);
                        }
                        if (schemesAdded > 0)
                        {
                            authenticationType.Append(",");
                        }
                        authenticationType.Append(scheme);
                        schemesAdded++;
                    }
                    return authenticationType.ToString();
                }
                return String.Empty;
            }
        }

        private bool HasAuthenticationSchemes
        {
            get
            {
                return _authenticationSchemes != null && _authenticationSchemes.Length > 0;
            }
        }
    }
}
