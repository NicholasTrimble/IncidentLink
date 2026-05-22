using IncidentLink.Data;
using IncidentLink.Models;
using IncidentLink.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IncidentLink.Controllers
{
    public class DispatchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDispatchService _dispatchService;

        public DispatchController(ApplicationDbContext context, IDispatchService dispatchService)
        {
            _context = context;
            _dispatchService = dispatchService;
        }

        // GET: Dispatch/Create
        public async Task<IActionResult> Create()
        {
            // Fetch only unresolved incidents
            var activeIncidents = await _context.Incidents
                .Where(i => i.Status != IncidentStatus.Resolved)
                .ToListAsync();

            // Fetch only available resources
            var availableResources = await _context.Resources
                .Where(r => r.Status == ResourceStatus.Available)
                .ToListAsync();

            // Load them into dropdown selection lists for the View
            ViewData["IncidentId"] = new SelectList(activeIncidents, "Id", "Title");
            ViewData["ResourceId"] = new SelectList(availableResources, "Id", "Name");

            return View();
        }

        // POST: Dispatch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int incidentId, int resourceId)
        {
            // Execute our strict custom business logic service
            bool success = await _dispatchService.AssignResourceToIncidentAsync(incidentId, resourceId);

            if (success)
            {
                // Send them over to the Incidents manager to see the updated status
                return RedirectToAction("Index", "Incidents");
            }

            // If it fails (e.g., resource was snatched up by another operator), reload the screen with an error
            ModelState.AddModelError("", "Unable to process dispatch assignment. Verify resource is still available.");

            return await Create();
        }
    }
}