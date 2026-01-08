using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Domain.Products.Ports.In;
using Microsoft.Extensions.Configuration;

namespace External.Products.API.Product;

[ExcludeFromCodeCoverage]
public class ProductManager : IProductManager
{
    private readonly HttpClient _httpClient;
    private readonly string _productsApiBaseUrl;
    
    public ProductManager(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _productsApiBaseUrl = configuration["ProductsApi:BaseUrl"] ?? throw new ArgumentNullException("ProductsApi:BaseUrl configuration is missing.");
    }
    
    public async Task<List<Domain.Products.Entities.Product>> GetActiveProductsByIds(int[] ids, CancellationToken cancellationToken)
    {
        var products = new List<Domain.Products.Entities.Product>();
        foreach (var id in ids)
        {
            var product = await _httpClient.GetFromJsonAsync<Domain.Products.Entities.Product>($"{_productsApiBaseUrl}/GetDetailedProduct/{id}", cancellationToken);
            if (product != null) products.Add(product);
        }
        return products;
    }
}