using Application.Order;
using Domain.Order.Dtos;
using Domain.Order.Entities;
using Domain.Order.Ports.Out;
using Domain.Order.ValueObjects;
using Domain.Products.Entities;
using Domain.Products.Ports.In;
using Domain.Products.ValueObjects;
using Moq;
using Reqnroll;
using Xunit;

namespace Orders.BDD.Tests.StepDefinitions
{
    [Binding]
    public class OrderStepDefinitions
    {
        private OrderRequestDto _orderRequest;
        private OrderResponseDto _result;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IProductManager> _productManagerMock;
        private OrderManager _orderManager;

        public OrderStepDefinitions()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productManagerMock = new Mock<IProductManager>();
            _orderManager = new OrderManager(_orderRepositoryMock.Object, _productManagerMock.Object);
        }

        [Given(@"que eu tenho um pedido válido")]
        public void DadoQueEuTenhoUmPedidoValido()
        {
            _orderRequest = new OrderRequestDto
            {
                Cpf = "12345678900",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 2 }
                }
            };

            _productManagerMock.Setup(pm => pm.GetActiveProductsByIds(It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>
                {
                    new Product("Lanche", "Hamburguer", ProductType.Snack, 20.0m, true)
                });
            
            _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Order.Entities.Order>(), It.IsAny<CancellationToken>()))
                .Returns((Task<Order>)Task.CompletedTask);
        }

        [When(@"eu envio o pedido")]
        public async Task QuandoEuEnvioOPedido()
        {
            _result = await _orderManager.CreateAsync(_orderRequest, CancellationToken.None);
        }

        [Then(@"o pedido deve ser criado com sucesso")]
        public void EntaoOPedidoDeveSerCriadoComSucesso()
        {
            Assert.NotNull(_result);
            Assert.Equal(_orderRequest.Cpf, _result.Cpf);
            Assert.Equal(OrderStatus.Received, _result.Status);
        }
    }
}
