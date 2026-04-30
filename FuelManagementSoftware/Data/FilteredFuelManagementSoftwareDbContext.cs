using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuelManagementSoftware.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Data;

/// <summary>
/// Application DbContext used by MVC controllers/services.
/// Organisation query filtering has been removed for single-organisation mode.
/// </summary>
public class FilteredFuelManagementSoftwareDbContext : FuelManagementSoftwareDbContext
{
    private readonly string? _creatorId;
    private readonly int? _managedStationId;
    private int? _defaultOrganisationId;

    public FilteredFuelManagementSoftwareDbContext(
        DbContextOptions<FuelManagementSoftwareDbContext> options,
        string? creatorId = null,
        int? managedStationId = null)
        : base(options)
    {
        _creatorId = creatorId;
        _managedStationId = managedStationId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Override SaveChanges to ensure required metadata values are set where needed.
    /// </summary>
    public override int SaveChanges()
    {
        EnsureOrganisationId();
        EnsureCreatorId();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to ensure required metadata values are set where needed.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnsureOrganisationId();
        EnsureCreatorId();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void EnsureOrganisationId()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var defaultOrganisationId = ResolveDefaultOrganisationId();
        if (!defaultOrganisationId.HasValue)
        {
            return;
        }

        foreach (var entry in entries)
        {
            var organisationIdProperty = entry.Entity.GetType().GetProperty("OrganisationId",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (organisationIdProperty != null && organisationIdProperty.PropertyType == typeof(int))
            {
                var currentValue = (int)organisationIdProperty.GetValue(entry.Entity)!;
                if (currentValue == 0)
                {
                    organisationIdProperty.SetValue(entry.Entity, defaultOrganisationId.Value);
                }
            }
        }
    }

    private int? ResolveDefaultOrganisationId()
    {
        if (_defaultOrganisationId.HasValue)
        {
            return _defaultOrganisationId;
        }

        _defaultOrganisationId = Organizations
            .OrderBy(o => o.Id)
            .Select(o => (int?)o.Id)
            .FirstOrDefault();

        return _defaultOrganisationId;
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

