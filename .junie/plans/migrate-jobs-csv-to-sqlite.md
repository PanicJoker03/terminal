---
sessionId: session-261029-104026-z8wq
---

# Requirements

### Overview & Goals
Replace the jobs service’s CSV-backed storage with SQLite while preserving the existing interactive `apply`, `load`, `update/status`, and `exit` workflow.

### Scope
#### In scope
- Store job applications with fields matching the current CSV columns: timestamp, company, role, status, payment, and origin.
- Add EF Core SQLite persistence and schema management.
- Seed SQLite directly from the user's separately backed-up data.
- Preserve user-visible behavior for creating, listing, and updating applications.
- Make the SQLite file location and container persistence explicit.

#### Out of scope
- Redesigning the terminal UI or job workflow.
- Changing unrelated services such as priorities, boss, work, counter, or reality check.
- Deleting the legacy CSV automatically before the migration has been verified.

### Acceptance Criteria
- New applications are committed as SQLite rows rather than appended to CSV.
- Loading applications reads from SQLite and displays the same fields.
- Updating a selected application changes only its status and persists it across process restarts.
- The legacy CSV is not read or written by the application.
- The application can initialize its database in a fresh environment and retain it through container recreation when the data directory is mounted.

# Technical Design

### Current Implementation
- `Services/JobApplicationTracker.cs` retains the terminal prompts and renders database entities in the existing grid format.
- `JobApplicationRepository` performs create, ordered list, and stable-ID status updates through EF Core.
- `IJobApplicationTracker` remains the terminal-facing contract, and `Program.cs:11-16` registers the implementation through dependency injection.
- `TerminalApi.cs:51-54` invokes `ManageApplications`; this call path should remain unchanged.

### Key Decisions
- Use EF Core with the SQLite provider and a dedicated `DbContext`.
- Introduce a `JobApplication` entity with a generated integer key so status updates target stable records rather than CSV line numbers.
- Keep terminal prompts and output in `JobApplicationTracker`, while persistence and migration logic live outside that class.
- Run schema creation/migrations during application startup through the existing DI setup, with the database path supplied by configuration or an environment variable.
- Do not include CSV import or CSV fallback logic; the user will seed SQLite independently.

### Proposed Components
- `JobApplication` entity: `Id`, `Timestamp`, `Company`, `Role`, `Status`, `Payment`, and `Origin`.
- `TerminalDbContext`: exposes `DbSet<JobApplication>` and configures the table, required fields, indexes, and timestamp representation.
- `IJobApplicationRepository`/implementation: creates applications, returns ordered applications, and updates status by `Id`.
- `JobApplicationTracker`: injects the repository or a jobs application service, replaces file existence/read/write logic, and retains console interaction plus table rendering.
- Startup database initialization: resolve the context in `Program.cs` and apply pending migrations.

### Data Migration Rules
- Seed the SQLite database directly from the user's separately retained backup.
- Do not add CSV parsing, import markers, or CSV fallback behavior to the application.

### Deployment and Storage
- Add the EF Core SQLite package references to `src/TerminalApi/TerminalApi.csproj` and keep the SQLite file under `/var/lib/terminal-app`.
- Configure the container runtime with a volume mounted at `/var/lib/terminal-app`; otherwise the database will be lost when the container is replaced.
- Review the attached `src/TerminalApi/Dockerfile`: its entrypoint targets `terminal-app.dll`, matching the published project assembly.
- Keep database initialization independent of any legacy CSV file.

### Risks
- Concurrent terminal sessions could update the same job; use database transactions and stable IDs.
- SQLite directory permissions may prevent startup; ensure the runtime user can create and write the configured directory.
- EF migration startup failures can block the whole API; surface a clear configuration/database error rather than silently falling back to CSV.

# Testing

### Validation Approach
- Add focused tests in `src/TerminalApi/Tests/TerminalApi.Tests` using a temporary SQLite database, preferably an isolated file per test or an open in-memory connection kept alive for the test lifetime.
- Preserve the existing `NormalizeAction` coverage in `JobApplicationTrackerTests.cs`, replacing CSV-specific status-line tests with repository/entity behavior tests where appropriate.

### Key Scenarios
- Create a job and verify all six fields are persisted and can be loaded after a new context is created.
- Load jobs in insertion or timestamp order and verify the terminal-facing rows contain the expected values.
- Update one job by its database ID and verify other fields and other jobs remain unchanged.
- Initialize an empty database and verify schema creation/migrations succeed.
- Import a CSV containing quoted commas, escaped quotes, empty fields, and multiple rows, then verify equivalent SQLite values.
- Run the importer twice and verify it does not duplicate records.

### Edge Cases
- Missing CSV, empty CSV, header-only CSV, malformed row, invalid timestamp, and duplicate import marker.
- Database directory missing or not writable.
- Two status updates targeting different records.
- Container restart with `/var/lib/terminal-app` mounted and without the volume, confirming the documented durability difference.

# Delivery Steps

### ✓ Step 1: Define EF Core jobs persistence
The jobs domain has a typed SQLite persistence model and schema configuration.
- Add EF Core SQLite dependencies to `src/TerminalApi/TerminalApi.csproj`.
- Add a `JobApplication` entity and `TerminalDbContext` under the existing `Services` or a focused persistence namespace.
- Configure the six CSV-derived fields, generated IDs, required constraints, ordering/indexes, and the database path from application configuration.
- Create the initial EF migration for the jobs table.

### ✓ Step 2: Move job workflows to database operations
The interactive jobs workflow reads and writes applications through EF Core instead of the CSV file.
- Add a repository or jobs data service for create, list, and update-status operations.
- Register the persistence components in `Program.cs` and inject them into `JobApplicationTracker` without changing `IJobApplicationTracker` or `TerminalApi` command routing.
- Replace file-based logic in `JobApplicationTracker.cs` while preserving prompts, empty-state messages, ordering, and ASCII rendering.
- Use stable database IDs for status selection rather than CSV line indexes.

### x Step 3: Import legacy CSV data safely
Skipped because the user will seed SQLite directly from an existing backup; the application contains no CSV import or fallback path.

### ✓ Step 4: Integrate startup and container persistence
A fresh deployment initializes the database and a configured container volume preserves it.
- Apply pending EF migrations during application startup using the existing DI pipeline.
- Add configuration for the SQLite file path and document the `/var/lib/terminal-app` volume requirement.
- Review the attached `src/TerminalApi/Dockerfile` entrypoint against the actual published assembly and update deployment configuration only if needed.
- Ensure runtime directory creation and permissions are handled before database initialization.

### ✓ Step 5: Verify migration and regressions
Automated checks demonstrate that SQLite behavior matches the prior jobs workflow.
- Add repository, migration, and status-update tests under `src/TerminalApi/Tests/TerminalApi.Tests`.
- Test fresh database initialization and seeded row preservation.
- Run the full Release build and test suite, then perform a container-level persistence check with and without the data volume.