using System;

namespace Adform.BusinessAccount.Contracts.Entities
{
	public class SsoConfiguration
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public SsoConfigurationType Type { get; set; }
		public string[] Domains { get; set; }
		public Oidc Oidc { get; set; }
		public Saml2 Saml2 { get; set; }
	}
}