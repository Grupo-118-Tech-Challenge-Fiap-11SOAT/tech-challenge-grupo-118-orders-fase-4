using System.Diagnostics.CodeAnalysis;
using Domain.Payment.Ports.Dtos;
using Domain.Payment.Ports.Out;

namespace External.Payments.API.Payment;

[ExcludeFromCodeCoverage]
public class PaymentManager : IPaymentManager
{
    public Task<IAsyncResult> CreatePaymentAsync(CreatePaymentRequest request)
    {
        throw new NotImplementedException();
    }
}