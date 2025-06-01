using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Biblioteca.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(150, ErrorMessage = "El autor no puede exceder los 150 caracteres")]
        [Display(Name = "Autor")]
        public string Autor { get; set; }

        [StringLength(20, ErrorMessage = "El ISBN no puede exceder los 20 caracteres")]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }

        [Range(1, 2024, ErrorMessage = "El año de publicación debe estar entre 1 y 2024")]
        [Display(Name = "Año de Publicación")]
        public int? AnioPublicacion { get; set; }

        [Display(Name = "Imagen de Portada")]
        public string ImagenPortada { get; set; }

        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Última Actualización")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime FechaActualizacion { get; set; }

        // Propiedad para el archivo de imagen (no se mapea a la base de datos)
        [Display(Name = "Subir Imagen de Portada")]
        public HttpPostedFileBase ArchivoImagen { get; set; }

        // Propiedades calculadas
        public bool TieneImagen => !string.IsNullOrEmpty(ImagenPortada);
        public string RutaImagenCompleta => TieneImagen ? $"~/Content/Images/Portadas/{ImagenPortada}" : "~/Content/Images/no-image.png";
    }
}