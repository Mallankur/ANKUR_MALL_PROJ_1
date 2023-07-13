using Adform.BusinessAccount.Application.Queries;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Repositories;
using MapsterMapper;
using MediatR;
using Mapster;

namespace Adform.BusinessAccount.Application.Handlers
{
	public class BusinessAccountQueryHandler :
		 IRequestHandler<BusinessAccountQueryById, Contracts.Entities.BusinessAccount?>,
		 IRequestHandler<BusinessAccountQuery, PagedList<Contracts.Entities.BusinessAccount>>
	{
		private readonly IBusinessAccountRepository _repository;
		private readonly ISsoConfigurationRepository _ssorepository;
		private readonly IMapper _mapper;

		public BusinessAccountQueryHandler(IMapper mapper, IBusinessAccountRepository repository, ISsoConfigurationRepository ssoConfigurationRepository)
		{
			_mapper = mapper;
			_repository = repository;
			_ssorepository = ssoConfigurationRepository;
		}
		public async Task<Contracts.Entities.BusinessAccount?> Handle(BusinessAccountQueryById request, CancellationToken cancellationToken)
		{
			var content = await _repository.GetByIdAsync(request.Id, cancellationToken);
			var businessEntity = _mapper.Map<Contracts.Entities.BusinessAccount>(content);
			if (content != null && content.SsoConfigurationId.HasValue)
			{
				var ssoConfiguration = await _ssorepository.GetByIdAsync(content.SsoConfigurationId.Value, cancellationToken);
				var ssoConfigurationEntity = _mapper.Map<Contracts.Entities.SsoConfiguration>(ssoConfiguration);
				businessEntity.SsoConfiguration = ssoConfigurationEntity;
			}
			return businessEntity;
		}
				
		public async Task<PagedList<Contracts.Entities.BusinessAccount>> Handle(BusinessAccountQuery request, CancellationToken cancellationToken)
		{
			var pagedList = await _repository.GetAllAsync(request.Page, request.Order, cancellationToken);
			var repoResult = new PagedList<Domain.Repositories.Entities.BusinessAccount>()
			{
				Page = pagedList.Page,
				Order = pagedList.Order,
				TotalCount = pagedList.TotalCount,
				Content = new List<Domain.Repositories.Entities.BusinessAccount>()
			};

			// group by ssoConfigurationId
			var businessAccountGroupBySso = pagedList.Content.GroupBy(x => x.SsoConfigurationId);

			foreach (var businessAccountGroup in businessAccountGroupBySso)
			{
				if (businessAccountGroup.Key.HasValue)
				{
					// get ssoConfiguration for each grouping key
					var ssoConfiguration = await _ssorepository.GetByIdAsync(businessAccountGroup.Key.Value, cancellationToken);// updated all ssoConfiguraiton in the entities within same group
                    repoResult.Content.AddRange(businessAccountGroup.Select(x =>
					{
						x.SsoConfiguration = ssoConfiguration;
						return x;
					}));
				}
				else
				{
                    repoResult.Content.AddRange(businessAccountGroup);
				}
			}

            return repoResult.Adapt<PagedList<Contracts.Entities.BusinessAccount>>();
        }
    }
}