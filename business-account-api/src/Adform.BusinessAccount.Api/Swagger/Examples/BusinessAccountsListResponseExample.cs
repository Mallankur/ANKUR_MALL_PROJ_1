using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

namespace Adform.BusinessAccount.Api.Swagger.Examples
{
    public class BusinessAccountsListResponseExample : IExamplesProvider<IEnumerable<Contracts.Entities.BusinessAccount>>
    {
        public IEnumerable<Contracts.Entities.BusinessAccount> GetExamples()
        {
            return new List<Contracts.Entities.BusinessAccount>
            {
                new Contracts.Entities.BusinessAccount
                {
                    Id = Guid.NewGuid(),
                    Name = "Tenant1",
                    Type = Contracts.Entities.BusinessAccountType.Agency,
                    IsActive = true,
                    SsoConfiguration = new Contracts.Entities.SsoConfiguration()
                    {
                        Id = Guid.NewGuid(),
                        Name = "SsoConfiguration",
                        Domains = new string[] { "*.adform.com" },
                        Type = Contracts.Entities.SsoConfigurationType.Saml2,
                        Oidc = null,
                        Saml2 = new Contracts.Entities.Saml2()
                        {
                            EntityID = "http://localhost:8080/myapi/TestSAML",
                            MetadataLocation = "http://localhost:8080/myapi/TestSAML/protocol/saml/descriptor"
                        }
                    }
                },
                new Contracts.Entities.BusinessAccount
                {
                    Id = Guid.NewGuid(),
                    Name = "Tenant2",
                    Type = Contracts.Entities.BusinessAccountType.Publisher,
                    IsActive = true,
                    SsoConfiguration = new Contracts.Entities.SsoConfiguration()
                    {
                        Id = Guid.NewGuid(),
                        Name = "SsoConfiguration",
                        Domains = new string[] { "*.adform.com" },
                        Type = Contracts.Entities.SsoConfigurationType.Saml2,
                        Oidc = null,
                        Saml2 = new Contracts.Entities.Saml2()
                        {
                            EntityID = "http://localhost:8080/myapi/TestSAML",
                            MetadataLocation = "http://localhost:8080/myapi/TestSAML/protocol/saml/descriptor"
                        }
                    }
                }
            };
        }
    }
}
