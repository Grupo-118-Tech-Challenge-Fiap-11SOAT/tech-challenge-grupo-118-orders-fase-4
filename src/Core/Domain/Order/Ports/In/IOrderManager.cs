using Domain.Order.Dtos;
using Domain.Order.ValueObjects;

namespace Domain.Order.Ports.In;

public interface IOrderManager
{
    /// <summary>
    /// Retrieves a list of orders based on pagination parameters.
    /// </summary>
    /// <param name="status">Order Status to filter list .</param>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The number of items to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of orders.</returns>
    Task<List<OrderResponseDto?>> GetAllAsync(OrderStatus status, int skip = 0, int take = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new order in the system.
    /// </summary>
    /// <param name="orderDto">The order dto containing the details of the order to be created.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The unique identifier of the created order.</returns>
    Task<OrderResponseDto> CreateAsync(OrderRequestDto orderDto, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing order with the provided details.
    /// Updates an existing order with the provided details.
    /// </summary>
    /// <param name="orderId">The order id to be update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated Order entity</returns>
    Task<OrderResponseDto> UpdateStatusAsync(int orderId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a oder by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order matching the specified identifier, or null if no such order exists.</returns>
    Task<OrderResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<List<OrderResponseDto>?> GetOrdersToMonitorAsync(CancellationToken cancellationToken = default,
        int skip = 0, int take = 10);
}