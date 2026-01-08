namespace Domain.Order.Dtos;

public class OrderRequestDto
{
    public string? Cpf { get; set; }
    
    public List<OrderItemDto> Items { get; set; }
}