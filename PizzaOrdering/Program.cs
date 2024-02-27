using Microsoft.EntityFrameworkCore;
using PizzaOrdering.Data;
using PizzaOrdering.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("OrderContext"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/orders", async (OrderContext ctx) => await ctx.Orders.ToListAsync());

app.MapGet("/orders/{id}", async (OrderContext ctx, int id) => await ctx.Orders.FindAsync(id));

app.MapPost(
    "/orders",
    async (OrderContext ctx, Order order) =>
    {
        await ctx.Orders.AddAsync(order);
        await ctx.SaveChangesAsync();
        return Results.Created($"/orders/{order.Id}", order);
    }
);

app.MapDelete(
    "/orders/{id}",
    async (OrderContext ctx, int id) =>
    {
        var order = await ctx.Orders.FindAsync(id);
        if (order is null)
            return Results.NotFound();
        ctx.Orders.Remove(order);
        await ctx.SaveChangesAsync();
        return Results.Ok();
    }
);

app.Run();
