using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Models;

[Index("BlockchainHash", Name = "IX_BlockchainTransactions_BlockchainHash")]
[Index("CreatedAt", Name = "IX_BlockchainTransactions_CreatedAt")]
[Index("FuelTransactionId", Name = "IX_BlockchainTransactions_FuelTransactionId")]
[Index("Status", Name = "IX_BlockchainTransactions_Status")]
[Index("CreatorId", Name = "IX_BlockchainTransactions_creator_id")]
[Index("OrganisationId", Name = "IX_BlockchainTransactions_organisation_id")]
[Index("BlockchainHash", Name = "UQ__Blockcha__D5ABDAFF2DA25C72", IsUnique = true)]
public partial class BlockchainTransaction
{
    [Key]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    public int FuelTransactionId { get; set; }

    [StringLength(255)]
    public string BlockchainHash { get; set; } = null!;

    [StringLength(255)]
    public string? PreviousHash { get; set; }

    public long? BlockNumber { get; set; }

    public int? TransactionIndex { get; set; }

    [StringLength(100)]
    public string? BlockchainNetwork { get; set; }

    [StringLength(255)]
    public string? SmartContractAddress { get; set; }

    [Column(TypeName = "decimal(18, 8)")]
    public decimal? GasUsed { get; set; }

    [Column(TypeName = "decimal(18, 8)")]
    public decimal? GasPrice { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public int ConfirmationCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    [Column("creator_id")]
    public string CreatorId { get; set; } = null!;

    [ForeignKey("CreatorId")]
    [InverseProperty("BlockchainTransactions")]
    public virtual User Creator { get; set; } = null!;

    [ForeignKey("FuelTransactionId")]
    [InverseProperty("BlockchainTransactions")]
    public virtual FuelTransaction FuelTransaction { get; set; } = null!;

    [ForeignKey("OrganisationId")]
    [InverseProperty("BlockchainTransactions")]
    public virtual Organization Organisation { get; set; } = null!;
}
