using System;

namespace Adform.BusinessAccount.Contracts.Entities
{
	public class CreateBusinessAccountInput
	{
		public int? LegacyId { get; set; }
		public BusinessAccountType Type { get; set; }
		public string Name { get; set; }
		public Guid? SsoConfigurationId { get; set; }
		public bool IsActive { get; set; }
	}

}