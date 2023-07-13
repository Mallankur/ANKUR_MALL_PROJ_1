 using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Repositories;
using Adform.Ciam.Cache.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Adform.BusinessAccount.Infrastructure.Decorators;

public class BusinessAccountRepositoryCache : IBusinessAccountRepository
{
	private readonly IDistributedCache _cache;
	private readonly IBusinessAccountRepository _repository;

	public BusinessAccountRepositoryCache(IBusinessAccountRepository repository, IDistributedCache cache)
	{
		_repository = repository;
		_cache = cache;
	}

	public async Task<Domain.Repositories.Entities.BusinessAccount> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var account = await _cache.GetAsync<Domain.Repositories.Entities.BusinessAccount>($"id_{id}");
		if (account == null)
		{
			account = await _repository.GetByIdAsync(id);
			if (account != null)
				await SetToCache(account);
		}
		return account;
	}
	public async Task<Domain.Repositories.Entities.BusinessAccount> CreateAsync(Domain.Repositories.Entities.BusinessAccount businessAccount, CancellationToken cancellationToken = default)
	{
		return await _repository.CreateAsync(businessAccount);
	}

	public async Task<Guid?> DeleteAsync(Guid id, long version, CancellationToken cancellationToken = default)
	{
		var deletedId = await _repository.DeleteAsync(id, version, cancellationToken).ConfigureAwait(false);
		if (deletedId.HasValue)
			await RemoveFromCache(deletedId.Value, cancellationToken);
		return deletedId;
	}

	public async Task<PagedList<Domain.Repositories.Entities.BusinessAccount>> GetAllAsync(Page page, Order order, CancellationToken cancellationToken)
	{
		PagedList<Domain.Repositories.Entities.BusinessAccount> pagedList = await _repository.GetAllAsync(page, order, cancellationToken);
		await UpdateMultipleToCache(pagedList.Content, cancellationToken);
		return pagedList;
	}

    private async Task SetToCache(Domain.Repositories.Entities.BusinessAccount businessAccount)
	{
		await _cache.SetAsync($"id_{businessAccount.Id}", businessAccount,
					new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) });
	}

	private async Task RemoveFromCache(Guid id, CancellationToken cancellationToken)
	{
		await _cache.RemoveAsync($"id_{id}", cancellationToken).ConfigureAwait(false);
	}

	private async Task UpdateMultipleToCache(List<Domain.Repositories.Entities.BusinessAccount> businessAccounts, CancellationToken cancellationToken)
	{
		foreach (var businessAccount in businessAccounts)
		{
			if (await _cache.GetAsync<Domain.Repositories.Entities.BusinessAccount>($"id_{businessAccount.Id}") != null)
				await RemoveFromCache(businessAccount.Id, cancellationToken);
			await SetToCache(businessAccount);
		}
	}

}