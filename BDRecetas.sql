-- Crear la base de datos
CREATE DATABASE CatalogoRecetas;
GO

USE CatalogoRecetas;
GO

-- Tabla de Categorías
CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200),
    FechaCreacion DATETIME2 DEFAULT GETDATE()
);

-- Tabla de Recetas
CREATE TABLE Recetas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Ingredientes NVARCHAR(MAX) NOT NULL,
    Instrucciones NVARCHAR(MAX) NOT NULL,
    CategoriaId INT NOT NULL,
    ImagenRuta NVARCHAR(500),
    TiempoPreparacion INT, -- en minutos
    Porciones INT,
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    FechaModificacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Recetas_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
);

-- Índices
CREATE INDEX IX_Recetas_CategoriaId ON Recetas(CategoriaId);
CREATE INDEX IX_Recetas_Nombre ON Recetas(Nombre);
CREATE INDEX IX_Recetas_FechaCreacion ON Recetas(FechaCreacion DESC);

-- Insertar categorías predeterminadas
INSERT INTO Categorias (Nombre, Descripcion) VALUES 
('Desayuno', 'Recetas para comenzar el día con energía'),
('Almuerzo', 'Comidas principales para el mediodía'),
('Cena', 'Recetas para la comida de la noche'),
('Postres', 'Dulces y postres deliciosos'),
('Bebidas', 'Bebidas refrescantes y nutritivas'),
('Aperitivos', 'Bocadillos y entradas'),
('Ensaladas', 'Platos frescos y saludables');

-- Insertar recetas de ejemplo
INSERT INTO Recetas (Nombre, Ingredientes, Instrucciones, CategoriaId, TiempoPreparacion, Porciones) VALUES 
('Huevos Revueltos Perfectos', 
 '• 4 huevos frescos
• 3 cucharadas de leche entera
• 1 cucharada de mantequilla
• Sal y pimienta al gusto
• Cebollín picado (opcional)', 
 '1. Batir los huevos con la leche, sal y pimienta en un bowl.
2. Calentar la mantequilla en una sartén antiadherente a fuego medio-bajo.
3. Verter la mezcla de huevos y revolver constantemente con una espátula.
4. Cocinar hasta que los huevos estén cremosos pero cuajados.
5. Servir inmediatamente decorado con cebollín.', 
 1, 8, 2),

('Ensalada César Clásica', 
 '• 2 lechugas romanas grandes
• 100g de queso parmesano rallado
• 1 taza de crutones caseros
• 200g de pechuga de pollo a la plancha
• 4 cucharadas de aderezo césar
• Jugo de 1 limón', 
 '1. Lavar y secar bien las hojas de lechuga, cortarlas en trozos.
2. Cocinar el pollo a la plancha y cortarlo en tiras.
3. En un bowl grande, mezclar la lechuga con el aderezo.
4. Agregar el pollo, crutones y queso parmesano.
5. Rociar con jugo de limón y servir inmediatamente.', 
 2, 20, 4),

('Pasta Alfredo Cremosa', 
 '• 400g de pasta fettuccine
• 200ml de crema de leche espesa
• 100g de mantequilla
• 150g de queso parmesano rallado
• 3 dientes de ajo picados
• Sal, pimienta y nuez moscada', 
 '1. Cocinar la pasta en agua hirviendo con sal según instrucciones del paquete.
2. En una sartén grande, derretir la mantequilla y sofreír el ajo.
3. Agregar la crema de leche y cocinar 2-3 minutos.
4. Incorporar el queso parmesano gradualmente hasta derretir.
5. Escurrir la pasta y mezclar con la salsa.
6. Sazonar con sal, pimienta y nuez moscada. Servir caliente.', 
 3, 25, 4);

GO

-- =====================================================
-- PROCEDIMIENTOS ALMACENADOS PARA CATEGORÍAS
-- =====================================================

-- Obtener todas las categorías
CREATE PROCEDURE sp_ObtenerCategorias
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Nombre,
        Descripcion,
        FechaCreacion
    FROM Categorias
    ORDER BY Nombre;
END
GO

-- Obtener categoría por ID
CREATE PROCEDURE sp_ObtenerCategoriaPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Nombre,
        Descripcion,
        FechaCreacion
    FROM Categorias
    WHERE Id = @Id;
END
GO

-- =====================================================
-- PROCEDIMIENTOS ALMACENADOS PARA RECETAS
-- =====================================================

