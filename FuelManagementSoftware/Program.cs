using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Services;
using FuelManagementSoftware.Hubs;
using FuelManagementSoftware.Jobs;
using Quartz;
using Quartz.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = $"Data Source={Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "fuelmanagement.db")}";

builder.Services.AddDbContext<FuelManagementSoftwareIdentityContext>(options => options.UseSqlite(connectionString));

// Register base DbContext (non-filtered)
builder.Services.AddDbContext<FuelManagementSoftwareDbContext>(options => options.UseSqlite(connectionString));

// Register filtered DbContext factory
builder.Services.AddScoped<FilteredFuelManagementSoftwareDbContext>(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<DbContextOptions<FuelManagementSoftwareDbContext>>();
    string? creatorId = null;
    int? managedStationId = null;
    var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
    if (httpContext?.User != null)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<FuelManagementSoftwareUser>>();
        var user = userManager.GetUserAsync(httpContext.User).GetAwaiter().GetResult();
        creatorId = user?.Id;
        managedStationId = user?.ManagedStationId;
    }
    return new FilteredFuelManagementSoftwareDbContext(options, creatorId, managedStationId);
});

builder.Services.AddDefaultIdentity<FuelManagementSoftwareUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FuelManagementSoftwareIdentityContext>();

// Add HTTP context accessor for request-scoped metadata
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Register blockchain configuration
builder.Services.Configure<BlockchainSettings>(builder.Configuration.GetSection("Blockchain"));
builder.Services.AddSingleton<IBlockchainService, BlockchainService>();

// Register application services
builder.Services.AddScoped<IOrganisationContextService, OrganisationContextService>();
builder.Services.AddScoped<INfcReaderService, NfcReaderService>();
builder.Services.AddScoped<IPetroCardService, PetroCardService>();
builder.Services.AddScoped<IFuelStockService, FuelStockService>();
builder.Services.AddScoped<IPumpControlService, PumpControlService>();
builder.Services.AddScoped<IFuelDispensingService, FuelDispensingService>();

// Register SignalR
builder.Services.AddSignalR();

// Register Quartz
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});
builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register role seeder
builder.Services.AddScoped<RoleSeederService>();
builder.Services.AddScoped<OrganizationSeederService>();
builder.Services.AddScoped<DefaultUserSeederService>();
builder.Services.AddScoped<FuelTypeSeederService>();

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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FuelManagementSoftwareDbContext>();
    await ApplyMakeFuelTypeGlobalIfNeededAsync(db);
    await db.Database.MigrateAsync();
    
    var orgSeeder = scope.ServiceProvider.GetRequiredService<OrganizationSeederService>();
    await orgSeeder.SeedOrganizationAsync();

    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeederService>();
    await roleSeeder.SeedRolesAsync();
    
    var userSeeder = scope.ServiceProvider.GetRequiredService<DefaultUserSeederService>();
    await userSeeder.SeedUsersAsync();

    var fuelTypeSeeder = scope.ServiceProvider.GetRequiredService<FuelTypeSeederService>();
    await fuelTypeSeeder.SeedFuelTypesAsync();
}

// Deploy blockchain smart contract if not yet deployed
try
{
    var blockchainService = app.Services.GetRequiredService<IBlockchainService>();
    if (blockchainService.IsConfigured())
    {
        var contractAddress = await blockchainService.EnsureContractDeployedAsync();
        if (!string.IsNullOrWhiteSpace(contractAddress))
        {
            app.Logger.LogInformation("PetroChain smart contract ready at: {Address}", contractAddress);
        }
    }
    else
    {
        app.Logger.LogWarning("Blockchain not configured. Fuel transactions will not be recorded on-chain.");
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Failed to deploy blockchain contract. App will continue without blockchain recording.");
}

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task ApplyMakeFuelTypeGlobalIfNeededAsync(FuelManagementSoftwareDbContext db)
{
    const string migrationId = "20260219120000_MakeFuelTypeGlobal";
    var hasOldColumn = await db.Database.SqlQueryRaw<string>(
        "SELECT name FROM pragma_table_info('FuelTypes') WHERE name = 'organisation_id' LIMIT 1").ToListAsync();
    if (hasOldColumn.Count == 0) return;

    var conn = db.Database.GetDbConnection();
    await conn.OpenAsync();
    try
    {
        async Task Exec(string sql)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }
        await Exec("PRAGMA foreign_keys = 0;");
        await Exec(@"
            CREATE TABLE FuelTypes_new (
                Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Code TEXT NULL,
                Description TEXT NULL,
                UnitPrice REAL NOT NULL,
                Unit TEXT NOT NULL DEFAULT 'Litre',
                IsActive INTEGER NOT NULL DEFAULT 1,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'utc')),
                UpdatedAt TEXT NULL,
                creator_id TEXT NULL,
                FOREIGN KEY (creator_id) REFERENCES Users(Id) ON DELETE SET NULL
            );");
        await Exec(@"
            INSERT INTO FuelTypes_new (Id, Name, Code, Description, UnitPrice, Unit, IsActive, CreatedAt, UpdatedAt, creator_id)
            SELECT Id, Name, Code, Description, UnitPrice, Unit, IsActive, CreatedAt, UpdatedAt, creator_id FROM FuelTypes;");
        await Exec("DROP TABLE FuelTypes;");
        await Exec("ALTER TABLE FuelTypes_new RENAME TO FuelTypes;");
        await Exec("CREATE INDEX IX_FuelTypes_creator_id ON FuelTypes(creator_id);");
        await Exec("PRAGMA foreign_keys = 1;");
        await db.Database.ExecuteSqlRawAsync(
            "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ({0}, '8.0.23');", migrationId);
    }
    finally
    {
        await conn.CloseAsync();
    }
}
