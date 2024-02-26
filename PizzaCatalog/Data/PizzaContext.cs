using Microsoft.EntityFrameworkCore;
using PizzaCatalog.Models;

namespace PizzaCatalog;

public class PizzaContext : DbContext
{
    public PizzaContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Pizza>? Pizzas { get; set; }
}
