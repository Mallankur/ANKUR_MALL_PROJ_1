
using Adform.BusinessAccount.Domain.Exceptions;
using Adform.BusinessAccount.Domain.Repositories;
using Adform.BusinessAccount.Domain.Repositories.Entities;
using Adform.Ciam.Mongo.Repositories;
using MongoDB.Driver;

namespace Adform.BusinessAccount.Infrastructure.Repositories;

public class SsoConfigurationRepository : ISsoConfigurationRepository
{
	private readonly IBaseMongoRepository<SsoConfiguration, Guid> _repository;

	public SsoConfigurationRepository(IBaseMongoRepository<SsoConfiguration, Guid> repository)
	{
		_repository = repository;
	}

	public async Task<SsoConfiguration> CreateAsync(SsoConfiguration ssoConfiguration, CancellationToken cancellationToken = default)
	{
		var validationErrors = await ValidateSsoConfiguration(ssoConfiguration, null,cancellationToken);

		if (validationErrors.Count == 0)
		{
			try
			{
				return await _repository.InsertAsync(ssoConfiguration, cancellationToken);
			}
			catch (MongoWriteException ex)
			{
				if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
				{
					validationErrors.Add(nameof(ssoConfiguration.Name), new ErrorDto(Messages.SsoConfigurationExist));
				}
				else
				{
					throw;
				}
			}
		}

		throw new ValidationException(validationErrors);
	}

