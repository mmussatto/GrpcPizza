using PizzaCatalog.Models;

namespace PizzaCatalog.Data;

public record PreparingPizza
{
    public Pizza Pizza { get; set; }
    public int OrderId { get; set; }
    public bool IsDone { get; set; } = false;

    public async void Prepare()
    {
        await Task.Delay(10000);
        IsDone = true;
    }
}

public class PreparingQueue
{
    private static List<PreparingPizza> _preparingQueue = [];

    public void PreparePizza(Pizza pizza, int OrderId)
    {
        var preparingPizza = new PreparingPizza { Pizza = pizza, OrderId = OrderId };
        preparingPizza.Prepare();
        _preparingQueue.Add(preparingPizza);
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
