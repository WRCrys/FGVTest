using FGVTest.Business.Interfaces.Repositories;
using FGVTest.Test.E2E.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FGVTest.Test.E2E;

public class SalesApiFactory : WebApplicationFactory<Program>
{
    private static readonly DbContextOptions<TestDbContext> DbOptions =
        new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveExistingRegistration<IClienteRepository>(services);
            RemoveExistingRegistration<IProdutoRepository>(services);
            RemoveExistingRegistration<IPedidoRepository>(services);

            services.AddSingleton(new TestDbContext(DbOptions));

            services.AddScoped<IClienteRepository>(sp =>
                new TestClienteRepository(sp.GetRequiredService<TestDbContext>()));
            services.AddScoped<IProdutoRepository>(sp =>
                new TestProdutoRepository(sp.GetRequiredService<TestDbContext>()));
            services.AddScoped<IPedidoRepository>(sp =>
                new TestPedidoRepository(sp.GetRequiredService<TestDbContext>()));
        });
    }

    private static void RemoveExistingRegistration<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor is not null)
            services.Remove(descriptor);
    }
}
