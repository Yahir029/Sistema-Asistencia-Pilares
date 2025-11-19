using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class RegistroAsistencia
    {
        public RegistroAsistencia()
        {
            Id = Guid.NewGuid();
            Fecha = DateTime.UtcNow.Date;
        }

        /// <summary>
        /// Llave primaria
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Fecha del registro (solo fecha)
        /// </summary>
        [Required]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Hora de entrada (nullable)
        /// </summary>
        public TimeSpan? HoraEntrada { get; set; }

        /// <summary>
        /// Hora de salida (nullable)
        /// </summary>
        public TimeSpan? HoraSalida { get; set; }

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
