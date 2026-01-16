using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("NormalizedEmail", Name = "EmailIndex")]
public partial class User
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(256)]
    public string? UserName { get; set; }

    [StringLength(256)]
    public string? NormalizedUserName { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(256)]
    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    [InverseProperty("Creator")]
    public virtual ICollection<AuditLog> AuditLogCreators { get; set; } = new List<AuditLog>();

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogUsers { get; set; } = new List<AuditLog>();

    [InverseProperty("Creator")]
    public virtual ICollection<BlockchainTransaction> BlockchainTransactions { get; set; } = new List<BlockchainTransaction>();

    [InverseProperty("Creator")]
    public virtual ICollection<CardTransaction> CardTransactions { get; set; } = new List<CardTransaction>();

    [InverseProperty("Creator")]
    public virtual ICollection<FuelPump> FuelPumps { get; set; } = new List<FuelPump>();

    [InverseProperty("Creator")]
    public virtual ICollection<FuelStation> FuelStations { get; set; } = new List<FuelStation>();

    [InverseProperty("Creator")]
    public virtual ICollection<FuelStock> FuelStocks { get; set; } = new List<FuelStock>();

    [InverseProperty("Creator")]
    public virtual ICollection<FuelTransaction> FuelTransactionCreators { get; set; } = new List<FuelTransaction>();

    [InverseProperty("User")]
    public virtual ICollection<FuelTransaction> FuelTransactionUsers { get; set; } = new List<FuelTransaction>();

    [InverseProperty("Creator")]
    public virtual ICollection<FuelType> FuelTypes { get; set; } = new List<FuelType>();

    [InverseProperty("Creator")]
    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    [InverseProperty("Creator")]
    public virtual ICollection<PetroCard> PetroCardCreators { get; set; } = new List<PetroCard>();

    [InverseProperty("User")]
    public virtual ICollection<PetroCard> PetroCardUsers { get; set; } = new List<PetroCard>();

    [InverseProperty("Creator")]
    public virtual ICollection<QueueInformation> QueueInformations { get; set; } = new List<QueueInformation>();

    [InverseProperty("Creator")]
    public virtual ICollection<StationStatusHistory> StationStatusHistories { get; set; } = new List<StationStatusHistory>();

    [InverseProperty("Creator")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    [InverseProperty("Creator")]
    public virtual ICollection<SystemConfiguration> SystemConfigurations { get; set; } = new List<SystemConfiguration>();

    [InverseProperty("User")]
    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    [InverseProperty("User")]
    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    [InverseProperty("User")]
    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
