using Domain.Payment.Ports.Dtos;

namespace Domain.Payment.Ports.Out;

public interface IPaymentManager
{
    Task<IAsyncResult> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);
}