using Domain.Products.Dtos;
using Domain.Products.Entities;

namespace Domain.Products.Ports.In
{
    public interface IProductManager
    {
        Task<List<Product>> GetActiveProductsByIds(string[] ids, CancellationToken cancellationToken);
    }
}
