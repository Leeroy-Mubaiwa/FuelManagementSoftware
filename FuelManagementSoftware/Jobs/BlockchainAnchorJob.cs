using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using FuelManagementSoftware.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace FuelManagementSoftware.Jobs;

public class BlockchainAnchorJob : IJob
{
    private readonly IBlockchainService _blockchainService;
    private readonly FuelManagementSoftwareDbContext _dbContext;
    private readonly ILogger<BlockchainAnchorJob> _logger;

    public BlockchainAnchorJob(
        IBlockchainService blockchainService,
        FuelManagementSoftwareDbContext dbContext,
        ILogger<BlockchainAnchorJob> logger)
    {
        _blockchainService = blockchainService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var transactionId = context.MergedJobDataMap.GetInt("FuelTransactionId");
        _logger.LogInformation("Background Job started: Anchoring transaction {Id} to blockchain", transactionId);

        try
        {
            var fuelTransaction = await _dbContext.FuelTransactions
                .Include(t => t.PetroCard)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (fuelTransaction == null)
            {
                _logger.LogWarning("Transaction {Id} not found for blockchain anchoring", transactionId);
                return;
            }

            if (!_blockchainService.IsConfigured())
            {
                _logger.LogWarning("Blockchain not configured. Skipping background anchoring for {Id}", transactionId);
                return;
            }

            var alreadyAnchored = await _dbContext.BlockchainTransactions
                .AnyAsync(bt => bt.FuelTransactionId == transactionId && bt.Status == "Confirmed");
            if (alreadyAnchored)
            {
                _logger.LogInformation("Transaction {Id} already anchored. Skipping background anchoring.", transactionId);
                return;
            }

            var blockchainResult = await _blockchainService.RecordTransactionAsync(fuelTransaction);

            if (blockchainResult.Success)
            {
                var blockchainTx = new BlockchainTransaction
                {
                    OrganisationId = fuelTransaction.OrganisationId,
                    FuelTransactionId = fuelTransaction.Id,
                    BlockchainHash = blockchainResult.TransactionHash!,
                    BlockchainNetwork = "Sepolia",
                    SmartContractAddress = blockchainResult.ContractAddress,
                    GasUsed = blockchainResult.GasUsed,
                    Status = "Confirmed",
                    CreatedAt = DateTime.Now,
                    ConfirmedAt = DateTime.Now,
                    CreatorId = fuelTransaction.CreatorId
                };

                _dbContext.BlockchainTransactions.Add(blockchainTx);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Background anchoring successful for Transaction {Tx}. Hash: {Hash}", 
                    fuelTransaction.TransactionNumber, blockchainResult.TransactionHash);
            }
            else
            {
                _logger.LogWarning("Background anchoring failed for {Tx}: {Error}", 
                    fuelTransaction.TransactionNumber, blockchainResult.ErrorMessage);
                // In a real app, you might reschedule here
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in BlockchainAnchorJob for transaction {Id}", transactionId);
            throw; // Job will be retried if configured
        }
    }
}
