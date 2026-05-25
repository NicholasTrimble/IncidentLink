using IncidentLink.Data;
using IncidentLink.Hubs;
using IncidentLink.Models;
using Microsoft.AspNetCore.SignalR;

namespace IncidentLink.Services
{
    public class DispatchService : IDispatchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<DispatchHub> _hubContext;

        
        public DispatchService(ApplicationDbContext context, IHubContext<DispatchHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext; 
        }

        public async Task<bool> AssignResourceToIncidentAsync(int incidentId, int resourceId)
        {
            var incident = await _context.Incidents.FindAsync(incidentId);
            var resource = await _context.Resources.FindAsync(resourceId);

            // Verify both entities actually exist in the database
            if (incident == null || resource == null)
            {
                return false;
            }

            // Prevent double-booking a resource
            if (resource.Status != ResourceStatus.Available)
            {
                return false;
            }

            // Enforce state transitions
            resource.Status = ResourceStatus.Dispatched;
            incident.Status = IncidentStatus.Assigned;

            // Log the tracking history record
            var log = new AssignmentLog
            {
                IncidentId = incidentId,
                ResourceId = resourceId
            };

            _context.AssignmentLogs.Add(log);

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("UpdateDashboard");

            return true;
        }
    }
}