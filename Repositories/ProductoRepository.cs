using System.Data;
using Microsoft.Data.SqlClient;
using ProductosApi.Models;

namespace ProductosApi.Repositories;

/// <summary>
/// Implementación de IProductoRepository utilizando ADO.NET con Microsoft.Data.SqlClient.
/// Cada método abre una conexión por operación y la libera automáticamente con using.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly string _connectionString;

    public ProductoRepository(IConfiguration configuration)
    {
        // Se obtiene la cadena de conexión configurada en appsettings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection' en appsettings.json.");
    }

    /// <summary>
    /// Consulta y retorna todos los productos registrados en la tabla dbo.Productos.
    /// </summary>
    public List<Producto> ObtenerTodos()
    {
        var productos = new List<Producto>();
        const string sql = "SELECT Id, Nombre, Precio, Cantidad FROM dbo.Productos ORDER BY Id;";

        // using asegura que la conexión y el comando se cierren y liberen al terminar
        using var conexion = new SqlConnection(_connectionString);
        using var comando = new SqlCommand(sql, conexion);

        conexion.Open();
        using var lector = comando.ExecuteReader();

        // Leemos fila por fila los resultados obtenidos
        while (lector.Read())
        {
            productos.Add(new Producto
            {
                Id = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Precio = lector.GetDecimal(2),
                Cantidad = lector.GetInt32(3)
            });
        }

        return productos;
    }

    /// <summary>
    /// Busca un producto por su Id. Retorna el producto si existe, o null si no se encuentra.
    /// </summary>
    public Producto? ObtenerPorId(int id)
    {
        const string sql = "SELECT Id, Nombre, Precio, Cantidad FROM dbo.Productos WHERE Id = @Id;";

        using var conexion = new SqlConnection(_connectionString);
        using var comando = new SqlCommand(sql, conexion);

        // Se usa parámetro SQL para evitar inyección de código SQL
        comando.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        conexion.Open();
        using var lector = comando.ExecuteReader();

        if (lector.Read())
        {
            return new Producto
            {
                Id = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Precio = lector.GetDecimal(2),
                Cantidad = lector.GetInt32(3)
            };
        }

        return null;
    }

    /// <summary>
    /// Inserta un nuevo producto y devuelve el Id generado por la columna IDENTITY.
    /// </summary>
    public int Insertar(Producto producto)
    {
        const string sql = @"
            INSERT INTO dbo.Productos (Nombre, Precio, Cantidad)
            VALUES (@Nombre, @Precio, @Cantidad);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var conexion = new SqlConnection(_connectionString);
        using var comando = new SqlCommand(sql, conexion);

        // Parámetros tipados para proteger y mapear los datos
        comando.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = producto.Nombre });
        comando.Parameters.Add(new SqlParameter("@Precio", SqlDbType.Decimal) { Value = producto.Precio });
        comando.Parameters.Add(new SqlParameter("@Cantidad", SqlDbType.Int) { Value = producto.Cantidad });

        conexion.Open();

        // ExecuteScalar ejecuta la consulta y devuelve la primera columna de la primera fila (SCOPE_IDENTITY)
        var resultado = comando.ExecuteScalar();
        return Convert.ToInt32(resultado);
    }

    /// <summary>
    /// Actualiza los datos de un producto existente. Devuelve true si afectó una fila, false si no existía.
    /// </summary>
    public bool Actualizar(Producto producto)
    {
        const string sql = @"
            UPDATE dbo.Productos
            SET Nombre = @Nombre,
                Precio = @Precio,
                Cantidad = @Cantidad
            WHERE Id = @Id;";

        using var conexion = new SqlConnection(_connectionString);
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = producto.Id });
        comando.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = producto.Nombre });
        comando.Parameters.Add(new SqlParameter("@Precio", SqlDbType.Decimal) { Value = producto.Precio });
        comando.Parameters.Add(new SqlParameter("@Cantidad", SqlDbType.Int) { Value = producto.Cantidad });

        conexion.Open();

        // ExecuteNonQuery devuelve el número de filas afectadas por la instrucción UPDATE
        int filasAfectadas = comando.ExecuteNonQuery();
        return filasAfectadas > 0;
    }

    /// <summary>
    /// Elimina un producto por su Id. Devuelve true si afectó una fila, false si no existía.
    /// </summary>
    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM dbo.Productos WHERE Id = @Id;";

        using var conexion = new SqlConnection(_connectionString);
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        conexion.Open();

        // ExecuteNonQuery devuelve el número de filas afectadas por la instrucción DELETE
        int filasAfectadas = comando.ExecuteNonQuery();
        return filasAfectadas > 0;
    }
}
