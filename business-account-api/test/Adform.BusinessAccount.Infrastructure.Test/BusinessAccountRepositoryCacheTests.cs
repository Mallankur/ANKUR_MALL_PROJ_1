using Adform.BusinessAccount.Domain.Repositories;
using Adform.BusinessAccount.Infrastructure.Decorators;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Adform.BusinessAccount.Infrastructure.Test
{
    public class BusinessAccountRepositoryCacheTests
    {

        [Fact]
        public async Task GetByIdAsync_ReturnsFromCache()
        {
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            var memoryCache = new MemoryDistributedCache(cacheOptions);
            var fakeRepository = new Mock<IBusinessAccountRepository>();
            var id = Guid.NewGuid();
            var businessAccount = new Domain.Repositories.Entities.BusinessAccount();
            businessAccount.Id = id;
            fakeRepository.Setup(t =>
                    t.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(businessAccount);
            var cachedRepo = new BusinessAccountRepositoryCache(fakeRepository.Object, memoryCache);

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
        public async Task DeleteAsync_RemovesFromCache()
        {
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            var memoryCache = new MemoryDistributedCache(cacheOptions);
            var fakeRepository = new Mock<IBusinessAccountRepository>();
            fakeRepository.Setup(t =>
                    t.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(new Domain.Repositories.Entities.BusinessAccount());
            fakeRepository.Setup(t =>
                    t.DeleteAsync(It.IsAny<Guid>(), 0, CancellationToken.None))
                .ReturnsAsync(Guid.NewGuid());

            var cachedRepo = new BusinessAccountRepositoryCache(fakeRepository.Object, memoryCache);

            var id = Guid.NewGuid();
            _ = await cachedRepo.GetByIdAsync(id);

            //Repository method should get called
            fakeRepository.Verify(t =>
                t.GetByIdAsync(id, CancellationToken.None), Times.Once);

            _ = await cachedRepo.DeleteAsync(id, 0, CancellationToken.None);

            //Repository method should get called again. cache miss
            _ = await cachedRepo.GetByIdAsync(id);
            fakeRepository.Verify(t =>
                t.GetByIdAsync(id, CancellationToken.None), Times.Exactly(2));
        }
    }
}