-- Crear la base de datos
CREATE DATABASE BDRegistroGP;
GO

USE BDRegistroGP;
GO

-- Crear tabla de categorías
CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(200),
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Crear tabla de gastos
CREATE TABLE Gastos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Monto DECIMAL(10,2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    Fecha DATE NOT NULL,
    CategoriaId INT NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
);

-- Insertar categorías predeterminadas
INSERT INTO Categorias (Nombre, Descripcion) VALUES 
('Comida', 'Gastos relacionados con alimentación'),
('Transporte', 'Gastos de movilidad y transporte'),
('Entretenimiento', 'Gastos de ocio y diversión'),
('Otros', 'Gastos varios no categorizados');

GO

-- Procedimiento para obtener todos los gastos
CREATE PROCEDURE sp_ObtenerTodosLosGastos
AS
BEGIN
    SELECT 
        g.Id,
        g.Monto,
        g.Descripcion,
        g.Fecha,
        c.Nombre as Categoria,
        g.CategoriaId
    FROM Gastos g
    INNER JOIN Categorias c ON g.CategoriaId = c.Id
    ORDER BY g.Fecha DESC;
END
GO

-- Procedimiento para obtener gasto por ID
CREATE PROCEDURE sp_ObtenerGastoPorId
    @Id INT
AS
BEGIN
    SELECT 
        g.Id,
        g.Monto,
        g.Descripcion,
        g.Fecha,
        g.CategoriaId,
        c.Nombre as Categoria
    FROM Gastos g
    INNER JOIN Categorias c ON g.CategoriaId = c.Id
    WHERE g.Id = @Id;
END
GO

-- Procedimiento para insertar gasto
CREATE PROCEDURE sp_InsertarGasto
    @Monto DECIMAL(10,2),
    @Descripcion NVARCHAR(500),
    @Fecha DATE,
    @CategoriaId INT
AS
BEGIN
    INSERT INTO Gastos (Monto, Descripcion, Fecha, CategoriaId)
    VALUES (@Monto, @Descripcion, @Fecha, @CategoriaId);
    
    SELECT SCOPE_IDENTITY() as NuevoId;
END
GO

-- Procedimiento para actualizar gasto
CREATE PROCEDURE sp_ActualizarGasto
    @Id INT,
    @Monto DECIMAL(10,2),
    @Descripcion NVARCHAR(500),
    @Fecha DATE,
    @CategoriaId INT
AS
BEGIN
    UPDATE Gastos 
    SET Monto = @Monto,
        Descripcion = @Descripcion,
        Fecha = @Fecha,
        CategoriaId = @CategoriaId
    WHERE Id = @Id;
END
GO

-- Procedimiento para eliminar gasto
CREATE PROCEDURE sp_EliminarGasto
    @Id INT
AS
BEGIN
    DELETE FROM Gastos WHERE Id = @Id;
END
GO

-- Procedimiento para obtener total de gastos
CREATE PROCEDURE sp_ObtenerTotalGastos
AS
BEGIN
    SELECT 
        ISNULL(SUM(Monto), 0) as TotalGastos,
        COUNT(*) as TotalRegistros
    FROM Gastos;
END
GO

-- Procedimiento para obtener gastos por categoría
CREATE PROCEDURE sp_ObtenerGastosPorCategoria
AS
BEGIN
    SELECT 
        c.Nombre as Categoria,
        ISNULL(SUM(g.Monto), 0) as TotalPorCategoria,
        COUNT(g.Id) as CantidadGastos
    FROM Categorias c
    LEFT JOIN Gastos g ON c.Id = g.CategoriaId
    GROUP BY c.Id, c.Nombre
    ORDER BY TotalPorCategoria DESC;
END
GO

-- Procedimiento para obtener todas las categorías
CREATE PROCEDURE sp_ObtenerCategorias
AS
BEGIN
    SELECT Id, Nombre, Descripcion
    FROM Categorias
    ORDER BY Nombre;
END
GO