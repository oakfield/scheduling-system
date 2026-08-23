using System.Diagnostics;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SchedulingSystem.Components;
using SchedulingSystem.Data;
using SchedulingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// `dotnet run` runs this app directly on the host, so unlike the containerized app (which
// already sequences this via docker-compose.yml's depends_on/healthcheck) it still needs
// its own Postgres brought up first. DOTNET_RUNNING_IN_CONTAINER is set by the official
// .NET container base images, so this only runs for the host-run scenario.
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    EnsurePostgresContainerRunning(builder.Environment.ContentRootPath);
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("SchedulingDb")
    ?? throw new InvalidOperationException("Missing connection string 'SchedulingDb'.");

// A factory (rather than a directly-injected DbContext) is used because Blazor Server
// components are long-lived per circuit; each service call creates and disposes its own
// short-lived context instead of sharing one that isn't thread-safe.
builder.Services.AddDbContextFactory<SchedulingDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// Without this, keys live only in the container's writable layer: every container
// recreate (a rebuild, a redeploy) invalidates every open browser tab's antiforgery
// cookie, breaking their Blazor Server circuit until they reload. The docker-compose
// volume below makes the directory survive recreation; the local dev folder does not
// need to (see .gitignore) since a local process restart already loses in-memory state.
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
    .SetApplicationName("SchedulingSystem");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<SchedulingDbContext>>();
    await using var db = await contextFactory.CreateDbContextAsync();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Starts (or confirms) the Postgres container via docker-compose.yml at the repo root,
// blocking until it reports healthy so the app never races its startup. Idempotent - a
// no-op if the container is already running.
static void EnsurePostgresContainerRunning(string contentRootPath)
{
    var repoRoot = Path.GetFullPath(Path.Combine(contentRootPath, "..", ".."));

    Process process;
    try
    {
        process = Process.Start(new ProcessStartInfo("docker", "compose up -d --wait postgres")
        {
            WorkingDirectory = repoRoot,
            UseShellExecute = false,
        }) ?? throw new InvalidOperationException("The docker process did not start.");
    }
    catch (Exception ex) when (ex is not InvalidOperationException)
    {
        throw new InvalidOperationException(
            "Could not run 'docker compose up -d --wait postgres'. Is Docker Desktop installed and running?",
            ex);
    }

    process.WaitForExit();
    if (process.ExitCode != 0)
    {
        throw new InvalidOperationException(
            $"'docker compose up -d --wait postgres' exited with code {process.ExitCode}. " +
            "Make sure Docker Desktop is running, then try again.");
    }
}
