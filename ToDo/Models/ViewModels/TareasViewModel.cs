using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDo.Models.ViewModels
{
    public class TareasViewModel
    {
        public List<Tarea> Tareas { get; set; }
        public List<Estado> Estados { get; set; }
        public EstadisticasTareas Estadisticas { get; set; }
        public int? FiltroEstado { get; set; }
        public string VistaActual { get; set; }
        public Tarea NuevaTarea { get; set; }

        public TareasViewModel()
        {
            Tareas = new List<Tarea>();
            Estados = new List<Estado>();
            NuevaTarea = new Tarea();
            VistaActual = "todas";
        }
    }
}