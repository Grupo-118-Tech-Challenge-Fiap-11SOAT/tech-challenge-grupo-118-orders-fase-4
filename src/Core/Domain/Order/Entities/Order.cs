using Domain.Order.Dtos;
using Domain.Base.Entities;
using Domain.Order.Exceptions;
using Domain.Products.Ports.In;
using System.Threading;
using Domain.Base.Exceptions;
using Domain.Base.Extensions;
using Domain.Order.ValueObjects;
using Domain.Products.Dtos;
using Domain.Products.Entities;

namespace Domain.Order.Entities;

public class Order : BaseEntity
{
    public int OrderNumber { get; protected set; }
    public string? Cpf { get; protected set; }
    public decimal Total { get; protected set; }
    public OrderStatus Status { get; protected set; }
    public ICollection<OrderItem> OrderItems { get; protected set; }

    private static readonly Dictionary<OrderStatus, OrderStatus?> NextStatus = new()
    {
        { OrderStatus.Received, OrderStatus.InPreparation },
        { OrderStatus.InPreparation, OrderStatus.Ready },
        { OrderStatus.Ready, OrderStatus.Completed },
        { OrderStatus.Completed, null },
        { OrderStatus.Canceled, null }
    };

    public Order()
    {
    }


    public Order(OrderRequestDto orderDto, List<ProductDto> products)
    {
        var random = new Random();

        OrderNumber = random.Next(100000, 1000000);

        Cpf = orderDto.Cpf is null ? orderDto.Cpf : orderDto.Cpf.SanitizeCpf();

        Status = OrderStatus.Received;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;

        OrderItems =
            orderDto.Items?.Select(item => new OrderItem(Id, item.ProductId, item.Quantity)).ToList() ??
            new List<OrderItem>();

        Total = orderDto.Items?.Sum(item =>
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            return product?.Price * item.Quantity;
        }) ?? 0;

        ValidateOrder();
    }

    public Order(OrderRequestDto orderDto, List<Product> activeProducts)
    {
        throw new NotImplementedException();
    }

    private void ValidateOrder()
    {
        if (Cpf is not null && !Cpf.IsValidCpf())
            throw new InvalidCpfException();

        if (OrderItems.Count == 0)
            throw new EmptyOrderItemsException();
    }

    public void ChangeStatus()
    {
        if (NextStatus.TryGetValue(Status, out OrderStatus? nextStatus))
        {
            if (nextStatus is null)
                throw new ChangeStatusNotAllowed(Status);

            Status = nextStatus.Value;
            UpdatedAt = DateTimeOffset.Now;
        }
        else
        {
            throw new ChangeStatusInvalidException();
        }
    }
}