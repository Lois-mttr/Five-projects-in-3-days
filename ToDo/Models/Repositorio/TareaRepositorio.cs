using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ToDo.Models.Repositorio
{
    public class TareaRepositorio
    {
        public List<Tarea> ObtenerTodasLasTareas()
        {
            List<Tarea> tareas = new List<Tarea>();
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerTodasLasTareas");

            foreach (DataRow fila in tabla.Rows)
            {
                tareas.Add(MapearTarea(fila));
            }

            return tareas;
        }

        public List<Tarea> ObtenerTareasPorEstado(int estadoId)
        {
            List<Tarea> tareas = new List<Tarea>();
            SqlParameter[] parametros = { new SqlParameter("@EstadoId", estadoId) };
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerTareasPorEstado", parametros);

            foreach (DataRow fila in tabla.Rows)
            {
                tareas.Add(MapearTarea(fila));
            }

            return tareas;
        }

        public List<Tarea> ObtenerTareasVencidas()
        {
            List<Tarea> tareas = new List<Tarea>();
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerTareasVencidas");

            foreach (DataRow fila in tabla.Rows)
            {
                var tarea = MapearTarea(fila);
                if (tabla.Columns.Contains("DiasVencida"))
                {
                    tarea.DiasVencida = Convert.ToInt32(fila["DiasVencida"]);
                }
                tareas.Add(tarea);
            }

            return tareas;
        }

        public Tarea ObtenerTareaPorId(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerTareaPorId", parametros);

            if (tabla.Rows.Count > 0)
            {
                return MapearTarea(tabla.Rows[0]);
            }

            return null;
        }

        public int InsertarTarea(Tarea tarea)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Descripcion", tarea.Descripcion),
                new SqlParameter("@FechaVencimiento", tarea.FechaVencimiento),
                new SqlParameter("@EstadoId", tarea.EstadoId)
            };

            object resultado = ConexionBD.EjecutarEscalar("sp_InsertarTarea", parametros);
            return Convert.ToInt32(resultado);
        }

        public bool ActualizarTarea(Tarea tarea)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Id", tarea.Id),
                new SqlParameter("@Descripcion", tarea.Descripcion),
                new SqlParameter("@FechaVencimiento", tarea.FechaVencimiento),
                new SqlParameter("@EstadoId", tarea.EstadoId)
            };

            return ConexionBD.EjecutarComando("sp_ActualizarTarea", parametros) > 0;
        }

        public bool CambiarEstadoTarea(int id, int estadoId)
        {
            SqlParameter[] parametros = {
                new SqlParameter("@Id", id),
                new SqlParameter("@EstadoId", estadoId)
            };

            return ConexionBD.EjecutarComando("sp_CambiarEstadoTarea", parametros) > 0;
        }

        public bool MarcarTareaCompletada(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            return ConexionBD.EjecutarComando("sp_MarcarTareaCompletada", parametros) > 0;
        }

        public bool EliminarTarea(int id)
        {
            SqlParameter[] parametros = { new SqlParameter("@Id", id) };
            return ConexionBD.EjecutarComando("sp_EliminarTarea", parametros) > 0;
        }

        public EstadisticasTareas ObtenerEstadisticas()
        {
            EstadisticasTareas estadisticas = new EstadisticasTareas();

            // Obtener estadísticas por estado
            DataTable tablaEstados = ConexionBD.EjecutarConsulta("sp_ObtenerEstadisticasTareas");
            estadisticas.EstadisticasPorEstado = new List<EstadisticaEstado>();

            foreach (DataRow fila in tablaEstados.Rows)
            {
                var estadistica = new EstadisticaEstado
                {
                    NombreEstado = fila["NombreEstado"].ToString(),
                    ColorEstado = fila["ColorEstado"].ToString(),
                    CantidadTareas = Convert.ToInt32(fila["CantidadTareas"]),
                    Orden = Convert.ToInt32(fila["Orden"])
                };
                estadisticas.EstadisticasPorEstado.Add(estadistica);
            }

            // Calcular totales
            estadisticas.TotalTareas = estadisticas.EstadisticasPorEstado.Sum(e => e.CantidadTareas);
            estadisticas.TareasCompletadas = estadisticas.EstadisticasPorEstado
                .FirstOrDefault(e => e.NombreEstado == "Completada")?.CantidadTareas ?? 0;

            // Obtener tareas vencidas
            var tareasVencidas = ObtenerTareasVencidas();
            estadisticas.TareasVencidas = tareasVencidas.Count;

            // Calcular tareas urgentes (vencen hoy o mañana)
            var todasTareas = ObtenerTodasLasTareas();
            estadisticas.TareasUrgentes = todasTareas.Count(t => t.EsUrgente);

            // Calcular porcentaje completado
            estadisticas.PorcentajeCompletado = estadisticas.TotalTareas > 0
                ? Math.Round((double)estadisticas.TareasCompletadas / estadisticas.TotalTareas * 100, 1)
                : 0;

            return estadisticas;
        }

        public List<Estado> ObtenerEstados()
        {
            List<Estado> estados = new List<Estado>();
            DataTable tabla = ConexionBD.EjecutarConsulta("sp_ObtenerEstados");

            foreach (DataRow fila in tabla.Rows)
            {
                estados.Add(new Estado
                {
                    Id = Convert.ToInt32(fila["Id"]),
                    Nombre = fila["Nombre"].ToString(),
                    Color = fila["Color"].ToString(),
                    Descripcion = fila["Descripcion"].ToString()
                });
            }

            return estados;
        }

        private Tarea MapearTarea(DataRow fila)
        {
            return new Tarea
            {
                Id = Convert.ToInt32(fila["Id"]),
                Descripcion = fila["Descripcion"].ToString(),
                FechaVencimiento = Convert.ToDateTime(fila["FechaVencimiento"]),
                EstadoId = Convert.ToInt32(fila["EstadoId"]),
                NombreEstado = fila["NombreEstado"].ToString(),
                ColorEstado = fila["ColorEstado"].ToString(),
                FechaCreacion = Convert.ToDateTime(fila["FechaCreacion"]),
                FechaActualizacion = Convert.ToDateTime(fila["FechaActualizacion"]),
                EsVencida = fila.Table.Columns.Contains("EsVencida") ? Convert.ToBoolean(fila["EsVencida"]) : false
            };
        }
    }
}