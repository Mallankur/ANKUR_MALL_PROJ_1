using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Application.Validators;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Adform.BusinessAccount.Application.Test
{
    public class SSOConfigurationUpdateValidatorTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]

        public void Should_have_error_when_Name_is_null(string name)
        {
            var command = new SsoConfigurationCreateCommand { Name = name };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(cmd => cmd.Name);
            Assert.Equal(ErrorReasons.Required, result.Errors[0].ErrorCode);
            Assert.Equal(Messages.NameIsNullorEmpty, result.Errors[0].ErrorMessage);
        }

        [Theory]
        [InlineData("Test.com")]
        [InlineData("Test1")]

        public void Should_not_have_error_when_Name_is_specified(string name)
        {
            var command = new SsoConfigurationCreateCommand { Name = name };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);


            result.ShouldNotHaveValidationErrorFor(cmd => cmd.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("","")]

        public void Should_have_error_when_Domain_is_null(params string[] domain)
        {
            var command = new SsoConfigurationCreateCommand { Domains  = domain };

            var validator = new SSOConfigurationCreateValidator();
            var result = validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(cmd => cmd.Name);
            Assert.Equal(ErrorReasons.Required, result.Errors[0].ErrorCode);
            Assert.Equal(Messages.NameIsNullorEmpty, result.Errors[0].ErrorMessage);
        }
    }
}
