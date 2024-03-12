namespace PizzaOrdering.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDateTime { get; set; } = DateTime.UtcNow;
    public int PizzaId { get; set; }
    public bool Done { get; set; } = false;
}
