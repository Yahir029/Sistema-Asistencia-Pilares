using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Rol
    {
        // Constructor: genera un Id por defecto
        public Rol()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Llave primaria
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del rol
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; }
    }
}
