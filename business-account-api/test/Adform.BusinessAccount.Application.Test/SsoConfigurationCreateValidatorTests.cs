using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Application.Validators;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentValidation.TestHelper;
using Xunit;

namespace Adform.BusinessAccount.Application.Test
{
    public class SsoConfigurationCreateValidatorTests
    {
        [Fact]
        public void Should_have_error_when_Name_is_null()
        {
            var command = new SsoConfigurationCreateCommand { Name = null };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(cmd => cmd.Name);
            Assert.Equal(ErrorReasons.Required, result.Errors[0].ErrorCode);
            Assert.Equal(Messages.NameIsNullorEmpty, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_have_error_when_Name_is_empty()
        {
            var command = new SsoConfigurationCreateCommand { Name = string.Empty };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(cmd => cmd.Name);
            Assert.Equal(ErrorReasons.Required, result.Errors[0].ErrorCode);
            Assert.Equal(Messages.NameIsNullorEmpty, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_not_have_error_when_name_is_specified()
        {
            var command = new SsoConfigurationCreateCommand { Name = "BusAccName", Domains = new string[] { "*.adform.com" } };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(cmd => cmd.Name);
        }

        [Fact]
        public void Should_have_error_when_Domains_is_null()
        {
            var command = new SsoConfigurationCreateCommand {Name="Test", Domains = null };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(cmd => cmd.Domains);
            Assert.Equal(ErrorReasons.Invalid, result.Errors[0].ErrorCode);
            Assert.Equal(Messages.InvalidDomain, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_have_error_when_Domains_is_set_but_withemptyitems()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name="Test",
                Domains = new string[] { "" }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(cmd => cmd.Domains);
            Assert.Equal(ErrorReasons.Invalid, result.Errors[0].ErrorCode);
            Assert.Equal(Messages.InvalidDomain, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_not_have_error_when_Domains_is_specified()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Domains = new string[] { "domain.com" }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(cmd => cmd.Domains);
        }

        [Fact]
        public void Should_have_error_when_OidcAndSaml2_are_set()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name", Domains = new string[] { "domain.com" },
                Saml2 = new Saml2(),
                Oidc = new Oidc()
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(nameof(command.Type).ToLower());
            Assert.Equal(Messages.InvalidConfiguration, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_have_error_when_Oidc_is_null()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Oidc,
                Oidc = null
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc));
            Assert.Equal(Messages.InvalidOidc, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_have_error_when_Oidc_has_Invalid_data()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Oidc,
                Oidc = new Oidc
                {
                    Authority = null,
                    ClientId = null,
                    RedirectUri = null,
                    Scope = null
                }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.Authority));
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.ClientId));
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.RedirectUri));
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.Scope));
            Assert.Equal(Messages.InvalidAuthority, result.Errors[0].ErrorMessage);
            Assert.Equal(Messages.InvalidClientId, result.Errors[1].ErrorMessage);
            Assert.Equal(Messages.InvalidRedirectUri, result.Errors[2].ErrorMessage);
            Assert.Equal(Messages.InvalidScope, result.Errors[3].ErrorMessage);
        }

        [Fact]
        public void Should_have_error_when_Oidc_has_Invalid_redirectUrl_scope()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Oidc,
                Oidc = new Oidc
                {
                    Authority = "TestAuthroity",
                    ClientId = "TestClientid",
                    RedirectUri = "testuri",
                    Scope = new string[] { ""}
                }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.RedirectUri));
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.Scope));
            Assert.Equal(Messages.InvalidRedirectUri, result.Errors[0].ErrorMessage);
            Assert.Equal(Messages.InvalidScope, result.Errors[1].ErrorMessage);
        }

        [Fact]
        public void Should_not_have_error_when_Oidc_has_valid_data()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Oidc,
                Oidc = new Oidc
                {
                    Authority = "TestAuthroity",
                    ClientId = "TestClientid",
                    RedirectUri = "https://adform.com",
                    Scope = new string[] { "Testscope" }
                }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.Authority));
            result.ShouldNotHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.ClientId));
            result.ShouldNotHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.RedirectUri));
            result.ShouldNotHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Oidc.Scope));
        }

        [Fact]
        public void Should_have_error_when_Saml2_is_null()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Saml2,
                Saml2 = null
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Saml2));
            Assert.Equal(Messages.InvalidSaml2, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_have_error_when_Saml2_has_Invalid_data()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Saml2,
                Saml2 = new Saml2
                {
                    EntityID = null,
                    MetadataLocation = null
                }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Saml2.EntityID));
            Assert.Equal(Messages.InvalidEntityId, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_have_error_when_Saml2_has_Invalid_entityId()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Saml2,
                Saml2 = new Saml2
                {
                    EntityID = "Testentity",
                    MetadataLocation = "TestMetadata"
                }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Saml2.EntityID));
            Assert.Equal(Messages.InvalidEntityId, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_have_error_when_Saml2_has_Invalid_metadata()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Saml2,
                Saml2 = new Saml2
                {
                    EntityID = "https://adform.com/Saml2Test",
                    MetadataLocation = "TestMetadata"
                }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Saml2.MetadataLocation));
            Assert.Equal(Messages.InvalidMetaData, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_not_have_error_when_Saml2_has_valid_data()
        {
            var command = new SsoConfigurationCreateCommand
            {
                Name = "Name",
                Domains = new string[] { "domain.com" },
                Type = SsoConfigurationType.Saml2,
                Saml2 = new Saml2
                {
                    EntityID = "https://adform.com/Saml2Test",
                    MetadataLocation = "https://adform.com/Saml2Test"
                }
            };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Saml2.MetadataLocation));
            result.ShouldNotHaveValidationErrorFor(nameof(SsoConfigurationCreateCommand.Saml2.EntityID));
        }
    }
}