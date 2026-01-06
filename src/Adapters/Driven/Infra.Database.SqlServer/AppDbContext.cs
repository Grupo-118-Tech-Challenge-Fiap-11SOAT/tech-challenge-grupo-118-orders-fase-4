using Infra.Database.SqlServer.Order.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer;

public class AppDbContext : DbContext
{
    public DbSet<Domain.Order.Entities.Order> Orders { get; set; }
    public DbSet<Domain.Order.Entities.OrderItem> OrderItems { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
    }
}