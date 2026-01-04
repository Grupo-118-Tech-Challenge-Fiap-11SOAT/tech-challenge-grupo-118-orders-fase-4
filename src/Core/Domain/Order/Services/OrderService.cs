using Domain.Base.Exceptions;
using Domain.Order.Ports.Out;
using Domain.Order.Services.Interfaces;
using Domain.Order.ValueObjects;

namespace Domain.Order.Services;

public class OrderService(IOrderRepository repository) : IOrderService
{
    public async Task<Entities.Order> ValidadeByIdAsync(int id, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        return order ?? throw new DomainException($"Order with id {id} not found.");
    }

    public async Task ConfirmAsync(Entities.Order order, CancellationToken cancellationToken)
    {
        if (order.Status != OrderStatus.Received)
            throw new DomainException($"Order is not in a received state.");

        order.ChangeStatus();
        
        await repository.UpdateAsync(order, cancellationToken);
    }
}