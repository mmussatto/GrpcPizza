using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaOrdering.Data;
using PizzaOrdering.Models;
using PizzaOrdering.Protos;

namespace PizzaOrdering.Endpoints;

public static class OrderingEndpoints
{
    public static void RegisterOrderingEndpoints(this IEndpointRouteBuilder routes)
    {
        var path = routes.MapGroup("/api/v1/ordering");

        path.MapGet(
            "/orders",
            async (OrderContext ctx) =>
            {
                var orders = await ctx.Orders.ToListAsync();
                if (orders.Count == 0)
                    return Results.NotFound(new { error = "No orders found." });
                else
                    return Results.Ok(orders);
            }
        );

        path.MapGet(
            "/orders/{id}",
            async (OrderContext ctx, int id) => await ctx.Orders.FindAsync(id)
        );

        path.MapPost(
            "/orders",
            async (OrderContext ctx, Order order) =>
            {
                await ctx.Orders.AddAsync(order);
                await ctx.SaveChangesAsync();
                return Results.Created($"/orders/{order.Id}", order);
            }
        );

        path.MapDelete(
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

        path.MapDelete(
            "/orders",
            (OrderContext ctx) =>
            {
                foreach (var order in ctx.Orders)
                    ctx.Orders.Remove(order);
                ctx.SaveChanges();
                return Results.Ok();
            }
        );

        path.MapPost(
            "/orderPizza",
            async (
                string pizzaName,
                [FromServices] OrderContext ctx,
                [FromServices] Catalog.CatalogClient client
            ) =>
            {
                var pizzaExistsResponse = client.VerifyPizzaExists(
                    new VerifyPizzaRequest { PizzaName = pizzaName }
                );

                Console.WriteLine(
                    @$"----- RESPONSE ----- \n 
			Exists = {pizzaExistsResponse.PizzaExists},
			Id = {pizzaExistsResponse.PizzaId},
			Description = {pizzaExistsResponse.PizzaDescription},
			TESTE = {pizzaExistsResponse.PizzaExists == true}"
                );

                if (!pizzaExistsResponse.PizzaExists)
                    return Results.NotFound(pizzaExistsResponse);

                int pizzaId = (int)pizzaExistsResponse.PizzaId;

                var order = new Order() { PizzaId = pizzaId };

                await ctx.Orders.AddAsync(order);
                await ctx.SaveChangesAsync();

                Console.WriteLine(
                    @$"----- RESPONSE ----- \n 
					ID: {order.Id},
					Pizza: {order.PizzaId},
					Date: {order.OrderDateTime},
					Done: {order.Done}"
                );

                var preparePizzaResponse = client.PreparePizza(
                    new PreparePizzaRequest { PizzaId = pizzaId, OrderId = order.Id }
                );

                return Results.Ok(preparePizzaResponse);
            }
        );
    }
}
