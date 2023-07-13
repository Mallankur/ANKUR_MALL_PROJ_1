using System.Collections.Generic;
using Adform.BusinessAccount.Contracts.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace Adform.BusinessAccount.Api.Swagger.Examples
{
	public class SsoConfigurationResponseExample : IExamplesProvider<Contracts.Entities.SsoConfiguration>
	{
		public SsoConfiguration GetExamples()
		{
			return new SsoConfiguration
			{
				Id = System.Guid.NewGuid(),
				Name = "SsoConfiguraiton1",
				Domains = new string[] { "*.adform.com" },
				Type = SsoConfigurationType.Saml2,
				Oidc = null,
				Saml2 = new Saml2()
				{
					EntityID = "http://localhost:8080/myapi/TestSAML",
					MetadataLocation = "http://localhost:8080/myapi/TestSAML/protocol/saml/descriptor"
				}

			};
		}
	}

	public class SsoConfigurationListExample : IExamplesProvider<IEnumerable<Contracts.Entities.SsoConfiguration>>
	{
		public IEnumerable<SsoConfiguration> GetExamples()
		{
			return new List<SsoConfiguration>()
			{
				new SsoConfiguration
				{
					Id = System.Guid.NewGuid(),
					Name = "SsoConfiguraiton1",
					Domains = new string[] { "*.adform.com" },
					Type = SsoConfigurationType.Saml2,
					Oidc = null,
					Saml2 = new Saml2()
					{
						EntityID = "http://localhost:8080/myapi/TestSAML",
						MetadataLocation = "http://localhost:8080/myapi/TestSAML/protocol/saml/descriptor"
					}

				}
			};
		}

	}
}
