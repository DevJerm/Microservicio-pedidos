using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using pedidos_service.API.Controllers;
using pedidos_service.Application.DTOs;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.ValueObjects;
using Xunit;

public class ClientesControllerTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<ILogger<ClientesController>> _loggerMock;
    private readonly ClientesController _controller;

    public ClientesControllerTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _loggerMock = new Mock<ILogger<ClientesController>>();
        _controller = new ClientesController(_clienteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ObtenerClientePorId_Retorna200OK_ClienteExiste()
    {
        // Arrange
        var clienteId = "123";
        var cliente = new Cliente("Juan Pérez", "juan@example.com", "123456789",
            new DireccionEntrega("Calle 10", "5", "Bogotá", "110111", "Frente al parque"));

        _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId))
            .ReturnsAsync(cliente);

        // Act
        var result = await _controller.ObtenerClientePorId(clienteId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ObtenerClientePorId_Retorna404NotFound_clienteNoExiste()
    {
        // Arrange
        var clienteId = "999";
        _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId))
            .ReturnsAsync((Cliente)null);

        // Act
        var result = await _controller.ObtenerClientePorId(clienteId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
}
