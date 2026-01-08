using System.Diagnostics.CodeAnalysis;

namespace Domain.Base.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an invalid email is encountered.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InvalidEmailException : DomainException
    {
        /// <summary>
        /// Gets the error message that explains the reason for the exception.
        /// </summary>
        public override string Message => "Email was invalid.";
    }
}