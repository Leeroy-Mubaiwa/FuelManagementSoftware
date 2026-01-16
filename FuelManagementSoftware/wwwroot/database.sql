-- =============================================
-- Fuel Management Software Database Schema
-- Extends ASP.NET Core Identity Database
-- =============================================
-- This script creates additional tables for the automated fuel management system
-- All tables include organisation_id and creator_id for multi-tenancy and audit tracking
-- =============================================

USE [FuelManagementSoftware]
GO

-- =============================================
-- Organizations Table
-- Represents fuel companies (e.g., Petrotrade)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Organizations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Organizations] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(255) NOT NULL,
        [Code] NVARCHAR(50) NULL,
        [Address] NVARCHAR(500) NULL,
        [Phone] NVARCHAR(50) NULL,
        [Email] NVARCHAR(255) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_Organizations_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_Organizations_creator_id] ON [dbo].[Organizations]([creator_id]);
    CREATE INDEX [IX_Organizations_Code] ON [dbo].[Organizations]([Code]);
END
GO

-- =============================================
-- Fuel Stations Table
-- Represents physical fuel station locations
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FuelStations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FuelStations] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [Name] NVARCHAR(255) NOT NULL,
        [Code] NVARCHAR(50) NULL,
        [Address] NVARCHAR(500) NOT NULL,
        [City] NVARCHAR(100) NULL,
        [Latitude] DECIMAL(10, 8) NULL,
        [Longitude] DECIMAL(11, 8) NULL,
        [Phone] NVARCHAR(50) NULL,
        [Email] NVARCHAR(255) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsOpen] BIT NOT NULL DEFAULT 1,
        [IsTankerOffloading] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_FuelStations_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FuelStations_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_FuelStations_organisation_id] ON [dbo].[FuelStations]([organisation_id]);
    CREATE INDEX [IX_FuelStations_creator_id] ON [dbo].[FuelStations]([creator_id]);
    CREATE INDEX [IX_FuelStations_IsActive] ON [dbo].[FuelStations]([IsActive]);
    CREATE INDEX [IX_FuelStations_IsOpen] ON [dbo].[FuelStations]([IsOpen]);
    CREATE INDEX [IX_FuelStations_Location] ON [dbo].[FuelStations]([Latitude], [Longitude]);
END
GO

-- =============================================
-- Fuel Types Table
-- Represents different types of fuel (Petrol, Diesel, etc.)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FuelTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FuelTypes] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Code] NVARCHAR(50) NULL,
        [Description] NVARCHAR(500) NULL,
        [UnitPrice] DECIMAL(18, 2) NOT NULL,
        [Unit] NVARCHAR(20) NOT NULL DEFAULT 'Litre',
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_FuelTypes_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FuelTypes_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_FuelTypes_organisation_id] ON [dbo].[FuelTypes]([organisation_id]);
    CREATE INDEX [IX_FuelTypes_creator_id] ON [dbo].[FuelTypes]([creator_id]);
END
GO

-- =============================================
-- Fuel Pumps Table
-- Represents individual fuel pumps at stations
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FuelPumps]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FuelPumps] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [FuelStationId] INT NOT NULL,
        [PumpNumber] NVARCHAR(50) NOT NULL,
        [FuelTypeId] INT NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsOperational] BIT NOT NULL DEFAULT 1,
        [LastMaintenanceDate] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_FuelPumps_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FuelPumps_FuelStations] FOREIGN KEY ([FuelStationId]) REFERENCES [dbo].[FuelStations]([Id]) ,
        CONSTRAINT [FK_FuelPumps_FuelTypes] FOREIGN KEY ([FuelTypeId]) REFERENCES [dbo].[FuelTypes]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FuelPumps_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_FuelPumps_organisation_id] ON [dbo].[FuelPumps]([organisation_id]);
    CREATE INDEX [IX_FuelPumps_FuelStationId] ON [dbo].[FuelPumps]([FuelStationId]);
    CREATE INDEX [IX_FuelPumps_creator_id] ON [dbo].[FuelPumps]([creator_id]);
END
GO

