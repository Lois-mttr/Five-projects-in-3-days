CREATE DATABASE BDContactos

USE BDContactos

-- Crear tabla de contactos
CREATE TABLE Contactos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Notas NVARCHAR(500) NULL,
    FechaCreacion DATETIME DEFAULT GETDATE()
);
GO

-- Procedimiento para obtener todos los contactos
CREATE PROCEDURE sp_ObtenerContactos
AS
BEGIN
    SELECT Id, Nombre, Telefono, Email, Notas, FechaCreacion
    FROM Contactos
    ORDER BY Nombre;
END
GO

-- Procedimiento para obtener un contacto por ID
CREATE PROCEDURE sp_ObtenerContactoPorId
    @Id INT
AS
BEGIN
    SELECT Id, Nombre, Telefono, Email, Notas, FechaCreacion
    FROM Contactos
    WHERE Id = @Id;
END
GO

-- Procedimiento para buscar contactos
CREATE PROCEDURE sp_BuscarContactos
    @Termino NVARCHAR(100)
AS
BEGIN
    SELECT Id, Nombre, Telefono, Email, Notas, FechaCreacion
    FROM Contactos
    WHERE Nombre LIKE '%' + @Termino + '%' OR Email LIKE '%' + @Termino + '%'
    ORDER BY Nombre;
END
GO

-- Procedimiento para insertar contacto
CREATE PROCEDURE sp_InsertarContacto
    @Nombre NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @Email NVARCHAR(100),
    @Notas NVARCHAR(500)
AS
BEGIN
    INSERT INTO Contactos (Nombre, Telefono, Email, Notas)
    VALUES (@Nombre, @Telefono, @Email, @Notas);
    
    SELECT SCOPE_IDENTITY() as NuevoId;
END
GO

-- Procedimiento para actualizar contacto
CREATE PROCEDURE sp_ActualizarContacto
    @Id INT,
    @Nombre NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @Email NVARCHAR(100),
    @Notas NVARCHAR(500)
AS
BEGIN
    UPDATE Contactos 
    SET Nombre = @Nombre,
        Telefono = @Telefono,
        Email = @Email,
        Notas = @Notas
    WHERE Id = @Id;
END
GO

-- Procedimiento para eliminar contacto
CREATE PROCEDURE sp_EliminarContacto
    @Id INT
AS
BEGIN
    DELETE FROM Contactos WHERE Id = @Id;
END
GO

-- Insertar algunos contactos de ejemplo
INSERT INTO Contactos (Nombre, Telefono, Email, Notas)
VALUES 
('Ana García', '555-123-4567', 'ana.garcia@ejemplo.com', 'Contacto de trabajo'),
('Carlos Rodríguez', '555-987-6543', 'carlos.rodriguez@ejemplo.com', 'Amigo de la universidad'),
('María López', '555-456-7890', 'maria.lopez@ejemplo.com', 'Cliente importante');
GO