using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CatalogoRecetas.Models;

namespace CatalogoRecetas.Data
{
    public class RepositorioRecetas
    {
        private readonly ConexionBaseDatos _conexionBD;

        public RepositorioRecetas()
        {
            _conexionBD = new ConexionBaseDatos();
        }

        // MÉTODOS PARA CATEGORÍAS

        public List<Categoria> ObtenerTodasLasCategorias()
        {
            var categorias = new List<Categoria>();

            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_ObtenerCategorias", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                categorias.Add(new Categoria
                                {
                                    Id = Convert.ToInt32(lector["Id"]),
                                    Nombre = lector["Nombre"].ToString(),
                                    Descripcion = lector["Descripcion"]?.ToString(),
                                    FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categorías: {ex.Message}", ex);
            }

            return categorias;
        }

        public Categoria ObtenerCategoriaPorId(int id)
        {
            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_ObtenerCategoriaPorId", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@Id", id);

                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                return new Categoria
                                {
                                    Id = Convert.ToInt32(lector["Id"]),
                                    Nombre = lector["Nombre"].ToString(),
                                    Descripcion = lector["Descripcion"]?.ToString(),
                                    FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categoría: {ex.Message}", ex);
            }

            return null;
        }

        // 
        // MÉTODOS PARA RECETAS
        // 

        public List<RecetaCompleta> ObtenerRecetasConFiltros(string buscar = null, int? categoriaId = null)
        {
            var recetas = new List<RecetaCompleta>();

            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_ObtenerRecetas", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        comando.Parameters.AddWithValue("@Buscar", (object)buscar ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@CategoriaId", (object)categoriaId ?? DBNull.Value);

                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                recetas.Add(MapearRecetaCompleta(lector));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener recetas: {ex.Message}", ex);
            }

            return recetas;
        }

        public RecetaCompleta ObtenerRecetaPorId(int id)
        {
            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_ObtenerRecetaPorId", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@Id", id);

                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                return MapearRecetaCompleta(lector);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener receta: {ex.Message}", ex);
            }

            return null;
        }

        public int CrearReceta(Receta receta)
        {
            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_CrearReceta", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        comando.Parameters.AddWithValue("@Nombre", receta.Nombre);
                        comando.Parameters.AddWithValue("@Ingredientes", receta.Ingredientes);
                        comando.Parameters.AddWithValue("@Instrucciones", receta.Instrucciones);
                        comando.Parameters.AddWithValue("@CategoriaId", receta.CategoriaId);
                        comando.Parameters.AddWithValue("@ImagenRuta", (object)receta.ImagenRuta ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@TiempoPreparacion", (object)receta.TiempoPreparacion ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@Porciones", (object)receta.Porciones ?? DBNull.Value);

                        var resultado = comando.ExecuteScalar();
                        return Convert.ToInt32(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear receta: {ex.Message}", ex);
            }
        }

        public void ActualizarReceta(Receta receta)
        {
            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_ActualizarReceta", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        comando.Parameters.AddWithValue("@Id", receta.Id);
                        comando.Parameters.AddWithValue("@Nombre", receta.Nombre);
                        comando.Parameters.AddWithValue("@Ingredientes", receta.Ingredientes);
                        comando.Parameters.AddWithValue("@Instrucciones", receta.Instrucciones);
                        comando.Parameters.AddWithValue("@CategoriaId", receta.CategoriaId);
                        comando.Parameters.AddWithValue("@ImagenRuta", (object)receta.ImagenRuta ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@TiempoPreparacion", (object)receta.TiempoPreparacion ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@Porciones", (object)receta.Porciones ?? DBNull.Value);

                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar receta: {ex.Message}", ex);
            }
        }

        public string EliminarReceta(int id)
        {
            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_EliminarReceta", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@Id", id);

                        var resultado = comando.ExecuteScalar();
                        return resultado?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar receta: {ex.Message}", ex);
            }
        }

        public EstadisticasGenerales ObtenerEstadisticas()
        {
            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_ObtenerEstadisticas", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                return new EstadisticasGenerales
                                {
                                    TotalRecetas = Convert.ToInt32(lector["TotalRecetas"]),
                                    TotalCategorias = Convert.ToInt32(lector["TotalCategorias"]),
                                    TiempoPromedioPreparacion = Convert.ToDouble(lector["TiempoPromedioPreparacion"]),
                                    RecetasConImagen = Convert.ToInt32(lector["RecetasConImagen"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas: {ex.Message}", ex);
            }

            return new EstadisticasGenerales();
        }

        // =====================================================
        // MÉTODOS AUXILIARES
        // =====================================================

        private RecetaCompleta MapearRecetaCompleta(SqlDataReader lector)
        {
            return new RecetaCompleta
            {
                Id = Convert.ToInt32(lector["Id"]),
                Nombre = lector["Nombre"].ToString(),
                Ingredientes = lector["Ingredientes"].ToString(),
                Instrucciones = lector["Instrucciones"].ToString(),
                CategoriaId = Convert.ToInt32(lector["CategoriaId"]),
                ImagenRuta = lector["ImagenRuta"]?.ToString(),
                TiempoPreparacion = lector["TiempoPreparacion"] == DBNull.Value ? null : (int?)Convert.ToInt32(lector["TiempoPreparacion"]),
                Porciones = lector["Porciones"] == DBNull.Value ? null : (int?)Convert.ToInt32(lector["Porciones"]),
                FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"]),
                FechaModificacion = Convert.ToDateTime(lector["FechaModificacion"]),
                CategoriaNombre = lector["CategoriaNombre"].ToString(),
                CategoriaDescripcion = lector["CategoriaDescripcion"]?.ToString()
            };
        }
    }
}