-- =============================================
-- Fuel Stock Table
-- Tracks real-time fuel availability at stations
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FuelStock]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FuelStock] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [FuelStationId] INT NOT NULL,
        [FuelTypeId] INT NOT NULL,
        [CurrentQuantity] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [Capacity] DECIMAL(18, 2) NOT NULL,
        [Unit] NVARCHAR(20) NOT NULL DEFAULT 'Litre',
        [LastUpdated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsLowStock] BIT NOT NULL DEFAULT 0,
        [LowStockThreshold] DECIMAL(18, 2) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_FuelStock_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FuelStock_FuelStations] FOREIGN KEY ([FuelStationId]) REFERENCES [dbo].[FuelStations]([Id])  ,
        CONSTRAINT [FK_FuelStock_FuelTypes] FOREIGN KEY ([FuelTypeId]) REFERENCES [dbo].[FuelTypes]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FuelStock_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_FuelStock_Station_FuelType] UNIQUE ([FuelStationId], [FuelTypeId])
    );
    
    CREATE INDEX [IX_FuelStock_organisation_id] ON [dbo].[FuelStock]([organisation_id]);
    CREATE INDEX [IX_FuelStock_FuelStationId] ON [dbo].[FuelStock]([FuelStationId]);
    CREATE INDEX [IX_FuelStock_FuelTypeId] ON [dbo].[FuelStock]([FuelTypeId]);
    CREATE INDEX [IX_FuelStock_creator_id] ON [dbo].[FuelStock]([creator_id]);
    CREATE INDEX [IX_FuelStock_LastUpdated] ON [dbo].[FuelStock]([LastUpdated]);
END
GO

-- =============================================
-- PetroCards Table
-- Represents prepaid smart cards (RFID-enabled)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PetroCards]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PetroCards] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [CardNumber] NVARCHAR(50) NOT NULL UNIQUE,
        [RFIDTag] NVARCHAR(100) NULL UNIQUE,
        [UserId] NVARCHAR(450) NOT NULL,
        [Balance] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [Currency] NVARCHAR(10) NOT NULL DEFAULT 'USD',
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsBlocked] BIT NOT NULL DEFAULT 0,
        [ExpiryDate] DATETIME2 NULL,
        [PinHash] NVARCHAR(255) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [LastUsedAt] DATETIME2 NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_PetroCards_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PetroCards_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])  ,
        CONSTRAINT [FK_PetroCards_Creator] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_PetroCards_organisation_id] ON [dbo].[PetroCards]([organisation_id]);
    CREATE INDEX [IX_PetroCards_CardNumber] ON [dbo].[PetroCards]([CardNumber]);
    CREATE INDEX [IX_PetroCards_RFIDTag] ON [dbo].[PetroCards]([RFIDTag]);
    CREATE INDEX [IX_PetroCards_UserId] ON [dbo].[PetroCards]([UserId]);
    CREATE INDEX [IX_PetroCards_creator_id] ON [dbo].[PetroCards]([creator_id]);
    CREATE INDEX [IX_PetroCards_IsActive] ON [dbo].[PetroCards]([IsActive]);
END
GO

-- =============================================
-- Card Transactions Table
-- Tracks card top-ups, refunds, and balance changes
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CardTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CardTransactions] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [PetroCardId] INT NOT NULL,
        [TransactionType] NVARCHAR(50) NOT NULL, -- 'TopUp', 'Refund', 'Adjustment', 'Fee'
        [Amount] DECIMAL(18, 2) NOT NULL,
        [BalanceBefore] DECIMAL(18, 2) NOT NULL,
        [BalanceAfter] DECIMAL(18, 2) NOT NULL,
        [Currency] NVARCHAR(10) NOT NULL DEFAULT 'USD',
        [PaymentMethod] NVARCHAR(50) NULL, -- 'Cash', 'BankTransfer', 'MobileMoney', etc.
        [ReferenceNumber] NVARCHAR(100) NULL,
        [Description] NVARCHAR(500) NULL,
        [TransactionDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_CardTransactions_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CardTransactions_PetroCards] FOREIGN KEY ([PetroCardId]) REFERENCES [dbo].[PetroCards]([Id]) ,
        CONSTRAINT [FK_CardTransactions_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_CardTransactions_organisation_id] ON [dbo].[CardTransactions]([organisation_id]);
    CREATE INDEX [IX_CardTransactions_PetroCardId] ON [dbo].[CardTransactions]([PetroCardId]);
    CREATE INDEX [IX_CardTransactions_creator_id] ON [dbo].[CardTransactions]([creator_id]);
    CREATE INDEX [IX_CardTransactions_TransactionDate] ON [dbo].[CardTransactions]([TransactionDate]);
    CREATE INDEX [IX_CardTransactions_TransactionType] ON [dbo].[CardTransactions]([TransactionType]);