-- Obtener todas las recetas con filtros
CREATE PROCEDURE sp_ObtenerRecetas
    @Buscar NVARCHAR(100) = NULL,
    @CategoriaId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.Id,
        r.Nombre,
        r.Ingredientes,
        r.Instrucciones,
        r.CategoriaId,
        r.ImagenRuta,
        r.TiempoPreparacion,
        r.Porciones,
        r.FechaCreacion,
        r.FechaModificacion,
        c.Nombre AS CategoriaNombre,
        c.Descripcion AS CategoriaDescripcion
    FROM Recetas r
    INNER JOIN Categorias c ON r.CategoriaId = c.Id
    WHERE 
        (@Buscar IS NULL OR r.Nombre LIKE '%' + @Buscar + '%' OR r.Ingredientes LIKE '%' + @Buscar + '%')
        AND (@CategoriaId IS NULL OR r.CategoriaId = @CategoriaId)
    ORDER BY r.FechaCreacion DESC;
END
GO

-- Obtener receta por ID
CREATE PROCEDURE sp_ObtenerRecetaPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.Id,
        r.Nombre,
        r.Ingredientes,
        r.Instrucciones,
        r.CategoriaId,
        r.ImagenRuta,
        r.TiempoPreparacion,
        r.Porciones,
        r.FechaCreacion,
        r.FechaModificacion,
        c.Nombre AS CategoriaNombre,
        c.Descripcion AS CategoriaDescripcion
    FROM Recetas r
    INNER JOIN Categorias c ON r.CategoriaId = c.Id
    WHERE r.Id = @Id;
END
GO

-- Crear nueva receta
CREATE PROCEDURE sp_CrearReceta
    @Nombre NVARCHAR(100),
    @Ingredientes NVARCHAR(MAX),
    @Instrucciones NVARCHAR(MAX),
    @CategoriaId INT,
    @ImagenRuta NVARCHAR(500) = NULL,
    @TiempoPreparacion INT = NULL,
    @Porciones INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdGenerado INT;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la categoría existe
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Id = @CategoriaId)
        BEGIN
            RAISERROR('La categoría especificada no existe', 16, 1);
            RETURN;
        END
        
        INSERT INTO Recetas (
            Nombre, 
            Ingredientes, 
            Instrucciones, 
            CategoriaId, 
            ImagenRuta, 
            TiempoPreparacion, 
            Porciones,
            FechaCreacion,
            FechaModificacion
        )
        VALUES (
            @Nombre, 
            @Ingredientes, 
            @Instrucciones, 
            @CategoriaId, 
            @ImagenRuta, 
            @TiempoPreparacion, 
            @Porciones,
            GETDATE(),
            GETDATE()
        );
        
        SET @IdGenerado = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        SELECT @IdGenerado AS Id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Actualizar receta
CREATE PROCEDURE sp_ActualizarReceta
    @Id INT,
    @Nombre NVARCHAR(100),
    @Ingredientes NVARCHAR(MAX),
    @Instrucciones NVARCHAR(MAX),
    @CategoriaId INT,
    @ImagenRuta NVARCHAR(500) = NULL,
    @TiempoPreparacion INT = NULL,
    @Porciones INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la receta existe
        IF NOT EXISTS (SELECT 1 FROM Recetas WHERE Id = @Id)
        BEGIN
            RAISERROR('La receta no existe', 16, 1);
            RETURN;
        END
        
        -- Verificar que la categoría existe
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Id = @CategoriaId)
        BEGIN
            RAISERROR('La categoría especificada no existe', 16, 1);
            RETURN;
        END
        
        UPDATE Recetas 
        SET 
            Nombre = @Nombre,
            Ingredientes = @Ingredientes,
            Instrucciones = @Instrucciones,
            CategoriaId = @CategoriaId,
            ImagenRuta = COALESCE(@ImagenRuta, ImagenRuta),
            TiempoPreparacion = @TiempoPreparacion,
            Porciones = @Porciones,
            FechaModificacion = GETDATE()
        WHERE Id = @Id;
        
        COMMIT TRANSACTION;
        
        SELECT 'Receta actualizada exitosamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Eliminar receta
CREATE PROCEDURE sp_EliminarReceta
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ImagenRuta NVARCHAR(500);
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Obtener la ruta de la imagen antes de eliminar
        SELECT @ImagenRuta = ImagenRuta 
        FROM Recetas 
        WHERE Id = @Id;
        
        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La receta no existe', 16, 1);
            RETURN;
        END
        
        DELETE FROM Recetas WHERE Id = @Id;
        
        COMMIT TRANSACTION;
        
        SELECT @ImagenRuta AS ImagenEliminada;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Obtener estadísticas
CREATE PROCEDURE sp_ObtenerEstadisticas
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        (SELECT COUNT(*) FROM Recetas) AS TotalRecetas,
        (SELECT COUNT(*) FROM Categorias) AS TotalCategorias,
        (SELECT ISNULL(AVG(CAST(TiempoPreparacion AS FLOAT)), 0) FROM Recetas WHERE TiempoPreparacion IS NOT NULL) AS TiempoPromedioPreparacion,
        (SELECT COUNT(*) FROM Recetas WHERE ImagenRuta IS NOT NULL) AS RecetasConImagen;
END
GO
