using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class HorarioLaboral
    {
        // Constructor: genera un Id por defecto
        public HorarioLaboral()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Llave primaria
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Día de la semana
        /// </summary>
        [Required]
        public DayOfWeek Dia { get; set; }

        /// <summary>
        /// Hora de inicio (nullable)
        /// </summary>
        public TimeSpan? HoraInicio { get; set; }

        /// <summary>
        /// Hora de fin (nullable)
        /// </summary>
        public TimeSpan? HoraFin { get; set; }

        /// <summary>
        /// Llave foránea al empleado
        /// </summary>
        [Required]
        public Guid EmpleadoId { get; set; }

        /// <summary>
        /// Propiedad de navegación al empleado
        /// </summary>
        [ForeignKey(nameof(EmpleadoId))]
        public Empleado Empleado { get; set; }
    }
}
