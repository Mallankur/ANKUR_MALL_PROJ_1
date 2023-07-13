using System;
using System.Collections.Generic;
using System.Text;

namespace Adform.BusinessAccount.Contracts.Entities
{
    public class UpdateSsoConfigurationInput
    {
        public string Name { get; set; }
        public string[] Domains { get; set; }
        public Oidc Oidc { get; set; }
        public Saml2 Saml2 { get; set; }
        public SsoConfigurationType Type { get; set; }
    }
}
