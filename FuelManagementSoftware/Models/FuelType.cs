using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("CreatorId", Name = "IX_FuelTypes_creator_id")]
public partial class FuelType
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [StringLength(20)]
    public string Unit { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Column("creator_id")]
    public string? CreatorId { get; set; }

    [ForeignKey("CreatorId")]
    [InverseProperty("FuelTypes")]
    public virtual User? Creator { get; set; }

    [InverseProperty("FuelType")]
    public virtual ICollection<FuelPump> FuelPumps { get; set; } = new List<FuelPump>();

    [InverseProperty("FuelType")]
    public virtual ICollection<FuelStock> FuelStocks { get; set; } = new List<FuelStock>();

    [InverseProperty("FuelType")]
    public virtual ICollection<FuelTransaction> FuelTransactions { get; set; } = new List<FuelTransaction>();

    [InverseProperty("FuelType")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
