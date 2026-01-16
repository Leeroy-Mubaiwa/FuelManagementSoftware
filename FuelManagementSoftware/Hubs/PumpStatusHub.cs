using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FuelManagementSoftware.Hubs;

/// <summary>
/// SignalR hub for real-time pump status updates.
/// Allows clients to receive real-time updates about pump status, dispensing progress, and transactions.
/// </summary>
public class PumpStatusHub : Hub
{
    private readonly ILogger<PumpStatusHub> _logger;

    public PumpStatusHub(ILogger<PumpStatusHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Join a group for a specific pump to receive updates for that pump.
    /// </summary>
    public async Task JoinPumpGroup(int pumpId)
    {
        var groupName = $"pump-{pumpId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} joined pump group {GroupName}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Leave a pump group.
    /// </summary>
    public async Task LeavePumpGroup(int pumpId)
    {
        var groupName = $"pump-{pumpId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} left pump group {GroupName}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Join a group for a specific station to receive updates for all pumps at that station.
    /// </summary>
    public async Task JoinStationGroup(int stationId)
    {
        var groupName = $"station-{stationId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} joined station group {GroupName}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Leave a station group.
    /// </summary>
    public async Task LeaveStationGroup(int stationId)
    {
        var groupName = $"station-{stationId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} left station group {GroupName}", Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client {ConnectionId} connected to PumpStatusHub", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client {ConnectionId} disconnected from PumpStatusHub", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}

/// <summary>
/// Extension methods for sending pump status updates via SignalR.
/// </summary>
public static class PumpStatusHubExtensions
{
    /// <summary>
    /// Send dispensing progress update to clients monitoring a specific pump.
    /// </summary>
    public static async Task SendDispensingProgress(this IHubContext<PumpStatusHub> hub, int pumpId, int transactionId, decimal quantity, decimal amount)
    {
        await hub.Clients.Group($"pump-{pumpId}").SendAsync("DispensingProgress", new
        {
            PumpId = pumpId,
            TransactionId = transactionId,
            Quantity = quantity,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Send transaction status update to clients monitoring a specific pump.
    /// </summary>
    public static async Task SendTransactionStatus(this IHubContext<PumpStatusHub> hub, int pumpId, int transactionId, string status, string? message = null)
    {
        await hub.Clients.Group($"pump-{pumpId}").SendAsync("TransactionStatus", new
        {
            PumpId = pumpId,
            TransactionId = transactionId,
            Status = status,
            Message = message,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Send pump status update (operational, active, etc.) to clients monitoring a specific pump.
    /// </summary>
    public static async Task SendPumpStatus(this IHubContext<PumpStatusHub> hub, int pumpId, bool isOperational, bool isActive)
    {
        await hub.Clients.Group($"pump-{pumpId}").SendAsync("PumpStatus", new
        {
            PumpId = pumpId,
            IsOperational = isOperational,
            IsActive = isActive,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Send NFC tag detected notification to clients monitoring a specific pump.
    /// </summary>
    public static async Task SendNfcTagDetected(this IHubContext<PumpStatusHub> hub, int pumpId, string nfcTag)
    {
        await hub.Clients.Group($"pump-{pumpId}").SendAsync("NfcTagDetected", new
        {
            PumpId = pumpId,
            NfcTag = nfcTag,
            Timestamp = DateTime.UtcNow
        });
    }
}

