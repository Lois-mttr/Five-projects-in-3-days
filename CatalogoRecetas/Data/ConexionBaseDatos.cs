using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CatalogoRecetas.Data
{
    public class ConexionBaseDatos
    {
        private readonly string _cadenaConexion;

        public ConexionBaseDatos()
        {
            _cadenaConexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public SqlConnection ObtenerConexion()
        {
            try
            {
                var conexion = new SqlConnection(_cadenaConexion);
                return conexion;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al conectar con la base de datos: {ex.Message}", ex);
            }
        }

        public void ProbarConexion()
        {
            try
            {
                using (var conexion = ObtenerConexion())
                {
                    conexion.Open();
                    if (conexion.State == System.Data.ConnectionState.Open)
                    {
                        System.Diagnostics.Debug.WriteLine("Conexión exitosa a la base de datos");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al probar la conexión: {ex.Message}", ex);
            }
        }
    }
}