using System;

namespace Adform.BusinessAccount.Contracts.Entities
{
	public class BusinessAccount
	{
		public Guid Id { get; set; }
		public int? LegacyId { get; set; }
		public BusinessAccountType Type { get; set; }
		public string Name { get; set; }
		public SsoConfiguration SsoConfiguration { get; set; }
		public bool IsActive { get; set; }
	}
}