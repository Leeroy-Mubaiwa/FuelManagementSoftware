using FuelManagementSoftware.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Data;

public class FuelManagementSoftwareIdentityContext : IdentityDbContext<FuelManagementSoftwareUser>
{
    public FuelManagementSoftwareIdentityContext(DbContextOptions<FuelManagementSoftwareIdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure the user entity to use FuelManagementSoftwareUser
        builder.Entity<FuelManagementSoftwareUser>(entity =>
        {
            entity.ToTable("Users"); // Remove AspNet prefix
        });
        
        // Remove AspNet prefix from all other Identity table names
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6)); // Remove "AspNet" prefix (6 characters)
            }
        }
    }
}
