using Domain.Products.Entities;
using Domain.Products.Exceptions;
using Domain.Products.ValueObjects;
using NUnit.Framework;

namespace UnitTests.Domain;

[TestFixture]
public class ProductTests
{
    [Test]
    public void Constructor_ShouldCreateProduct_WhenDataIsValid()
    {
        // Arrange
        var name = "Test Product";
        var description = "Test Description";
        var category = ProductType.Snack;
        var price = 10.0m;
        var isActive = true;

        // Act
        var product = new Product(name, description, category, price, isActive);

        // Assert
        Assert.IsNotNull(product);
        Assert.AreEqual(name, product.Name);
        Assert.AreEqual(description, product.Description);
        Assert.AreEqual(category, product.Category);
        Assert.AreEqual(price, product.Price);
        Assert.AreEqual(isActive, product.IsActive);
        Assert.IsNotNull(product.Images);
        Assert.IsEmpty(product.Images);
    }

    [Test]
    public void Constructor_ShouldThrowProductInvalidPriceException_WhenPriceIsZeroOrNegative()
    {
        // Arrange
        var name = "Test Product";
        var description = "Test Description";
        var category = ProductType.Snack;
        var price = 0.0m;
        var isActive = true;

        // Act & Assert
        Assert.Throws<ProductInvalidPriceException>(() => new Product(name, description, category, price, isActive));
        Assert.Throws<ProductInvalidPriceException>(() => new Product(name, description, category, -10.0m, isActive));
    }

    [Test]
    public void UpdateProduct_ShouldUpdateProductDetails_WhenDataIsValid()
    {
        // Arrange
        var product = new Product("Old Name", "Old Desc", ProductType.Snack, 10.0m, true);
        var newName = "New Name";
        var newDesc = "New Desc";
        var newCategory = ProductType.Drink;
        var newPrice = 20.0m;
        var newActive = false;

        // Act
        product.UpdateProduct(newName, newDesc, newCategory, newPrice, newActive);

        // Assert
        Assert.AreEqual(newName, product.Name);
        Assert.AreEqual(newDesc, product.Description);
        Assert.AreEqual(newCategory, product.Category);
        Assert.AreEqual(newPrice, product.Price);
        Assert.AreEqual(newActive, product.IsActive);
    }

    [Test]
    public void UpdateProduct_ShouldThrowProductInvalidPriceException_WhenNewPriceIsInvalid()
    {
        // Arrange
        var product = new Product("Name", "Desc", ProductType.Snack, 10.0m, true);

        // Act & Assert
        Assert.Throws<ProductInvalidPriceException>(() => product.UpdateProduct("Name", "Desc", ProductType.Snack, 0.0m, true));
    }

    [Test]
    public void AddImage_ShouldAddImage_WhenLimitIsNotReached()
    {
        // Arrange
        var product = new Product("Name", "Desc", ProductType.Snack, 10.0m, true);
        var image = new ImageProduct(1, 1, "http://example.com/image.jpg");

        // Act
        product.AddImage(image);

        // Assert
        Assert.AreEqual(1, product.Images.Count);
        Assert.Contains(image, product.Images);
    }

    [Test]
    public void AddImage_ShouldThrowProductMaxImageException_WhenLimitIsReached()
    {
        // Arrange
        var product = new Product("Name", "Desc", ProductType.Snack, 10.0m, true);
        for (int i = 0; i < 5; i++)
        {
            product.AddImage(new ImageProduct(1, i + 1, $"http://example.com/image{i}.jpg"));
        }
        var extraImage = new ImageProduct(1, 6, "http://example.com/image6.jpg");

        // Act & Assert
        Assert.Throws<ProductMaxImageException>(() => product.AddImage(extraImage));
    }

    [Test]
    public void ChangeImage_ShouldUpdateImage_WhenImageExists()
    {
        // Arrange
        var product = new Product("Name", "Desc", ProductType.Snack, 10.0m, true);
        var image = new ImageProduct(1, 1, "http://example.com/image.jpg", id: 1);
        product.AddImage(image);

        var updatedImage = new ImageProduct(1, 2, "http://example.com/updated.jpg", id: 1);

        // Act
        product.ChangeImage(updatedImage);

        // Assert
        var currentImage = product.Images.First();
        Assert.AreEqual(updatedImage.Url, currentImage.Url);
        Assert.AreEqual(updatedImage.Position, currentImage.Position);
    }
}
