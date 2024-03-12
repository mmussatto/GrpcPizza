using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PizzaCatalog.Data;
using PizzaCatalog.Protos;

namespace PizzaCatalog.Services;

public class CatalogService : Catalog.CatalogBase
{
    private readonly PizzaContext _context;
    private readonly PreparingQueue _queue;

    public CatalogService(PizzaContext context, PreparingQueue queue)
    {
        _context = context;
        _queue = queue;
    }

    public override async Task<VerifyPizzaResponse> VerifyPizzaExists(
        VerifyPizzaRequest request,
        ServerCallContext context
    )
    {
        var pizza = await _context.Pizzas.FirstOrDefaultAsync(p =>
            p.Name.ToUpper() == request.PizzaName.ToUpper()
        );

        return new VerifyPizzaResponse
        {
            PizzaExists = pizza?.Id != null,
            PizzaId = pizza?.Id,
            PizzaDescription = pizza?.Description
        };
    }

    public override async Task<PreparePizzaResponse> PreparePizza(
        PreparePizzaRequest request,
        ServerCallContext context
    )
    {
        var pizza = await _context.Pizzas.FirstOrDefaultAsync(p => p.Id == request.PizzaId);

        if (pizza is null)
            return new PreparePizzaResponse
            {
                PizzaIsPreparing = false,
                Message = $"Pizza with Id {request.PizzaId} could not be found!"
            };

        _queue.PreparePizza(pizza, request.OrderId);

        return new PreparePizzaResponse
        {
            PizzaIsPreparing = true,
            Message = $"{pizza.Name} pizza is being prepared!"
        };
    }

    public override async Task<CancelPizzaResponse> CancelPizza(
        CancelPizzaRequest request,
        ServerCallContext context
    )
    {
        var pizzaWasCanceled = _queue.CancelPizza(request.OrderId);

        return new CancelPizzaResponse
        {
            PizzaCanceled = pizzaWasCanceled,
            Response = pizzaWasCanceled
                ? $"Pizza for Order {request.OrderId} was canceled successfully"
                : $"Order {request.OrderId} was not found!"
        };
    }
}
