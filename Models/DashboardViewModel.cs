namespace IncidentLink.Models
{
    public class DashboardViewModel
    { 
        public List<Incident> ActiveIncidents { get; set; } = new();
        public List<Resource> AvailableResources { get; set; } = new();
        public List<AssignmentLog> RecentAssignments { get; set; } = new();
    }
}
