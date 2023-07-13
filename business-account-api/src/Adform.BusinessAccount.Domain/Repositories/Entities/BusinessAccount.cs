using Adform.Ciam.SharedKernel.Entities;
using System;

namespace Adform.BusinessAccount.Domain.Repositories.Entities;

public class BusinessAccount : IEntity<Guid>
{
	public Guid Id { get; set; }
	public int? LegacyId { get; set; }
	public string Type { get; set; }
	public string Name { get; set; }
	public bool IsActive { get; set; }
	public Guid? SsoConfigurationId { get; set; }
	public SsoConfiguration? SsoConfiguration { get; set; }

	public long CreatedAt { get; set; }
	public long UpdatedAt { get; set; }
}