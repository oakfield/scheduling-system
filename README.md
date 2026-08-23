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

Start just the database, then run the app normally:

```bash
docker compose up postgres -d
cd SchedulingSystem/SchedulingSystem
dotnet run
```

`appsettings.json` points at Postgres on `localhost:5433` for this scenario (mapped from
the container's standard 5432 — deliberately not 5432 on the host, in case a machine
already has its own local Postgres bound there; see the comment in `docker-compose.yml`).
The containerized app instead gets its connection string from an environment variable in
`docker-compose.yml` (pointing at the `postgres` service by container name on its
internal 5432), so no code change is needed to move between the two.

If `dotnet run` fails with a Postgres password/auth error, it's almost always this: the
Docker Postgres container isn't running (`docker compose up postgres -d` first), or
something else on this machine is answering on the port `appsettings.json` points at.
