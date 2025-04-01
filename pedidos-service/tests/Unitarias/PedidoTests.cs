using System;
using Xunit;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.tests.Unitarias
{
    public class PedidoTests
    {
        //agg algunas pruebas unitarias de pedidos. Revisemos que mas podemos testear. Preguntar al profe si solo se requiren pruebas unitarias.

        private readonly DireccionEntrega _direccionPrueba = new DireccionEntrega(
            "Calle 33", "123", "medellin", "12345", "Cerca del parque");


        [Fact]
        public void AgregarItem_DebeCalcularTotalCorrectamente()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act
            pedido.AgregarItem("producto1", 2, 10);
            pedido.AgregarItem("producto2", 1, 15);

            // Assert
            Assert.Equal(2, pedido.Items.Count);
            Assert.Equal(35, pedido.Total.Valor);
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
