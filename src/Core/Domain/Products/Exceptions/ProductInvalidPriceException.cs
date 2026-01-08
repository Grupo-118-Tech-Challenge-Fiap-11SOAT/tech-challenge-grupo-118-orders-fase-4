using System.Diagnostics.CodeAnalysis;
using Domain.Base.Exceptions;

namespace Domain.Products.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a product has an invalid price.
/// </summary>
/// <remarks>
/// This exception is specifically used in scenarios where a product's price is
/// set to zero or a negative value, which is considered invalid.
/// </remarks>
/// <seealso cref="DomainException" />
[ExcludeFromCodeCoverage]
public class ProductInvalidPriceException: DomainException
{
    /// <summary>
    /// Gets the message that describes the current exception.
    /// </summary>
    /// <remarks>
    /// This property overrides the base `Message` property of the `Exception` class
    /// and provides a specific error message indicating that a product's price
    /// cannot be zero or negative.
    /// </remarks>
    public override string Message => "Product Price cannot be zero or negative.";
}