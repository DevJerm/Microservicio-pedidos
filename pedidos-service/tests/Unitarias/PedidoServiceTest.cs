using Moq;
using pedidos_service.Application.DTOs;
using pedidos_service.Application.Services;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.Services;
using pedidos_service.Domain.ValueObjects;
using Xunit;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<ICreacionPedidoService> _creacionPedidoServiceMock;
    private readonly PedidoService _pedidoService;

    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _creacionPedidoServiceMock = new Mock<ICreacionPedidoService>();

        _pedidoService = new PedidoService(
            _pedidoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _creacionPedidoServiceMock.Object
        );
    }

    [Fact]
    public async Task CrearPedidoAsync_DeberiaCrearPedido_CuandoClienteExiste()
    {
        // Arrange
        var clienteId = "123";
        var direccion = new DireccionEntrega("Calle 33", "4735", "medellin", "050016", "buenos aires");
        var cliente = new Cliente("John Estiven", "johnrestrepo@correo.itm.edu.co", "312312312", direccion);

        var crearPedidoDTO = new CrearPedidoDTO
        {
            ClienteId = clienteId,
            DireccionEntrega = new DireccionEntregaDTO
            {
                Calle = direccion.Calle,
                Numero = direccion.Numero,
                Ciudad = direccion.Ciudad,
                CodigoPostal = direccion.CodigoPostal,
                Referencia = direccion.Referencia
            },
            Items = new List<ItemPedidoDTO>
            {
                new ItemPedidoDTO { ProductoId = "prod1", Cantidad = 2, PrecioUnitario = 5000 }
            }
        };

        var pedido = new Pedido(clienteId, direccion);
        pedido.AgregarItem("prod1", 2, 5000);

        _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _creacionPedidoServiceMock.Setup(service => service.CrearPedidoAsync(
            clienteId,
            It.IsAny<DireccionEntrega>(),
            It.IsAny<List<(string, int, decimal)>>()))
            .ReturnsAsync(pedido);

        _pedidoRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Pedido>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _pedidoService.CrearPedidoAsync(crearPedidoDTO);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(clienteId, resultado.ClienteId);
        Assert.Single(resultado.Items);
        _pedidoRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task CrearPedidoAsync_DeberiaLanzarExcepcion_CuandoClienteNoExiste()
    {
        // Arrange
        var clienteId = "123";
        var crearPedidoDTO = new CrearPedidoDTO
        {
            ClienteId = clienteId,
            DireccionEntrega = new DireccionEntregaDTO
            {
                Calle = "Calle 33",
                Numero = "4735",
                Ciudad = "medellin",
                CodigoPostal = "050016",
                Referencia = "buenos aires"
            },
            Items = new List<ItemPedidoDTO>
            {
                new ItemPedidoDTO { ProductoId = "prod1", Cantidad = 2, PrecioUnitario = 5000 }
            }
        };

        _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId))
            .ReturnsAsync((Cliente)null);

        // Act Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _pedidoService.CrearPedidoAsync(crearPedidoDTO));
    }
}