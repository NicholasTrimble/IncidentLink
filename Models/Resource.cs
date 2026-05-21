using System.ComponentModel.DataAnnotations;

namespace IncidentLink.Models
{
    public class Resource
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        public ResourceStatus Status { get; set; } = ResourceStatus.Available;

        [StringLength(200)]
        public string CurrentLocation { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }
}
