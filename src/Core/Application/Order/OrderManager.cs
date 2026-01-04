using Domain.Order.Dtos;
using Domain.Order.Entities;
using Domain.Order.Exceptions;
using Domain.Order.Ports.In;
using Domain.Order.Ports.Out;
using Domain.Order.ValueObjects;
using Domain.Products.Entities;
using Domain.Products.Ports.In;

namespace Application.Order;

public class OrderManager : IOrderManager
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductManager _productManager;

    public OrderManager(IOrderRepository orderRepository, IProductManager productManager)
    {
        _orderRepository = orderRepository;
        _productManager = productManager;
    }


    public async Task<List<OrderResponseDto?>> GetAllAsync(OrderStatus status, int skip, int take,
        CancellationToken cancellationToken)
    {
        var ordersList = await _orderRepository.GetAllAsync(status, cancellationToken, skip, take);

        var result = new List<OrderResponseDto>(ordersList.Count);

        foreach (var order in ordersList)
        {
            result.Add(new OrderResponseDto(order));
        }

        return result;
    }
    
    public async Task<List<OrderResponseDto>?> GetOrdersToMonitorAsync(CancellationToken cancellationToken = default,
        int skip = 0, int take = 10)
    {
        {
            var orderEntities = await _orderRepository.GetOrdersToMonitorAsync(cancellationToken, skip, take);

            if (orderEntities is null)
                return null;
            
            var orderDtos = new List<OrderResponseDto>();
            
            orderEntities.ForEach(orderEntity =>
            {
                var orderItems = CreateOrderItemsFromOrder(orderEntity);

                orderDtos.Add(new OrderResponseDto(orderEntity.OrderNumber,
                    orderEntity.Cpf,
                    orderEntity.Total,
                    orderEntity.Status,
                    orderEntity.IsActive,
                    orderItems,
                    orderEntity.Id,
                    orderEntity.CreatedAt,
                    orderEntity.UpdatedAt));
            });
            
            return orderDtos;
        }
    }
    
    public async Task<OrderResponseDto> CreateAsync(OrderRequestDto orderDto, CancellationToken cancellationToken)
    {
        int[] productIds = orderDto.Items.Select(i => i.ProductId).ToArray();
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
            Id = order.Id
        };

        return orderDtoResult;
    }

    public async Task<OrderResponseDto> UpdateStatusAsync(int orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order is null)
            throw new OrderNotFoundExpetion(orderId);

        order.ChangeStatus();
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new OrderResponseDto(order);
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (order is null)
            return null;
        
        var result = new OrderResponseDto(order);

        return result;
    }
    
    private List<OrderItem> CreateOrderItemsFromOrder(Domain.Order.Entities.Order order)
    {
        var orderItems = new List<OrderItem>();

        order.OrderItems.ToList().ForEach(item =>
        {
            orderItems.Add(new OrderItem(item.ProductId, item.Quantity, order.Id,
                new Product(item.Product.Name,
                    item.Product.Description,
                    item.Product.Category,
                    item.Product.Price,
                    item.Product.IsActive,
                    item.Product.Id)));
        });

        return orderItems;
    }
}


