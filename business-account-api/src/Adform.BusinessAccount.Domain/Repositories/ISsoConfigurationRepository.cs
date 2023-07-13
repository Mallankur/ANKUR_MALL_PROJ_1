using Adform.BusinessAccount.Domain.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Domain.Repositories
{
	public interface ISsoConfigurationRepository
	{
		Task<SsoConfiguration> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

		Task<SsoConfiguration> GetByDomainAsync(string domainName,
			CancellationToken cancellationToken = default);

		Task<SsoConfiguration> CreateAsync(Entities.SsoConfiguration ssoConfiguration, CancellationToken cancellationToken = default);

		Task<SsoConfiguration> GetByNameAsync(string name, CancellationToken cancellationToken = default);

		Task<Contracts.Entities.PagedList<SsoConfiguration>> GetAsync(string type, Contracts.Entities.Page page, Contracts.Entities.Order order, CancellationToken cancellationToken = default);
        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ssoConfiguration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SsoConfiguration> UpdateAsync(Guid id,SsoConfiguration updatessoConfiguration, CancellationToken cancellationToken = default);
        Task<string?> DeleteAsync(string name,  CancellationToken cancellationToken = default);

    }
}