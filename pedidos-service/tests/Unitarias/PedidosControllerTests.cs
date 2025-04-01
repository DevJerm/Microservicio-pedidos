using Microsoft.AspNetCore.Mvc;
using Moq;
using pedidos_service.API.Controllers;
using pedidos_service.Application.DTOs;
using pedidos_service.Application.Interfaces;
using Xunit;

public class PedidosControllerTests
{
    private readonly Mock<IPedidoService> _pedidoServiceMock;
    private readonly Mock<ILogger<PedidosController>> _loggerMock;
    private readonly PedidosController _controller;

    public PedidosControllerTests()
    {
        _pedidoServiceMock = new Mock<IPedidoService>();
        _loggerMock = new Mock<ILogger<PedidosController>>();
        _controller = new PedidosController(_pedidoServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CrearPedido_RetornaOK()
    {
        var pedidoDTO = new PedidoDTO { Id = "1", Estado = "Pendiente" };
        var crearPedidoDTO = new CrearPedidoDTO();
        _pedidoServiceMock.Setup(s => s.CrearPedidoAsync(crearPedidoDTO))
                          .ReturnsAsync(pedidoDTO);

        var result = await _controller.CrearPedido(crearPedidoDTO);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }



    [Fact]
    public async Task ObtenerPedidoPorId_RetorrnaOk()
    {
        var pedidoDTO = new PedidoDTO { Id = "1", Estado = "Pendiente" };
        _pedidoServiceMock.Setup(s => s.GetPedidoByIdAsync("1"))
                          .ReturnsAsync(pedidoDTO);

        var result = await _controller.ObtenerPedidoPorId("1");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ObtenerPedidoPorId_RetornaNoEncontrado_PedidoNoExiste()
    {
        _pedidoServiceMock.Setup(s => s.GetPedidoByIdAsync("1"))
                          .ThrowsAsync(new KeyNotFoundException("Pedido no encontrado"));

        var result = await _controller.ObtenerPedidoPorId("1");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task ActualizarEstadoPedido_RetornaOK()
    {
        var pedidoDTO = new PedidoDTO { Id = "1", Estado = "Enviado" };
        _pedidoServiceMock.Setup(s => s.ActualizarEstadoPedidoAsync("1", "Enviado"))
                          .ReturnsAsync(pedidoDTO);

        var result = await _controller.ActualizarEstadoPedido("1", new CambioEstadoDTO { NuevoEstado = "Enviado" });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
