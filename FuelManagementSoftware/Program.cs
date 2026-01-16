using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Services;
using FuelManagementSoftware.Hubs;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("FuelManagementSoftwareConnection") ?? throw new InvalidOperationException("Connection string 'FuelManagementSoftwareIdentityContextConnection' not found.");

builder.Services.AddDbContext<FuelManagementSoftwareIdentityContext>(options => options.UseSqlServer(connectionString));

// Register base DbContext (non-filtered)
builder.Services.AddDbContext<FuelManagementSoftwareDbContext>(options => options.UseSqlServer(connectionString));

// Register filtered DbContext factory
builder.Services.AddScoped<FilteredFuelManagementSoftwareDbContext>(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<DbContextOptions<FuelManagementSoftwareDbContext>>();
    var organisationContextService = serviceProvider.GetRequiredService<IOrganisationContextService>();
    var organisationId = organisationContextService.GetCurrentOrganisationIdAsync().GetAwaiter().GetResult();
    return new FilteredFuelManagementSoftwareDbContext(options, organisationId);
});

builder.Services.AddDefaultIdentity<FuelManagementSoftwareUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FuelManagementSoftwareIdentityContext>();

// Add HTTP context accessor for organisation context service
builder.Services.AddHttpContextAccessor();

// Register application services
builder.Services.AddScoped<IOrganisationContextService, OrganisationContextService>();
builder.Services.AddScoped<INfcReaderService, NfcReaderService>();
builder.Services.AddScoped<IPetroCardService, PetroCardService>();
builder.Services.AddScoped<IFuelStockService, FuelStockService>();
builder.Services.AddScoped<IPumpControlService, PumpControlService>();
builder.Services.AddScoped<IFuelDispensingService, FuelDispensingService>();

// Register SignalR
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register role seeder
builder.Services.AddScoped<RoleSeederService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<PumpStatusHub>("/hubs/pumpstatus");

// Seed roles on application startup
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeederService>();
    await roleSeeder.SeedRolesAsync();
}

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
