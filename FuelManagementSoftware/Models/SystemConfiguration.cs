using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("ConfigurationKey", Name = "IX_SystemConfigurations_ConfigurationKey")]
[Index("CreatorId", Name = "IX_SystemConfigurations_creator_id")]
[Index("OrganisationId", Name = "IX_SystemConfigurations_organisation_id")]
[Index("OrganisationId", "ConfigurationKey", Name = "UQ_SystemConfigurations_Org_Key", IsUnique = true)]
public partial class SystemConfiguration
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    [StringLength(255)]
    public string ConfigurationKey { get; set; } = null!;

    public string? ConfigurationValue { get; set; }

    [StringLength(50)]
    public string? ValueType { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("SystemConfigurations")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("SystemConfigurations")]
    public virtual Organization Organisation { get; set; } = null!;
}
