using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adform.Ciam.ExceptionHandling.Abstractions.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Adform.BusinessAccount.Api.Middlewares.Filters
{
    /// <inheritdoc />
    /// <summary>
    /// Ensures that the provided model is valid
    /// </summary>
    public class ModelValidatorActionFilter : IAsyncActionFilter
    {
        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var parameters = new Dictionary<string, object>();

                foreach (var modelState in context.ModelState)
                {
                    var state = context.ModelState[modelState.Key];

                    if (state.ValidationState == ModelValidationState.Invalid &&
                        !parameters.ContainsKey(modelState.Key))
                    {
                        parameters.Add(modelState.Key, new
                        {
                            Reason = "invalid",
                            Message = !string.IsNullOrWhiteSpace(state.Errors?.FirstOrDefault()?.ErrorMessage)
                                ? state.Errors?.FirstOrDefault()?.ErrorMessage
                                : null
                        });
                    }
                }

                throw new BadRequestException(parameters: parameters);
            }


            // Move to the next action filter:
            await next();
        }
    }

}