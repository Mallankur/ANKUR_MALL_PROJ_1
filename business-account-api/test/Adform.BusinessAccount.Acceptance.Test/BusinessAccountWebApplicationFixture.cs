using Adform.BusinessAccount.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Adform.BusinessAccount.Acceptance.Test;

public class BusinessAccountWebApplicationFixture : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("AcceptanceTest");
    }
}