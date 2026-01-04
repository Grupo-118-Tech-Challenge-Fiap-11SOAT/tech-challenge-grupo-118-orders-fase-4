namespace Domain.Order.Services.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Retrieves a oder by its unique identifier and thorows an exception if not found.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order matching the specified identifier</returns>
    Task<Entities.Order> ValidadeByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Confurm a order.
    /// </summary>
    /// <param name="order">The order to confirm.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task ConfirmAsync(Entities.Order order, CancellationToken cancellationToken);
}