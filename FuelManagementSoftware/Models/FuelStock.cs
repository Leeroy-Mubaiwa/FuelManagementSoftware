using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Table("FuelStock")]
[Index("FuelStationId", Name = "IX_FuelStock_FuelStationId")]
[Index("FuelTypeId", Name = "IX_FuelStock_FuelTypeId")]
[Index("LastUpdated", Name = "IX_FuelStock_LastUpdated")]
[Index("CreatorId", Name = "IX_FuelStock_creator_id")]
[Index("OrganisationId", Name = "IX_FuelStock_organisation_id")]
[Index("FuelStationId", "FuelTypeId", Name = "UQ_FuelStock_Station_FuelType", IsUnique = true)]
public partial class FuelStock
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    public int FuelStationId { get; set; }

    public int FuelTypeId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CurrentQuantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Capacity { get; set; }

    [StringLength(20)]
    public string Unit { get; set; } = null!;

    public DateTime LastUpdated { get; set; }

    public bool IsLowStock { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? LowStockThreshold { get; set; }

    public DateTime CreatedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("FuelStocks")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("FuelStationId")]
    [InverseProperty("FuelStocks")]
    public virtual FuelStation FuelStation { get; set; } = null!;

    [ForeignKey("FuelTypeId")]
    [InverseProperty("FuelStocks")]
    public virtual FuelType FuelType { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("FuelStocks")]
    public virtual Organization Organisation { get; set; } = null!;
}
