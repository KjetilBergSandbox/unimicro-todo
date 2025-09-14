using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Api.Data;

namespace Todo.Api.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program> // Todo.Api.Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("InMemoryTestDb"));
            });
        }
    }
}