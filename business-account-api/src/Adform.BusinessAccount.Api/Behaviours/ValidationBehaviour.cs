using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Adform.BusinessAccount.Domain.Exceptions;
using FluentValidation;
using MediatR;
using ValidationException = Adform.BusinessAccount.Domain.Exceptions.ValidationException;

namespace Adform.BusinessAccount.Api.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
	{
		if (_validators.Any())
		{
			var context = new ValidationContext<TRequest>(request);

			var validationResults = await Task.WhenAll(
				_validators.Select(v =>
					v.ValidateAsync(context, cancellationToken)));

			var failures = validationResults
				.Where(r => r.Errors.Any())
				.SelectMany(r => r.Errors)
				.ToList();

			if (failures.Any())
			{
				var errors = new Dictionary<string, object>();
				foreach (var error in failures)
				{
					if (!errors.ContainsKey(error.PropertyName))
						errors.Add(error.PropertyName, new ErrorDto(error.ErrorCode,error.ErrorMessage));
				}

				throw new ValidationException(errors);
			}
		}
		return await next();
	}
}