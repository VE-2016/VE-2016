
using System;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    /// <summary>
    /// Holds information needed when an async web discovery call has completed.
    /// </summary>
    public class AsyncDiscoveryState
    {
        private WebServiceDiscoveryClientProtocol _protocol;
        private Uri _uri;
        private DiscoveryNetworkCredential _credential;

        public WebServiceDiscoveryClientProtocol Protocol
        {
            get
            {
                return _protocol;
            }
        }

        public Uri Uri
        {
            get
            {
                return _uri;
            }
        }

        public DiscoveryNetworkCredential Credential
        {
            get
            {
                return _credential;
            }
        }

        public AsyncDiscoveryState(WebServiceDiscoveryClientProtocol protocol, Uri uri, DiscoveryNetworkCredential credential)
        {
            _protocol = protocol;
            _uri = uri;
            _credential = credential;
        }
    }
}
