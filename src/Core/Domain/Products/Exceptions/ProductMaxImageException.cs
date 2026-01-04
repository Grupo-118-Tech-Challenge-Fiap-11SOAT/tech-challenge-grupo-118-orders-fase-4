using Domain.Base.Exceptions;

namespace Domain.Products.Exceptions;

/// <summary>
/// Exception that is thrown when a product exceeds the allowed maximum number of images.
/// </summary>
public class ProductMaxImageException : DomainException
{
    /// <summary>
    /// Gets the message that describes the exception. This property is overridden to provide
    /// a specific error message indicating that the product has reached its maximum allowed number of images.
    /// </summary>
    public override string Message => "The product already has the maximum number of images.";
}