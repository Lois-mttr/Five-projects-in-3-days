using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteca.Models.ViewModels
{
    public class BibliotecaViewModel
    {
        public List<Libro> Libros { get; set; }
        public EstadisticasBiblioteca Estadisticas { get; set; }
        public string TerminoBusqueda { get; set; }

        public BibliotecaViewModel()
        {
            Libros = new List<Libro>();
            Estadisticas = new EstadisticasBiblioteca();
        }
        public class EstadisticasBiblioteca
        {
            public int TotalLibros { get; set; }
            public int TotalAutores { get; set; }
            public int? AnioMasAntiguo { get; set; }
            public int? AnioMasReciente { get; set; }
        }

    }
}