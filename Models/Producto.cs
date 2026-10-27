namespace ProductosApi.Models;

/// <summary>
/// Representa la entidad Producto en la base de datos.
/// </summary>
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
}
