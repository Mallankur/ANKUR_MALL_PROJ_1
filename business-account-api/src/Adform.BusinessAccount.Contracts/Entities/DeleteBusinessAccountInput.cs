using System;

namespace Adform.BusinessAccount.Contracts.Entities
{

    public class DeleteBusinessAccountInput
    {
        public Guid Id { get; set; }
        public long Version { get; set; }
    }

}