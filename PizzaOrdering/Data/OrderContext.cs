using Microsoft.EntityFrameworkCore;
using PizzaOrdering.Models;

namespace PizzaOrdering.Data;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Order> Orders { get; set; } = null!;
}
