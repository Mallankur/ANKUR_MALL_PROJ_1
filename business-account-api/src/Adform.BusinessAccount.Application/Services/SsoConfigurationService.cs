using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Contracts;
using Adform.BusinessAccount.Contracts.Entities;
using MapsterMapper;
using MediatR;

namespace Adform.BusinessAccount.Application.Services;

public class SsoConfigurationService : ISsoConfigurationService
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public SsoConfigurationService(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}
    public async Task<SsoConfiguration> GetByDomainNameAsync(string domainName,
        CancellationToken cancellationToken = default)
    {
        Application.Queries.SsoConfigurationQueryByDomainName ssoConfigurationQueryByDomainName = new Queries.SsoConfigurationQueryByDomainName(domainName);    
        return await _mediator.Send(ssoConfigurationQueryByDomainName, cancellationToken);
    }

	public async Task<SsoConfiguration> CreateAsync(CreateSsoConfigurationInput configuration,
		CancellationToken cancellationToken = default)
	{
		var createCommand = _mapper.Map<SsoConfigurationCreateCommand>(configuration);
		var result = await _mediator.Send(createCommand, cancellationToken);

		return result;
	}

	public async Task<PagedList<SsoConfiguration>> GetAsync(SsoConfigurationType? type, Page page, Order order, CancellationToken cancellationToken = default)
	{
		var getSsoConfigurationQuery = new Queries.SsoConfigurationQuery(page, order, type);
		var result = await _mediator.Send(getSsoConfigurationQuery, cancellationToken);
		return result;
	}

    public async Task<SsoConfiguration> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
		Application.Queries.SsoConfigurationQueryByName ssoConfigurationQueryByName = new Queries.SsoConfigurationQueryByName(name);
		return await _mediator.Send(ssoConfigurationQueryByName, cancellationToken);
	}

	public async Task<SsoConfiguration> UpdateAsync(Guid Id, UpdateSsoConfigurationInput configuration, CancellationToken cancellationToken = default)
	{
        var result = await _mediator.Send(new SsoConfigurationUpdateCommand
		{ Id = Id, SsoConfiguration = configuration }, 
		cancellationToken);

        return result;
    }

	public async Task<SsoConfiguration> UpadteAsync(string name,UpdateSsoConfigurationInputbyName configuration, CancellationToken cancellationToken = default)
	{
		var result = await _mediator.Send(new SsoconfigurationCommenndbyName
		{
			name = name,
			ssoconfigurationcommendbyname = configuration
		}, cancellationToken);
		return result; 

	}
}