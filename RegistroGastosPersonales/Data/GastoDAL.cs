using RegistroGastosPersonales.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace RegistroGastosPersonales.Data
{
    public class GastoDAL
    {
        public List<Gasto> ObtenerTodosLosGastos()
        {
            List<Gasto> gastos = new List<Gasto>();
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerTodosLosGastos");

            foreach (DataRow fila in tabla.Rows)
            {
                gastos.Add(new Gasto
                {
                    Id = Convert.ToInt32(fila["Id"]),
                    Monto = Convert.ToDecimal(fila["Monto"]),
                    Descripcion = fila["Descripcion"].ToString(),
                    Fecha = Convert.ToDateTime(fila["Fecha"]),
                    CategoriaId = Convert.ToInt32(fila["CategoriaId"]),
                    NombreCategoria = fila["Categoria"].ToString()
                });
            }

            return gastos;
        }

        public Gasto ObtenerGastoPorId(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerGastoPorId", parametros);

            if (tabla.Rows.Count > 0)
            {
                DataRow fila = tabla.Rows[0];
                return new Gasto
                {
                    Id = Convert.ToInt32(fila["Id"]),
                    Monto = Convert.ToDecimal(fila["Monto"]),
                    Descripcion = fila["Descripcion"].ToString(),
                    Fecha = Convert.ToDateTime(fila["Fecha"]),
                    CategoriaId = Convert.ToInt32(fila["CategoriaId"]),
                    NombreCategoria = fila["Categoria"].ToString()
                };
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

        public decimal ObtenerTotalGastos()
        {
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerTotalGastos");
            if (tabla.Rows.Count > 0)
            {
                return Convert.ToDecimal(tabla.Rows[0]["TotalGastos"]);
            }
            return 0;
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
    }
}