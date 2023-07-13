using Adform.BusinessAccount.Application.Commands;
using FluentValidation;
using Adform.BusinessAccount.Domain.Exceptions;
using Adform.BusinessAccount.Contracts.Entities;

namespace Adform.BusinessAccount.Application.Validators
{
    internal static class CommanValidations
    {
        static CommanValidations() { }
        public static void ValidateSchemeType <T>(SsoConfigurationType type, Oidc oidc, Saml2 saml2, ValidationContext<T> context)
        {
            if (oidc != null && saml2 != null)
            {
                context.AddFailure(nameof(type), Messages.InvalidConfiguration);
                return;
            }

            if (type == SsoConfigurationType.Saml2)
            {
                ValidateSaml2Scheme(saml2, context);
            }
            else if (type == SsoConfigurationType.Oidc)
            {
                ValidateOidcScheme(oidc, context);
            }
        }
        public static void ValidateSaml2Scheme<T>(Saml2 saml2, ValidationContext<T> context)
        {
            if (saml2 == null)
            {
                context.AddFailure(nameof(Saml2), Messages.InvalidSaml2);
            }
            else
            {
                if (saml2.EntityID == null || string.IsNullOrWhiteSpace(saml2.EntityID) || !IsValidUrl(saml2.EntityID))
                {
                    context.AddFailure(nameof(Saml2.EntityID), Messages.InvalidEntityId);
                }
                else if (!string.IsNullOrWhiteSpace(saml2.MetadataLocation) && !IsValidUrl(saml2.MetadataLocation))
                {
                    context.AddFailure(nameof(Saml2.MetadataLocation), Messages.InvalidMetaData);
                }
            }
        }

        public static void ValidateOidcScheme<T>(Oidc oidc, ValidationContext<T> context)
        {
            if (oidc == null)
            {
                context.AddFailure(nameof(Oidc), Messages.InvalidOidc);
            }
            else
            {
                if (string.IsNullOrEmpty(oidc.Authority))
                {
                    context.AddFailure(nameof(Oidc.Authority), Messages.InvalidAuthority);
                }

                if (string.IsNullOrEmpty(oidc.ClientId))
                {
                    context.AddFailure(nameof(Oidc.ClientId), Messages.InvalidClientId);
                }

                if (string.IsNullOrEmpty(oidc.RedirectUri) || !IsValidUrl(oidc.RedirectUri))
                {
                    context.AddFailure(nameof(Oidc.RedirectUri), Messages.InvalidRedirectUri);
                }

                if (oidc.Scope == null || oidc.Scope.Length < 1 || oidc.Scope.Any(sc => string.IsNullOrWhiteSpace(sc)))
                {
                    context.AddFailure(nameof(Oidc.Scope), Messages.InvalidScope);
                }
            }
        }

        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
        }
    }
}
