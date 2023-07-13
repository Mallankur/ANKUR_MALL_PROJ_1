using Adform.BusinessAccount.Application.Queries;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace Adform.BusinessAccount.Application.Handlers
{
	public class SsoConfigurationQueryHandler :
		 IRequestHandler<SsoConfigurationQueryByDomainName, Contracts.Entities.SsoConfiguration>,
		 IRequestHandler<SsoConfigurationQueryByName, Contracts.Entities.SsoConfiguration>,
		 IRequestHandler<SsoConfigurationQuery, PagedList<Contracts.Entities.SsoConfiguration>>
	{
		private readonly ISsoConfigurationRepository _repository;
		private readonly IMapper _mapper;

		public SsoConfigurationQueryHandler(IMapper mapper, ISsoConfigurationRepository repository)
		{
			_mapper = mapper;
			_repository = repository;
		}

		public async Task<Contracts.Entities.SsoConfiguration> Handle(SsoConfigurationQueryByDomainName request, CancellationToken cancellationToken)
		{
			var content = await _repository.GetByDomainAsync(request.DomainName, cancellationToken);
			var resultEntity = _mapper.Map<Contracts.Entities.SsoConfiguration>(content);
			return resultEntity;
		}

		public async Task<Contracts.Entities.SsoConfiguration> Handle(SsoConfigurationQueryByName request, CancellationToken cancellationToken)
		{
			var content = await _repository.GetByNameAsync(request.Name, cancellationToken);
			var resultEntity = _mapper.Map<Contracts.Entities.SsoConfiguration>(content);
			return resultEntity;
		}    

        public async Task<PagedList<SsoConfiguration>> Handle(SsoConfigurationQuery request, CancellationToken cancellationToken)
        {
			var pagedList = await _repository.GetAsync(request.Type, request.Page, request.Order, cancellationToken);
			var resultEntity = _mapper.Map<PagedList<SsoConfiguration>>(pagedList);
			return resultEntity;

		}
    }
}