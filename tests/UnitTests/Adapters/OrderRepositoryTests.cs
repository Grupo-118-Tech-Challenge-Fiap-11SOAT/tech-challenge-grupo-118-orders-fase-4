using Domain.Order.Dtos;
using Domain.Order.Entities;
using Domain.Order.ValueObjects;
using Domain.Products.Dtos;
using Infra.Database.SqlServer;
using Infra.Database.SqlServer.Order.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Adapters;

[TestFixture]
public class OrderRepositoryTests
{
    private AppDbContext _dbContext;
    private OrderRepository _orderRepository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _orderRepository = new OrderRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task CreateAsync_ShouldAddOrderToDatabase()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = "1", Quantity = 1 } }
        };
        var products = new List<ProductDto> { new ProductDto {  Price = 10.0m } };
        var order = new Order(orderDto, products);

        // Act
        var result = await _orderRepository.CreateAsync(order, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
        var savedOrder = await _dbContext.Orders.FindAsync(result.Id);
        Assert.IsNotNull(savedOrder);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = "1", Quantity = 1 } }
        };
        var products = new List<ProductDto> { new ProductDto { Price = 10.0m } };
        var order = new Order(orderDto, products);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetByIdAsync(order.Id, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(order.Id, result.Id);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Act
        var result = await _orderRepository.GetByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnOrdersWithSpecificStatus()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = "1", Quantity = 1 } }
        };
        var products = new List<ProductDto> { new ProductDto {  Price = 10.0m } };
        var order1 = new Order(orderDto, products); // Status Received
        
        // Manually change status for order2 to simulate different status if needed, 
        // but Order constructor sets Received. 
        // We can use reflection or a helper if we need to set status directly for test setup,
        // or just use the public method ChangeStatus if applicable.
        // Here both are Received.
        var order2 = new Order(orderDto, products); 

        await _dbContext.Orders.AddRangeAsync(order1, order2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetAllAsync(OrderStatus.Received, 0, 10, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateOrderInDatabase()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = "1", Quantity = 1 } }
        };
        var products = new List<ProductDto> { new ProductDto { Price = 10.0m } };
        var order = new Order(orderDto, products);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        order.ChangeStatus(); // Changes to InPreparation
        var result = await _orderRepository.UpdateAsync(order, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(OrderStatus.InPreparation, result.Status);
        
        // Verify in DB
        var savedOrder = await _dbContext.Orders.FindAsync(order.Id);
        Assert.AreEqual(OrderStatus.InPreparation, savedOrder.Status);
    }
    
    [Test]
    public async Task GetOrdersToMonitorAsync_ShouldReturnOrdersNotCompletedOrCanceled()
    {
        // Arrange
        var orderDto = new OrderRequestDto
        {
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = "1", Quantity = 1 } }
        };
        var products = new List<ProductDto> { new ProductDto {  Price = 10.0m } };
        
        var order1 = new Order(orderDto, products); // Received
        var order2 = new Order(orderDto, products); // Received -> InPreparation
        order2.ChangeStatus();
        
        var order3 = new Order(orderDto, products); // Received -> InPreparation -> Ready -> Completed
        order3.ChangeStatus();
        order3.ChangeStatus();
        order3.ChangeStatus();

        await _dbContext.Orders.AddRangeAsync(order1, order2, order3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetOrdersToMonitorAsync(0, 10, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count); // Should contain order1 and order2, but not order3 (Completed)
        Assert.IsTrue(result.Any(o => o.Id == order1.Id));
        Assert.IsTrue(result.Any(o => o.Id == order2.Id));
        Assert.IsFalse(result.Any(o => o.Id == order3.Id));
    }
}
