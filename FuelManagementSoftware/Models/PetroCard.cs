using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("CardNumber", Name = "IX_PetroCards_CardNumber")]
[Index("IsActive", Name = "IX_PetroCards_IsActive")]
[Index("Rfidtag", Name = "IX_PetroCards_RFIDTag")]
[Index("UserId", Name = "IX_PetroCards_UserId")]
[Index("CreatorId", Name = "IX_PetroCards_creator_id")]
[Index("OrganisationId", Name = "IX_PetroCards_organisation_id")]
[Index("Rfidtag", Name = "UQ__PetroCar__411E34E6E9D8F135", IsUnique = true)]
[Index("CardNumber", Name = "UQ__PetroCar__A4E9FFE9DD1223D2", IsUnique = true)]
public partial class PetroCard
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    [StringLength(50)]
    public string CardNumber { get; set; } = null!;

    [Column("RFIDTag")]
    [StringLength(100)]
    public string? Rfidtag { get; set; }

    public string UserId { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsBlocked { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [StringLength(255)]
    public string? PinHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [InverseProperty("PetroCard")]
    public virtual ICollection<CardTransaction> CardTransactions { get; set; } = new List<CardTransaction>();

    [ForeignKey("CreatorId")]
    [InverseProperty("PetroCardCreators")]
    public virtual User Creator { get; set; } = null!;

    [InverseProperty("PetroCard")]
    public virtual ICollection<FuelTransaction> FuelTransactions { get; set; } = new List<FuelTransaction>();

    [ForeignKey("OrganisationId")]
    [InverseProperty("PetroCards")]
    public virtual Organization Organisation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("PetroCardUsers")]
    public virtual User User { get; set; } = null!;
}