END
GO

-- =============================================
-- Fuel Transactions Table
-- Records fuel dispensing transactions
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FuelTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FuelTransactions] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [TransactionNumber] NVARCHAR(100) NOT NULL UNIQUE,
        [FuelStationId] INT NOT NULL,
        [FuelPumpId] INT NOT NULL,
        [FuelTypeId] INT NOT NULL,
        [PetroCardId] INT NULL,
        [UserId] NVARCHAR(450) NULL,
        [Quantity] DECIMAL(18, 2) NOT NULL,
        [UnitPrice] DECIMAL(18, 2) NOT NULL,
        [TotalAmount] DECIMAL(18, 2) NOT NULL,
        [Currency] NVARCHAR(10) NOT NULL DEFAULT 'USD',
        [PaymentMethod] NVARCHAR(50) NULL, -- 'PetroCard', 'Cash', 'MobileMoney', etc.
        [TransactionStatus] NVARCHAR(50) NOT NULL DEFAULT 'Completed', -- 'Pending', 'Completed', 'Failed', 'Cancelled'
        [TransactionDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [StartedAt] DATETIME2 NULL,
        [CompletedAt] DATETIME2 NULL,
        [Notes] NVARCHAR(1000) NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_FuelTransactions_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FuelTransactions_FuelStations] FOREIGN KEY ([FuelStationId]) REFERENCES [dbo].[FuelStations]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FuelTransactions_FuelPumps] FOREIGN KEY ([FuelPumpId]) REFERENCES [dbo].[FuelPumps]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FuelTransactions_FuelTypes] FOREIGN KEY ([FuelTypeId]) REFERENCES [dbo].[FuelTypes]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FuelTransactions_PetroCards] FOREIGN KEY ([PetroCardId]) REFERENCES [dbo].[PetroCards]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FuelTransactions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FuelTransactions_Creator] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_FuelTransactions_organisation_id] ON [dbo].[FuelTransactions]([organisation_id]);
    CREATE INDEX [IX_FuelTransactions_TransactionNumber] ON [dbo].[FuelTransactions]([TransactionNumber]);
    CREATE INDEX [IX_FuelTransactions_FuelStationId] ON [dbo].[FuelTransactions]([FuelStationId]);
    CREATE INDEX [IX_FuelTransactions_FuelPumpId] ON [dbo].[FuelTransactions]([FuelPumpId]);
    CREATE INDEX [IX_FuelTransactions_PetroCardId] ON [dbo].[FuelTransactions]([PetroCardId]);
    CREATE INDEX [IX_FuelTransactions_UserId] ON [dbo].[FuelTransactions]([UserId]);
    CREATE INDEX [IX_FuelTransactions_creator_id] ON [dbo].[FuelTransactions]([creator_id]);
    CREATE INDEX [IX_FuelTransactions_TransactionDate] ON [dbo].[FuelTransactions]([TransactionDate]);
    CREATE INDEX [IX_FuelTransactions_TransactionStatus] ON [dbo].[FuelTransactions]([TransactionStatus]);
END
GO

