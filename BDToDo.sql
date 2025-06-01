CREATE DATABASE BDToDo;
GO

USE BDToDo;
GO

-- Crear tabla de estados
CREATE TABLE Estados (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL,
    Color NVARCHAR(7) NOT NULL, -- Para códigos de color hexadecimal
    Descripcion NVARCHAR(200),
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Crear tabla de tareas
CREATE TABLE Tareas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaVencimiento DATE NOT NULL,
    EstadoId INT NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaActualizacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EstadoId) REFERENCES Estados(Id)
);

-- Insertar estados predeterminados
INSERT INTO Estados (Nombre, Color, Descripcion) VALUES 
('Pendiente', '#ffc107', 'Tarea pendiente por realizar'),
('En Progreso', '#17a2b8', 'Tarea en proceso de realización'),
('Completada', '#28a745', 'Tarea completada exitosamente'),
('Cancelada', '#dc3545', 'Tarea cancelada o descartada');

GO

-- Procedimiento para obtener todas las tareas
CREATE PROCEDURE sp_ObtenerTodasLasTareas
AS
BEGIN
    SELECT 
        t.Id,
        t.Descripcion,
        t.FechaVencimiento,
        t.EstadoId,
        e.Nombre as NombreEstado,
        e.Color as ColorEstado,
        t.FechaCreacion,
        t.FechaActualizacion,
        CASE 
            WHEN t.FechaVencimiento < CAST(GETDATE() AS DATE) AND e.Nombre != 'Completada' 
            THEN 1 
            ELSE 0 
        END as EsVencida
    FROM Tareas t
    INNER JOIN Estados e ON t.EstadoId = e.Id
    ORDER BY 
        CASE WHEN e.Nombre = 'Completada' THEN 1 ELSE 0 END,
        t.FechaVencimiento ASC;
END
GO

-- Procedimiento para obtener tareas por estado
CREATE PROCEDURE sp_ObtenerTareasPorEstado
    @EstadoId INT
AS
BEGIN
    SELECT 
        t.Id,
        t.Descripcion,
        t.FechaVencimiento,
        t.EstadoId,
        e.Nombre as NombreEstado,
        e.Color as ColorEstado,
        t.FechaCreacion,
        t.FechaActualizacion,
        CASE 
            WHEN t.FechaVencimiento < CAST(GETDATE() AS DATE) AND e.Nombre != 'Completada' 
            THEN 1 
            ELSE 0 
        END as EsVencida
    FROM Tareas t
    INNER JOIN Estados e ON t.EstadoId = e.Id
    WHERE t.EstadoId = @EstadoId
    ORDER BY t.FechaVencimiento ASC;
END
GO

-- Procedimiento para obtener tarea por ID
CREATE PROCEDURE sp_ObtenerTareaPorId
    @Id INT
AS
BEGIN
    SELECT 
        t.Id,
        t.Descripcion,
        t.FechaVencimiento,
        t.EstadoId,
        e.Nombre as NombreEstado,
        e.Color as ColorEstado,
        t.FechaCreacion,
        t.FechaActualizacion
    FROM Tareas t
    INNER JOIN Estados e ON t.EstadoId = e.Id
    WHERE t.Id = @Id;
END
GO

-- Procedimiento para insertar tarea
CREATE PROCEDURE sp_InsertarTarea
    @Descripcion NVARCHAR(500),
    @FechaVencimiento DATE,
    @EstadoId INT = 1 -- Por defecto 'Pendiente'
AS
BEGIN
    INSERT INTO Tareas (Descripcion, FechaVencimiento, EstadoId)
    VALUES (@Descripcion, @FechaVencimiento, @EstadoId);
    
    SELECT SCOPE_IDENTITY() as NuevoId;
END
GO

-- Procedimiento para actualizar tarea
CREATE PROCEDURE sp_ActualizarTarea
    @Id INT,
    @Descripcion NVARCHAR(500),
    @FechaVencimiento DATE,
    @EstadoId INT
AS
BEGIN
    UPDATE Tareas 
    SET Descripcion = @Descripcion,
        FechaVencimiento = @FechaVencimiento,
        EstadoId = @EstadoId,
        FechaActualizacion = GETDATE()
    WHERE Id = @Id;
END
GO

-- Procedimiento para cambiar estado de tarea
CREATE PROCEDURE sp_CambiarEstadoTarea
    @Id INT,
    @EstadoId INT
AS
BEGIN
    UPDATE Tareas 
    SET EstadoId = @EstadoId,
        FechaActualizacion = GETDATE()
    WHERE Id = @Id;
END
GO

-- Procedimiento para eliminar tarea
CREATE PROCEDURE sp_EliminarTarea
    @Id INT
AS
BEGIN
    DELETE FROM Tareas WHERE Id = @Id;
END
GO

-- Procedimiento para obtener estadísticas
CREATE PROCEDURE sp_ObtenerEstadisticasTareas
AS
BEGIN
    SELECT 
        e.Nombre as NombreEstado,
        e.Color as ColorEstado,
        COUNT(t.Id) as CantidadTareas,
        CASE 
            WHEN e.Nombre = 'Pendiente' THEN 1
            WHEN e.Nombre = 'En Progreso' THEN 2
            WHEN e.Nombre = 'Completada' THEN 3
            WHEN e.Nombre = 'Cancelada' THEN 4
            ELSE 5
        END as Orden
    FROM Estados e
    LEFT JOIN Tareas t ON e.Id = t.EstadoId
    GROUP BY e.Id, e.Nombre, e.Color
    ORDER BY Orden;
END
GO

-- Procedimiento para obtener tareas vencidas
CREATE PROCEDURE sp_ObtenerTareasVencidas
AS
BEGIN
    SELECT 
        t.Id,
        t.Descripcion,
        t.FechaVencimiento,
        t.EstadoId,
        e.Nombre as NombreEstado,
        e.Color as ColorEstado,
        t.FechaCreacion,
        t.FechaActualizacion,
        DATEDIFF(DAY, t.FechaVencimiento, GETDATE()) as DiasVencida
    FROM Tareas t
    INNER JOIN Estados e ON t.EstadoId = e.Id
    WHERE t.FechaVencimiento < CAST(GETDATE() AS DATE) 
    AND e.Nombre != 'Completada'
    ORDER BY t.FechaVencimiento ASC;
END
GO

-- Procedimiento para obtener todos los estados
CREATE PROCEDURE sp_ObtenerEstados
AS
BEGIN
    SELECT Id, Nombre, Color, Descripcion
    FROM Estados
    ORDER BY 
        CASE 
            WHEN Nombre = 'Pendiente' THEN 1
            WHEN Nombre = 'En Progreso' THEN 2
            WHEN Nombre = 'Completada' THEN 3
            WHEN Nombre = 'Cancelada' THEN 4
            ELSE 5
        END;
END
GO

-- Procedimiento para marcar tarea como completada
CREATE PROCEDURE sp_MarcarTareaCompletada
    @Id INT
AS
BEGIN
    DECLARE @EstadoCompletadaId INT;
    SELECT @EstadoCompletadaId = Id FROM Estados WHERE Nombre = 'Completada';
    
    UPDATE Tareas 
    SET EstadoId = @EstadoCompletadaId,
        FechaActualizacion = GETDATE()
    WHERE Id = @Id;
END
GO