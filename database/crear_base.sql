IF DB_ID(N'ProductosDb') IS NULL
    CREATE DATABASE ProductosDb;
GO

USE ProductosDb;
GO

IF OBJECT_ID(N'dbo.Productos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Precio DECIMAL(10,2) NOT NULL CHECK (Precio >= 0),
        Cantidad INT NOT NULL CHECK (Cantidad >= 0)
    );
END
GO
