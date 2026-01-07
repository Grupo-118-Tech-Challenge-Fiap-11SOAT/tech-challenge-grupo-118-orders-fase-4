using Domain.Base.Exceptions;

namespace Domain.Order.Exceptions;

public class OrderNotFoundException(int orderId) : DomainException
{
    public override string Message => $"Order {orderId} not found.";
}