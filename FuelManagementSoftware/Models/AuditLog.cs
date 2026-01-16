using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("Action", Name = "IX_AuditLogs_Action")]
[Index("CreatedAt", Name = "IX_AuditLogs_CreatedAt")]
[Index("EntityId", Name = "IX_AuditLogs_EntityId")]
[Index("EntityType", Name = "IX_AuditLogs_EntityType")]
[Index("UserId", Name = "IX_AuditLogs_UserId")]
[Index("CreatorId", Name = "IX_AuditLogs_creator_id")]
[Index("OrganisationId", Name = "IX_AuditLogs_organisation_id")]
public partial class AuditLog
{
    [Key]
    public long Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    [StringLength(100)]
    public string EntityType { get; set; } = null!;

    public int? EntityId { get; set; }

    [StringLength(50)]
    public string Action { get; set; } = null!;

    public string? UserId { get; set; }

    [StringLength(255)]
    public string? UserName { get; set; }

    public string? Changes { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("AuditLogCreators")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("AuditLogs")]
    public virtual Organization Organisation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("AuditLogUsers")]
    public virtual User? User { get; set; }
}
