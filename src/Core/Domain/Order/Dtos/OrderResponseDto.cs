using Domain.Order.Entities;
using System.Text.Json.Serialization;
using Domain.Order.ValueObjects;


namespace Domain.Order.Dtos;

public class OrderResponseDto
{
    public int Id { get; set; }
    public int OrderNumber { get; set; }
    public string? Cpf { get; set; }
    public decimal Total { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus Status { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt  { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; }

    public OrderResponseDto()
    {
        
    }

    public OrderResponseDto(Entities.Order order)
    {
        OrderNumber = order.OrderNumber;
        Cpf = order.Cpf;
        Total = order.Total;
        Status = order.Status;
        IsActive = order.IsActive;
        Items = order.OrderItems?.Select(item => new OrderItemDto
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        }).ToList() ?? new List<OrderItemDto>();
        Id = order.Id;
        CreatedAt = order.CreatedAt;
        UpdatedAt = order.UpdatedAt;
    }

    public OrderResponseDto(int orderEntityOrderNumber, string? orderEntityCpf, decimal orderEntityTotal, OrderStatus orderEntityStatus, bool orderEntityIsActive, List<OrderItem> orderItems, int orderEntityId, DateTimeOffset orderEntityCreatedAt, DateTimeOffset orderEntityUpdatedAt)
    {
        OrderNumber = orderEntityOrderNumber;
        Cpf = orderEntityCpf;
        Total = orderEntityTotal;
        Status = orderEntityStatus;
        IsActive = orderEntityIsActive;
        Items = orderItems?.Select(item => new OrderItemDto
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        }).ToList() ?? new List<OrderItemDto>();
        Id = orderEntityId;
        CreatedAt = orderEntityCreatedAt;
        UpdatedAt = orderEntityUpdatedAt;
    }
}