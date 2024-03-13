using Microsoft.EntityFrameworkCore;
using PizzaCatalog;
using PizzaCatalog.Data;
using PizzaCatalog.Protos;
using PizzaCatalog.Services;

public static class Configuration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc();

        builder.Services.AddDbContext<PizzaContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("PizzaContext"))
        );

        builder.Services.AddSingleton<PreparingQueue>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddGrpcClient<Ordering.OrderingClient>(o =>
        {
            o.Address = new Uri("http://ordering:5002");
        });
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<PizzaContext>();
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

        app.MapGrpcService<CatalogService>();
    }
}
