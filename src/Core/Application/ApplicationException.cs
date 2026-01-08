using System.Diagnostics.CodeAnalysis;

namespace Application;

[ExcludeFromCodeCoverage]
public class ApplicationException(string errorRetrievingOrders, Exception exception) : Exception
{
}