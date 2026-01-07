using Application.Order;
using Domain.Order.Entities;
using Domain.Order.Exceptions;
using Domain.Order.Ports.Out;
using Domain.Products.Ports.In;
using Moq;

namespace UnitTests.Application;

[TestFixture]
public class OrderManagerTests
{
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IProductManager> _productManagerMock;
    private OrderManager _orderManager;

    [SetUp]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productManagerMock = new Mock<IProductManager>();
        _orderManager = new OrderManager(_orderRepositoryMock.Object, _productManagerMock.Object);
    }

    [Test]
    public async Task GetOrdersToMonitorAsync_ShouldReturnNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        _orderRepositoryMock.Setup(r => r.GetOrdersToMonitorAsync(0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Order>)null);

        // Act
        var result = await _orderManager.GetOrdersToMonitorAsync(CancellationToken.None, 0, 10);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void UpdateStatusAsync_ShouldThrowOrderNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = 1;
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null);

        // Act & Assert
        Assert.ThrowsAsync<OrderNotFoundException>(async () => await _orderManager.UpdateStatusAsync(orderId, CancellationToken.None));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = 1;
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null);

        // Act
        var result = await _orderManager.GetByIdAsync(orderId, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }
}
