using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("FuelStationId", Name = "IX_StockMovements_FuelStationId")]
[Index("FuelTypeId", Name = "IX_StockMovements_FuelTypeId")]
[Index("MovementDate", Name = "IX_StockMovements_MovementDate")]
[Index("MovementType", Name = "IX_StockMovements_MovementType")]
[Index("CreatorId", Name = "IX_StockMovements_creator_id")]
[Index("OrganisationId", Name = "IX_StockMovements_organisation_id")]
public partial class StockMovement
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    public int FuelStationId { get; set; }

    public int FuelTypeId { get; set; }

    [StringLength(50)]
    public string MovementType { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Quantity { get; set; }

    [StringLength(20)]
    public string Unit { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal StockBefore { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal StockAfter { get; set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    [StringLength(100)]
    public string? DeliveryNoteNumber { get; set; }

    [StringLength(50)]
    public string? TankerRegistration { get; set; }

    [StringLength(255)]
    public string? DriverName { get; set; }

    public DateTime MovementDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("StockMovements")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("FuelStationId")]
    [InverseProperty("StockMovements")]
    public virtual FuelStation FuelStation { get; set; } = null!;

    [ForeignKey("FuelTypeId")]
    [InverseProperty("StockMovements")]
    public virtual FuelType FuelType { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("StockMovements")]
    public virtual Organization Organisation { get; set; } = null!;
}
