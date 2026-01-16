using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("PetroCardId", Name = "IX_CardTransactions_PetroCardId")]
[Index("TransactionDate", Name = "IX_CardTransactions_TransactionDate")]
[Index("TransactionType", Name = "IX_CardTransactions_TransactionType")]
[Index("CreatorId", Name = "IX_CardTransactions_creator_id")]
[Index("OrganisationId", Name = "IX_CardTransactions_organisation_id")]
public partial class CardTransaction
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    public int PetroCardId { get; set; }

    [StringLength(50)]
    public string TransactionType { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalanceBefore { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalanceAfter { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("CardTransactions")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("CardTransactions")]
    public virtual Organization Organisation { get; set; } = null!;

    [ForeignKey("PetroCardId")]
    [InverseProperty("CardTransactions")]
    public virtual PetroCard PetroCard { get; set; } = null!;
}
