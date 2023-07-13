using Adform.BusinessAccount.Domain.Repositories;
using Adform.BusinessAccount.Domain.Repositories.Entities;
using Polly;
using Polly.Registry;

namespace Adform.BusinessAccount.Infrastructure.Decorators;

public class SsoConfigurationRepositoryRetry : ISsoConfigurationRepository
{
	private readonly IAsyncPolicy _retryPolicy;
	private readonly ISsoConfigurationRepository _repository;

	public SsoConfigurationRepositoryRetry(ISsoConfigurationRepository repository, IReadOnlyPolicyRegistry<string> policyRegistry)
	{
		_repository = repository;
		_retryPolicy = policyRegistry.Get<IAsyncPolicy>(PolicyNames.BasicRetry)
					   ?? Policy.NoOpAsync();
	}

	public async Task<SsoConfiguration> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _retryPolicy.AsAsyncPolicy<SsoConfiguration>()
		   .ExecuteAsync(() => _repository.GetByIdAsync(id));
	}

	public async Task<SsoConfiguration> GetByDomainAsync(string domainName, CancellationToken cancellationToken = default)
	{
		return await _retryPolicy.AsAsyncPolicy<SsoConfiguration>()
		  .ExecuteAsync(() => _repository.GetByDomainAsync(domainName));
	}

	public async Task<SsoConfiguration> CreateAsync(SsoConfiguration ssoConfiguration, CancellationToken cancellationToken = default)
	{
		return await _retryPolicy
				.ExecuteAsync(() => _repository.CreateAsync(ssoConfiguration, cancellationToken));
	}

	public async Task<SsoConfiguration> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return await _retryPolicy.AsAsyncPolicy<SsoConfiguration>()
			.ExecuteAsync(() => _repository.GetByNameAsync(name, cancellationToken));
	}

    public async Task<Contracts.Entities.PagedList<SsoConfiguration>> GetAsync(string type, Contracts.Entities.Page page, Contracts.Entities.Order order, CancellationToken cancellationToken = default)
    {
		return await _retryPolicy.AsAsyncPolicy<Contracts.Entities.PagedList<SsoConfiguration>>()
			.ExecuteAsync(() => _repository.GetAsync(type,page,order,cancellationToken));
	}

	public async Task<SsoConfiguration> UpdateAsync(Guid Id, SsoConfiguration ssoConfiguration, CancellationToken cancellationToken = default)
	{
        return await _retryPolicy
                .ExecuteAsync(() => _repository.UpdateAsync(Id,ssoConfiguration, cancellationToken));
    }

	public Task<BusinessAccount.Contracts.Entities.PagedList<SsoConfiguration>> GetAsync(string type, BusinessAccount.Contracts.Entities.Page page, BusinessAccount.Contracts.Entities.Order order, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<string?> DeleteAsync(string name, CancellationToken cancellationToken = default)
	{
		return await _retryPolicy
			                    .ExecuteAsync(() => _repository.DeleteAsync(name, cancellationToken));
	}
}

