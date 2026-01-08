namespace Domain.Order.ValueObjects
{
    public enum OrderStatus
    {
        Received = 0,
        InPreparation = 1,
        Ready = 2,
        Completed = 3,
        Canceled = 4
    }
}