using Domain.Base.Exceptions;

namespace Domain.Order.Exceptions;

public class OrderNotFoundExpetion(int orderId) : DomainException
{
    public override string Message => $"Order {orderId} not found.";
}