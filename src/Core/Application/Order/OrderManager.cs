using Domain.Order.Dtos;
using Domain.Order.Entities;
using Domain.Order.Exceptions;
using Domain.Order.Ports.In;
using Domain.Order.Ports.Out;
using Domain.Order.ValueObjects;
using Domain.Payment.Ports.Dtos;
using Domain.Payment.Ports.Out;
using Domain.Products.Entities;
using Domain.Products.Ports.In;

namespace Application.Order;

public class OrderManager : IOrderManager
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductManager _productManager;
    private readonly IPaymentManager _paymentManager;

    public OrderManager(IOrderRepository orderRepository, IProductManager productManager,
        IPaymentManager paymentManager)
    {
        _orderRepository = orderRepository;
        _productManager = productManager;
        _paymentManager = paymentManager;
    }


    public async Task<List<OrderResponseDto?>> GetAllAsync(OrderStatus status, int skip = 0, int take = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ordersList = await _orderRepository.GetAllAsync(status, skip, take, cancellationToken);

            var result = new List<OrderResponseDto?>(ordersList.Count);

            foreach (var order in ordersList)
            {
                result.Add(new OrderResponseDto(order));
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error retrieving orders", ex);
        }
    }

    public async Task<List<OrderResponseDto>?> GetOrdersToMonitorAsync(CancellationToken cancellationToken = default,
        int skip = 0, int take = 10)
    {
        try
        {
            var orderEntities = await _orderRepository.GetOrdersToMonitorAsync(skip, take, cancellationToken);

            if (orderEntities is null)
                return null;

            var orderDtos = new List<OrderResponseDto>();

            orderEntities.ForEach(orderEntity =>
            {
                orderDtos.Add(new OrderResponseDto(orderEntity.OrderNumber,
                    orderEntity.Cpf,
                    orderEntity.Total,
                    orderEntity.Status,
                    orderEntity.IsActive,
                    orderEntity.OrderItems.ToList(),
                    orderEntity.Id,
                    orderEntity.CreatedAt,
                    orderEntity.UpdatedAt));
            });

            return orderDtos;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error retrieving orders to monitor", ex);
        }
    }

    public async Task<OrderResponseDto> CreateAsync(OrderRequestDto orderDto, CancellationToken cancellationToken)
    {
        string[] productIds = orderDto.Items.Select(i => i.ProductId).ToArray();
        var activeProducts = await _productManager.GetActiveProductsByIds(productIds, cancellationToken);

        var order = new Domain.Order.Entities.Order(orderDto, activeProducts);

        await _orderRepository.CreateAsync(order, cancellationToken);

        var orderDtoResult = new OrderResponseDto
        {
            Cpf = order.Cpf,
            Items = orderDto.Items,
            Total = order.Total,
            Status = order.Status,
            OrderNumber = order.OrderNumber,
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };

        var requestPayment = new CreatePaymentRequest
        {
            OrderId = order.Id.ToString(),
            Value = order.Total
        };

        var _ = await _paymentManager.CreatePaymentAsync(requestPayment);
        return orderDtoResult;
    }

    public async Task<OrderResponseDto> UpdateStatusAsync(int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

            if (order is null)
                throw new OrderNotFoundException(orderId);

            order.ChangeStatus();
            await _orderRepository.UpdateAsync(order, cancellationToken);

            return new OrderResponseDto(order);
        }
        catch (OrderNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error updating status for order {orderId}", ex);
        }
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

            if (order is null)
                return null;

            var result = new OrderResponseDto(order);

            return result;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error retrieving order {id}", ex);
        }
    }
}