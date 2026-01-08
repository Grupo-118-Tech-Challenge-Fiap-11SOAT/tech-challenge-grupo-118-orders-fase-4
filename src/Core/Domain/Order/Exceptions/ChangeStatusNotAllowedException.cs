
using Domain.Base.Exceptions;
using Domain.Order.Entities;
using Domain.Order.ValueObjects;

namespace Domain.Order.Exceptions;

public class ChangeStatusNotAllowedException(OrderStatus status) : DomainException
{
    public override string Message => $"It is not possible to change the status when it is as {status}";
}

