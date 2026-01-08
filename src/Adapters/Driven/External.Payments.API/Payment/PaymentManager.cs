using System.Diagnostics.CodeAnalysis;
using Domain.Payment.Ports.Dtos;
using Domain.Payment.Ports.Out;
using Microsoft.Extensions.Configuration;

namespace External.Payments.API.Payment;

[ExcludeFromCodeCoverage]
public class PaymentManager : IPaymentManager
{
    private readonly HttpClient _httpClient;
    private readonly string _productsApiBaseUrl;
    
    public PaymentManager(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _productsApiBaseUrl = configuration["PaymentsApi:BaseUrl"] ?? throw new ArgumentNullException("PaymentsApi:BaseUrl configuration is missing.");
    }
    public Task<IAsyncResult> CreatePaymentAsync(CreatePaymentRequest request)
    {
        throw new NotImplementedException();
    }
}