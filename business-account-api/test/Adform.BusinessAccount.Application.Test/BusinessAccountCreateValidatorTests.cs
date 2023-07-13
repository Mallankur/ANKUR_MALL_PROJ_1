using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Application.Validators;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentValidation.TestHelper;
using Xunit;

namespace Adform.BusinessAccount.Application.Test
{
    public class BusinessAccountCreateValidatorTests
    {
        [Fact]
        public void Should_have_error_when_Name_is_null()
        {
            var command = new BusinessAccountCreateCommand { Name = null };

            var validator = new BusinessAccountCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(cmd => cmd.Name);
            Assert.Equal(ErrorReasons.Required, result.Errors[0].ErrorCode);
            Assert.Equal(Messages.NameIsNullorEmpty, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_not_have_error_when_name_is_specified()
        {
            var command = new BusinessAccountCreateCommand { Name = "BusAccName" };

            var validator = new BusinessAccountCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(cmd => cmd.Name);
        }

        [Fact]
        public void Should_have_error_when_Type_is_NotSet()
        {
            var command = new BusinessAccountCreateCommand { Name = "Name" };

            var validator = new BusinessAccountCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(cmd => cmd.Type);
            Assert.Equal(ErrorReasons.Required, result.Errors[0].ErrorCode);
            Assert.Equal(Messages.InvalidBusinessAccountType, result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_not_have_error_when_Type_is_Set()
        {
            var command = new BusinessAccountCreateCommand
            {
                Name = "Name",
                Type = BusinessAccountType.Agency
            };

            var validator = new BusinessAccountCreateValidator();
            var result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(cmd => cmd.Type);
        }
    }
}