using System;
using Xunit;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.tests.Domain
{
    public class PedidoTests
    {
        //agg algunas pruebas unitarias de pedidos. Revisemos que mas podemos testear. Preguntar al profe si solo se requiren pruebas unitarias.

        private readonly DireccionEntrega _direccionPrueba = new DireccionEntrega(
            "Calle Principal", "123", "Ciudad", "12345", "Cerca del parque");

        [Fact]
        public void CrearPedido_DebeInicializarCorrectamente()
        {
            // Arrange y Act
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Assert
            Assert.NotNull(pedido);
            Assert.Equal("cliente123", pedido.ClienteId);
            Assert.Equal(EstadoPedido.CREADO, pedido.Estado);
            Assert.Empty(pedido.Items);
            Assert.Equal(0, pedido.Total.Valor);
            Assert.Equal(_direccionPrueba, pedido.DireccionEntrega);
        }

        [Fact]
        public void AgregarItem_DebeCalcularTotalCorrectamente()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act
            pedido.AgregarItem("producto1", 2, 10.5m);
            pedido.AgregarItem("producto2", 1, 15.75m);

            // Assert
            Assert.Equal(2, pedido.Items.Count);
            Assert.Equal(36.75m, pedido.Total.Valor);
        }

        [Fact]
        public void ConfirmarPedido_CambiaEstadoCorrectamente()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act
            pedido.ConfirmarPedido();

            // Assert
            Assert.Equal(EstadoPedido.CONFIRMADO, pedido.Estado);
        }

        [Fact]
        public void MarcarEnPreparacion_LanzaExcepcionSiNoEstaConfirmado()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act y Assert
            var ex = Assert.Throws<InvalidOperationException>(() => pedido.MarcarEnPreparacion());
            Assert.Contains("CONFIRMADOS", ex.Message);
        }

        [Fact]
        public void FlujoCompleto_DebePermitirCambiosDeEstadoEnOrden()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act y Assert
            pedido.ConfirmarPedido();
            Assert.Equal(EstadoPedido.CONFIRMADO, pedido.Estado);

            pedido.MarcarEnPreparacion();
            Assert.Equal(EstadoPedido.EN_PREPARACION, pedido.Estado);

            pedido.MarcarListo();
            Assert.Equal(EstadoPedido.LISTO, pedido.Estado);

            pedido.MarcarEntregado();
            Assert.Equal(EstadoPedido.ENTREGADO, pedido.Estado);
        }
    }
}
