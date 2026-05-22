
using System;
using System.Linq;
using IncidentLink.Models;
using IncidentLink.Data;

namespace IncidentLink.Models
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // 1. Ensure the database actually exists
            context.Database.EnsureCreated();

            // 2. Check if we already have data. If we do, exit early so we don't duplicate records.
            if (context.Incidents.Any() || context.Resources.Any())
            {
                return;
            }

            // 3. Seed Tactical Units (Resources)
            var resources = new Resource[]
            {
                new Resource { Name = "Medic 4", Type = "Ambulance", Status = ResourceStatus.Available },
                new Resource { Name = "Engine 12", Type = "Fire Engine", Status = ResourceStatus.Available },
                new Resource { Name = "Rescue Squad 7", Type = "Heavy Rescue", Status = ResourceStatus.Available },
                new Resource { Name = "Battalion Chief 1", Type = "Command Vehicle", Status = ResourceStatus.Available },
                new Resource { Name = "Hazmat 2", Type = "Specialized Unit", Status = ResourceStatus.Available }
            };

            context.Resources.AddRange(resources);

            // 4. Seed Live Emergencies (Incidents)
            // 4. Seed Live Emergencies (Incidents)
            var incidents = new Incident[]
            {
    new Incident
    {
        Title = "Commercial Structure Fire",
        Location = "Grid 14 - Industrial Park",
        Severity = "Critical", // Changed from enum to plain string
        Status = IncidentStatus.Reported,
        CreatedAt = DateTime.Now.AddMinutes(-42)
    },
    new Incident
    {
        Title = "Multi-Vehicle Collision",
        Location = "Highway 64 / Intersection 4",
        Severity = "High", // Changed from enum to plain string
        Status = IncidentStatus.Reported,
        CreatedAt = DateTime.Now.AddMinutes(-18)
    },
    new Incident
    {
        Title = "Hazardous Material Leak",
        Location = "Rail Yard Spur B",
        Severity = "Critical", // Changed from enum to plain string
        Status = IncidentStatus.Reported,
        CreatedAt = DateTime.Now.AddMinutes(-5)
    },
    new Incident
    {
        Title = "Residential Gas Odor",
        Location = "812 Maple Street",
        Severity = "Medium", // Changed from enum to plain string
        Status = IncidentStatus.Reported,
        CreatedAt = DateTime.Now.AddMinutes(-65)
    }
            };

            context.Incidents.AddRange(incidents);

            // Save everything directly to SQL Server
            context.SaveChanges();
        }
    }
}