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
/// Uses a property for the filter value so the cached model never evaluates .Value on null.
/// </summary>
public class FilteredFuelManagementSoftwareDbContext : FuelManagementSoftwareDbContext
{
    private readonly int? _organisationId;
    private readonly string? _creatorId;

    /// <summary>Value used in query filters. When organisation is null, use -1 so no rows match (avoids Nullable.Value exception with cached model).</summary>
    private int _organisationIdForFilter => _organisationId ?? -1;

    public FilteredFuelManagementSoftwareDbContext(DbContextOptions<FuelManagementSoftwareDbContext> options, int? organisationId = null, string? creatorId = null)
        : base(options)
    {
        _organisationId = organisationId;
        _creatorId = creatorId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filters using a property so we never use .Value on null (model is cached per context type).
        modelBuilder.Entity<AuditLog>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<BlockchainTransaction>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<CardTransaction>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<FuelPump>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<FuelStation>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<FuelStock>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<FuelTransaction>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<PetroCard>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<QueueInformation>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<StationStatusHistory>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<StockMovement>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
        modelBuilder.Entity<SystemConfiguration>().HasQueryFilter(e => e.OrganisationId == _organisationIdForFilter);
    }

    /// <summary>
    /// Override SaveChanges to ensure organisation_id and creator_id are set where needed.
    /// </summary>
    public override int SaveChanges()
    {
        EnsureOrganisationId();
        EnsureCreatorId();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to ensure organisation_id and creator_id are set where needed.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnsureOrganisationId();
        EnsureCreatorId();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void EnsureOrganisationId()
    {
        if (!_organisationId.HasValue) return;

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var organisationIdProperty = entry.Entity.GetType().GetProperty("OrganisationId",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (organisationIdProperty != null && organisationIdProperty.PropertyType == typeof(int))
            {
                var currentValue = (int)organisationIdProperty.GetValue(entry.Entity)!;
                if (currentValue == 0 || entry.State == EntityState.Added)
                {
                    organisationIdProperty.SetValue(entry.Entity, _organisationId.Value);
                }
            }
        }
    }

    private void EnsureCreatorId()
    {
        if (string.IsNullOrEmpty(_creatorId)) return;

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var creatorIdProperty = entry.Entity.GetType().GetProperty("CreatorId",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (creatorIdProperty != null && creatorIdProperty.PropertyType == typeof(string))
            {
                var currentValue = (string?)creatorIdProperty.GetValue(entry.Entity);
                if (string.IsNullOrEmpty(currentValue))
                {
                    creatorIdProperty.SetValue(entry.Entity, _creatorId);
                }
            }
        }
    }
}

