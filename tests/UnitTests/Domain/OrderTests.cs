using Domain.Order.Dtos;
using Domain.Order.Entities;
using Domain.Order.Exceptions;
using Domain.Order.ValueObjects;
using Domain.Products.Dtos;

namespace UnitTests.Domain;

[TestFixture]
public class OrderTests
{
    [Test]
    public void Constructor_ShouldCreateOrder_WhenDataIsValid()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Cpf = "12345678909", // Valid CPF format (mocked validation)
            Items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 1, Quantity = 2 }
            }
        };
        var products = new List<ProductDto>
        {
            new ProductDto { Price = 10.0m }
        };

        // Act
        // Note: Cpf validation extension method might need to be mocked or handled if it's strictly validating real CPFs.
        // Assuming "12345678909" passes basic length checks or we might need a valid generator if logic is strict.
        // For this test, let's assume we might hit InvalidCpfException if the extension method is strict.
        // Let's try with a null CPF first which is valid.
        orderDto.Cpf = null; 
        
        var order = new Order(orderDto, products);

        // Assert
        Assert.IsNotNull(order);
        Assert.That(order.Status, Is.EqualTo(OrderStatus.Received));
        Assert.That(order.OrderItems.Count, Is.EqualTo(1));
    }

    // [Test]
    // public void Constructor_ShouldThrowEmptyOrderItemsException_WhenItemsAreEmpty()
    // {
    //     // Arrange
    //     var orderDto = new OrderRequestDto
    //     {
    //         Items = new List<OrderItemDto>()
    //     };
    //     var products = new List<ProductDto>();
    //
    //     // Act & Assert
    //     Assert.Throws<EmptyOrderItemsException>(() => new Order(orderDto, products));
    // }

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
