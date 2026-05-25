
using System.ComponentModel.DataAnnotations;

namespace IncidentLink.Models
{
    public class Incident
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public IncidentSeverity Severity { get; set; } = IncidentSeverity.Medium;

        public IncidentStatus Status { get; set; } = IncidentStatus.Reported;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
    }
}