-- =============================================
-- Blockchain Transactions Table
-- Immutable records of transactions for blockchain integration
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BlockchainTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BlockchainTransactions] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [FuelTransactionId] INT NOT NULL,
        [BlockchainHash] NVARCHAR(255) NOT NULL UNIQUE,
        [PreviousHash] NVARCHAR(255) NULL,
        [BlockNumber] BIGINT NULL,
        [TransactionIndex] INT NULL,
        [BlockchainNetwork] NVARCHAR(100) NULL,
        [SmartContractAddress] NVARCHAR(255) NULL,
        [GasUsed] DECIMAL(18, 8) NULL,
        [GasPrice] DECIMAL(18, 8) NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- 'Pending', 'Confirmed', 'Failed'
        [ConfirmationCount] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ConfirmedAt] DATETIME2 NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_BlockchainTransactions_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BlockchainTransactions_FuelTransactions] FOREIGN KEY ([FuelTransactionId]) REFERENCES [dbo].[FuelTransactions]([Id]) ON DELETE no action,
        CONSTRAINT [FK_BlockchainTransactions_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_BlockchainTransactions_organisation_id] ON [dbo].[BlockchainTransactions]([organisation_id]);
    CREATE INDEX [IX_BlockchainTransactions_FuelTransactionId] ON [dbo].[BlockchainTransactions]([FuelTransactionId]);
    CREATE INDEX [IX_BlockchainTransactions_BlockchainHash] ON [dbo].[BlockchainTransactions]([BlockchainHash]);
    CREATE INDEX [IX_BlockchainTransactions_creator_id] ON [dbo].[BlockchainTransactions]([creator_id]);
    CREATE INDEX [IX_BlockchainTransactions_Status] ON [dbo].[BlockchainTransactions]([Status]);
    CREATE INDEX [IX_BlockchainTransactions_CreatedAt] ON [dbo].[BlockchainTransactions]([CreatedAt]);
END
GO

-- =============================================
-- Station Status History Table
-- Tracks station status changes (open/closed, tanker offloading)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StationStatusHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StationStatusHistory] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [FuelStationId] INT NOT NULL,
        [StatusType] NVARCHAR(50) NOT NULL, -- 'Open', 'Closed', 'TankerOffloading', 'Maintenance', etc.
        [PreviousStatus] NVARCHAR(50) NULL,
        [NewStatus] NVARCHAR(50) NOT NULL,
        [Reason] NVARCHAR(500) NULL,
        [ExpectedReopenTime] DATETIME2 NULL,
        [StatusChangedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_StationStatusHistory_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StationStatusHistory_FuelStations] FOREIGN KEY ([FuelStationId]) REFERENCES [dbo].[FuelStations]([Id]) ON DELETE no action,
        CONSTRAINT [FK_StationStatusHistory_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_StationStatusHistory_organisation_id] ON [dbo].[StationStatusHistory]([organisation_id]);
    CREATE INDEX [IX_StationStatusHistory_FuelStationId] ON [dbo].[StationStatusHistory]([FuelStationId]);
    CREATE INDEX [IX_StationStatusHistory_creator_id] ON [dbo].[StationStatusHistory]([creator_id]);
    CREATE INDEX [IX_StationStatusHistory_StatusChangedAt] ON [dbo].[StationStatusHistory]([StatusChangedAt]);
    CREATE INDEX [IX_StationStatusHistory_StatusType] ON [dbo].[StationStatusHistory]([StatusType]);
END
GO

-- =============================================
-- Queue Information Table
-- Tracks queue lengths and wait times at stations
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueueInformation]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QueueInformation] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [FuelStationId] INT NOT NULL,
        [EstimatedQueueLength] INT NOT NULL DEFAULT 0,
        [EstimatedWaitTimeMinutes] INT NULL,
        [ActivePumps] INT NOT NULL DEFAULT 0,
        [RecordedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_QueueInformation_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QueueInformation_FuelStations] FOREIGN KEY ([FuelStationId]) REFERENCES [dbo].[FuelStations]([Id]) ON DELETE no action,
        CONSTRAINT [FK_QueueInformation_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_QueueInformation_organisation_id] ON [dbo].[QueueInformation]([organisation_id]);
    CREATE INDEX [IX_QueueInformation_FuelStationId] ON [dbo].[QueueInformation]([FuelStationId]);
    CREATE INDEX [IX_QueueInformation_creator_id] ON [dbo].[QueueInformation]([creator_id]);
    CREATE INDEX [IX_QueueInformation_RecordedAt] ON [dbo].[QueueInformation]([RecordedAt]);
END
GO

