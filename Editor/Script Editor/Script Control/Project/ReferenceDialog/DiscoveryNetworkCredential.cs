using System;
using System.Net;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    /// <summary>
    /// Adds an authentication type to the standard NetworkCredential class.
    /// </summary>
    public class DiscoveryNetworkCredential : NetworkCredential
    {
        public const string DefaultAuthenticationType = "Default";

        private string _authenticationType = String.Empty;

        public DiscoveryNetworkCredential(string userName, string password, string domain, string authenticationType) : base(userName, password, domain)
        {
            _authenticationType = authenticationType;
        }

        public DiscoveryNetworkCredential(NetworkCredential credential, string authenticationType) : this(credential.UserName, credential.Password, credential.Domain, authenticationType)
        {
        }

        public string AuthenticationType
        {
            get
            {
                return _authenticationType;
            }
        }

        public bool IsDefaultAuthenticationType
        {
            get
            {
                return String.Compare(_authenticationType, DefaultAuthenticationType, true) == 0;
            }
        }
    }
}
