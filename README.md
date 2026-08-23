# scheduling-system
A scheduling system for academic staff to assign students to courses for an upcoming semester.

## Running with Docker

```bash
docker compose up --build
```

Then browse to http://localhost:8080. This starts Postgres and the web app together; on
first run the app applies EF Core migrations and seeds the database from the sample CSVs
under `SchedulingSystem/SchedulingSystem/Data/SeedData/`.

## Running with `dotnet run` (outside Docker)

```bash
cd SchedulingSystem/SchedulingSystem
dotnet run
```

This runs the app itself directly on the host, but it still needs Postgres, so on startup
`Program.cs` runs `docker compose up -d --wait postgres` on your behalf — creating and
starting that container (and its volume) if it doesn't already exist, or just confirming
it's healthy if it does — before touching the database. Requires Docker Desktop to be
installed and running; if it isn't, `dotnet run` fails fast with a clear message rather
than the opaque Postgres connection error you'd otherwise get. (The containerized app
skips this step itself — see `DOTNET_RUNNING_IN_CONTAINER` in `Program.cs` — since
`docker-compose.yml`'s own `depends_on`/healthcheck already sequences that case.)

`appsettings.json` points at Postgres on `localhost:5433` for this scenario (mapped from
the container's standard 5432 — deliberately not 5432 on the host, in case a machine
already has its own local Postgres bound there; see the comment in `docker-compose.yml`).
The containerized app instead gets its connection string from an environment variable in
`docker-compose.yml` (pointing at the `postgres` service by container name on its
internal 5432), so no code change is needed to move between the two.

If `dotnet run` fails with a Postgres password/auth error rather than the Docker-related
error above, something else on this machine is answering on the port `appsettings.json`
points at (5433) — check `docker ps` for what's actually listening there.
