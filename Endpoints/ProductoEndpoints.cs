using Microsoft.Data.SqlClient;
using ProductosApi.Models;
using ProductosApi.Repositories;

namespace ProductosApi.Endpoints;

/// <summary>
/// Define y mapea los endpoints de la Minimal API para el recurso Producto.
/// </summary>
public static class ProductoEndpoints
{
    public static RouteGroupBuilder MapProductoEndpoints(this IEndpointRouteBuilder routes)
    {
        // Se crea el grupo base con la ruta /api/productos y la etiqueta "Productos" para Swagger
        var group = routes.MapGroup("/api/productos")
                          .WithTags("Productos");

        // GET /api/productos - Obtener todos los productos
        group.MapGet("/", (IProductoRepository repositorio, ILogger<IProductoRepository> logger) =>
        {
            try
            {
                var productos = repositorio.ObtenerTodos();
                return Results.Ok(productos);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Error de SQL Server al obtener todos los productos.");
                return ManejarErrorBaseDatos();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al obtener todos los productos.");
                return ManejarErrorInesperado();
            }
        })
        .WithName("ObtenerTodosLosProductos")
        .WithSummary("Obtiene la lista completa de productos");

        // GET /api/productos/{id} - Buscar producto por Id
        group.MapGet("/{id:int}", (int id, IProductoRepository repositorio, ILogger<IProductoRepository> logger) =>
        {
            try
            {
                var producto = repositorio.ObtenerPorId(id);
                if (producto is null)
                {
                    return Results.NotFound(new { mensaje = $"No se encontró el producto con Id {id}." });
                }

                return Results.Ok(producto);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Error de SQL Server al consultar el producto con Id {Id}.", id);
                return ManejarErrorBaseDatos();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al consultar el producto con Id {Id}.", id);
                return ManejarErrorInesperado();
            }
        })
        .WithName("ObtenerProductoPorId")
        .WithSummary("Busca un producto por su identificador");

        // POST /api/productos - Insertar un nuevo producto
        group.MapPost("/", (Producto producto, IProductoRepository repositorio, ILogger<IProductoRepository> logger) =>
        {
            // Validaciones de negocio
            string? errorValidacion = ValidarProducto(producto);
            if (errorValidacion is not null)
            {
                return Results.BadRequest(new { mensaje = errorValidacion });
            }

            try
            {
                int nuevoId = repositorio.Insertar(producto);
                producto.Id = nuevoId;

                // Responder 201 Created con la ubicación del recurso y el objeto creado
                return Results.Created($"/api/productos/{nuevoId}", producto);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Error de SQL Server al insertar producto.");
                return ManejarErrorBaseDatos();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al insertar producto.");
                return ManejarErrorInesperado();
            }
        })
        .WithName("CrearProducto")
        .WithSummary("Inserta un nuevo producto en el catálogo");

        // PUT /api/productos/{id} - Actualizar producto existente
        group.MapPut("/{id:int}", (int id, Producto producto, IProductoRepository repositorio, ILogger<IProductoRepository> logger) =>
        {
            // Validaciones de negocio
            string? errorValidacion = ValidarProducto(producto);
            if (errorValidacion is not null)
            {
                return Results.BadRequest(new { mensaje = errorValidacion });
            }

            try
            {
                producto.Id = id;
                bool actualizado = repositorio.Actualizar(producto);

                if (!actualizado)
                {
                    return Results.NotFound(new { mensaje = $"No se encontró el producto con Id {id} para actualizar." });
                }

                // Responder 204 No Content indicando actualización exitosa sin contenido en el cuerpo
                return Results.NoContent();
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Error de SQL Server al actualizar el producto con Id {Id}.", id);
                return ManejarErrorBaseDatos();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar el producto con Id {Id}.", id);
                return ManejarErrorInesperado();
            }
        })
        .WithName("ActualizarProducto")
        .WithSummary("Actualiza los datos de un producto existente");

        // DELETE /api/productos/{id} - Eliminar producto por Id
        group.MapDelete("/{id:int}", (int id, IProductoRepository repositorio, ILogger<IProductoRepository> logger) =>
        {
            try
            {
                bool eliminado = repositorio.Eliminar(id);

                if (!eliminado)
                {
                    return Results.NotFound(new { mensaje = $"No se encontró el producto con Id {id} para eliminar." });
                }

                // Responder 204 No Content indicando eliminación exitosa
                return Results.NoContent();
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Error de SQL Server al eliminar el producto con Id {Id}.", id);
                return ManejarErrorBaseDatos();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar el producto con Id {Id}.", id);
                return ManejarErrorInesperado();
            }
        })
        .WithName("EliminarProducto")
        .WithSummary("Elimina un producto del catálogo por su Id");

        return group;
    }

    /// <summary>
    /// Valida las reglas requeridas: Nombre no vacío, Precio >= 0 y Cantidad >= 0.
    /// </summary>
    private static string? ValidarProducto(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
        {
            return "El nombre del producto no puede estar vacío.";
        }

        if (producto.Precio < 0)
        {
            return "El precio no puede ser negativo.";
        }

        if (producto.Cantidad < 0)
        {
            return "La cantidad no puede ser negativa.";
        }

        return null;
    }

    /// <summary>
    /// Respuesta genérica 500 para errores de base de datos, evitando filtrar credenciales o cadenas de conexión.
    /// </summary>
    private static IResult ManejarErrorBaseDatos()
    {
        return Results.Problem(
            detail: "Ocurrió un error al comunicarse con la base de datos SQL Server.",
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Error de base de datos"
        );
    }

    /// <summary>
    /// Respuesta genérica 500 para excepciones no controladas.
    /// </summary>
    private static IResult ManejarErrorInesperado()
    {
        return Results.Problem(
            detail: "Ocurrió un error inesperado al procesar la solicitud.",
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Error interno del servidor"
        );
    }
}
