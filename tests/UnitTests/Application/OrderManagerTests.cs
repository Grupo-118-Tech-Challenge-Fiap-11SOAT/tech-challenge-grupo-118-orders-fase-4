using Application.Order;
using Domain.Order.Dtos;
using Domain.Order.Entities;
using Domain.Order.Exceptions;
using Domain.Order.Ports.Out;
using Domain.Order.ValueObjects;
using Domain.Payment.Ports.Dtos;
using Domain.Payment.Ports.Out;
using Domain.Products.Entities;
using Domain.Products.Ports.In;
using Domain.Products.ValueObjects;
using Moq;

namespace UnitTests.Application;

[TestFixture]
public class OrderManagerTests
{
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IProductManager> _productManagerMock;
    private Mock<IPaymentManager> _paymentManagerMock;
    private OrderManager _orderManager;

    [SetUp]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productManagerMock = new Mock<IProductManager>();
        _paymentManagerMock = new Mock<IPaymentManager>();
        _orderManager = new OrderManager(_orderRepositoryMock.Object, _productManagerMock.Object, _paymentManagerMock.Object);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnListOfOrderResponseDto()
    {
        // Arrange
        var status = OrderStatus.Received;
        var order = new Order(new OrderRequestDto { Items = new List<OrderItemDto>() }, new List<Product>());
        var ordersList = new List<Order> { order };
        _orderRepositoryMock.Setup(r => r.GetAllAsync(status, 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordersList);

        // Act
        var result = await _orderManager.GetAllAsync(status, 0, 10, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOf<OrderResponseDto>(result[0]);
    }

    [Test]
    public async Task GetOrdersToMonitorAsync_ShouldReturnListOfOrderResponseDto_WhenOrdersExist()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1 } }
        };
        var order = new Order(new OrderRequestDto { Items = new List<OrderItemDto>() }, new List<Product>());
        var ordersList = new List<Order> { order };
        _orderRepositoryMock.Setup(r => r.GetOrdersToMonitorAsync(0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordersList);

        // Act
        var result = await _orderManager.GetOrdersToMonitorAsync(CancellationToken.None, 0, 10);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.IsInstanceOf<OrderResponseDto>(result[0]);
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
    public async Task CreateAsync_ShouldCreateOrderAndReturnDto()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 1 } }
        };
        var product = new Product("Test Product", "Description", ProductType.Snack, 10.0m, true, 1);
        var activeProducts = new List<Product> { product };

        _productManagerMock.Setup(p => p.GetActiveProductsByIds(It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeProducts);

        _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order order, CancellationToken token) => order); // Changed to return the order object
        
        _paymentManagerMock.Setup(p => p.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()))
            .ReturnsAsync(new Mock<IAsyncResult>().Object);

        // Act
        var result = await _orderManager.CreateAsync(orderDto, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(orderDto.Items, result.Items);
        _orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateStatusAsync_ShouldUpdateStatusAndReturnDto_WhenOrderExists()
    {
        // Arrange
        var orderId = 1;
        var order = new Order(new OrderRequestDto { Items = new List<OrderItemDto>() }, new List<Product>());
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _orderRepositoryMock.Setup(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order order, CancellationToken token) => order); // Changed to return the order object

        // Act
        var result = await _orderManager.UpdateStatusAsync(orderId, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Status, Is.Not.EqualTo(OrderStatus.Received)); // Assuming initial status is Received and it changes
        _orderRepositoryMock.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
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
    public async Task GetByIdAsync_ShouldReturnDto_WhenOrderExists()
    {
        // Arrange
        var orderId = 1;
        var order = new Order(new OrderRequestDto { Items = new List<OrderItemDto>() }, new List<Product>());
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderManager.GetByIdAsync(orderId, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OrderResponseDto>(result);
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
