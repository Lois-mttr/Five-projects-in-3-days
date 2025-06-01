using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CatalogoRecetas.Models
{
    public class Receta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la receta es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre de la Receta")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los ingredientes son obligatorios")]
        [Display(Name = "Ingredientes")]
        public string Ingredientes { get; set; }

        [Required(ErrorMessage = "Las instrucciones son obligatorias")]
        [Display(Name = "Instrucciones de Preparación")]
        public string Instrucciones { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Display(Name = "Imagen")]
        public string ImagenRuta { get; set; }

        [Display(Name = "Tiempo de Preparación (minutos)")]
        [Range(1, 1440, ErrorMessage = "El tiempo debe estar entre 1 y 1440 minutos")]
        public int? TiempoPreparacion { get; set; }

        [Display(Name = "Número de Porciones")]
        [Range(1, 50, ErrorMessage = "Las porciones deben estar entre 1 y 50")]
        public int? Porciones { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Última Modificación")]
        public DateTime FechaModificacion { get; set; }

        // Propiedad para subir archivos
        [Display(Name = "Seleccionar Imagen")]
        public HttpPostedFileBase ArchivoImagen { get; set; }

        public Receta()
        {
            FechaCreacion = DateTime.Now;
            FechaModificacion = DateTime.Now;
        }
    }

    // Clase para recetas con información de categoría
    public class RecetaCompleta : Receta
    {
        [Display(Name = "Categoría")]
        public string CategoriaNombre { get; set; }

        [Display(Name = "Descripción de Categoría")]
        public string CategoriaDescripcion { get; set; }
    }

    // Clase para estadísticas
    public class EstadisticasGenerales
    {
        [Display(Name = "Total de Recetas")]
        public int TotalRecetas { get; set; }

        [Display(Name = "Total de Categorías")]
        public int TotalCategorias { get; set; }

        [Display(Name = "Tiempo Promedio de Preparación")]
        public double TiempoPromedioPreparacion { get; set; }

        [Display(Name = "Recetas con Imagen")]
        public int RecetasConImagen { get; set; }
    }
}
