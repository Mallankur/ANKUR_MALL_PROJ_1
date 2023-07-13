using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Exceptions;
using Adform.BusinessAccount.Domain.Repositories;
using Adform.Ciam.Mongo.Repositories;
using MongoDB.Driver;

namespace Adform.BusinessAccount.Infrastructure.Repositories;

public class BusinessAccountRepository : IBusinessAccountRepository
{
	private readonly IBaseMongoRepository<Domain.Repositories.Entities.BusinessAccount, Guid> _repository;
	private readonly ISsoConfigurationRepository _ssoConfigurationRepository;
	public BusinessAccountRepository(IBaseMongoRepository<Domain.Repositories.Entities.BusinessAccount, Guid> baseMongoRepository, ISsoConfigurationRepository ssoConfigurationRepository)

	{
		_repository = baseMongoRepository ?? throw new ArgumentNullException(nameof(baseMongoRepository));
		_ssoConfigurationRepository = ssoConfigurationRepository;
	}

	public async Task<Domain.Repositories.Entities.BusinessAccount> GetByIdAsync(Guid id,
		CancellationToken cancellationToken = default)
	{
		var filter = Builders<Domain.Repositories.Entities.BusinessAccount>.Filter.And(
				  Builders<Domain.Repositories.Entities.BusinessAccount>.Filter.Eq(u => u.Id, id),
				  Builders<Domain.Repositories.Entities.BusinessAccount>.Filter.Eq(u => u.IsActive, true));
		return await _repository.SearchFirstAsync(filter, cancellationToken);
	}


	public async Task<Domain.Repositories.Entities.BusinessAccount> CreateAsync(
		Domain.Repositories.Entities.BusinessAccount businessAccount,
		CancellationToken cancellationToken = default)
	{
		var validationErrors = await ValidateBusinessAccount(businessAccount, cancellationToken);

		if (validationErrors.Count == 0)
		{
			try
			{
				return await _repository.InsertAsync(businessAccount, cancellationToken);
			}
			catch (MongoWriteException ex)
			{
				if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
				{
					validationErrors.Add(nameof(businessAccount.Name), new ErrorDto(Messages.BusinessAccountExist));
				}
				else
				{
					throw;
				}
			}
		}

		throw new ValidationException(validationErrors);
	}

	public async Task<Domain.Repositories.Entities.BusinessAccount> GetByNameTypeAsync(
		string name,
		string type,
		CancellationToken cancellationToken = default)
	{
		var filter = Builders<Domain.Repositories.Entities.BusinessAccount>.Filter.Where(x => x.Name == name && x.Type == type);
		return await _repository.SearchFirstAsync(filter, cancellationToken);
	}

	private async Task<IDictionary<string, object>> ValidateBusinessAccount(Domain.Repositories.Entities.BusinessAccount businessAccount, CancellationToken cancellationToken)
	{
		var errors = new Dictionary<string, object>();

		// if ssoConfigurationId exists then check if that is valid
		if (businessAccount.SsoConfigurationId.HasValue)
		{
			var ssoConfiguration = await _ssoConfigurationRepository.GetByIdAsync(businessAccount.SsoConfigurationId.Value, cancellationToken);
			if (ssoConfiguration is null)
			{
				errors.Add(nameof(businessAccount.SsoConfigurationId), new ErrorDto(Messages.SsoConfigurationNotExist));
				return errors;
			}
		}

		return errors;
	}

	public async Task<Guid?> DeleteAsync(Guid id, long version, CancellationToken cancellationToken = default)
	{
        var filter = Builders<Domain.Repositories.Entities.BusinessAccount>.Filter.And(
            Builders<Domain.Repositories.Entities.BusinessAccount>.Filter.Eq(u => u.Id, id),
            Builders<Domain.Repositories.Entities.BusinessAccount>.Filter.Eq(u => u.IsActive, true));
        var businessAccount = await _repository.SearchFirstAsync(filter, cancellationToken);
        if (businessAccount == null)
        {
            return null;
        }

        var result = await _repository.Collection.UpdateOneAsync(
            filter,
            Builders<Domain.Repositories.Entities.BusinessAccount>.Update
                .Set(p => p.IsActive, false), 
                null, cancellationToken);

        if (result.IsAcknowledged && result.ModifiedCount == 1)
        {
            return id;
        }

        return null;
    }

	public async Task<PagedList<Domain.Repositories.Entities.BusinessAccount>> GetAllAsync(Page page, Order order, CancellationToken cancellationToken)
	{
		var sortDefinition = order.OrderDirection == OrderDirection.Ascending
			? Builders<Domain.Repositories.Entities.BusinessAccount>.Sort.Ascending(x => x.Id)
			: Builders<Domain.Repositories.Entities.BusinessAccount>.Sort.Descending(x => x.Id);

		var content = await _repository.Collection.Find(o => true)
			.Sort(sortDefinition).Skip(page.Offset).Limit(page.Limit).ToListAsync().ConfigureAwait(false);

        long? totalCount = null;
        if (page.ReturnTotalCount) //TODO:: get total Records
        {
            totalCount = content.Count;
        }
        var pagedList = new PagedList<Domain.Repositories.Entities.BusinessAccount>()
        {
            Content = content,
			Page = page,
			Order = order,
			TotalCount = totalCount
		};

        return pagedList;
	}
}