using Adform.BusinessAccount.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Adform.BusinessAccount.Api.Capabilities
{
	public static class StartupMvc
	{
		public static IMvcBuilder ConfigureMvc(this IServiceCollection services)
		{
			return services
				.AddApiVersioning()
				.AddControllers()
				.ConfigureApiBehaviorOptions(options =>
				{
					options.InvalidModelStateResponseFactory = context =>
					{
						var errors = new Dictionary<string, object>();
						foreach (var key in context.ModelState.Keys)
						{
							foreach (var errorMessage in context.ModelState[key].Errors.Select(x => x.ErrorMessage))
							{
								errors.Add(key, new ErrorDto(errorMessage));
							}
						}

						throw new ValidationException(errors);
					};
				})
				.ConfigureJson();
		}

		private static IMvcBuilder ConfigureJson(this IMvcBuilder builder)
		{
			builder.AddJsonOptions(x =>
			{
				x.JsonSerializerOptions.WriteIndented = true;
				x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
				x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				x.JsonSerializerOptions.IgnoreReadOnlyFields = true;
				x.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
				x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});
			return builder;
		}
	}
}
