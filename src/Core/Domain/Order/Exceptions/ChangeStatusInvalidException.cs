using Domain.Base.Exceptions;

namespace Domain.Order.Exceptions;

public class ChangeStatusInvalidException : DomainException
{
    public override string Message => $"Current status is not recognized.";
}