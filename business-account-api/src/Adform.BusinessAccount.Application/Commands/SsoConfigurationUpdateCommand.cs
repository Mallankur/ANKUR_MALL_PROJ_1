using Adform.BusinessAccount.Contracts.Entities;
using MediatR;

namespace Adform.BusinessAccount.Application.Commands
{
    public class SsoConfigurationUpdateCommand: IRequest<SsoConfiguration>
    {
        public Guid Id { get; set; }
        public UpdateSsoConfigurationInput SsoConfiguration { get; set; }=default!;
    }
}
