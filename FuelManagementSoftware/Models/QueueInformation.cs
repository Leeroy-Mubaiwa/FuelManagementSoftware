using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Table("QueueInformation")]
[Index("FuelStationId", Name = "IX_QueueInformation_FuelStationId")]
[Index("RecordedAt", Name = "IX_QueueInformation_RecordedAt")]
[Index("CreatorId", Name = "IX_QueueInformation_creator_id")]
[Index("OrganisationId", Name = "IX_QueueInformation_organisation_id")]
public partial class QueueInformation
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    public int FuelStationId { get; set; }

    public int EstimatedQueueLength { get; set; }

    public int? EstimatedWaitTimeMinutes { get; set; }

    public int ActivePumps { get; set; }

    public DateTime RecordedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("QueueInformations")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("FuelStationId")]
    [InverseProperty("QueueInformations")]
    public virtual FuelStation FuelStation { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("QueueInformations")]
    public virtual Organization Organisation { get; set; } = null!;
}
