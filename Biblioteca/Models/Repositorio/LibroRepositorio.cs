using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static Biblioteca.Models.ViewModels.BibliotecaViewModel;

namespace Biblioteca.Models.Repositorio
{
    public class LibroRepositorio
    {
        public List<Libro> ObtenerLibros()
        {
            List<Libro> libros = new List<Libro>();
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerLibros");

            foreach (DataRow fila in tabla.Rows)
            {
                libros.Add(MapearLibro(fila));
            }

            return libros;
        }

        public Libro ObtenerLibroPorId(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerLibroPorId", parametros);

            if (tabla.Rows.Count > 0)
            {
                return MapearLibro(tabla.Rows[0]);
            }

            return null;
        }

        public List<Libro> BuscarLibros(string termino)
        {
            List<Libro> libros = new List<Libro>();
            SqlParameter[] parametros = { new SqlParameter("@Termino", termino) };
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_BuscarLibros", parametros);

            foreach (DataRow fila in tabla.Rows)
            {
                libros.Add(MapearLibro(fila));
            }

            return libros;
        }

        public int InsertarLibro(Libro libro)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Titulo", libro.Titulo),
                new SqlParameter("@Autor", libro.Autor),
                new SqlParameter("@ISBN", (object)libro.ISBN ?? DBNull.Value),
                new SqlParameter("@AnioPublicacion", (object)libro.AnioPublicacion ?? DBNull.Value),
                new SqlParameter("@ImagenPortada", (object)libro.ImagenPortada ?? DBNull.Value)
            };

            object resultado = ConexionBD.EjecutarEscalar("sp_InsertarLibro", parametros);
            return Convert.ToInt32(resultado);
        }

        public bool ActualizarLibro(Libro libro)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Id", libro.Id),
                new SqlParameter("@Titulo", libro.Titulo),
                new SqlParameter("@Autor", libro.Autor),
                new SqlParameter("@ISBN", (object)libro.ISBN ?? DBNull.Value),
                new SqlParameter("@AnioPublicacion", (object)libro.AnioPublicacion ?? DBNull.Value),
                new SqlParameter("@ImagenPortada", (object)libro.ImagenPortada ?? DBNull.Value)
            };

            return ConexionBD.EjecutarComando("sp_ActualizarLibro", parametros) > 0;
        }

        public bool EliminarLibro(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            return ConexionBD.EjecutarComando("sp_EliminarLibro", parametros) > 0;
        }

        public EstadisticasBiblioteca ObtenerEstadisticas()
        {
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerEstadisticas");

            if (tabla.Rows.Count > 0)
            {
                DataRow fila = tabla.Rows[0];
                return new EstadisticasBiblioteca
                {
                    TotalLibros = Convert.ToInt32(fila["TotalLibros"]),
                    TotalAutores = Convert.ToInt32(fila["TotalAutores"]),
                    AnioMasAntiguo = fila["AnioMasAntiguo"] != DBNull.Value ? Convert.ToInt32(fila["AnioMasAntiguo"]) : (int?)null,
                    AnioMasReciente = fila["AnioMasReciente"] != DBNull.Value ? Convert.ToInt32(fila["AnioMasReciente"]) : (int?)null
                };
            }

            return new EstadisticasBiblioteca();
        }

        private Libro MapearLibro(DataRow fila)
        {
            return new Libro
            {
                Id = Convert.ToInt32(fila["Id"]),
                Titulo = fila["Titulo"].ToString(),
                Autor = fila["Autor"].ToString(),
                ISBN = fila["ISBN"] != DBNull.Value ? fila["ISBN"].ToString() : null,
                AnioPublicacion = fila["AnioPublicacion"] != DBNull.Value ? Convert.ToInt32(fila["AnioPublicacion"]) : (int?)null,
                ImagenPortada = fila["ImagenPortada"] != DBNull.Value ? fila["ImagenPortada"].ToString() : null,
                FechaCreacion = Convert.ToDateTime(fila["FechaCreacion"]),
                FechaActualizacion = Convert.ToDateTime(fila["FechaActualizacion"])
            };
        }
    }
}