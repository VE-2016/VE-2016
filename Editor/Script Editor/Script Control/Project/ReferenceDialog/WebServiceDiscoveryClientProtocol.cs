using System;
using System.Net;
using System.Web.Services.Discovery;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    /// <summary>
    /// Custom DiscoveryClientProtocol that determines whether user authentication
    /// is required.
    /// </summary>
    public class WebServiceDiscoveryClientProtocol : DiscoveryClientProtocol
    {
        private HttpWebResponse _lastResponseReceived;

        public WebServiceDiscoveryClientProtocol()
        {
        }

        public HttpAuthenticationHeader GetAuthenticationHeader()
        {
            if (_lastResponseReceived != null)
            {
                return new HttpAuthenticationHeader(_lastResponseReceived.Headers);
            }
            return null;
        }

        public bool IsAuthenticationRequired
        {
            get
            {
                if (_lastResponseReceived != null)
                {
                    return _lastResponseReceived.StatusCode == HttpStatusCode.Unauthorized;
                }
                return false;
            }
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            _lastResponseReceived = response as HttpWebResponse;
            return response;
        }
    }
}
