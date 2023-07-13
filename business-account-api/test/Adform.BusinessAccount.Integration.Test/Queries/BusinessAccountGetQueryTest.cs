using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Adform.AspNetCore.Paging;
using FluentAssertions;
using Adform.BusinessAccount.Contracts.Entities;
using Order = Adform.BusinessAccount.Contracts.Entities.Order;
using Page = Adform.BusinessAccount.Contracts.Entities.Page;

namespace Adform.BusinessAccount.Integration.Test.Queries
{
    using static Testing;
    public class BusinessAccountGetQueryTest : TestFixtureBase
    {

        [Test]
        public async Task GetBusinessAccount_Notfound()
        {
            var guid = Guid.NewGuid();
            var businessAccount = await BusinessAccountService().GetByIdAsync(guid);
            businessAccount.Should().BeNull();
        }

        [Test]
        public async Task GetBusinessAccount_Success()
        {
            Contracts.Entities.BusinessAccount createdBusinessAccount = await CreateBusinessAccount();
            var businessAccount = await BusinessAccountService().GetByIdAsync(createdBusinessAccount.Id);
            businessAccount.Should().NotBeNull();
            businessAccount.Id.Should().Be(createdBusinessAccount.Id);
        }

        [Test]
        public async Task GetBusinessAccountAll_Notfound()
        {
            await MongoDatabase().DropCollectionAsync("BusinessAccounts");
            var guid = Guid.NewGuid();
            var businessAccounts = await BusinessAccountService().GetAsync(new Page(), new Order());
            businessAccounts.Content.Should().HaveCount(0);
        }

        [Test]
        public async Task GetBusinessAccountAll_Success()
        {
            await MongoDatabase().DropCollectionAsync("BusinessAccounts");
            Contracts.Entities.BusinessAccount createdBusinessAccount = await CreateBusinessAccount();
            var businessAccount = await BusinessAccountService().GetAsync(new Page(), new Order());
            businessAccount.Content.Should().HaveCount(1);
        }

        private async Task<Contracts.Entities.BusinessAccount> CreateBusinessAccount(Guid ssoConfigId = default)
        {
            var command = new CreateBusinessAccountInput
            {
                Name = "Intensa_" + Guid.NewGuid(),
                Type = BusinessAccountType.Agency,
                IsActive = true,
            };
            if (ssoConfigId != Guid.Empty) { command.SsoConfigurationId = ssoConfigId; }

            return await BusinessAccountService().CreateAsync(command);
        }

        private async Task<SsoConfiguration> CreateSaml2SsoConfiguration()
        {
            var guid = Guid.NewGuid();
            var input = new CreateSsoConfigurationInput
            {
                Name = "IntensaSaml2_" + guid,
                Domains = new[] { guid + "_*.intensa.com", guid + "_*.intensaSys.com" },
                Type = SsoConfigurationType.Saml2,
                Saml2 = new Contracts.Entities.Saml2
                {
                    EntityID = "http://xyz.intesa.com"
                }
            };

            return await SsoConfigService().CreateAsync(input);
        }
    }
}