CREATE DATABASE BDBiblioteca;
GO

USE BDBiblioteca;
GO

-- Crear tabla de libros
CREATE TABLE Libros (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(200) NOT NULL,
    Autor NVARCHAR(150) NOT NULL,
    ISBN NVARCHAR(20) NULL,
    AnioPublicacion INT NULL,
    ImagenPortada NVARCHAR(255) NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaActualizacion DATETIME DEFAULT GETDATE()
);
GO

-- Procedimiento para obtener todos los libros
CREATE PROCEDURE sp_ObtenerLibros
AS
BEGIN
    SELECT Id, Titulo, Autor, ISBN, AnioPublicacion, ImagenPortada, FechaCreacion, FechaActualizacion
    FROM Libros
    ORDER BY Titulo;
END
GO

-- Procedimiento para obtener un libro por ID
CREATE PROCEDURE sp_ObtenerLibroPorId
    @Id INT
AS
BEGIN
    SELECT Id, Titulo, Autor, ISBN, AnioPublicacion, ImagenPortada, FechaCreacion, FechaActualizacion
    FROM Libros
    WHERE Id = @Id;
END
GO

-- Procedimiento para buscar libros
CREATE PROCEDURE sp_BuscarLibros
    @Termino NVARCHAR(200)
AS
BEGIN
    SELECT Id, Titulo, Autor, ISBN, AnioPublicacion, ImagenPortada, FechaCreacion, FechaActualizacion
    FROM Libros
    WHERE Titulo LIKE '%' + @Termino + '%' OR Autor LIKE '%' + @Termino + '%'
    ORDER BY Titulo;
END
GO

-- Procedimiento para insertar libro
CREATE PROCEDURE sp_InsertarLibro
    @Titulo NVARCHAR(200),
    @Autor NVARCHAR(150),
    @ISBN NVARCHAR(20),
    @AnioPublicacion INT,
    @ImagenPortada NVARCHAR(255)
AS
BEGIN
    INSERT INTO Libros (Titulo, Autor, ISBN, AnioPublicacion, ImagenPortada)
    VALUES (@Titulo, @Autor, @ISBN, @AnioPublicacion, @ImagenPortada);
    
    SELECT SCOPE_IDENTITY() as NuevoId;
END
GO

-- Procedimiento para actualizar libro
CREATE PROCEDURE sp_ActualizarLibro
    @Id INT,
    @Titulo NVARCHAR(200),
    @Autor NVARCHAR(150),
    @ISBN NVARCHAR(20),
    @AnioPublicacion INT,
    @ImagenPortada NVARCHAR(255)
AS
BEGIN
    UPDATE Libros 
    SET Titulo = @Titulo,
        Autor = @Autor,
        ISBN = @ISBN,
        AnioPublicacion = @AnioPublicacion,
        ImagenPortada = @ImagenPortada,
        FechaActualizacion = GETDATE()
    WHERE Id = @Id;
END
GO

-- Procedimiento para eliminar libro
CREATE PROCEDURE sp_EliminarLibro
    @Id INT
AS
BEGIN
    DELETE FROM Libros WHERE Id = @Id;
END
GO

-- Procedimiento para obtener estadísticas
CREATE PROCEDURE sp_ObtenerEstadisticas
AS
BEGIN
    SELECT 
        COUNT(*) as TotalLibros,
        COUNT(DISTINCT Autor) as TotalAutores,
        MIN(AnioPublicacion) as AnioMasAntiguo,
        MAX(AnioPublicacion) as AnioMasReciente
    FROM Libros;
END
GO

-- Insertar algunos libros de ejemplo
INSERT INTO Libros (Titulo, Autor, ISBN, AnioPublicacion, ImagenPortada)
VALUES 
('Cien años de soledad', 'Gabriel García Márquez', '978-84-376-0494-7', 1967, NULL),
('Don Quijote de la Mancha', 'Miguel de Cervantes', '978-84-376-0495-4', 1605, NULL),
('La casa de los espíritus', 'Isabel Allende', '978-84-376-0496-1', 1982, NULL),
('Rayuela', 'Julio Cortázar', '978-84-376-0497-8', 1963, NULL);
GO