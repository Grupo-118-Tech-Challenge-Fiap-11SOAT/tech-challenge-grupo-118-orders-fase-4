using Domain.Order.Dtos;
using Domain.Order.Entities;
using Domain.Order.ValueObjects;

namespace Domain.Order.Ports.Out;

public interface IOrderRepository
{
    /// <summary>
    /// Retrieves a list of orders based on pagination parameters.
    /// </summary>
    /// <param name="status">Order Status to filter list .</param>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The number of items to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of orders.</returns>
    /// 
    Task<List<Entities.Order>> GetAllAsync(OrderStatus status, CancellationToken cancellationToken = default, int skip = 0, int take = 10);
    /// <summary>
    /// Retrieves a list of orders following a specific criteria for monitoring purposes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    Task<List<Entities.Order>> GetOrdersToMonitorAsync(CancellationToken cancellationToken = default, int skip = 0, int take = 10);
    
    /// <summary>
    /// Creates a new order in the system.
    /// </summary>
    /// <param name="order">The order entity containing the details of the order to be created.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The unique identifier of the created order.</returns>
    Task<Entities.Order> CreateAsync(Entities.Order order, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a oder by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The number of items to retrieve.</param>   
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order matching the specified identifier, or null if no such order exists.</returns>
    Task<Entities.Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order with the provided details.
    /// Updates an existing order with the provided details.
    /// </summary>
    /// <param name="order">The order data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated Order entity</returns>
    Task<Entities.Order> UpdateAsync(Entities.Order order, CancellationToken cancellationToken = default);
}