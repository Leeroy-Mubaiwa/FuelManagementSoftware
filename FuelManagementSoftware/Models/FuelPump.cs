using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("FuelStationId", Name = "IX_FuelPumps_FuelStationId")]
[Index("CreatorId", Name = "IX_FuelPumps_creator_id")]
[Index("OrganisationId", Name = "IX_FuelPumps_organisation_id")]
public partial class FuelPump
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    public int FuelStationId { get; set; }

    [StringLength(50)]
    public string PumpNumber { get; set; } = null!;

    public int FuelTypeId { get; set; }

    public bool IsActive { get; set; }

    public bool IsOperational { get; set; }

    public DateTime? LastMaintenanceDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("FuelPumps")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("FuelStationId")]
    [InverseProperty("FuelPumps")]
    public virtual FuelStation FuelStation { get; set; } = null!;

    [InverseProperty("FuelPump")]
    public virtual ICollection<FuelTransaction> FuelTransactions { get; set; } = new List<FuelTransaction>();

    [ForeignKey("FuelTypeId")]
    [InverseProperty("FuelPumps")]
    public virtual FuelType FuelType { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("FuelPumps")]
    public virtual Organization Organisation { get; set; } = null!;
}
