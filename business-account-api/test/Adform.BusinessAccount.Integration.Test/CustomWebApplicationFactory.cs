using Adform.BusinessAccount.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Adform.BusinessAccount.Integration.Test;
internal class CustomWebApplicationFactory : WebApplicationFactory<Startup>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("IntegrationTest");
	}
}