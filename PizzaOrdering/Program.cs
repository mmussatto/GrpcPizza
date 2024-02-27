using Microsoft.EntityFrameworkCore;
using PizzaOrdering.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("OrderContext"))
);

var app = builder.Build();

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

app.MapGet("/", () => "Hello World!");

app.Run();
