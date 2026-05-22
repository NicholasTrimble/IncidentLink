using IncidentLink.Data;
using IncidentLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IncidentLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                // 1. Get unresolved emergencies, newest first
                ActiveIncidents = await _context.Incidents
                    .Where(i => i.Status != IncidentStatus.Resolved)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync(),

                // 2. Get ready response crews
                AvailableResources = await _context.Resources
                    .Where(r => r.Status == ResourceStatus.Available)
                    .ToListAsync(),

                // 3. Get the last 5 dispatch actions, including entity names
                RecentAssignments = await _context.AssignmentLogs
                    .Include(a => a.Incident)
                    .Include(a => a.Resource)
                    .OrderByDescending(a => a.AssignedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}