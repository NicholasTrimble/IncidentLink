using Microsoft.EntityFrameworkCore;
using IncidentLink.Models;

namespace IncidentLink.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Resource> Resources { get; set; }
    }
}
