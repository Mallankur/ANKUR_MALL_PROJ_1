using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Adform.BusinessAccount.Contracts.Entities;

namespace Adform.BusinessAccount.Contracts
{
	public interface ISsoConfigurationService
	{
		Task<SsoConfiguration> GetByDomainNameAsync(string domainName, CancellationToken cancellationToken = default);
		Task<SsoConfiguration> CreateAsync(CreateSsoConfigurationInput configuration, CancellationToken cancellationToken = default);
        Task<PagedList<SsoConfiguration>> GetAsync(SsoConfigurationType? type, Page page, Order order, CancellationToken cancellationToken = default);
        Task<SsoConfiguration> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        // Update by id 
        Task<SsoConfiguration>UpdateAsyncbyid(Guid Id, UpdateSsoConfigurationInput configuration, CancellationToken cancellationToken = default);
        //  Update by Name 
        Task<SsoConfiguration> UpdatAsyncbyName(String Name, UpdateSsoConfigurationInputbyName configer2, CancellationToken CacillationToken);
        
        // Delete Commend 
        Task<string> DeleteAsync(string name, CancellationToken cancellationToken = default);


    }
}