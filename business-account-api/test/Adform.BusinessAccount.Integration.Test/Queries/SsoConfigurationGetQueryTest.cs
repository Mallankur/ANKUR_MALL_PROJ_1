using Adform.BusinessAccount.Application.Queries;
using Adform.BusinessAccount.Contracts;
using Adform.BusinessAccount.Contracts.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Integration.Test.Queries
{
    using static Testing;
    public class SsoConfigurationGetQueryTest : TestFixtureBase
    {
        [Test]
        public async Task GetSsoConfiguration_Notfound()
        {

            var ssoConfiguration = await SsoConfigService().GetByDomainNameAsync("test_domain_not_found");
            ssoConfiguration.Should().BeNull();
        }

        [Test]
        public async Task GetSsoConfiguration_Success()
        {
            SsoConfiguration createdSsoConfiguration = await CreateSaml2SsoConfiguration();
            var ssoConfiguration = await SsoConfigService().GetByDomainNameAsync(createdSsoConfiguration.Domains[0]);
            ssoConfiguration.Should().NotBeNull();
            ssoConfiguration.Domains.Length.Should().BeGreaterThan(0);
            ssoConfiguration.Domains.Should().Contain(createdSsoConfiguration.Domains[0]);

        }

        [Test]
        public async Task GetSsoConfigurationByName_Notfound()
        {

	        var ssoConfiguration = await SsoConfigService().GetByNameAsync("test");
	        ssoConfiguration.Should().BeNull();
        }

        [Test]
        public async Task GetSsoConfigurationByName_Success()
        {
	        SsoConfiguration createdSsoConfiguration = await CreateSaml2SsoConfiguration();
	        var ssoConfiguration = await SsoConfigService().GetByNameAsync(createdSsoConfiguration.Name);
	        ssoConfiguration.Should().NotBeNull();
	        ssoConfiguration.Name.Should().Be(createdSsoConfiguration.Name);
        }

        [Test]
        public async Task GetSsoConfigurationByType_Notfound()
        {
	        var ssoConfiguration = await SsoConfigService().GetAsync(SsoConfigurationType.Oidc,new Page(),new Order());
            ssoConfiguration.Content.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task GetSsoConfigurationByType_Success()
        {
	        _ = await CreateSaml2SsoConfiguration();
            var ssoConfiguration = await SsoConfigService().GetAsync(SsoConfigurationType.Saml2, new Page(), new Order());
            ssoConfiguration.Content.Should().NotBeNull();
	        ssoConfiguration.Content.First().Type.Should().Be(SsoConfigurationType.Saml2);
        }

        [Test]
        public async Task GetAllSsoConfiguration_Notfound()
        {
            await MongoDatabase().DropCollectionAsync("SsoConfigurations");
            var businessAccounts = await SsoConfigService().GetAsync(null, new Page(), new Order());
            businessAccounts.Content.Should().HaveCount(0);
        }

        [Test]
        public async Task GetAllSsoConfiguration_Success()
        {
            await MongoDatabase().DropCollectionAsync("SsoConfigurations");
            _= await CreateSaml2SsoConfiguration();
            var ssoConfiguration = await SsoConfigService().GetAsync(null, new Page(), new Order());
            ssoConfiguration.Content.Should().HaveCount(1);
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