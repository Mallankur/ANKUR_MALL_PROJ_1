using Adform.BusinessAccount.Contracts.Entities;
using Xunit;

namespace Adform.BusinessAccount.Acceptance.Test
{
    public static class Helper
    {
        public static void AssertBusinessAccount(Contracts.Entities.BusinessAccount actualbusinessAccount,
        Contracts.Entities.BusinessAccount expectedbusinessAccount)
        {
            if (expectedbusinessAccount != null)
                Assert.NotNull(actualbusinessAccount);
            Assert.Equal(actualbusinessAccount.Id, expectedbusinessAccount.Id);
            Assert.Equal(actualbusinessAccount.Name, expectedbusinessAccount.Name);
            Assert.Equal(actualbusinessAccount.Type, expectedbusinessAccount.Type);
            Assert.Equal(actualbusinessAccount.IsActive, expectedbusinessAccount.IsActive);

            AssertSsoConfiguration(actualbusinessAccount.SsoConfiguration, expectedbusinessAccount.SsoConfiguration);

        }
        public static void AssertSsoConfiguration(SsoConfiguration actualSsoConfiguration,
            SsoConfiguration expectedSsoConfiguration)
        {
            if (expectedSsoConfiguration != null)
                Assert.NotNull(actualSsoConfiguration);
            Assert.Equal(actualSsoConfiguration.Id, expectedSsoConfiguration.Id);
            Assert.Equal(actualSsoConfiguration.Name, expectedSsoConfiguration.Name);
            Assert.Equal(actualSsoConfiguration.Type, expectedSsoConfiguration.Type);
            Assert.Equal(actualSsoConfiguration.Domains.Length, expectedSsoConfiguration.Domains.Length);
            for (int i = 0; i < actualSsoConfiguration.Domains.Length; i++)
            {
                Assert.Equal(actualSsoConfiguration.Domains[i], expectedSsoConfiguration.Domains[i]);
            }
            if (expectedSsoConfiguration.Saml2 != null)
            {
                Assert.Null(actualSsoConfiguration.Oidc);
                Assert.NotNull(actualSsoConfiguration.Saml2);
                Assert.Equal(actualSsoConfiguration.Saml2.EntityID, expectedSsoConfiguration.Saml2.EntityID);
                Assert.Equal(actualSsoConfiguration.Saml2.MetadataLocation, expectedSsoConfiguration.Saml2.MetadataLocation);
            }
            if (expectedSsoConfiguration.Oidc != null)
            {
                Assert.Null(actualSsoConfiguration.Saml2);
                Assert.NotNull(actualSsoConfiguration.Oidc);
                Assert.Equal(actualSsoConfiguration.Oidc.Authority, expectedSsoConfiguration.Oidc.Authority);
                Assert.Equal(actualSsoConfiguration.Oidc.ClientId, expectedSsoConfiguration.Oidc.ClientId);
                Assert.Equal(actualSsoConfiguration.Oidc.RedirectUri, expectedSsoConfiguration.Oidc.RedirectUri);
                Assert.Equal(actualSsoConfiguration.Oidc.Scope.Length, expectedSsoConfiguration.Oidc.Scope.Length);
                for (int i = 0; i < actualSsoConfiguration.Oidc.Scope.Length; i++)
                {
                    Assert.Equal(actualSsoConfiguration.Oidc.Scope[i], expectedSsoConfiguration.Oidc.Scope[i]);
                }
            }


        }
    }
}
