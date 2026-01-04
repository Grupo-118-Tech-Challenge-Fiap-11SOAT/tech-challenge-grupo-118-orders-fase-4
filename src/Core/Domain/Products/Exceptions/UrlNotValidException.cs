using Domain.Base.Exceptions;

namespace Domain.Products.Exceptions;

/// <summary>
/// Represents an exception that occurs when a provided URL is not valid.
/// </summary>
/// <remarks>
/// This exception is typically thrown when an invalid URL format is encountered in domain operations,
/// such as during the validation of an image URL within the product domain.
/// </remarks>
public class UrlNotValidException : DomainException
{
    /// <summary>
    /// Gets the error message indicating that provider url is not valid.
    /// </summary>
    public override string Message => "The provided URL is not valid.";
}