	public async Task<string?> DeleteAsync(string  name,  CancellationToken cancellationToken = default)
	{
        var  filter = Builders<Domain.Repositories.Entities.SsoConfiguration>.Filter.And(
                  Builders<Domain.Repositories.Entities.SsoConfiguration>.Filter.Eq(u =>u.Name, name),
                  Builders<Domain.Repositories.Entities.SsoConfiguration>.Filter.Eq(u => u.IsActive, true));

		var deletename = await _repository.SearchfirstAsync(filter, CancellationToken);
		 if(deletename == null)
		{
			return null;
		}
		var result = await _repository.Collection.UpdateOneAsync(
			filter,
			Builders<Domain.Repositories.Entities.SsoConfiguration>.Update.Set(u => u.IsActive, false),
			null, CancellationToken);
		if (result.IsAcknowladment && result.ModifiedCount == 1)
		{
			return name;

		}
		    return null; 
		

	public async Task<Contracts.Entities.PagedList<SsoConfiguration>> GetAsync(string type, Contracts.Entities.Page page, Contracts.Entities.Order order, CancellationToken cancellationToken = default)
	{
		List<SsoConfiguration> content;
		if (!string.IsNullOrEmpty(type))
			content = await _repository.Collection.Find(o => o.Type.Equals(type)).ToListAsync().ConfigureAwait(false);
		else
			content = await _repository.Collection.Find(o => true).ToListAsync().ConfigureAwait(false);
		IOrderedEnumerable<SsoConfiguration> sortedList;
		if (order.OrderDirection == Contracts.Entities.OrderDirection.Ascending)
			sortedList = content.OrderBy(x => x.Id);
		else
			sortedList = content.OrderByDescending(x => x.Id);

		var pagedList = new Contracts.Entities.PagedList<SsoConfiguration>()
		{
			Content = sortedList.Skip(page.Offset).Take(page.Limit).ToList(),
			Page = page,
			Order = order,
			TotalCount = content.Count
		};
		return pagedList;
	}

	public Task<BusinessAccount.Contracts.Entities.PagedList<SsoConfiguration>> GetAsync(string type, BusinessAccount.Contracts.Entities.Page page, BusinessAccount.Contracts.Entities.Order order, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<SsoConfiguration> GetByDomainAsync(string domainName,
		CancellationToken cancellationToken = default)
	{
		var filter = Builders<SsoConfiguration>.Filter.Where(u => u.Domains.Contains(domainName));
		return await _repository.SearchFirstAsync(filter, cancellationToken);
	}

	public async Task<SsoConfiguration> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var filter = Builders<SsoConfiguration>.Filter.Eq(u => u.Id, id);
		return await _repository.SearchFirstAsync(filter, cancellationToken);
	}

	public async Task<SsoConfiguration> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		var filter = Builders<SsoConfiguration>.Filter.Where(x => x.Name == name);
		return await _repository.SearchFirstAsync(filter, cancellationToken);
	}
		// Update by Name 
	public async Task<SsoConfiguration> UpdateAsync(Guid Id, SsoConfiguration updatessoConfiguration, CancellationToken cancellationToken = default)
	{
        var ssoConfigurationById = await _repository.GetByIdAsync(Id, cancellationToken);
        var validationErrors = await ValidateSsoConfiguration( updatessoConfiguration, name,cancellationToken);

        if (ssoConfigurationById is null)
		{
            validationErrors.Add(nameof(Id), new ErrorDto(Messages.SsoConfigurationNotExist));
            
        }
        if (validationErrors.Count == 0) 
        {
            try
            {
				var update = Builders<SsoConfiguration>.Update.Set(x => x.Name, ssoConfiguration.Name)
					.Set(x => x.Type, ssoConfiguration.Type)
					.Set(x => x.Saml2, ssoConfiguration.Saml2)
					.Set(x => x.Oidc, ssoConfiguration.Oidc)
					.Set(x => x.Domains, ssoConfiguration.Domains);
				var result = await _repository.UpdateAsync(Id, ssoConfigurationById.UpdatedAt, update, cancellationToken);               
				return result;

			}
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    validationErrors.Add(nameof(ssoConfiguration.Name), new ErrorDto(Messages.SsoConfigurationExist));
                }
                else
                {
                    throw;
                }
            }
			catch(Exception ex)
			{
				throw ;
			}
			
        }

        throw new ValidationException(validationErrors);
    }public async Task<SsoConfiguration> UpdateAsyncbyname(string  name, SsoConfiguration updatessoConfiguration, CancellationToken cancellationToken = default)
	{
			var ssoConfigurationByname = await GetByNameAsync(name, cancellationToken);
        var validationErrors = await ValidateSsoConfiguration( updatessoConfiguration, name,cancellationToken);

        if (ssoConfigurationByname is null)
		{
            validationErrors.Add(nameof(name), new ErrorDto(Messages.SsoConfigurationNotExist));
            
        }
        if (validationErrors.Count == 0) 
        {
            try
            {
				var update = Builders<SsoConfiguration>.Update
					.Set(x => x.Type, ssoConfiguration.Type)
					.Set(x => x.Saml2, ssoConfiguration.Saml2)
					.Set(x => x.Oidc, ssoConfiguration.Oidc)
					.Set(x => x.Domains, ssoConfiguration.Domains);
				var result = await _repository.UpdateAsync(name, ssoConfigurationByname.UpdatedAt, update, cancellationToken);               
				return result;

			}
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    validationErrors.Add(nameof(ssoConfiguration.Name), new ErrorDto(Messages.SsoConfigurationExist));
                }
                else
                {
                    throw;
                }
            }
			catch(Exception ex)
			{
				throw ;
			}
			
        }

        throw new ValidationException(validationErrors);
    
    };

	private async Task<SsoConfiguration> GetByDomainsAsync(string[] domainName, CancellationToken cancellationToken)
	{
		var filter = Builders<SsoConfiguration>.Filter.Where(u => domainName.Any(x => u.Domains.Contains(x)));
		var result = await _repository.SearchFirstAsync(filter, cancellationToken);
		return result;
	}

	private async Task<IDictionary<string, object>> ValidateSsoConfiguration(SsoConfiguration ssoConfiguration,string name, CancellationToken cancellationToken)
	{
		var errors = new Dictionary<string, object>();

		// validate duplicate domains do not exist.
		try
		{
			var filter = Builders<SsoConfiguration>.Filter.Where(u => ssoConfiguration.Domains.Any(x => u.Domains.Contains(x)) && (name == null || u.name != name));

			
			var ssoConfigurationByDomain = await _repository.SearchFirstAsync(filter, cancellationToken);
		
		if (ssoConfigurationByDomain != null)
		{
			string duplicateDomain = ssoConfigurationByDomain.Domains.Select(x => x).First(x => ssoConfiguration.Domains.Contains(x));
			errors.Add(nameof(ssoConfiguration.Domains), new ErrorDto(Messages.DomainExist));
		}
        }
        catch (Exception ex)
        {
            throw;
        }
        return errors;
	}
}