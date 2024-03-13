using Microsoft.EntityFrameworkCore;
using PizzaOrdering.Data;
using PizzaOrdering.Protos;
using PizzaOrdering.Services;

namespace PizzaOrdering;

public static class Configuration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc();

        builder.Services.AddDbContext<OrderContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("OrderContext"))
        );

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddGrpcClient<Catalog.CatalogClient>(o =>
        {
            o.Address = new Uri("http://catalog:5000");
        });
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<OrderContext>();
                var created = context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapGrpcService<OrderingService>();
    }
}
