using Domain.Order.Ports.Out;
using Domain.Order.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer.Order.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Domain.Order.Entities.Order>> GetAllAsync(OrderStatus status, int skip = 0, int take = 10,
        CancellationToken cancellationToken = default)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.Status.Equals(status))
            .Skip(skip)
            .Take(take)
            .Include(o => o.OrderItems)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return orders;
    }
    
    public async Task<List<Domain.Order.Entities.Order>> GetOrdersToMonitorAsync(
         int skip = 0, int take = 10, CancellationToken cancellationToken = default)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Canceled)
            .Skip(skip)
            .Take(take)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.Status == OrderStatus.Ready)
            .ThenByDescending(o => o.Status == OrderStatus.InPreparation)
            .ThenByDescending(o => o.Status == OrderStatus.Received)
            .ThenBy(o => o.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return orders;
    }

    public async Task<Domain.Order.Entities.Order> CreateAsync(Domain.Order.Entities.Order order,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return order;
    }

    public async Task<Domain.Order.Entities.Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Domain.Order.Entities.Order> UpdateAsync(Domain.Order.Entities.Order order,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return order;
    }
}