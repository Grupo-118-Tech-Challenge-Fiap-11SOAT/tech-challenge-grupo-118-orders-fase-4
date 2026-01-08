using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Domain.Payment.Ports.Dtos;
using Domain.Payment.Ports.Out;
using Microsoft.Extensions.Configuration;

namespace External.Payments.API.Payment;

[ExcludeFromCodeCoverage]
public class PaymentManager : IPaymentManager
{
    private readonly HttpClient _httpClient;
    private readonly string _paymentsApiBaseUrl;
    
    public PaymentManager(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _paymentsApiBaseUrl = configuration["PaymentsApi:BaseUrl"] ?? throw new ArgumentNullException("PaymentsApi:BaseUrl configuration is missing.");
    }
    public async Task<IAsyncResult> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync<CreatePaymentRequest>($"{_paymentsApiBaseUrl}/CreatePayment", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return Task.CompletedTask;
    }
}