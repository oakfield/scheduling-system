using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SchedulingSystem.Components;
using SchedulingSystem.Data;
using SchedulingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

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
