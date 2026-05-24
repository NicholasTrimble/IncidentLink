[!Alt Text](images/incidentlink.png)

IncidentLink

Emergency dispatch coordination system for tracking incidents and assigning response resources in real time.

Overview

Built to model real-world dispatch workflows, including incident tracking, resource assignment, and operational history logging.

Features

- Real-time incident dashboard
- Resource assignment with availability validation
- Service-layer architecture for dispatch logic
- Prevention of double-booking resources
- Assignment history tracking with relational logs

Architecture

- Controllers handle UI layer only
- Business logic isolated in service layer (Dependency Injection)
- EF Core handles relational persistence and tracking

Tech Stack

- ASP.NET Core MVC
- PostgreSQL
- Entity Framework Core
- SignalR

Deployment

Deployment via Docker + Render