using Microsoft.AspNetCore.Http.Connections;

namespace IncidentLink.Models
{
    public enum ResourceStatus
    {
        Available,
        Dispatched,
        Maintenance,
        Unavailable,

    }
}
