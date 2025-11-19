using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Area
    {
        // Constructor: genera un Id por defecto
        public Area()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Llave primaria
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del área
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; }
    }
}