using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Application.Handlers
{
    public class SsoconfigurationUpdatebyNameCommendHandler : IRequestHandler<SsoconfigurationCommenndbyName, Contracts.Entities.SsoConfiguration>
    {
        private readonly IMapper _mapper;
        private readonly ISsoConfigurationRepository _repository;

        public SsoconfigurationUpdatebyNameCommendHandler(IMapper mapper, ISsoConfigurationRepository repository)
        {
            _mapper = mapper;
            _repository = repository;

        }
        public async Task<Contracts.Entities.SsoConfiguration> Handle(SsoconfigurationCommenndbyName request, CancellationToken cancellationToken)
        {
            
            var result = await _repository.UpdateAsync(request.name,
                _mapper.Map<Domain.Repositories.Entities.SsoConfiguration>(request.ssoconfigurationcommendbyname),
                cancellationToken);
            return _mapper.Map<Contracts.Entities.SsoConfiguration>(result);
        }

    }
    
}
