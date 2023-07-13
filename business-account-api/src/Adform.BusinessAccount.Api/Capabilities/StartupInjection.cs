using Adform.BusinessAccount.Api.Metrics;
using Adform.BusinessAccount.Domain.Repositories;
using Adform.BusinessAccount.Infrastructure;
using Adform.BusinessAccount.Infrastructure.Decorators;
using Adform.BusinessAccount.Infrastructure.Repositories;
using Adform.Ciam.Cache.Decorators;
using Adform.Ciam.Health.Extensions;
using Adform.Ciam.Mongo.Configuration;
using Adform.Ciam.Mongo.Extensions;
using Adform.Ciam.Mongo.Patterns;
using Adform.Ciam.Mongo.Repositories;
using Adform.Ciam.Monitoring.Abstractions.Provider;
using Adform.Ciam.Monitoring.Handlers;
using Adform.Ciam.SharedKernel.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Pluralize.NET.Core;
using Polly;
using Polly.Registry;
using System;
using Adform.BusinessAccount.Api.Behaviours;
using Adform.BusinessAccount.Application.Decorators;
using Adform.Ciam.TokenProvider.Services;

namespace Adform.BusinessAccount.Api.Capabilities
{
	public static class StartupInjection
	{

		private const int RetryCount = 3;
		private const int SleepBetweenRetriesMs = 2000;

		public static IServiceCollection ConfigureInjection(this IServiceCollection services,
			IConfigurationRoot configuration)
		{
			//services.AddScoped<IUserService, UserService>();
			services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

			services.AddDistributedMemoryCache();
			services.ConfigureMongo(configuration);
            
			var plural = new Pluralizer();
			services.AddSingleton<IBaseMongoRepository<Domain.Repositories.Entities.BusinessAccount, Guid>>(provider =>
			{
				var database = provider.GetRequiredService<IMongoDatabase>();
				var dateTimeProvider = provider.GetRequiredService<IDateTimeProvider>();
				var options = provider.GetRequiredService<IOptions<MongoConfiguration>>();
				return new BaseMongoRepository<Domain.Repositories.Entities.BusinessAccount>(
					options,
					database.GetCollection<Domain.Repositories.Entities.BusinessAccount>(
						plural.Pluralize(nameof(Domain.Repositories.Entities.BusinessAccount))),
					dateTimeProvider);
			});

			services.AddSingleton<IBaseMongoRepository<Domain.Repositories.Entities.SsoConfiguration, Guid>>(provider =>
			{
				var database = provider.GetRequiredService<IMongoDatabase>();
				var dateTimeProvider = provider.GetRequiredService<IDateTimeProvider>();
				var options = provider.GetRequiredService<IOptions<MongoConfiguration>>();
				return new BaseMongoRepository<Domain.Repositories.Entities.SsoConfiguration>(
					options,
					database.GetCollection<Domain.Repositories.Entities.SsoConfiguration>(
						plural.Pluralize(nameof(Domain.Repositories.Entities.SsoConfiguration))),
					dateTimeProvider);
			});

			services.Decorate<IBaseMongoRepository<Domain.Repositories.Entities.BusinessAccount, Guid>>((inner, sp) =>
				new MeasuredBaseMongoRepository<Domain.Repositories.Entities.BusinessAccount>(inner, sp.GetRequiredService<IMetricsProvider>()
					.GetHistogram(CommonMetrics.MongoExecutionDuration.Name)));

			services.Decorate<IBaseMongoRepository<Domain.Repositories.Entities.SsoConfiguration, Guid>>((inner, sp) =>
				new MeasuredBaseMongoRepository<Domain.Repositories.Entities.SsoConfiguration>(inner, sp.GetRequiredService<IMetricsProvider>()
					.GetHistogram(CommonMetrics.MongoExecutionDuration.Name)));

			var registry = services.AddPolicyRegistry();
			registry.AddBasicRetryPolicy(RetryCount, SleepBetweenRetriesMs);

			services.AddScoped<IBusinessAccountRepository, BusinessAccountRepository>();
			services.Decorate<IBusinessAccountRepository, BusinessAccountRepositoryCache>();

			services.AddScoped<ISsoConfigurationRepository, SsoConfigurationRepository>();
			services.Decorate<ISsoConfigurationRepository, SsoConfigurationRepositoryCache>();

			services.Decorate<IDistributedCache>((inner, sp) =>
				new MeasuredDistributedCache(inner,
					sp.GetRequiredService<IMetricsProvider>().GetHistogram(CommonMetrics.CacheExecutionDuration.Name)));

			services.Decorate<IBusinessAccountRepository>((inner, provider) =>
				new BusinessAccountRepositoryRetry(inner, provider.GetRequiredService<IReadOnlyPolicyRegistry<string>>()));

			services.Decorate<ISsoConfigurationRepository>((inner, provider) =>
				new SsoConfigurationRepositoryRetry(inner, provider.GetRequiredService<IReadOnlyPolicyRegistry<string>>()));

			//services.Configure<GeoClientSettings>(configuration.GetSection("GeoApi"));

			services.AddTransient(sp => new LatencyHandler(sp.GetRequiredService<IMetricsProvider>()
				.GetHistogram(CommonMetrics.ClientExecutionDuration.Name)));

			services.Decorate<ITokenProvider>((inner, sp) =>
				new MeasuredTokenProvider(inner, sp.GetRequiredService<IMetricsProvider>()
					.GetHistogram(CommonMetrics.TokenProviderExecutionDuration.Name)));

			/* services.AddHttpClient<IGeoClient, GeoClient>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("GeoApi")["Host"]);
            })
                .ConfigurePrimaryHttpMessageHandler(provider =>
                {
                    var tokenProvider =
                        (ITokenProvider)provider.GetRequiredService(typeof(ITokenProvider));
                    var oauthOptions = (IOptions<OAuth2Configuration>)provider
                        .GetRequiredService(typeof(IOptions<OAuth2Configuration>));
                    var clientOptions = (IOptions<GeoClientSettings>)provider
                        .GetRequiredService(typeof(IOptions<GeoClientSettings>));
                    return new OAuthHandler(tokenProvider, oauthOptions, clientOptions);
                })
                .AddHttpMessageHandler<LatencyHandler>(); */

			ConventionRegistry.Register(
			 "MyConvensions",
			 new ConventionPack
			 {
					new CamelCaseElementNameConvention()
			 },
			 _ => true);

			services.AddMediatR(typeof(Application.AssemblyReference).Assembly)
				.Decorate(typeof(IRequestHandler<,>), typeof(MeasuredHandler<,>));

			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
			services.AddValidatorsFromAssembly(typeof(Application.AssemblyReference).Assembly);

			services.AddWarmupFilter();

			return services;
		}


		private static IPolicyRegistry<string> AddBasicRetryPolicy(this IPolicyRegistry<string> policyRegistry,
			int retryCount, int sleepBetweenRetriesMs)
		{
			var retryPolicy = Policy
				.Handle<Exception>()
				.WaitAndRetryAsync(
					retryCount,
					c => TimeSpan.FromMilliseconds(sleepBetweenRetriesMs),
					(result, timeSpan, c, context) =>
					{
#warning TODO: Missing Logs
					})
				.WithPolicyKey(PolicyNames.BasicRetry);

			policyRegistry.Add(PolicyNames.BasicRetry, retryPolicy);

			return policyRegistry;
		}
	}
}
