using MediatR;

namespace Adform.BusinessAccount.Application.Queries
{
    public sealed class SsoConfigurationQueryByDomainName : IRequest<Contracts.Entities.SsoConfiguration>
    {
        public string DomainName { get; }

        public SsoConfigurationQueryByDomainName(string domainName)
        {
            DomainName = domainName.ToLowerInvariant();
        }
    }
}