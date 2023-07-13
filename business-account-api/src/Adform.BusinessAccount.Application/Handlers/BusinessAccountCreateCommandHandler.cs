using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace Adform.BusinessAccount.Application.Handlers
{

	public class BusinessAccountCreateCommandHandler : IRequestHandler<BusinessAccountCreateCommand, Contracts.Entities.BusinessAccount>
	{
		private readonly IMapper _mapper;
		private readonly IBusinessAccountRepository _repository;
		private readonly ISsoConfigurationRepository _ssorepository;

		public BusinessAccountCreateCommandHandler(IMapper mapper, IBusinessAccountRepository repository, ISsoConfigurationRepository ssoConfigurationRepository)
		{
			_mapper = mapper;
			_repository = repository;
			_ssorepository = ssoConfigurationRepository;
		}

		public async Task<Contracts.Entities.BusinessAccount> Handle(BusinessAccountCreateCommand request, CancellationToken cancellationToken)
		{
			var businessEntity = _mapper.Map<Domain.Repositories.Entities.BusinessAccount>(request);
			var result = await _repository.CreateAsync(businessEntity, cancellationToken);
			var createdAccount = _mapper.Map<Contracts.Entities.BusinessAccount>(result);

			if (result != null && result.SsoConfigurationId.HasValue)
			{
				var ssoConfiguration = await _ssorepository.GetByIdAsync(result.SsoConfigurationId.Value, cancellationToken);
				var ssoConfigurationEntity = _mapper.Map<Contracts.Entities.SsoConfiguration>(ssoConfiguration);
				createdAccount.SsoConfiguration = ssoConfigurationEntity;
			}

			return createdAccount;
		}
	}

}