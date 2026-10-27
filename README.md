# ProductosApi

Backend sencillo para administración de productos desarrollado con **ASP.NET Core Minimal API**, **SQL Server** y **ADO.NET** (`Microsoft.Data.SqlClient`). Diseñado como una práctica de clase directa, sin Entity Framework ni capas complejas innecesarias.

---

## 📁 Estructura del Proyecto

```text
ProductosApi/
│
├── Models/
│   └── Producto.cs               # Entidad Producto (Id, Nombre, Precio, Cantidad)
│
├── Repositories/
│   ├── IProductoRepository.cs    # Interfaz con los métodos CRUD
│   └── ProductoRepository.cs     # Implementación con ADO.NET (SqlConnection, SqlCommand, SqlDataReader)
│
├── Endpoints/
│   └── ProductoEndpoints.cs      # Mapeo del grupo /api/productos, validaciones y respuestas HTTP
│
├── database/
│   └── crear_base.sql            # Script para crear la base ProductosDb y la tabla dbo.Productos
│
├── appsettings.json              # Configuración de cadena de conexión a SQL Server
├── Program.cs                    # Configuración de la aplicación, inyección de dependencias y Swagger
└── ProductosApi.csproj           # Archivo de proyecto (.NET 10)
```

---

## 🗄️ 1. Preparación de la Base de Datos

El script se encuentra en [`database/crear_base.sql`](database/crear_base.sql).

### Opción A: Desde SQL Server Management Studio (SSMS) o Azure Data Studio
1. Abre tu herramienta de SQL Server y conéctate a la instancia local `.\SQLEXPRESS`.
2. Abre el archivo `database/crear_base.sql`.
3. Ejecuta el script. Esto creará la base de datos `ProductosDb` y la tabla `dbo.Productos`.

### Opción B: Mediante línea de comandos (`sqlcmd`)
Si tienes instalada la herramienta `sqlcmd`, ejecuta desde la raíz del proyecto:
```bash
sqlcmd -S .\SQLEXPRESS -E -i database/crear_base.sql
```

---

## ⚙️ 2. Cadena de Conexión

En [`appsettings.json`](appsettings.json) se encuentra configurada la conexión:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ProductosDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True"
}
```
*Si tu instancia de SQL Server tiene otro nombre o usa autenticación SQL con usuario/contraseña, ajústala aquí.*

---

## 🚀 3. Ejecutar la API

Desde la carpeta `ProductosApi`:

```bash
dotnet run
```

La consola mostrará las URLs locales disponibles (por ejemplo `http://localhost:5189` o `https://localhost:7221`).

---

## 📑 4. Abrir Swagger UI

Abre tu navegador web y navega a:
- **`http://localhost:5189/swagger`** (o `https://localhost:7221/swagger`)
- *(Nota: Si ingresas a la raíz `http://localhost:5189/`, serás redirigido automáticamente a `/swagger`).*

---

## 🔌 5. Endpoints Disponibles (`/api/productos`)

| Método | Ruta | Código HTTP | Descripción |
|---|---|---|---|
| **GET** | `/api/productos` | `200 OK` | Retorna la lista completa de productos. |
| **GET** | `/api/productos/{id}` | `200 OK` / `404 Not Found` | Busca un producto por su Id. |
| **POST** | `/api/productos` | `201 Created` / `400 Bad Request` | Inserta un nuevo producto y devuelve su Id generado y la cabecera `Location`. |
| **PUT** | `/api/productos/{id}` | `204 No Content` / `400 Bad Request` / `404 Not Found` | Actualiza un producto existente. |
| **DELETE** | `/api/productos/{id}` | `204 No Content` / `404 Not Found` | Elimina un producto por su Id. |

### Ejemplo de JSON para POST / PUT:
```json
{
  "nombre": "Teclado Mecánico",
  "precio": 899.50,
  "cantidad": 15
}
```

### Reglas de Validación:
- `Nombre`: No puede estar vacío ni contener solo espacios en blanco.
- `Precio`: Debe ser mayor o igual a 0.
- `Cantidad`: Debe ser mayor o igual a 0.
- En caso de error de validación, la API responde `400 Bad Request` con un mensaje descriptivo.
- En caso de fallo en SQL Server, la API responde `500 Internal Server Error` sin exponer cadenas de conexión ni detalles sensibles.
