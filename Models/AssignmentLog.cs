using System.ComponentModel.DataAnnotations;

namespace IncidentLink.Models
{
    public class AssignmentLog
    {
        public int Id { get; set; }
        [Required]
        public int IncidentId { get; set; }
        public Incident? Incident { get; set; }

        [Required]
        public int ResourceId { get; set; }
        public Resource? Resource { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReleasedAt { get; set; }
    }
}
