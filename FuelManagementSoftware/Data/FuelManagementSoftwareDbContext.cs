using System;
using System.Collections.Generic;
using FuelManagementSoftware.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Data;

public partial class FuelManagementSoftwareDbContext : DbContext
{
    public FuelManagementSoftwareDbContext()
    {
    }

    public FuelManagementSoftwareDbContext(DbContextOptions<FuelManagementSoftwareDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BlockchainTransaction> BlockchainTransactions { get; set; }

    public virtual DbSet<CardTransaction> CardTransactions { get; set; }

    public virtual DbSet<FuelPump> FuelPumps { get; set; }

    public virtual DbSet<FuelStation> FuelStations { get; set; }

    public virtual DbSet<FuelStock> FuelStocks { get; set; }

    public virtual DbSet<FuelTransaction> FuelTransactions { get; set; }

    public virtual DbSet<FuelType> FuelTypes { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<PetroCard> PetroCards { get; set; }

    public virtual DbSet<QueueInformation> QueueInformations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<StationStatusHistory> StationStatusHistories { get; set; }

    public virtual DbSet<StockMovement> StockMovements { get; set; }

    public virtual DbSet<SystemConfiguration> SystemConfigurations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("name=FuelManagementSoftwareConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC0766CE7448");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");

            entity.HasOne(d => d.Creator).WithMany(p => p.AuditLogCreators)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLogs_Creator");

            entity.HasOne(d => d.Organisation).WithMany(p => p.AuditLogs).HasConstraintName("FK_AuditLogs_Organizations");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogUsers).HasConstraintName("FK_AuditLogs_Users");
        });

        modelBuilder.Entity<BlockchainTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Blockcha__3214EC0703A4D565");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.Creator).WithMany(p => p.BlockchainTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BlockchainTransactions_Users");

            entity.HasOne(d => d.FuelTransaction).WithMany(p => p.BlockchainTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BlockchainTransactions_FuelTransactions");

            entity.HasOne(d => d.Organisation).WithMany(p => p.BlockchainTransactions).HasConstraintName("FK_BlockchainTransactions_Organizations");
        });

        modelBuilder.Entity<CardTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CardTran__3214EC07015D32F2");

            entity.Property(e => e.Currency).HasDefaultValue("USD");
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("datetime('now', 'utc')");

            entity.HasOne(d => d.Creator).WithMany(p => p.CardTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CardTransactions_Users");

            entity.HasOne(d => d.Organisation).WithMany(p => p.CardTransactions).HasConstraintName("FK_CardTransactions_Organizations");

            entity.HasOne(d => d.PetroCard).WithMany(p => p.CardTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CardTransactions_PetroCards");
        });

        modelBuilder.Entity<FuelPump>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FuelPump__3214EC0766109EC3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsOperational).HasDefaultValue(true);

            entity.HasOne(d => d.Creator).WithMany(p => p.FuelPumps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelPumps_Users");

            entity.HasOne(d => d.FuelStation).WithMany(p => p.FuelPumps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelPumps_FuelStations");

            entity.HasOne(d => d.FuelType).WithMany(p => p.FuelPumps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelPumps_FuelTypes");

            entity.HasOne(d => d.Organisation).WithMany(p => p.FuelPumps).HasConstraintName("FK_FuelPumps_Organizations");
        });

        modelBuilder.Entity<FuelStation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FuelStat__3214EC07AA22693D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsOpen).HasDefaultValue(true);

            entity.HasOne(d => d.Creator).WithMany(p => p.FuelStations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelStations_Users");

            entity.HasOne(d => d.Organisation).WithMany(p => p.FuelStations).HasConstraintName("FK_FuelStations_Organizations");
        });

        modelBuilder.Entity<FuelStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FuelStoc__3214EC07B8E88608");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.Unit).HasDefaultValue("Litre");

            entity.HasOne(d => d.Creator).WithMany(p => p.FuelStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelStock_Users");

            entity.HasOne(d => d.FuelStation).WithMany(p => p.FuelStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelStock_FuelStations");

            entity.HasOne(d => d.FuelType).WithMany(p => p.FuelStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelStock_FuelTypes");

            entity.HasOne(d => d.Organisation).WithMany(p => p.FuelStocks).HasConstraintName("FK_FuelStock_Organizations");
        });

        modelBuilder.Entity<FuelTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FuelTran__3214EC0768AD478D");

            entity.Property(e => e.Currency).HasDefaultValue("USD");
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.TransactionStatus).HasDefaultValue("Completed");

            entity.HasOne(d => d.Creator).WithMany(p => p.FuelTransactionCreators)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelTransactions_Creator");

            entity.HasOne(d => d.FuelPump).WithMany(p => p.FuelTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelTransactions_FuelPumps");

            entity.HasOne(d => d.FuelStation).WithMany(p => p.FuelTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelTransactions_FuelStations");

            entity.HasOne(d => d.FuelType).WithMany(p => p.FuelTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelTransactions_FuelTypes");

            entity.HasOne(d => d.Organisation).WithMany(p => p.FuelTransactions).HasConstraintName("FK_FuelTransactions_Organizations");

            entity.HasOne(d => d.PetroCard).WithMany(p => p.FuelTransactions).HasConstraintName("FK_FuelTransactions_PetroCards");

            entity.HasOne(d => d.User).WithMany(p => p.FuelTransactionUsers).HasConstraintName("FK_FuelTransactions_Users");
        });

        modelBuilder.Entity<FuelType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FuelType__3214EC0785D74B7C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Unit).HasDefaultValue("Litre");

            entity.HasOne(d => d.Creator).WithMany(p => p.FuelTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelTypes_Users");

            entity.HasOne(d => d.Organisation).WithMany(p => p.FuelTypes).HasConstraintName("FK_FuelTypes_Organizations");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Organiza__3214EC0701075EE5");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Creator).WithMany(p => p.Organizations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Organizations_Users");
        });

        modelBuilder.Entity<PetroCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetroCar__3214EC0781ADAEA4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.Currency).HasDefaultValue("USD");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Creator).WithMany(p => p.PetroCardCreators)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PetroCards_Creator");

            entity.HasOne(d => d.Organisation).WithMany(p => p.PetroCards).HasConstraintName("FK_PetroCards_Organizations");

            entity.HasOne(d => d.User).WithMany(p => p.PetroCardUsers).HasConstraintName("FK_PetroCards_Users");
        });

        modelBuilder.Entity<QueueInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QueueInf__3214EC07222CE06D");

            entity.Property(e => e.RecordedAt).HasDefaultValueSql("datetime('now', 'utc')");

            entity.HasOne(d => d.Creator).WithMany(p => p.QueueInformations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QueueInformation_Users");

            entity.HasOne(d => d.FuelStation).WithMany(p => p.QueueInformations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QueueInformation_FuelStations");

            entity.HasOne(d => d.Organisation).WithMany(p => p.QueueInformations).HasConstraintName("FK_QueueInformation_Organizations");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        });

        modelBuilder.Entity<StationStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StationS__3214EC07C6F93A9A");

            entity.Property(e => e.StatusChangedAt).HasDefaultValueSql("datetime('now', 'utc')");

            entity.HasOne(d => d.Creator).WithMany(p => p.StationStatusHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StationStatusHistory_Users");

            entity.HasOne(d => d.FuelStation).WithMany(p => p.StationStatusHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StationStatusHistory_FuelStations");

            entity.HasOne(d => d.Organisation).WithMany(p => p.StationStatusHistories).HasConstraintName("FK_StationStatusHistory_Organizations");
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockMov__3214EC077571867C");

            entity.Property(e => e.MovementDate).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.Unit).HasDefaultValue("Litre");

            entity.HasOne(d => d.Creator).WithMany(p => p.StockMovements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_Users");

            entity.HasOne(d => d.FuelStation).WithMany(p => p.StockMovements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_FuelStations");

            entity.HasOne(d => d.FuelType).WithMany(p => p.StockMovements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_FuelTypes");

            entity.HasOne(d => d.Organisation).WithMany(p => p.StockMovements).HasConstraintName("FK_StockMovements_Organizations");
        });

        modelBuilder.Entity<SystemConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemCo__3214EC07E66E31AD");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now', 'utc')");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Creator).WithMany(p => p.SystemConfigurations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemConfigurations_Users");

            entity.HasOne(d => d.Organisation).WithMany(p => p.SystemConfigurations).HasConstraintName("FK_SystemConfigurations_Organizations");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_UserRoles_RoleId");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
