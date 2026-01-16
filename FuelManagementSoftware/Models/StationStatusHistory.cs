using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Table("StationStatusHistory")]
[Index("FuelStationId", Name = "IX_StationStatusHistory_FuelStationId")]
[Index("StatusChangedAt", Name = "IX_StationStatusHistory_StatusChangedAt")]
[Index("StatusType", Name = "IX_StationStatusHistory_StatusType")]
[Index("CreatorId", Name = "IX_StationStatusHistory_creator_id")]
[Index("OrganisationId", Name = "IX_StationStatusHistory_organisation_id")]
public partial class StationStatusHistory
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    public int FuelStationId { get; set; }

    [StringLength(50)]
    public string StatusType { get; set; } = null!;

    [StringLength(50)]
    public string? PreviousStatus { get; set; }

    [StringLength(50)]
    public string NewStatus { get; set; } = null!;

    [StringLength(500)]
    public string? Reason { get; set; }

    public DateTime? ExpectedReopenTime { get; set; }

    public DateTime StatusChangedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("StationStatusHistories")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("FuelStationId")]
    [InverseProperty("StationStatusHistories")]
    public virtual FuelStation FuelStation { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("StationStatusHistories")]
    public virtual Organization Organisation { get; set; } = null!;
}
