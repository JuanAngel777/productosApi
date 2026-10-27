using ProductosApi.Models;

namespace ProductosApi.Repositories;

/// <summary>
/// Contrato para las operaciones de acceso a datos de productos.
/// </summary>
public interface IProductoRepository
{
    List<Producto> ObtenerTodos();
    Producto? ObtenerPorId(int id);
    int Insertar(Producto producto);
    bool Actualizar(Producto producto);
    bool Eliminar(int id);
}
