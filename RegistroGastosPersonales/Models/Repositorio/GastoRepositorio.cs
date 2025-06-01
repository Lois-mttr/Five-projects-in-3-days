using RegistroGastosPersonales.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace RegistroGastosPersonales.Models.Repositorio
{
    public class GastoRepositorio
    {
        public List<Gasto> ObtenerTodosLosGastos()
        {
            List<Gasto> gastos = new List<Gasto>();
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerTodosLosGastos");

            foreach (DataRow fila in tabla.Rows)
            {
                gastos.Add(MapearGasto(fila));
            }

            return gastos;
        }

        public Gasto ObtenerGastoPorId(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerGastoPorId", parametros);

            if (tabla.Rows.Count > 0)
            {
                return MapearGasto(tabla.Rows[0]);
            }

            return null;
        }

        public int InsertarGasto(Gasto gasto)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Monto", gasto.Monto),
                new SqlParameter("@Descripcion", gasto.Descripcion),
                new SqlParameter("@Fecha", gasto.Fecha),
                new SqlParameter("@CategoriaId", gasto.CategoriaId)
            };

            object resultado = ConexionBD.EjecutarEscalar("sp_InsertarGasto", parametros);
            return Convert.ToInt32(resultado);
        }

        public bool ActualizarGasto(Gasto gasto)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Id", gasto.Id),
                new SqlParameter("@Monto", gasto.Monto),
                new SqlParameter("@Descripcion", gasto.Descripcion),
                new SqlParameter("@Fecha", gasto.Fecha),
                new SqlParameter("@CategoriaId", gasto.CategoriaId)
            };

            return ConexionBD.EjecutarComando("sp_ActualizarGasto", parametros) > 0;
        }

        public bool EliminarGasto(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            return ConexionBD.EjecutarComando("sp_EliminarGasto", parametros) > 0;
        }

        public ResumenGastos ObtenerResumenGastos()
        {
            ResumenGastos resumen = new ResumenGastos();

            // Obtener total de gastos
            DataTable tablaTotal = ConexionBD.EjecutarConsulta("sp_ObtenerTotalGastos");
            if (tablaTotal.Rows.Count > 0)
            {
                resumen.TotalGastos = Convert.ToDecimal(tablaTotal.Rows[0]["TotalGastos"]);
                resumen.TotalRegistros = Convert.ToInt32(tablaTotal.Rows[0]["TotalRegistros"]);
            }

            // Obtener gastos por categoría
            DataTable tablaCategorias = ConexionBD.EjecutarConsulta("sp_ObtenerGastosPorCategoria");
            resumen.GastosPorCategoria = new List<ResumenCategoria>();

            foreach (DataRow fila in tablaCategorias.Rows)
            {
                resumen.GastosPorCategoria.Add(new ResumenCategoria
                {
                    NombreCategoria = fila["Categoria"].ToString(),
                    TotalPorCategoria = Convert.ToDecimal(fila["TotalPorCategoria"]),
                    CantidadGastos = Convert.ToInt32(fila["CantidadGastos"])
                });
            }

            return resumen;
        }

        public List<Categoria> ObtenerCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerCategorias");

            foreach (DataRow fila in tabla.Rows)
            {
                categorias.Add(new Categoria
                {
                    Id = Convert.ToInt32(fila["Id"]),
                    Nombre = fila["Nombre"].ToString(),
                    Descripcion = fila["Descripcion"].ToString()
                });
            }

            return categorias;
        }

        private Gasto MapearGasto(DataRow fila)
        {
            return new Gasto
            {
                Id = Convert.ToInt32(fila["Id"]),
                Monto = Convert.ToDecimal(fila["Monto"]),
                Descripcion = fila["Descripcion"].ToString(),
                Fecha = Convert.ToDateTime(fila["Fecha"]),
                CategoriaId = Convert.ToInt32(fila["CategoriaId"]),
                NombreCategoria = fila["Categoria"].ToString(),
                FechaCreacion = fila.Table.Columns.Contains("FechaCreacion") ?
                         Convert.ToDateTime(fila["FechaCreacion"]) :
                         DateTime.Now // o algún valor por defecto
            };
        }
    }
}