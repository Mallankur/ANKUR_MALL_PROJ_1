using MediatR;

namespace Adform.BusinessAccount.Application.Queries
{
    public sealed class SsoConfigurationQueryByName : IRequest<Contracts.Entities.SsoConfiguration>
    {
        public string Name { get; }

        public SsoConfigurationQueryByName(string name)
        {
            Name = name.ToLowerInvariant();
        }
    }
}