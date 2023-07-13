using Adform.BusinessAccount.Api.Capabilities;
using Adform.BusinessAccount.Api.Metrics;
using Adform.BusinessAccount.Application;
using Adform.Ciam.Authorization.Extensions;
using Adform.Ciam.ExceptionHandling.Extensions;
using Adform.Ciam.Health.Extensions;
using Adform.Ciam.Logging.Extensions;
using Adform.Ciam.Mongo.Services;
using Adform.Ciam.Monitoring.Abstractions.Provider;
using Adform.Ciam.Monitoring.Extensions;
using Adform.Ciam.Monitoring.Metrics;
using Adform.Ciam.SharedKernel.Configuration;
using Adform.Ciam.TokenProvider.Extensions;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using System;
using System.Linq;

namespace Adform.BusinessAccount.Api
{
	public class Startup
	{
		private IConfigurationRoot Configuration { get; }

		public Startup(IWebHostEnvironment hostingEnvironment)
		{
			Configuration = hostingEnvironment.ConfigureConfig();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.ConfigureConfigurationValidation()
				.ConfigureOAuth(Configuration)
				.ConfigureOauthClient(Configuration)
				.ConfigureInjection(Configuration)
				.ConfigureMetrics(ApiMetrics.HelloWorldGauge, CommonMetrics.ClientExecutionDuration,
					CommonMetrics.MongoExecutionDuration, CommonMetrics.TokenProviderExecutionDuration,
					CommonMetrics.CacheExecutionDuration)
				.ConfigureSwagger(Configuration)
                .ConfigureAuthorization((options) =>
                {
                    options.AddScopePolicy(StartupOAuth.Scopes.Full);
                    options.AddScopePolicy(StartupOAuth.Scopes.Readonly);
					options.AddScopePolicy(StartupOAuth.Scopes.Readonly,StartupOAuth.Scopes.Full);
				})
				.ConfigureHealth(Configuration,
					b => b.AddCheck<MongoHealthCheck>(Ciam.Mongo.Tags.Mongo, HealthStatus.Unhealthy,
							new[] { Ciam.Mongo.Tags.Mongo }))
				.ConfigureLogging(Configuration)
				.ConfigureApplicationServices()
				.ConfigureMvc();
			services.AddDefaultCorrelationId(o =>
			{
				o.UpdateTraceIdentifier = false;
				o.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
				o.ResponseHeader = "CorrelationId";
				o.IncludeInResponse = true;
			});

		}

		public void Configure(
			IApplicationBuilder app,
			IWebHostEnvironment env,
			IApiVersionDescriptionProvider apiVersionDescriptionProvider,
			IServiceProvider serviceProvider
		)
		{
			app.UseCorrelationId()
				.UseMaaSHostMiddleware()
				.UseErrorHandler(serviceProvider.GetRequiredService<IMetricsProvider>()
					.GetCounter(DefaultMetrics.ErrorCounter.Name))
				.UseRouting()
				.UseAuthentication()
				.UseAuthorization()
				.UseLaaSMiddleware()
				.UseEndpoints(endpoints =>
				{
					endpoints.MapMetrics();
					endpoints.MapControllers();
				})
				.UseHealthEndpoints(health =>
					new[] { Ciam.Health.Tags.Readiness, Ciam.Mongo.Tags.Mongo }.Contains(health.Name))
				.UseSwaggerUi(apiVersionDescriptionProvider, Configuration);
		}
	}
}
