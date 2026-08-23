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
