namespace PizzaCatalog.Models;

public class Pizza
{
    public int? Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
