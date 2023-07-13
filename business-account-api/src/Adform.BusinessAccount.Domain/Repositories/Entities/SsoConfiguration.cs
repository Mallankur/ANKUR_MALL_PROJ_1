using Adform.Ciam.SharedKernel.Entities;
using System;

namespace Adform.BusinessAccount.Domain.Repositories.Entities;
public class SsoConfiguration : IEntity<Guid>
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Type { get; set; }
	public string[] Domains { get; set; }
	public Oidc? Oidc { get; set; }
	public Saml2? Saml2 { get; set; }
	public long CreatedAt { get; set; }
	public long UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true ; 

}