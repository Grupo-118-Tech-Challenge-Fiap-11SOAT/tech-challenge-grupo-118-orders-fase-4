using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Domain.Products.Entities;
using Domain.Products.Ports.In;
using Microsoft.Extensions.Configuration;

namespace External.Products.API.ProductsManagerAPI;

[ExcludeFromCodeCoverage]
public class ProductManager : IProductManager
{
    private readonly HttpClient _httpClient;
    private readonly string _productsApiBaseUrl;
    
    public ProductManager(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _productsApiBaseUrl = configuration["ProductsApi:BaseUrl"] ?? throw new ArgumentNullException("ProductsApi:BaseUrl configuration is missing.");
    }
    
    public async Task<List<Product>> GetActiveProductsByIds(int[] ids, CancellationToken cancellationToken)
    {
        var products = new List<Product>();
        foreach (var id in ids)
        {
            var product = await _httpClient.GetFromJsonAsync<Product>($"{_productsApiBaseUrl}/GetDetailedProduct/{id}", cancellationToken);
            if (product != null) products.Add(product);
        }
        return products;
    }
}