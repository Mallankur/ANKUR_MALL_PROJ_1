 using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Repositories;
using Adform.BusinessAccount.Domain.Repositories.Entities;
using Adform.BusinessAccount.Infrastructure.Decorators;
using Adform.Ciam.Cache.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Adform.BusinessAccount.Infrastructure.Test
{
    public class SsoConfigurationRepositoryCacheTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<ISsoConfigurationRepository> _mockSsoConfigurationRepository;
        public SsoConfigurationRepositoryCacheTests()
         {

            _mockRepository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            _mockSsoConfigurationRepository = _mockRepository.Create<ISsoConfigurationRepository>(MockBehavior.Strict);
        }

        [Fact]                
        public async Task GetByIdAsync_ReturnsFromCache()    
        {
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            var memoryCache = new MemoryDistributedCache(cacheOptions);
            var fakeRepository = new Mock<ISsoConfigurationRepository>();
            var id = Guid.NewGuid();
            var ssoConfiguration = new Domain.Repositories.Entities.SsoConfiguration();
            ssoConfiguration.Id = id;
            fakeRepository.Setup(t =>
                    t.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(ssoConfiguration);
            var cachedRepo = new SsoConfigurationRepositoryCache(fakeRepository.Object, memoryCache);

            _ = await cachedRepo.GetByIdAsync(id);

            //Repository method should get called
            fakeRepository.Verify(t =>
                t.GetByIdAsync(id, CancellationToken.None), Times.Once);

            _ = await cachedRepo.GetByIdAsync(id);
            //Repository method should NOT get called. Data returned from Cache
            fakeRepository.Verify(t =>
                t.GetByIdAsync(id, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetByDomainAsync_ReturnsFromCache()
        {
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            var memoryCache = new MemoryDistributedCache(cacheOptions);
            var fakeRepository = new Mock<ISsoConfigurationRepository>();
            var domainName = "SomeDomain";
            var ssoConfiguration = new Domain.Repositories.Entities.SsoConfiguration();
            ssoConfiguration.Domains = new string[] { domainName };
            fakeRepository.Setup(t =>
                    t.GetByDomainAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(ssoConfiguration);
            var cachedRepo = new SsoConfigurationRepositoryCache(fakeRepository.Object, memoryCache);

            _ = await cachedRepo.GetByDomainAsync(domainName);

            //Repository method should get called
            fakeRepository.Verify(t =>
                t.GetByDomainAsync(domainName, CancellationToken.None), Times.Once);

            _ = await cachedRepo.GetByDomainAsync(domainName);
            //Repository method should NOT get called. Data returned from Cache
            fakeRepository.Verify(t =>
                t.GetByDomainAsync(domainName, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsFromCache()
        {
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            var memoryCache = new MemoryDistributedCache(cacheOptions);
            var fakeRepository = new Mock<ISsoConfigurationRepository>();
            var ssoConfigName = "SomeName";
            var ssoConfiguration = new Domain.Repositories.Entities.SsoConfiguration();
            ssoConfiguration.Name = ssoConfigName;



            fakeRepository.Setup(t =>
                    t.GetByNameAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(ssoConfiguration);
            var cachedRepo = new SsoConfigurationRepositoryCache(fakeRepository.Object, memoryCache);

            _ = await cachedRepo.GetByNameAsync(ssoConfigName);

            //Repository method should get called
            fakeRepository.Verify(t =>
                t.GetByNameAsync(ssoConfigName, CancellationToken.None), Times.Once);

            _ = await cachedRepo.GetByNameAsync(ssoConfigName);
            //Repository method should NOT get called. Data returned from Cache
            fakeRepository.Verify(t =>
                t.GetByNameAsync(ssoConfigName, CancellationToken.None), Times.Once);
        }


        [Fact]
        public async Task GUpdateAsync_RemoveFromCache()
        {
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            var memoryCache = new MemoryDistributedCache(cacheOptions);
            CancellationToken cancellationToken = new CancellationToken();
            Domain.Repositories.Entities.SsoConfiguration ssoConfiguration = new Domain.Repositories.Entities.SsoConfiguration { Domains = new string[] {"t.com","t1.com" },Name="test" ,Id=Guid.NewGuid()};
          
            _mockSsoConfigurationRepository.Setup(t =>
                    t.UpdateAsync(It.IsAny<Guid>(),It.IsAny<Domain.Repositories.Entities.SsoConfiguration>(), CancellationToken.None))
                .ReturnsAsync(ssoConfiguration);

            _mockSsoConfigurationRepository.Setup(t =>
                  t.GetByIdAsync(ssoConfiguration.Id, It.IsAny<CancellationToken>()))
              .ReturnsAsync(ssoConfiguration);

            var cachedRepo = new SsoConfigurationRepositoryCache(_mockSsoConfigurationRepository.Object, memoryCache);

            _ = await cachedRepo.GetByIdAsync(ssoConfiguration.Id);

            _ = await cachedRepo.UpdateAsync(ssoConfiguration.Id, It.IsAny<Domain.Repositories.Entities.SsoConfiguration>());

            string key = "ssoid_{ssoConfiguration.Id}";
            Assert.True(memoryCache.GetAsync<string>(key).Result == null);
            _mockRepository.VerifyAll();

         // var cacheResult = 
        }

    }
}