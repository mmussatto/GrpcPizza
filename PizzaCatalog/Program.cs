using Microsoft.EntityFrameworkCore;
using PizzaCatalog;
using PizzaCatalog.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PizzaContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PizzaContext"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/pizzas/{id}", async (PizzaContext ctx, int id) => await ctx.Pizzas.FindAsync(id));

app.MapGet("/pizzas", async (PizzaContext ctx) => await ctx.Pizzas.ToListAsync());

app.MapPost(
    "/pizzas",
    async (PizzaContext ctx, Pizza pizza) =>
    {
        await ctx.Pizzas.AddAsync(pizza);
        await ctx.SaveChangesAsync();
        return Results.Created($"/pizza/{pizza.Id}", pizza);
    }
);

app.MapPut(
    "/pizzas/{id}",
    async (PizzaContext ctx, Pizza updatePizza, int id) =>
    {
        var pizza = await ctx.Pizzas.FindAsync(id);
        if (pizza is null)
            return Results.NotFound();
        pizza.Name = updatePizza.Name;
        pizza.Description = updatePizza.Description;
        await ctx.SaveChangesAsync();
        return Results.NoContent();
    }
);

app.MapDelete(
    "/pizzas/{id}",
    async (PizzaContext ctx, int id) =>
    {
        var pizza = await ctx.Pizzas.FindAsync(id);
        if (pizza is null)
            return Results.NotFound();
        ctx.Pizzas.Remove(pizza);
        await ctx.SaveChangesAsync();
        return Results.Ok();
    }
);

app.Run();
