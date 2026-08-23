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

`appsettings.json` points at Postgres on `localhost:5432` for this scenario. The
containerized app instead gets its connection string from an environment variable in
`docker-compose.yml` (pointing at the `postgres` service by container name), so no code
change is needed to move between the two.
