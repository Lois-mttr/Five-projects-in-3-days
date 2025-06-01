using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Biblioteca.Services
{
    public class ImagenService
    {
        private readonly string _rutaImagenes;

        public ImagenService()
        {
            _rutaImagenes = HttpContext.Current.Server.MapPath("~/Content/Images/Portadas/");

            // Crear directorio si no existe
            if (!Directory.Exists(_rutaImagenes))
            {
                Directory.CreateDirectory(_rutaImagenes);
            }
        }

        public string GuardarImagen(HttpPostedFileBase archivo)
        {
            if (archivo == null || archivo.ContentLength == 0)
                return null;

            // Validar tipo de archivo
            string[] tiposPermitidos = { ".jpg", ".jpeg", ".png", ".gif" };
            string extension = Path.GetExtension(archivo.FileName).ToLower();

            if (Array.IndexOf(tiposPermitidos, extension) == -1)
                throw new ArgumentException("Tipo de archivo no permitido. Solo se permiten imágenes JPG, PNG y GIF.");

            // Validar tamaño (máximo 5MB)
            if (archivo.ContentLength > 5 * 1024 * 1024)
                throw new ArgumentException("El archivo es demasiado grande. Tamaño máximo: 5MB.");

            // Generar nombre único
            string nombreArchivo = Guid.NewGuid().ToString() + extension;
            string rutaCompleta = Path.Combine(_rutaImagenes, nombreArchivo);

            // Guardar archivo
            archivo.SaveAs(rutaCompleta);

            return nombreArchivo;
        }

        public bool EliminarImagen(string nombreArchivo)
        {
            if (string.IsNullOrEmpty(nombreArchivo))
                return false;

            try
            {
                string rutaCompleta = Path.Combine(_rutaImagenes, nombreArchivo);
                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                    return true;
                }
            }
            catch (Exception)
            {
                // Log error si es necesario
            }

            return false;
        }

        public bool ExisteImagen(string nombreArchivo)
        {
            if (string.IsNullOrEmpty(nombreArchivo))
                return false;

            string rutaCompleta = Path.Combine(_rutaImagenes, nombreArchivo);
            return File.Exists(rutaCompleta);
        }
    }
}