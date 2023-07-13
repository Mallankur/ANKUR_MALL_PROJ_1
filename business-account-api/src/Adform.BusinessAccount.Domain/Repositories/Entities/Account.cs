using System;
using Adform.Ciam.SharedKernel.Entities;

namespace Adform.BusinessAccount.Domain.Repositories.Entities
{
    public class Account
    {
        public string Id { get; set; }
        public string PrimaryEmail { get; set; }
        public string Type { get; set; }
        public string ExternalId { get; set; }
        public string IdentityProvider { get; set; }
        
    }

}