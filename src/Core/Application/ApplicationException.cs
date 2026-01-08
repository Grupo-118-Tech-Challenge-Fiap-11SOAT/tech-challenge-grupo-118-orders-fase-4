namespace Application;

public class ApplicationException(string errorRetrievingOrders, Exception exception) : Exception
{
}