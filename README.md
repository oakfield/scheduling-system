# scheduling-system
A scheduling system for academic staff to assign students to courses for an upcoming semester.

## Running with Docker

```bash
docker compose up --build
```

Then browse to http://localhost:8080.

This currently runs just the web app in a container (Interactive Server Blazor). A Postgres
service will be added to `docker-compose.yml` once the data layer is in place.
