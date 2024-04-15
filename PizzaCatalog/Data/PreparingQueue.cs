using PizzaCatalog.Models;
using PizzaCatalog.Protos;

namespace PizzaCatalog.Data;

public record PreparingPizza
{
    public required Pizza Pizza { get; set; }
    public int OrderId { get; set; }
    public bool IsDone { get; set; } = false;

    public async Task<bool> Prepare()
    {
        await Task.Delay(10000);
        IsDone = true;
        return true;
    }
}

public class PreparingQueue
{
    private static List<PreparingPizza> _preparingQueue = [];

    private readonly Ordering.OrderingClient _client;

    public PreparingQueue(Ordering.OrderingClient client)
    {
        _client = client;
    }

    public async void PreparePizza(Pizza pizza, int OrderId)
    {
        var preparingPizza = new PreparingPizza { Pizza = pizza, OrderId = OrderId };
        _preparingQueue.Add(preparingPizza);

        await Task.Run(() => preparingPizza.Prepare());
        await _client.PizzaIsDoneAsync(new PizzaIsDoneRequest { OrderId = preparingPizza.OrderId });
    }

    public bool CancelPizza(int orderId)
    {
        var preparingPizza = _preparingQueue.Find(p => p.OrderId == orderId);

        if (preparingPizza is null)
            return false;

        _preparingQueue.Remove(preparingPizza);

        return true;
    }

    public string PrintQueue()
    {
        if (_preparingQueue.Count == 0)
        {
            return "No Pizzas are being prepared right now!";
        }

        var queueStr = $"";
        foreach (var pizza in _preparingQueue)
        {
            queueStr +=
                $"\n --- Pizza:"
                + pizza.Pizza.Name
                + ", Order: "
                + pizza.OrderId
                + ", IsDone: "
                + pizza.IsDone;
        }
        return queueStr;
    }
}
