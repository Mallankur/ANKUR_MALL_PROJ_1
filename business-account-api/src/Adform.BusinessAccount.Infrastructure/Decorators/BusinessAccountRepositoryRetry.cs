using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Repositories;
using Polly;
using Polly.Registry;

namespace Adform.BusinessAccount.Infrastructure.Decorators;
public class BusinessAccountRepositoryRetry : IBusinessAccountRepository
{
	private readonly IAsyncPolicy _retryPolicy;
	private readonly IBusinessAccountRepository _repository;

	public BusinessAccountRepositoryRetry(IBusinessAccountRepository repository, IReadOnlyPolicyRegistry<string> policyRegistry)
	{
		_repository = repository;
		_retryPolicy = policyRegistry.Get<IAsyncPolicy>(PolicyNames.BasicRetry)
					   ?? Policy.NoOpAsync();
	}

	public async Task<Domain.Repositories.Entities.BusinessAccount> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _retryPolicy.AsAsyncPolicy<Domain.Repositories.Entities.BusinessAccount?>()
		   .ExecuteAsync(() => _repository.GetByIdAsync(id));
	}

	public async Task<Domain.Repositories.Entities.BusinessAccount> CreateAsync(Domain.Repositories.Entities.BusinessAccount businessAccount, CancellationToken cancellationToken = default)
	{
		return await _retryPolicy
				.ExecuteAsync(() => _repository.CreateAsync(businessAccount, cancellationToken));
	}

    public async Task<Guid?> DeleteAsync(Guid id, long version, CancellationToken cancellationToken = default)
    {
		return await _retryPolicy
            .ExecuteAsync(() => _repository.DeleteAsync(id, version,cancellationToken));
	}

    public async Task<PagedList<Domain.Repositories.Entities.BusinessAccount>> GetAllAsync(Page page, Order order, CancellationToken cancellationToken)
    {
        return await _retryPolicy.AsAsyncPolicy<PagedList<Domain.Repositories.Entities.BusinessAccount>>()
            .ExecuteAsync(() => _repository.GetAllAsync(page, order, cancellationToken));

    }
}