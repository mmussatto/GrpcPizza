using Microsoft.EntityFrameworkCore;
using PizzaCatalog.Data;
using PizzaCatalog.Models;

namespace PizzaCatalog.Endpoints;

public static class CatalogEndpoints
{
    public static void RegisterCatalogEndpoints(this IEndpointRouteBuilder routes)
    {
        var path = routes.MapGroup("/api/v1/catalog");

        path.MapGet(
            "/pizzas/{id}",
            async (PizzaContext ctx, int id) => await ctx.Pizzas.FindAsync(id)
        );

        path.MapGet("/pizzas", async (PizzaContext ctx) => await ctx.Pizzas.ToListAsync());

        path.MapPost(
            "/pizzas",
            async (PizzaContext ctx, Pizza pizza) =>
            {
                await ctx.Pizzas.AddAsync(pizza);
                await ctx.SaveChangesAsync();
                return Results.Created($"/pizzas/{pizza.Id}", pizza);
            }
        );

        path.MapPut(
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

        path.MapDelete(
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

        path.MapGet(
            "/preparing",
            (PreparingQueue queue) =>
            {
                return queue.PrintQueue();
            }
        );
    }
}
