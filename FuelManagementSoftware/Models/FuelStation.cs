using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("IsActive", Name = "IX_FuelStations_IsActive")]
[Index("IsOpen", Name = "IX_FuelStations_IsOpen")]
[Index("Latitude", "Longitude", Name = "IX_FuelStations_Location")]
[Index("CreatorId", Name = "IX_FuelStations_creator_id")]
[Index("OrganisationId", Name = "IX_FuelStations_organisation_id")]
public partial class FuelStation
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(500)]
    public string Address { get; set; } = null!;

    [StringLength(100)]
    public string? City { get; set; }

    [Column(TypeName = "decimal(10, 8)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(11, 8)")]
    public decimal? Longitude { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public bool IsOpen { get; set; }

    public bool IsTankerOffloading { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("FuelStations")]
    public virtual User Creator { get; set; } = null!;

    [InverseProperty("FuelStation")]
    public virtual ICollection<FuelPump> FuelPumps { get; set; } = new List<FuelPump>();

    [InverseProperty("FuelStation")]
    public virtual ICollection<FuelStock> FuelStocks { get; set; } = new List<FuelStock>();

    [InverseProperty("FuelStation")]
    public virtual ICollection<FuelTransaction> FuelTransactions { get; set; } = new List<FuelTransaction>();

    [ForeignKey("OrganisationId")]
    [InverseProperty("FuelStations")]
    public virtual Organization Organisation { get; set; } = null!;

    [InverseProperty("FuelStation")]
    public virtual ICollection<QueueInformation> QueueInformations { get; set; } = new List<QueueInformation>();

    [InverseProperty("FuelStation")]
    public virtual ICollection<StationStatusHistory> StationStatusHistories { get; set; } = new List<StationStatusHistory>();

    [InverseProperty("FuelStation")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
