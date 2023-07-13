namespace Adform.BusinessAccount.Contracts.Entities
{
	public class CreateSsoConfigurationInput
	{
		private SsoConfigurationType? _ssoConfigurationType;

		public string Name { get; set; }
		public string[] Domains { get; set; }
		public Oidc Oidc { get; set; }
		public Saml2 Saml2 { get; set; }

		public SsoConfigurationType? Type
		{
			get
			{
				if (!_ssoConfigurationType.HasValue)
				{
					if (Oidc != null)
					{
						_ssoConfigurationType = SsoConfigurationType.Oidc;
					}
					else if (Saml2 != null)
					{
						_ssoConfigurationType = SsoConfigurationType.Saml2;
					}
				}

				return _ssoConfigurationType;
			}
			set
			{
				_ssoConfigurationType = value;
			}
		}
	}
}