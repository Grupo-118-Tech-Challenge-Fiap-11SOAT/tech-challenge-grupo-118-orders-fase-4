using Domain.Base.Exceptions;

namespace Domain.Products.Exceptions;

/// <summary>
/// Represents an exception that occurs when a provided URL does not point to a valid image.
/// </summary>
/// <remarks>
/// This exception is typically thrown when the format of the given URL is correctly structured
/// but does not correspond to a valid image format, such as ".jpg", ".png", ".gif", etc.
/// </remarks>
public class UrlIsNotAnImageException : DomainException
{
    /// <summary>
    /// Gets a message that describes the error.
    /// </summary>
    /// <remarks>
    /// This property provides a default error message indicating that the provided URL
    /// does not reference a valid image format. It overrides the base exception message
    /// to give a more specific reason regarding the error.
    /// </remarks>
    public override string Message => "The provided URL is not valid image";
}