using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IncidentLink.Data;
using IncidentLink.Models;
using IncidentLink.Services;
using IncidentLink.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace IncidentLink.Controllers
{
    public class IncidentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeocodingService _geo;
        private readonly IHubContext<DispatchHub> _hubContext; // Added for live UI updates

        // FIXED: Injecting Hub Context here
        public IncidentsController(ApplicationDbContext context, GeocodingService geo, IHubContext<DispatchHub> hubContext)
        {
            _context = context;
            _geo = geo;
            _hubContext = hubContext;
        }

        // GET: Incidents
        public async Task<IActionResult> Index()
        {
            var incidents = await _context.Incidents
                .AsNoTracking()
                .ToListAsync();

            return View(incidents);
        }

        // GET: Incidents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.Incidents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incident == null)
            {
                return NotFound();
            }

            return View(incident);
        }

        // GET: Incidents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Incidents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Severity,Status,Location,CreatedAt,ResolvedAt")] Incident incident)
        {
            if (ModelState.IsValid)
            {
                var coords = await _geo.GeocodeAsync(incident.Location);

                if (coords != null)
                {
                    incident.Latitude = coords.Value.lat;
                    incident.Longitude = coords.Value.lng;
                }

                _context.Add(incident);
                await _context.SaveChangesAsync();

                // Live update the dashboard map/counters
                await _hubContext.Clients.All.SendAsync("UpdateDashboard");

                return RedirectToAction(nameof(Index));
            }

            return View(incident);
        }

        // GET: Incidents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }
            return View(incident);
        }

        // POST: Incidents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Severity,Status,Location,CreatedAt,ResolvedAt")] Incident incident)
        {
            if (id != incident.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incident);

                    // ⚡ FIXED: TRUCK AUTO-RECOVERY LOGIC ON RESOLUTION
                    if (incident.Status == IncidentStatus.Resolved)
                    {
                        // Set resolution time stamp
                        incident.ResolvedAt = DateTime.Now;

                        // Find any vehicles assigned to this specific incident
                        var activeAssignments = await _context.AssignmentLogs
                            .Include(l => l.Resource)
                            .Where(l => l.IncidentId == id)
                            .ToListAsync();

                        foreach (var log in activeAssignments)
                        {
                            if (log.Resource != null)
                            {
                                // Return the truck back to base
                                log.Resource.Status = ResourceStatus.Available;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Signal UI components to update instantly
                    await _hubContext.Clients.All.SendAsync("UpdateDashboard");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncidentExists(incident.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(incident);
        }

        // GET: Incidents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.Incidents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incident == null)
            {
                return NotFound();
            }

            return View(incident);
        }

        // POST: Incidents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident != null)
            {
               
                // If an operator obliterates an active incident, make sure the unit doesn't stay locked down forever
                var activeAssignments = await _context.AssignmentLogs
                    .Include(l => l.Resource)
                    .Where(l => l.IncidentId == id)
                    .ToListAsync();

                foreach (var log in activeAssignments)
                {
                    if (log.Resource != null)
                    {
                        log.Resource.Status = ResourceStatus.Available;
                    }
                }

                _context.Incidents.Remove(incident);
            }

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("UpdateDashboard");

            return RedirectToAction(nameof(Index));
        }

        private bool IncidentExists(int id)
        {
            return _context.Incidents.Any(e => e.Id == id);
        }
    }
}