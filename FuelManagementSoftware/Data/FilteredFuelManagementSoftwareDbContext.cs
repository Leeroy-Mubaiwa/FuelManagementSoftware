using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuelManagementSoftware.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Data;

/// <summary>
/// Filtered DbContext that automatically applies organisation_id filtering to all queries.
/// This ensures multi-tenancy by filtering all data by the current organisation.
/// </summary>
public class FilteredFuelManagementSoftwareDbContext : FuelManagementSoftwareDbContext
{
    private readonly int? _organisationId;

    public FilteredFuelManagementSoftwareDbContext(DbContextOptions<FuelManagementSoftwareDbContext> options, int? organisationId = null)
        : base(options)
    {
        _organisationId = organisationId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filters for organisation_id on all entities that have it
        // This ensures all queries are automatically filtered by organisation
        
        if (_organisationId.HasValue)
        {
            modelBuilder.Entity<AuditLog>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<BlockchainTransaction>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<CardTransaction>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<FuelPump>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<FuelStation>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<FuelStock>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<FuelTransaction>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<FuelType>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<PetroCard>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<QueueInformation>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<StationStatusHistory>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<StockMovement>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
            modelBuilder.Entity<SystemConfiguration>().HasQueryFilter(e => e.OrganisationId == _organisationId.Value);
        }
    }

    /// <summary>
    /// Override SaveChanges to ensure organisation_id is set on new entities
    /// </summary>
    public override int SaveChanges()
    {
        EnsureOrganisationId();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to ensure organisation_id is set on new entities
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnsureOrganisationId();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void EnsureOrganisationId()
    {
        if (!_organisationId.HasValue) return;

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Use reflection to set OrganisationId if the entity has it
            // Try both "OrganisationId" (property name) and check if it's an int
            var organisationIdProperty = entry.Entity.GetType().GetProperty("OrganisationId", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (organisationIdProperty != null && organisationIdProperty.PropertyType == typeof(int))
            {
                var currentValue = (int)organisationIdProperty.GetValue(entry.Entity)!;
                // Set if value is 0 (default) or if it's a new entity
                if (currentValue == 0 || entry.State == EntityState.Added)
                {
                    organisationIdProperty.SetValue(entry.Entity, _organisationId.Value);
                }
            }
        }
    }
}

