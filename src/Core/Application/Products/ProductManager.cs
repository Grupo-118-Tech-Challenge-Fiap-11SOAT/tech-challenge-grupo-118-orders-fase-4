using Domain.Products.Entities;
using Domain.Products.Ports.In;

namespace Application.Products
{
    public class ProductManager : IProductManager
    {
        public Task<List<Product>> GetActiveProductsByIds(int[] ids, CancellationToken cancellationToken)
        {
            // Implementation would go here, but for now we just need the class to exist
            // This is likely a mock implementation or will be replaced by actual logic
            return Task.FromResult(new List<Product>());
        }
    }
}
