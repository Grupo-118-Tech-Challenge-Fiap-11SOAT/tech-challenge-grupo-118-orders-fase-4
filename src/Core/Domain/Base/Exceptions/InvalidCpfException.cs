using System.Diagnostics.CodeAnalysis;

namespace Domain.Base.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an invalid CPF is encountered.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InvalidCpfException : DomainException
    {
        /// <summary>
        /// Gets the error message that explains the reason for the exception.
        /// </summary>
        public override string Message => "CPF was invalid.";
    }
}
