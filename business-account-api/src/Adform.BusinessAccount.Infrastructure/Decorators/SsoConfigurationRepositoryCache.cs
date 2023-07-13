using Adform.BusinessAccount.Domain.Repositories;
using Adform.BusinessAccount.Domain.Repositories.Entities;
using Adform.Ciam.Cache.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Adform.BusinessAccount.Infrastructure.Decorators;

public class SsoConfigurationRepositoryCache : ISsoConfigurationRepository
{
	private readonly IDistributedCache _cache;
	private readonly ISsoConfigurationRepository _repository;
	const string SsoName_ = "SsoName_";

	public SsoConfigurationRepositoryCache(ISsoConfigurationRepository repository, IDistributedCache cache)
	{
		_repository = repository;
		_cache = cache;
	}

	public async Task<Domain.Repositories.Entities.SsoConfiguration> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var ssoConfiguration = await _cache.GetAsync<Domain.Repositories.Entities.SsoConfiguration>($"ssoid_{id}");
		if (ssoConfiguration == null)
		{
			ssoConfiguration = await _repository.GetByIdAsync(id);
			if (ssoConfiguration != null)
			{
				await SetToCache(ssoConfiguration);
			}
		}
		return ssoConfiguration;
	}

	public async Task<Domain.Repositories.Entities.SsoConfiguration> GetByDomainAsync(string domainName, CancellationToken cancellationToken = default)
	{
		SsoConfiguration ssoConfiguration;
		string id= await _cache.GetAsync<string>($"ssodomain_{domainName}");
		if(!string.IsNullOrEmpty(id))
        {
			var guid = new Guid(id);
			ssoConfiguration= await GetByIdAsync(guid,cancellationToken);
		}
        else
        {
			ssoConfiguration = await _repository.GetByDomainAsync(domainName);
			if (ssoConfiguration != null)
			{
				await _cache.SetAsync<string>($"ssodomain_{domainName}", ssoConfiguration.Id.ToString(),
					new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) });
				await SetToCache(ssoConfiguration);
			}
		}
		return ssoConfiguration;
	}

	public async Task<SsoConfiguration> CreateAsync(SsoConfiguration ssoConfiguration, CancellationToken cancellationToken = default)
	{
		return await _repository.CreateAsync(ssoConfiguration, cancellationToken);
	}

	public async Task<SsoConfiguration> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		SsoConfiguration ssoConfiguration;
		string id = await _cache.GetAsync<string>($"{SsoName_}{name}");
		if (!string.IsNullOrEmpty(id))
		{
			var guid = new Guid(id);
			ssoConfiguration = await GetByIdAsync(guid, cancellationToken);
		}
		else
		{
			ssoConfiguration = await _repository.GetByNameAsync(name, cancellationToken);
			if (ssoConfiguration != null)
			{
				await _cache.SetAsync<string>($"ssoName_{name}", ssoConfiguration.Id.ToString(),
					new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) });
				await SetToCache(ssoConfiguration);
			}
		}
		return ssoConfiguration;
	}

    public async Task<Contracts.Entities.PagedList<SsoConfiguration>> GetAsync(string type, Contracts.Entities.Page page, Contracts.Entities.Order order, CancellationToken cancellationToken = default)
    {
		return await _repository.GetAsync(type,page,order, cancellationToken);
	}

	private async Task SetToCache(SsoConfiguration ssoConfiguration)
	{
		await _cache.SetAsync($"ssoid_{ssoConfiguration.Id}", ssoConfiguration,
					new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) });

	}

	public async Task<SsoConfiguration> UpdateAsync(Guid Id, SsoConfiguration updatessoConfiguration, CancellationToken cancellationToken = default)
	{
        var result = await _repository.UpdateAsync(Id, updatessoConfiguration, cancellationToken).ConfigureAwait(false);
		if (result != null)
		{ 
			await RemoveFromCache(Id, cancellationToken); 
		}
        return result;
    }
    private async Task RemoveFromCache(Guid id, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync($"ssoid_{id}", cancellationToken).ConfigureAwait(false);
    }

	public Task<BusinessAccount.Contracts.Entities.PagedList<SsoConfiguration>> GetAsync(string type, BusinessAccount.Contracts.Entities.Page page, BusinessAccount.Contracts.Entities.Order order, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<string?> DeleteAsync(string name, CancellationToken cancellationToken = default)
	{
		var result = await _repository.DeleteAsync(name, cancellationToken);
		if (result!= null)
		{
			return  await _cache.RemoveAsync($"{ SsoName_}{name}",cancellationToken).ConfigureAwait(false);	
		}

		return result;

	
}