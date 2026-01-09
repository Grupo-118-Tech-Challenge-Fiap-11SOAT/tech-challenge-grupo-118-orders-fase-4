using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Products.Ports.In;
using Domain.Products.ValueObjects;
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
        _productsApiBaseUrl = configuration["ProductsApi:BaseUrl"] ??
                              throw new ArgumentNullException("ProductsApi:BaseUrl configuration is missing.");
    }

    public async Task<List<Domain.Products.Entities.Product>> GetActiveProductsByIds(string[] ids,
        CancellationToken cancellationToken)
    {
        var products = new List<Domain.Products.Entities.Product>();
        foreach (var id in ids)
        {
            var response = await _httpClient.GetAsync($"{_productsApiBaseUrl}/Products/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using var jsonDoc = await JsonDocument.ParseAsync(
                    await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
                var root = jsonDoc.RootElement;

                // Acessar o objeto 'data'
                if (root.TryGetProperty("data", out var dataElement))
                {
                    var productId = dataElement.GetProperty("id").GetString();
                    var isActive = dataElement.GetProperty("isActive").GetBoolean();
                    var price = dataElement.GetProperty("price").GetDecimal();
                    var productName = dataElement.GetProperty("name").GetString() ?? string.Empty;
                    var productType = dataElement.GetProperty("type").GetString() ?? string.Empty;

                    // ProductType.TryParse(productType, out ProductType category);

                    Enum.TryParse(productType, ignoreCase: true, out ProductType category);

                    var product = new Domain.Products.Entities.Product(
                        name: productName,
                        description: productName,
                        productType: category,
                        price: price,
                        isActive: isActive,
                        id: productId ?? string.Empty
                    );

                    if (product != null) products.Add(product);
                }
            }
        }

        return products;
    }
}