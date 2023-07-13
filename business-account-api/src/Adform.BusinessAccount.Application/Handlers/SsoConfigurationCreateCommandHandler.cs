using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Domain.Repositories;
using Adform.BusinessAccount.Domain.Repositories.Entities;
using MapsterMapper;
using MediatR;

namespace Adform.BusinessAccount.Application.Handlers;
public class SsoConfigurationCreateCommandHandler : IRequestHandler<SsoConfigurationCreateCommand, Contracts.Entities.SsoConfiguration>
{
	private readonly IMapper _mapper;
	private readonly ISsoConfigurationRepository _repository;

	public SsoConfigurationCreateCommandHandler(IMapper mapper, ISsoConfigurationRepository repository)
	{
		_mapper = mapper;
		_repository = repository;
	}

	public async Task<Contracts.Entities.SsoConfiguration> Handle(SsoConfigurationCreateCommand request, CancellationToken cancellationToken)
	{
		var ssoConfiguration = _mapper.Map<SsoConfiguration>(request);
		var result = await _repository.CreateAsync(ssoConfiguration, cancellationToken);
		return _mapper.Map<Contracts.Entities.SsoConfiguration>(result);
	}
}
