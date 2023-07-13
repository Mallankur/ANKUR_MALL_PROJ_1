using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Application.Validators;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Integration.Test.Commands;
using static Testing;
public class BusinessAccountCreateCommandTests : TestFixtureBase
{
	[Test]
	public async Task ShouldRequireMinimumFields()
	{
		var validator = new BusinessAccountCreateValidator();
		var command = new BusinessAccountCreateCommand() { };
		var validations = await validator.TestValidateAsync(command);

		validations.ShouldHaveValidationErrorFor(x => x.Name)
			.WithErrorMessage(Messages.NameIsNullorEmpty)
			.WithSeverity(Severity.Error);

		validations.ShouldNotHaveValidationErrorFor(x => x.SsoConfigurationId);
	}

	[Test]
	public async Task ShouldCreateBusinessAccount()
	{
		var command = new CreateBusinessAccountInput
		{
			Name = "Intensa",
			Type = BusinessAccountType.Agency,
			IsActive = true,
		};

		var businessAccount = await BusinessAccountService().CreateAsync(command);

		businessAccount.Should().NotBeNull();
		businessAccount.Name.Should().Be(command.Name.ToLower());
		businessAccount.Type.Should().Be(BusinessAccountType.Agency);
		businessAccount.LegacyId.Should().BeNull();
		businessAccount.SsoConfiguration.Should().BeNull();
		businessAccount.IsActive.Should().BeTrue();
	}



	[Test]
	public void ShouldThrowNameTypeValidation()
	{
		var command = new CreateBusinessAccountInput
		{
			Name = "Intensa",
			Type = BusinessAccountType.Agency,
			IsActive = true,
		};

		var validationException = Assert.ThrowsAsync<Domain.Exceptions.ValidationException>(async () => await BusinessAccountService().CreateAsync(command));
		validationException.Params.Count.Should().Be(1);
		validationException.Params.ContainsKey("Name");
	}

	[Test]
	public void ShouldThrowSsoConfigurationValidation()
	{
		var command = new CreateBusinessAccountInput
		{
			Name = "Intensa1",
			Type = BusinessAccountType.Agency,
			SsoConfigurationId = Guid.NewGuid(),
		};

		var validationException = Assert.ThrowsAsync<Domain.Exceptions.ValidationException>(async () => await BusinessAccountService().CreateAsync(command));
		validationException.Params.Count.Should().Be(1);
		validationException.Params.ContainsKey("SSOConfigurationId");
	}
}