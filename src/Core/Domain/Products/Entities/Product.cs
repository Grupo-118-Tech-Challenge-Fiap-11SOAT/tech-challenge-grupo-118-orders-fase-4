using Domain.Base;
using Domain.Order.Entities;
using Domain.Products.Exceptions;
using Domain.Products.ValueObjects;

namespace Domain.Products.Entities
{
    public class Product : BaseDomain<string>
    {
        private const int MAX_IMAGES = 5;
        
        public string Name { get; set; }

        public string Description { get; protected set; }

        public ProductType Category { get; protected set; }

        public decimal Price { get; protected set; }

        public ICollection<OrderItem> OrderItems { get; protected set; }

        public List<ImageProduct> Images { get; protected set; }

        public Product(string name,
            string description,
            ProductType productType,
            decimal price,
            bool isActive,
            string id = "",
            List<ImageProduct>? images = null,
            DateTimeOffset? createdAt = null,
            DateTimeOffset? updatedAt = null) : base(id, createdAt, updatedAt)
        {
            this.Name = name;
            this.Description = description;
            this.Category = productType;
            this.IsActive = isActive;
            this.Price = price;

            CheckProductValue();

            Images = images ?? new List<ImageProduct>();
        }

        protected Product()
        {
            
        }

        public void UpdateProduct(string name, string description, ProductType category, decimal price, bool isActive)
        {
            this.Name = name;
            this.Description = description;
            this.Category = category;
            this.Price = price;
            this.IsActive = isActive;
            this.UpdatedAt = DateTimeOffset.Now;

            CheckProductValue();
        }

        public void AddImage(ImageProduct image)
        {
            if (this.Images.Count == MAX_IMAGES)
                throw new ProductMaxImageException();

            this.Images.Add(image);
        }

        public void ChangeImage(ImageProduct image)
        {
            var index = this.Images.FindIndex(i => i.Id == image.Id);

            if (index == -1)
                return;

            this.Images[index] = image;
        }

        private void CheckProductValue()
        {
            if (this.Price <= decimal.Zero)
                throw new ProductInvalidPriceException();
        }
    }
}
