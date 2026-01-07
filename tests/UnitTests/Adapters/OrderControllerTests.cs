using Adapters.Driving.Orders.API;
using Domain.Order.Dtos;
using Domain.Order.Ports.In;
using Domain.Order.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Adapters;

[TestFixture]
public class OrderControllerTests
{
    private Mock<IOrderManager> _orderManagerMock;
    private OrderController _orderController;

    [SetUp]
    public void Setup()
    {
        _orderManagerMock = new Mock<IOrderManager>();
        _orderController = new OrderController(_orderManagerMock.Object);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnOk_WhenOrdersExist()
    {
        // Arrange
        var status = OrderStatus.Received;
        var orders = new List<OrderResponseDto> { new OrderResponseDto() };
        _orderManagerMock.Setup(m => m.GetAllAsync(status, 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _orderController.GetAllAsync(status, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.AreEqual(orders, okResult.Value);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnNotFound_WhenNoOrdersExist()
    {
        // Arrange
        var status = OrderStatus.Received;
        _orderManagerMock.Setup(m => m.GetAllAsync(status, 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrderResponseDto>());

        // Act
        var result = await _orderController.GetAllAsync(status, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }

    [Test]
    public async Task GetOrdersToMonitorAsync_ShouldReturnOk_WhenOrdersExist()
    {
        // Arrange
        var orders = new List<OrderResponseDto> { new OrderResponseDto() };
        _orderManagerMock.Setup(m => m.GetOrdersToMonitorAsync(It.IsAny<CancellationToken>(), 0, 10))
            .ReturnsAsync(orders);

        // Act
        var result = await _orderController.GetOrdersToMonitorAsync(CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.AreEqual(orders, okResult.Value);
    }

    [Test]
    public async Task GetOrdersToMonitorAsync_ShouldReturnNotFound_WhenNoOrdersExist()
    {
        // Arrange
        _orderManagerMock.Setup(m => m.GetOrdersToMonitorAsync(It.IsAny<CancellationToken>(), 0, 10))
            .ReturnsAsync(new List<OrderResponseDto>());

        // Act
        var result = await _orderController.GetOrdersToMonitorAsync(CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }

    [Test]
    public async Task PostAsync_ShouldReturnCreated_WhenOrderIsCreated()
    {
        // Arrange
        var orderDto = new OrderRequestDto();
        var createdOrder = new OrderResponseDto { Id = 1 };
        _orderManagerMock.Setup(m => m.CreateAsync(orderDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdOrder);

        // Act
        var result = await _orderController.PostAsync(orderDto, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<CreatedAtActionResult>(result);
        var createdResult = result as CreatedAtActionResult;
        Assert.AreEqual(createdOrder, createdResult.Value);
        Assert.AreEqual("GetById", createdResult.ActionName);
        Assert.AreEqual(1, createdResult.RouteValues["Id"]);
    }

    [Test]
    public async Task PatchStatus_ShouldReturnOk_WhenStatusIsUpdated()
    {
        // Arrange
        var id = 1;
        var updatedOrder = new OrderResponseDto();
        _orderManagerMock.Setup(m => m.UpdateStatusAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedOrder);

        // Act
        var result = await _orderController.PatchStatus(id, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.AreEqual(updatedOrder, okResult.Value);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnOk_WhenOrderExists()
    {
        // Arrange
        var id = 1;
        var order = new OrderResponseDto();
        _orderManagerMock.Setup(m => m.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderController.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.AreEqual(order, okResult.Value);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var id = 1;
        _orderManagerMock.Setup(m => m.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderResponseDto)null);

        // Act
        var result = await _orderController.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }
}
