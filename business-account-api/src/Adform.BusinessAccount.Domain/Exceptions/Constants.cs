using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Domain.Exceptions
{
    public static class Messages
    {
        public const string InvalidBusinessAccountType = "Invalid BusinessAccount type. Expected values Publisher, Agency, DataProvider";
        public const string InvalidDomain = "Empty or Invalid domain.";
        public const string InvalidConfiguration = "SSO configuration must either have saml2 or oidc element, not both.";
        public const string InvalidSaml2 = "Saml2 element should be provided for Type 'saml2'.";
        public const string InvalidEntityId = "Invalid Saml2 EntityID.";
        public const string InvalidMetaData = "Invalid Saml2 MetadataLocation.";
        public const string InvalidOidc = "Oidc element should be provided for Type 'oidc'.";
        public const string InvalidAuthority = "Invalid Oidc Authority.";
        public const string InvalidClientId = "Invalid Oidc ClientId.";
        public const string InvalidRedirectUri = "Invalid Oidc RedirectUri.";
        public const string InvalidScope = "Invalid Oidc Scope.";
        public const string SsoConfigurationExist = "SSO Configuration already exists.";
        public const string DomainExist = "SSO Configuration for this domain already exists.";
        public const string InvalidRequest = "request is invalid.";
        public const string BusinessAccountExist = "Business Account with this name and type already exists.";
        public const string SsoConfigurationNotExist = "SSO Configuration does not exist.";
        public const string SsoConfigurationNotFound = "SSO Configuration not found.";
        public const string BusinessAccountNotFound = "Business Account not found.";
        public const string NameIsNullorEmpty = "Name must not be null or empty.";
        public const string TypeIsNullorEmpty = "Type must not be null or empty.";
    }

    public static class ErrorReasons
    {
        public const string Required = "required";
        public const string Invalid = "invalid";
        public const string NotFound = "notFound";
    }
}
