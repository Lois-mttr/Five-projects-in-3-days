using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RegistroGastosPersonales.Models
{
    public class ResumenGastos
    {
        public decimal TotalGastos { get; set; }
        public int TotalRegistros { get; set; }
        public List<ResumenCategoria> GastosPorCategoria { get; set; }
    }

    public class ResumenCategoria
    {
        public string NombreCategoria { get; set; }
        public decimal TotalPorCategoria { get; set; }
        public int CantidadGastos { get; set; }
    }
}