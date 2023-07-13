using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using Adform.Ciam.ExceptionHandling.Abstractions.Exceptions;

namespace Adform.BusinessAccount.Domain.Exceptions
{
	public class ValidationException : BaseErrorException
	{
		public ValidationException(IDictionary<string, object> fields)
			: base("validationFailed", "The request is invalid.", null, fields)
		{
		}

		[JsonConstructor]
		public ValidationException()
		{
		}

		public override int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;
	}
}
