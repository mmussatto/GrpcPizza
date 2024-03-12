using Microsoft.EntityFrameworkCore;
using PizzaCatalog.Models;

namespace PizzaCatalog;

public class PizzaContext : DbContext
{
    public PizzaContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Pizza> Pizzas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Pizza>()
            .HasData(
                [
                    new Pizza
                    {
                        Id = 1,
                        Name = "Pepperoni",
                        Description = "Nice Pizza"
                    },
                    new Pizza
                    {
                        Id = 2,
                        Name = "Pepperoni and Cheese",
                        Description = "Very Nice Pizza"
                    }
                ]
            );
    }
}
