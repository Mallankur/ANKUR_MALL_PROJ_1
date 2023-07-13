using System;
using Adform.BusinessAccount.Api;
using Adform.BusinessAccount.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Adform.BusinessAccount.Integration.Test;

[SetUpFixture]
public partial class Testing
{
	private static WebApplicationFactory<Startup> _factory = null!;
	private static IServiceScopeFactory _scopeFactory = null!;
	private static IBusinessAccountService businessAccountService = null;
	private static ISsoConfigurationService ssoConfigurationService = null;
	private static IMongoDatabase mongodb = null;

	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		_factory = new CustomWebApplicationFactory();
		_scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
		businessAccountService = _factory.Services.GetRequiredService<IBusinessAccountService>();
		ssoConfigurationService = _factory.Services.GetRequiredService<ISsoConfigurationService>();
		mongodb = _factory.Services.GetRequiredService<IMongoDatabase>();
	}

	public static IBusinessAccountService BusinessAccountService() => businessAccountService;
	public static ISsoConfigurationService SsoConfigService() => ssoConfigurationService;

	public static IMongoDatabase MongoDatabase() => mongodb;

	public static void CreateTestMongoDb()
	{
		var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var connectionString = configuration["Mongo:ConnectionString"];
        var log = $"Connection String: {connectionString}, DB: {mongodb.Client.Settings.Server.ToString()}";
		System.Diagnostics.Debug.WriteLine(log);
        Console.WriteLine(log);
		
		mongodb.CreateCollection("SsoConfigurations", new CreateCollectionOptions { Capped = false });
		var ssoConfiguration = MongoDatabase().GetCollection<BsonDocument>("SsoConfigurations");
		var name = new BsonDocument
			{
				{ "name", 1 }
			};

		var ssoConfigIndex = new CreateIndexModel<BsonDocument>(name, new CreateIndexOptions { Unique = true });
		ssoConfiguration.Indexes.CreateOne(ssoConfigIndex);

		mongodb.CreateCollection("BusinessAccounts", new CreateCollectionOptions { Capped = false });
		var businessAccount = MongoDatabase().GetCollection<BsonDocument>("BusinessAccounts");
		var nameType = new BsonDocument
			{
				{ "name", 1 },
				{ "type", 1 }
			};

		var businessAccountIndex = new CreateIndexModel<BsonDocument>(nameType, new CreateIndexOptions { Unique = true });
		businessAccount.Indexes.CreateOne(businessAccountIndex);
	}

	public static void DropTestMongoDb()
	{
		mongodb.Client.DropDatabase(mongodb.DatabaseNamespace.DatabaseName);
	}

	public static Task ResetState()
	{
		return Task.CompletedTask;
	}

	[OneTimeTearDown]
	public void RunAfterAnyTests()
	{
	}

	protected async Task DropBusinessAccounts()
	{
		await mongodb.DropCollectionAsync("BusinessAccounts");
	}
}