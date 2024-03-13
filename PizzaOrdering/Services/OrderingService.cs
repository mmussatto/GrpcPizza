using Grpc.Core;
using PizzaOrdering.Data;
using PizzaOrdering.Protos;

namespace PizzaOrdering.Services;

public class OrderingService : Ordering.OrderingBase
{
    private readonly OrderContext _context;

    public OrderingService(OrderContext context)
    {
        _context = context;
    }

    public override async Task<PizzaIsDoneResponse> PizzaIsDone(
        PizzaIsDoneRequest request,
        ServerCallContext context
    )
    {
        var order = await _context.Orders.FindAsync(request.OrderId);

        if (order is not null)
            order.Done = true;

        await _context.SaveChangesAsync();

        return new PizzaIsDoneResponse { };
    }
}
