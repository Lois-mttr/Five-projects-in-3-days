using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Contactos.Models.Repositorio
{
    public class ContactoRepositorio
    {
        public List<Contacto> ObtenerContactos()
        {
            List<Contacto> contactos = new List<Contacto>();
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerContactos");

            foreach (DataRow fila in tabla.Rows)
            {
                contactos.Add(MapearContacto(fila));
            }

            return contactos;
        }

        public Contacto ObtenerContactoPorId(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerContactoPorId", parametros);

            if (tabla.Rows.Count > 0)
            {
                return MapearContacto(tabla.Rows[0]);
            }

            return null;
        }

        public List<Contacto> BuscarContactos(string termino)
        {
            List<Contacto> contactos = new List<Contacto>();
            SqlParameter[] parametros = { new SqlParameter("@Termino", termino) };
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_BuscarContactos", parametros);

            foreach (DataRow fila in tabla.Rows)
            {
                contactos.Add(MapearContacto(fila));
            }

            return contactos;
        }

        public int InsertarContacto(Contacto contacto)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Nombre", contacto.Nombre),
                new SqlParameter("@Telefono", (object)contacto.Telefono ?? DBNull.Value),
                new SqlParameter("@Email", (object)contacto.Email ?? DBNull.Value),
                new SqlParameter("@Notas", (object)contacto.Notas ?? DBNull.Value)
            };

            object resultado = ConexionBD.EjecutarEscalar("sp_InsertarContacto", parametros);
            return Convert.ToInt32(resultado);
        }

        public bool ActualizarContacto(Contacto contacto)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Id", contacto.Id),
                new SqlParameter("@Nombre", contacto.Nombre),
                new SqlParameter("@Telefono", (object)contacto.Telefono ?? DBNull.Value),
                new SqlParameter("@Email", (object)contacto.Email ?? DBNull.Value),
                new SqlParameter("@Notas", (object)contacto.Notas ?? DBNull.Value)
            };

            return ConexionBD.EjecutarComando("sp_ActualizarContacto", parametros) > 0;
        }

        public bool EliminarContacto(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            return ConexionBD.EjecutarComando("sp_EliminarContacto", parametros) > 0;
        }

        private Contacto MapearContacto(DataRow fila)
        {
            return new Contacto
            {
                Id = Convert.ToInt32(fila["Id"]),
                Nombre = fila["Nombre"].ToString(),
                Telefono = fila["Telefono"] != DBNull.Value ? fila["Telefono"].ToString() : null,
                Email = fila["Email"] != DBNull.Value ? fila["Email"].ToString() : null,
                Notas = fila["Notas"] != DBNull.Value ? fila["Notas"].ToString() : null,
                FechaCreacion = Convert.ToDateTime(fila["FechaCreacion"])
            };
        }
    }
}