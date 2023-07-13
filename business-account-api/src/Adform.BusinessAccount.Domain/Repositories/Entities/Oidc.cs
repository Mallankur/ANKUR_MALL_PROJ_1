namespace Adform.BusinessAccount.Domain.Repositories.Entities;

public class Oidc
{
	public string Authority { get; set; }
	public string ClientId { get; set; }
	public string ClientSecret { get; set; }
	public string RedirectUri { get; set; }
	public string[] Scope { get; set; }
}