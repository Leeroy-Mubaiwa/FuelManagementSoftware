using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("FuelPumpId", Name = "IX_FuelTransactions_FuelPumpId")]
[Index("FuelStationId", Name = "IX_FuelTransactions_FuelStationId")]
[Index("PetroCardId", Name = "IX_FuelTransactions_PetroCardId")]
[Index("TransactionDate", Name = "IX_FuelTransactions_TransactionDate")]
[Index("TransactionNumber", Name = "IX_FuelTransactions_TransactionNumber")]
[Index("TransactionStatus", Name = "IX_FuelTransactions_TransactionStatus")]
[Index("UserId", Name = "IX_FuelTransactions_UserId")]
[Index("CreatorId", Name = "IX_FuelTransactions_creator_id")]
[Index("OrganisationId", Name = "IX_FuelTransactions_organisation_id")]
[Index("TransactionNumber", Name = "UQ__FuelTran__E733A2BF3216A763", IsUnique = true)]
public partial class FuelTransaction
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    [StringLength(100)]
    public string TransactionNumber { get; set; } = null!;

    public int FuelStationId { get; set; }

    public int FuelPumpId { get; set; }

    public int FuelTypeId { get; set; }

    public int? PetroCardId { get; set; }

    public string? UserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(50)]
    public string TransactionStatus { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [InverseProperty("FuelTransaction")]
    public virtual ICollection<BlockchainTransaction> BlockchainTransactions { get; set; } = new List<BlockchainTransaction>();

    [ForeignKey("CreatorId")]
    [InverseProperty("FuelTransactionCreators")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("FuelPumpId")]
    [InverseProperty("FuelTransactions")]
    public virtual FuelPump FuelPump { get; set; } = null!;

    [ForeignKey("FuelStationId")]
    [InverseProperty("FuelTransactions")]
    public virtual FuelStation FuelStation { get; set; } = null!;

    [ForeignKey("FuelTypeId")]
    [InverseProperty("FuelTransactions")]
    public virtual FuelType FuelType { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("FuelTransactions")]
    public virtual Organization Organisation { get; set; } = null!;

    [ForeignKey("PetroCardId")]
    [InverseProperty("FuelTransactions")]
    public virtual PetroCard? PetroCard { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("FuelTransactionUsers")]
    public virtual User? User { get; set; }
}
