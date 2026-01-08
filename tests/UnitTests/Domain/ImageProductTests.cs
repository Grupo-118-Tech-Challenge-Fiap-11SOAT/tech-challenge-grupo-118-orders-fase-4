using Domain.Products.Entities;
using Domain.Products.Exceptions;
using NUnit.Framework;

namespace UnitTests.Domain;

[TestFixture]
public class ImageProductTests
{
    [Test]
    public void Constructor_ShouldCreateImageProduct_WhenDataIsValid()
    {
        // Arrange
        var productId = 1;
        var position = 1;
        var url = "http://example.com/image.jpg";

        // Act
        var image = new ImageProduct(productId, position, url);

        // Assert
        Assert.IsNotNull(image);
        Assert.AreEqual(productId, image.ProductId);
        Assert.AreEqual(position, image.Position);
        Assert.AreEqual(url, image.Url);
    }

    [Test]
    public void Constructor_ShouldThrowUrlNotValidException_WhenUrlIsInvalid()
    {
        // Arrange
        var productId = 1;
        var position = 1;
        var url = "invalid-url";

        // Act & Assert
        Assert.Throws<UrlNotValidException>(() => new ImageProduct(productId, position, url));
    }

    [Test]
    public void Constructor_ShouldThrowUrlIsNotAnImageException_WhenUrlDoesNotPointToImage()
    {
        // Arrange
        var productId = 1;
        var position = 1;
        var url = "http://example.com/file.txt";

        // Act & Assert
        Assert.Throws<UrlIsNotAnImageException>(() => new ImageProduct(productId, position, url));
    }

    [Test]
    public void Constructor_ShouldThrowImagePositionException_WhenPositionIsInvalid()
    {
        // Arrange
        var productId = 1;
        var position = 0;
        var url = "http://example.com/image.jpg";

        // Act & Assert
        Assert.Throws<ImagePositionException>(() => new ImageProduct(productId, position, url));
    }

    [Test]
    public void UpdateImageProduct_ShouldUpdateDetails_WhenDataIsValid()
    {
        // Arrange
        var image = new ImageProduct(1, 1, "http://example.com/image.jpg");
        var updatedImage = new ImageProduct(1, 2, "http://example.com/new-image.png");

        // Act
        image.UpdateImageProduct(updatedImage);

        // Assert
        Assert.AreEqual(updatedImage.Position, image.Position);
        Assert.AreEqual(updatedImage.Url, image.Url);
    }
}
