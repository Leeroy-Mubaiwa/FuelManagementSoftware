using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("Code", Name = "IX_Organizations_Code")]
[Index("CreatorId", Name = "IX_Organizations_creator_id")]
public partial class Organization
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [InverseProperty("Organisation")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("Organisation")]
    public virtual ICollection<BlockchainTransaction> BlockchainTransactions { get; set; } = new List<BlockchainTransaction>();

    [InverseProperty("Organisation")]
    public virtual ICollection<CardTransaction> CardTransactions { get; set; } = new List<CardTransaction>();

    [ForeignKey("CreatorId")]
    [InverseProperty("Organizations")]
    public virtual User Creator { get; set; } = null!;

    [InverseProperty("Organisation")]
    public virtual ICollection<FuelPump> FuelPumps { get; set; } = new List<FuelPump>();

    [InverseProperty("Organisation")]
    public virtual ICollection<FuelStation> FuelStations { get; set; } = new List<FuelStation>();

    [InverseProperty("Organisation")]
    public virtual ICollection<FuelStock> FuelStocks { get; set; } = new List<FuelStock>();

    [InverseProperty("Organisation")]
    public virtual ICollection<FuelTransaction> FuelTransactions { get; set; } = new List<FuelTransaction>();

    [InverseProperty("Organisation")]
    public virtual ICollection<FuelType> FuelTypes { get; set; } = new List<FuelType>();

    [InverseProperty("Organisation")]
    public virtual ICollection<PetroCard> PetroCards { get; set; } = new List<PetroCard>();

    [InverseProperty("Organisation")]
    public virtual ICollection<QueueInformation> QueueInformations { get; set; } = new List<QueueInformation>();

    [InverseProperty("Organisation")]
    public virtual ICollection<StationStatusHistory> StationStatusHistories { get; set; } = new List<StationStatusHistory>();

    [InverseProperty("Organisation")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    [InverseProperty("Organisation")]
    public virtual ICollection<SystemConfiguration> SystemConfigurations { get; set; } = new List<SystemConfiguration>();
}
