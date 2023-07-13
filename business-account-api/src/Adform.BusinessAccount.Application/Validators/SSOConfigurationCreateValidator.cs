using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentValidation;

namespace Adform.BusinessAccount.Application.Validators
{
	public class SSOConfigurationCreateValidator : AbstractValidator<SsoConfigurationCreateCommand>
	{
		public SSOConfigurationCreateValidator()
		{
			RuleFor(c => c.Name).NotEmpty().WithMessage(Messages.NameIsNullorEmpty).WithErrorCode(ErrorReasons.Required);
			RuleFor(x => x.Domains).Must(x => x != null && x.Length > 0 && x.Any(dmn => !string.IsNullOrWhiteSpace(dmn))).WithMessage(Messages.InvalidDomain).WithErrorCode(ErrorReasons.Invalid);
			RuleFor(c => new { c.Type, c.Oidc, c.Saml2 }).Custom((x, context) => CommanValidations.ValidateSchemeType(x.Type, x.Oidc, x.Saml2, context));

			// verify single SSO configuration
		}

	}
}
