namespace IncidentLink.Services
{
    public interface IDispatchService
    {
        Task<bool> AssignResourceToIncidentAsync(int incidentId, int resourceId);
    }
}
