using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Contactos.Models.Repositorio
{
    public class ConexionBD
    {
        private static string cadenaConexion = ConfigurationManager.ConnectionStrings["BDContactosConnection"].ConnectionString;

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }

        public static DataTable EjecutarConsulta(string nombreProcedimiento, params SqlParameter[] parametros)
        {
            DataTable tabla = new DataTable();

            using (SqlConnection conexion = ObtenerConexion())
            {
                using (SqlCommand comando = new SqlCommand(nombreProcedimiento, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                    {
                        comando.Parameters.AddRange(parametros);
                    }

                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        adaptador.Fill(tabla);
                    }
                }
            }

            return tabla;
        }

        public static int EjecutarComando(string nombreProcedimiento, params SqlParameter[] parametros)
        {
            using (SqlConnection conexion = ObtenerConexion())
            {
                using (SqlCommand comando = new SqlCommand(nombreProcedimiento, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                    {
                        comando.Parameters.AddRange(parametros);
                    }

                    conexion.Open();
                    return comando.ExecuteNonQuery();
                }
            }
        }

        public static object EjecutarEscalar(string nombreProcedimiento, params SqlParameter[] parametros)
        {
            using (SqlConnection conexion = ObtenerConexion())
            {
                using (SqlCommand comando = new SqlCommand(nombreProcedimiento, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                    {
                        comando.Parameters.AddRange(parametros);
                    }

                    conexion.Open();
                    return comando.ExecuteScalar();
                }
            }
        }
    }
}