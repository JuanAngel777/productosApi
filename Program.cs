using ProductosApi.Endpoints;
using ProductosApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de servicios para documentación con OpenAPI y Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Productos API",
        Version = "v1",
        Description = "API de práctica para administración de productos con ASP.NET Core Minimal API y ADO.NET"
    });
});

// 2. Registro del repositorio con ciclo de vida Scoped (una instancia por solicitud HTTP)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

var app = builder.Build();

// 3. Habilitar Swagger y Swagger UI para interactuar con la API desde el navegador
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Productos API v1");
    options.RoutePrefix = "swagger";
});

// Redirigir la raíz al panel de Swagger UI para facilitar las pruebas
app.MapGet("/", () => Results.Redirect("/swagger"));

// 4. Mapeo del grupo de endpoints /api/productos
app.MapProductoEndpoints();

app.Run();
