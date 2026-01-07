using Domain.Base.Exceptions;
using Domain.Order.Dtos;
using Domain.Order.Entities;
using Domain.Order.Exceptions;
using Domain.Order.ValueObjects;
using Domain.Products.Dtos;
using Domain.Products.ValueObjects;
using NUnit.Framework;

namespace UnitTests.Domain;

[TestFixture]
public class OrderTests
{

    [Test]
    public void Constructor_ShouldThrowEmptyOrderItemsException_WhenItemsAreEmpty()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto>()
        };
        var products = new List<ProductDto>();

        // Act & Assert
        Assert.Throws<EmptyOrderItemsException>(() => new Order(orderDto, products));
    }

    [Test]
    public void ChangeStatus_ShouldAdvanceStatus_WhenTransitionIsValid()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1 } }
        };
        var products = new List<ProductDto> { new ProductDto {  Price = 10.0m } };
        var order = new Order(orderDto, products); // Status: Received

        // Act
        order.ChangeStatus(); // Received -> InPreparation

        // Assert
        Assert.AreEqual(OrderStatus.InPreparation, order.Status);
        
        // Act
        order.ChangeStatus(); // InPreparation -> Ready

        // Assert
        Assert.AreEqual(OrderStatus.Ready, order.Status);

        // Act
        order.ChangeStatus(); // Ready -> Completed

        // Assert
        Assert.AreEqual(OrderStatus.Completed, order.Status);
    }

    [Test]
    public void ChangeStatus_ShouldThrowChangeStatusNotAllowedException_WhenStatusIsCompleted()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1 } }
        };
        var products = new List<ProductDto> { new ProductDto { Price = 10.0m } };
        var order = new Order(orderDto, products);
        
        // Advance to Completed
        order.ChangeStatus(); // InPreparation
        order.ChangeStatus(); // Ready
        order.ChangeStatus(); // Completed

        // Act & Assert
        Assert.Throws<ChangeStatusNotAllowedException>(() => order.ChangeStatus());
    }
}
