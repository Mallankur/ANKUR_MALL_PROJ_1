using Adform.BusinessAccount.Contracts.Entities;
using MediatR;

namespace Adform.BusinessAccount.Application.Commands
{
	public class SsoConfigurationCreateCommand : IRequest<SsoConfiguration>
	{
		public string Name { get; set; }
		public string[] Domains { get; set; }
		public SsoConfigurationType Type { get; set; }
		public Oidc? Oidc { get; set; }
		public Saml2? Saml2 { get; set; }
	}

	//public class Oidc
	//{
	//	public string Authority { get; set; }
	//	public string ClientId { get; set; }
	//	public string RedirectUri { get; set; }
	//	public string[] Scope { get; set; }
	//}

	//public class Saml2
	//{
	//	public string EntityID { get; set; }
	//	public string? MetadataLocation { get; set; }
	//}

}