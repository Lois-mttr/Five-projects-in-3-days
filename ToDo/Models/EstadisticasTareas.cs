using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDo.Models
{
    public class EstadisticasTareas
    {
        public List<EstadisticaEstado> EstadisticasPorEstado { get; set; }
        public int TotalTareas { get; set; }
        public int TareasVencidas { get; set; }
        public int TareasUrgentes { get; set; }
        public int TareasCompletadas { get; set; }
        public double PorcentajeCompletado { get; set; }
    }

    public class EstadisticaEstado
    {
        public string NombreEstado { get; set; }
        public string ColorEstado { get; set; }
        public int CantidadTareas { get; set; }
        public int Orden { get; set; }
    }
}