using Microsoft.EntityFrameworkCore;
using PizzaCatalog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PizzaContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PizzaContext"))
);

var app = builder.Build();

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

app.MapGet("/", () => "Hello World!");

app.Run();
