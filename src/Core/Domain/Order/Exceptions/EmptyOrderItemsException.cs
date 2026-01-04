using Domain.Base.Exceptions;

namespace Domain.Order.Exceptions;

public class EmptyOrderItemsException : DomainException
{
    public override string Message => $"Cannot create an order with no items.";
}