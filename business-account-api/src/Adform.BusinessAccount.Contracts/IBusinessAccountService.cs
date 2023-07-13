using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Adform.BusinessAccount.Contracts.Entities;

namespace Adform.BusinessAccount.Contracts
{
    public interface IBusinessAccountService
    {
        Task<Entities.BusinessAccount> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<PagedList<Entities.BusinessAccount>> GetAsync(Page page, Order order,
            CancellationToken cancellationToken = default);

        Task<Entities.BusinessAccount> CreateAsync(CreateBusinessAccountInput input,
            CancellationToken cancellationToken = default);

        Task<Guid?> DeleteAsync(DeleteBusinessAccountInput input, CancellationToken cancellationToken = default);
    }
}