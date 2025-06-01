using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDo.Models
{
    public class Tarea
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Vencimiento")]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado")]
        public int EstadoId { get; set; }

        [Display(Name = "Estado")]
        public string NombreEstado { get; set; }

        [Display(Name = "Color del Estado")]
        public string ColorEstado { get; set; }

        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Última Actualización")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime FechaActualizacion { get; set; }

        [Display(Name = "¿Está Vencida?")]
        public bool EsVencida { get; set; }

        [Display(Name = "Días Vencida")]
        public int DiasVencida { get; set; }

        // Propiedades calculadas
        public bool EsUrgente => FechaVencimiento.Date <= DateTime.Now.Date.AddDays(1) && NombreEstado != "Completada";
        public bool EsHoy => FechaVencimiento.Date == DateTime.Now.Date;
        public string ClaseCss => EsVencida ? "tarea-vencida" : EsUrgente ? "tarea-urgente" : EsHoy ? "tarea-hoy" : "";
    }
}