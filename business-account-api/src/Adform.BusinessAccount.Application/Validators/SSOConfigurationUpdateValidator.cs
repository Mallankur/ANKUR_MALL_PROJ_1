using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentValidation;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Application.Validators
{
    public class SSOConfigurationUpdateValidator : AbstractValidator<SsoConfigurationUpdateCommand>
    {
        public SSOConfigurationUpdateValidator()
        {
            RuleFor(c => c.SsoConfiguration.Name).NotEmpty().WithMessage(Messages.NameIsNullorEmpty).WithErrorCode(ErrorReasons.Required);
            RuleFor(x => x.SsoConfiguration.Domains).Must(x => x != null && x.Length > 0 && x.Any(dmn => !string.IsNullOrWhiteSpace(dmn))).WithMessage(Messages.InvalidDomain).WithErrorCode(ErrorReasons.Invalid);
            RuleFor(c => new
            {
                c.SsoConfiguration.Type,
                c.SsoConfiguration.Oidc,
                c.SsoConfiguration.Saml2
            }).Custom((x, context) => CommanValidations.ValidateSchemeType(x.Type, x.Oidc,x.Saml2,context));
        }
}
}
