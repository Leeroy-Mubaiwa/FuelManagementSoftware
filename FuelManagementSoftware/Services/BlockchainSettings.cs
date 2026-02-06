namespace FuelManagementSoftware.Services;

public class BlockchainSettings
{
    public string RpcUrl { get; set; } = "";
    public string PrivateKey { get; set; } = "";
    public string WalletAddress { get; set; } = "";
    public string ContractAddress { get; set; } = "";
    public string NetworkName { get; set; } = "Sepolia";
    public int ChainId { get; set; } = 11155111;
    public string ExplorerUrl { get; set; } = "https://sepolia.etherscan.io";
}

