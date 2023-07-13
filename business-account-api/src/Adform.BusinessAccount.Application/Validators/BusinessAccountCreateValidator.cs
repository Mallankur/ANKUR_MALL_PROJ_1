using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentValidation;

namespace Adform.BusinessAccount.Application.Validators
{
	public class BusinessAccountCreateValidator : AbstractValidator<BusinessAccountCreateCommand>
	{
		public BusinessAccountCreateValidator()
		{
			RuleFor(c => c.Name).NotEmpty().WithMessage(Messages.NameIsNullorEmpty).WithErrorCode(ErrorReasons.Required);
			RuleFor(c => c.Type).NotEmpty().WithMessage(Messages.InvalidBusinessAccountType).WithErrorCode(ErrorReasons.Required);
		}
	}
}
