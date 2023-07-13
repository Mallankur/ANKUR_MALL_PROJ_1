using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Repositories;
using MapsterMapper;
using MediatR;
namespace Adform.BusinessAccount.Application.Handlers
{
    public class SsoConfigurationUpdateCommandHandler : IRequestHandler<SsoConfigurationUpdateCommand, Contracts.Entities.SsoConfiguration>
    {
        private readonly IMapper _mapper;
        private readonly ISsoConfigurationRepository _repository;

        public SsoConfigurationUpdateCommandHandler(IMapper mapper, ISsoConfigurationRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Contracts.Entities.SsoConfiguration> Handle(SsoConfigurationUpdateCommand request, CancellationToken cancellationToken)
        {
            var ssoConfiguration = _mapper.Map<Domain.Repositories.Entities.SsoConfiguration>(request);
           
            var result = await _repository.UpdateAsync(request.Id, 
                _mapper.Map<Domain.Repositories.Entities.SsoConfiguration>(request.SsoConfiguration), 
                cancellationToken);
            return _mapper.Map<Contracts.Entities.SsoConfiguration>(result);      
        }
    }
}
