using System;

namespace Adform.BusinessAccount.Contracts.Entities
{
    public class UpdateBusinessAccountInput
    {
        public Guid Id { get; set; }
        public int? LegacyId { get; set; }
        public bool IsActive { get; set; }
    }

}