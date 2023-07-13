using Adform.BusinessAccount.Contracts.Entities;
using MediatR;

namespace Adform.BusinessAccount.Application.Commands
{
	public sealed class BusinessAccountCreateCommand : IRequest<Contracts.Entities.BusinessAccount>
	{
		public int? LegacyId { get; set; }
		public BusinessAccountType Type { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; }
		public Guid? SsoConfigurationId { get; set; }
	}

}