-- =============================================
-- Stock Movements Table
-- Tracks fuel stock additions (tanker deliveries) and deductions
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StockMovements]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StockMovements] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [FuelStationId] INT NOT NULL,
        [FuelTypeId] INT NOT NULL,
        [MovementType] NVARCHAR(50) NOT NULL, -- 'Delivery', 'Dispensed', 'Adjustment', 'Loss', 'Transfer'
        [Quantity] DECIMAL(18, 2) NOT NULL,
        [Unit] NVARCHAR(20) NOT NULL DEFAULT 'Litre',
        [StockBefore] DECIMAL(18, 2) NOT NULL,
        [StockAfter] DECIMAL(18, 2) NOT NULL,
        [ReferenceNumber] NVARCHAR(100) NULL,
        [DeliveryNoteNumber] NVARCHAR(100) NULL,
        [TankerRegistration] NVARCHAR(50) NULL,
        [DriverName] NVARCHAR(255) NULL,
        [MovementDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [Notes] NVARCHAR(1000) NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_StockMovements_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StockMovements_FuelStations] FOREIGN KEY ([FuelStationId]) REFERENCES [dbo].[FuelStations]([Id]) ON DELETE no action,
        CONSTRAINT [FK_StockMovements_FuelTypes] FOREIGN KEY ([FuelTypeId]) REFERENCES [dbo].[FuelTypes]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StockMovements_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_StockMovements_organisation_id] ON [dbo].[StockMovements]([organisation_id]);
    CREATE INDEX [IX_StockMovements_FuelStationId] ON [dbo].[StockMovements]([FuelStationId]);
    CREATE INDEX [IX_StockMovements_FuelTypeId] ON [dbo].[StockMovements]([FuelTypeId]);
    CREATE INDEX [IX_StockMovements_creator_id] ON [dbo].[StockMovements]([creator_id]);
    CREATE INDEX [IX_StockMovements_MovementDate] ON [dbo].[StockMovements]([MovementDate]);
    CREATE INDEX [IX_StockMovements_MovementType] ON [dbo].[StockMovements]([MovementType]);
END
GO

-- =============================================
-- Audit Log Table
-- Comprehensive audit trail for all system activities
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [EntityType] NVARCHAR(100) NOT NULL,
        [EntityId] INT NULL,
        [Action] NVARCHAR(50) NOT NULL, -- 'Create', 'Update', 'Delete', 'View', 'Login', 'Logout', etc.
        [UserId] NVARCHAR(450) NULL,
        [UserName] NVARCHAR(255) NULL,
        [Changes] NVARCHAR(MAX) NULL, -- JSON format for change tracking
        [IpAddress] NVARCHAR(50) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_AuditLogs_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AuditLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AuditLogs_Creator] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_AuditLogs_organisation_id] ON [dbo].[AuditLogs]([organisation_id]);
    CREATE INDEX [IX_AuditLogs_EntityType] ON [dbo].[AuditLogs]([EntityType]);
    CREATE INDEX [IX_AuditLogs_EntityId] ON [dbo].[AuditLogs]([EntityId]);
    CREATE INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs]([UserId]);
    CREATE INDEX [IX_AuditLogs_creator_id] ON [dbo].[AuditLogs]([creator_id]);
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [dbo].[AuditLogs]([CreatedAt]);
    CREATE INDEX [IX_AuditLogs_Action] ON [dbo].[AuditLogs]([Action]);
END
GO

-- =============================================
-- System Configuration Table
-- Stores system-wide configuration settings
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemConfigurations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemConfigurations] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [organisation_id] INT NOT NULL,
        [ConfigurationKey] NVARCHAR(255) NOT NULL,
        [ConfigurationValue] NVARCHAR(MAX) NULL,
        [ValueType] NVARCHAR(50) NULL, -- 'String', 'Number', 'Boolean', 'JSON'
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [creator_id] NVARCHAR(450) NOT NULL,
        CONSTRAINT [FK_SystemConfigurations_Organizations] FOREIGN KEY ([organisation_id]) REFERENCES [dbo].[Organizations]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_SystemConfigurations_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_SystemConfigurations_Org_Key] UNIQUE ([organisation_id], [ConfigurationKey])
    );
    
    CREATE INDEX [IX_SystemConfigurations_organisation_id] ON [dbo].[SystemConfigurations]([organisation_id]);
    CREATE INDEX [IX_SystemConfigurations_creator_id] ON [dbo].[SystemConfigurations]([creator_id]);
    CREATE INDEX [IX_SystemConfigurations_ConfigurationKey] ON [dbo].[SystemConfigurations]([ConfigurationKey]);
END
GO

-- =============================================
-- End of Database Schema Script
-- =============================================

