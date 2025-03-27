using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.Domain.Services
{
    public interface ICalculoPrecioService
    {
        decimal CalcularPrecioTotal(List<ItemPedido> items);
        decimal CalcularImpuestos(decimal subtotal);
        decimal CalcularCostoEnvio(DireccionEntrega direccionEntrega);
    }
}
