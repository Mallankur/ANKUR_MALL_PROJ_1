using System;
using System.Threading;
using System.Threading.Tasks;
using Adform.BusinessAccount.Contracts.Entities;

namespace Adform.BusinessAccount.Domain.Repositories;

public interface IBusinessAccountRepository
{
	Task<Entities.BusinessAccount> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

	Task<Entities.BusinessAccount> CreateAsync(Entities.BusinessAccount businessAccount, CancellationToken cancellationToken = default);

	Task<Guid?> DeleteAsync(Guid id, long version, CancellationToken cancellationToken = default);

	Task<PagedList<Entities.BusinessAccount>> GetAllAsync(Page page, Order order, CancellationToken cancellationToken);
	//Task<Guid?> UpdateAsync(Entities.BusinessAccount businessAccount, CancellationToken cancellationToken = default);

}