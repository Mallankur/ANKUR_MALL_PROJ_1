using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Application.Validators;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Integration.Test.Commands
{
	using static Testing;
	public class SsoConfigurationCreateCommandTests : TestFixtureBase
	{
		[Test]
		public async Task ShouldRequireMinimumFields()
		{
			SSOConfigurationCreateValidator validator = new SSOConfigurationCreateValidator();
			var input = new SsoConfigurationCreateCommand();
			var validations = await validator.TestValidateAsync(input);

			validations.ShouldHaveValidationErrorFor(x => x.Name)
				.WithErrorMessage(Messages.NameIsNullorEmpty)
				.WithSeverity(Severity.Error);
			validations.ShouldHaveValidationErrorFor(x => x.Domains)
				.WithErrorMessage(Messages.InvalidDomain)
				.WithSeverity(Severity.Error);
		}

		[Test]
		public async Task ShouldCreateSaml2SsoConfiguration()
		{
			var input = new CreateSsoConfigurationInput
			{
				Name = "IntensaSaml2",
				Domains = new[] { "*.intensa.com", "*.intensaSys.com" },
				Type = SsoConfigurationType.Saml2,
				Saml2 = new Contracts.Entities.Saml2
				{
					EntityID = "http://xyz.intesa.com"
				}
			};

			var ssoConfiguration = await SsoConfigService().CreateAsync(input);

			ssoConfiguration.Should().NotBeNull();
			ssoConfiguration.Name.Should().Be(input.Name.ToLower());
			ssoConfiguration.Domains.Should().HaveCount(input.Domains.Length);
			ssoConfiguration.Type.Should().Be(SsoConfigurationType.Saml2);
			ssoConfiguration.Saml2.Should().NotBeNull();
			ssoConfiguration.Saml2.EntityID.Should().Be(ssoConfiguration.Saml2.EntityID);
		}

		[Test]
		public async Task ShouldCreateOidcSsoConfiguration()
		{
			var input = new CreateSsoConfigurationInput
			{
				Name = "IntensaOidc",
				Domains = new[] { "*.intensa1.com", "*.intensaSys1.com" },
				Type = SsoConfigurationType.Oidc,
				Oidc = new Contracts.Entities.Oidc
				{
					Authority = "xyz",
					ClientId = "adform",
					RedirectUri = "https://xyz.com",
					Scope = new[] { "read", "write" }
				}
			};

			var ssoConfiguration = await SsoConfigService().CreateAsync(input);

			ssoConfiguration.Should().NotBeNull();
			ssoConfiguration.Name.Should().Be(input.Name.ToLower());
			ssoConfiguration.Domains.Should().HaveCount(input.Domains.Length);
			ssoConfiguration.Type.Should().Be(SsoConfigurationType.Oidc);
			ssoConfiguration.Oidc.Should().NotBeNull();
			ssoConfiguration.Oidc.Authority.Should().Be(ssoConfiguration.Oidc.Authority);
			ssoConfiguration.Oidc.ClientId.Should().Be(ssoConfiguration.Oidc.ClientId);
			ssoConfiguration.Oidc.RedirectUri.Should().Be(ssoConfiguration.Oidc.RedirectUri);
			ssoConfiguration.Oidc.Scope.Should().HaveCount(ssoConfiguration.Oidc.Scope.Length);
		}

		[Test]
		public void ShouldThrowNameValidation()
		{
			var input = new CreateSsoConfigurationInput
			{
				Name = "IntensaSaml2",
				Domains = new[] { "*.xr.com", "*.mn.com" },
				Type = SsoConfigurationType.Saml2,
				Saml2 = new Contracts.Entities.Saml2
				{
					EntityID = "https://xyz.intesa.com"
				}
			};

			var validationException = Assert.ThrowsAsync<Domain.Exceptions.ValidationException>(async () => await SsoConfigService().CreateAsync(input));
			validationException.Params.Should().NotBeEmpty();
			validationException.Params.ContainsKey("Name");
		}

		[Test]
		public void ShouldThrowDomainValidation()
		{
			var input = new CreateSsoConfigurationInput
			{
				Name = "Intensa",
				Domains = new[] { "*.intensa.com", "*.intensaSys.com" },
				Type = SsoConfigurationType.Saml2,
				Saml2 = new Contracts.Entities.Saml2
				{
					EntityID = "https://xyz.intesa.com"
				}
			};

			var validationException = Assert.ThrowsAsync<Domain.Exceptions.ValidationException>(async () => await SsoConfigService().CreateAsync(input));
			validationException.Params.Should().NotBeEmpty();
			validationException.Params.ContainsKey("Domain");
		}

		[Test]
		public void ShouldThrowSaml2Validation()
		{
			var input = new CreateSsoConfigurationInput
			{
				Name = "Intensa",
				Domains = new[] { "*.intensa.com", "*.intensaSys.com" },
				Type = SsoConfigurationType.Saml2,
				Oidc = new Contracts.Entities.Oidc { }
			};

			var validationException = Assert.ThrowsAsync<Domain.Exceptions.ValidationException>(async () => await SsoConfigService().CreateAsync(input));
			validationException.Params.Should().NotBeEmpty();
			validationException.Params.ContainsKey("Type");
		}

		[Test]
		public void ShouldThrowSaml2ElementValidation()
		{
			var input = new CreateSsoConfigurationInput
			{
				Name = "Intensa",
				Domains = new[] { "*.intensa.com", "*.intensaSys.com" },
				Type = SsoConfigurationType.Saml2,
				Saml2 = new Contracts.Entities.Saml2 { }
			};

			var validationException = Assert.ThrowsAsync<Domain.Exceptions.ValidationException>(async () => await SsoConfigService().CreateAsync(input));
			validationException.Params.Should().NotBeEmpty();
			validationException.Params.ContainsKey("EntityId");
		}

		[Test]
		public void ShouldThrowOidcValidation()
		{
			var input = new CreateSsoConfigurationInput
			{
				Name = "Intensa",
				Domains = new[] { "*.intensa.com", "*.intensaSys.com" },
				Type = SsoConfigurationType.Oidc,
				Saml2 = new Contracts.Entities.Saml2 { }
			};

			var validationException = Assert.ThrowsAsync<Domain.Exceptions.ValidationException>(async () => await SsoConfigService().CreateAsync(input));
			validationException.Params.Should().NotBeEmpty();
			validationException.Params.ContainsKey("Type");
		}

		[Test]
		public void ShouldThrowOidcElementValidation()
		{
			var inputEmptyAuthority = new CreateSsoConfigurationInput
			{
				Name = "Intensa",
				Domains = new[] { "*.intensa.com", "*.intensaSys.com" },
				Type = SsoConfigurationType.Oidc,
				Oidc = new Contracts.Entities.Oidc { }
			};

			var validationException = Assert.ThrowsAsync<Domain.Exceptions.ValidationException>(async () => await SsoConfigService().CreateAsync(inputEmptyAuthority));
			validationException.Params.Should().NotBeEmpty();
			validationException.Params.ContainsKey("Authority");
		}
	}
}
