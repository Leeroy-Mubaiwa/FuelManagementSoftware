/****** Object:  Database [FuelManagementSoftware]    Script Date: 1/26/2026 11:35:45 AM ******/
CREATE DATABASE [FuelManagementSoftware]
go
use FuelManagementSoftware
go
 
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditLogs]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[EntityType] [nvarchar](100) NOT NULL,
	[EntityId] [int] NULL,
	[Action] [nvarchar](50) NOT NULL,
	[UserId] [nvarchar](450) NULL,
	[UserName] [nvarchar](255) NULL,
	[Changes] [nvarchar](max) NULL,
	[IpAddress] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlockchainTransactions]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlockchainTransactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[FuelTransactionId] [int] NOT NULL,
	[BlockchainHash] [nvarchar](255) NOT NULL,
	[PreviousHash] [nvarchar](255) NULL,
	[BlockNumber] [bigint] NULL,
	[TransactionIndex] [int] NULL,
	[BlockchainNetwork] [nvarchar](100) NULL,
	[SmartContractAddress] [nvarchar](255) NULL,
	[GasUsed] [decimal](18, 8) NULL,
	[GasPrice] [decimal](18, 8) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[ConfirmationCount] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[ConfirmedAt] [datetime2](7) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CardTransactions]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CardTransactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[PetroCardId] [int] NOT NULL,
	[TransactionType] [nvarchar](50) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[BalanceBefore] [decimal](18, 2) NOT NULL,
	[BalanceAfter] [decimal](18, 2) NOT NULL,
	[Currency] [nvarchar](10) NOT NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[ReferenceNumber] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NULL,
	[TransactionDate] [datetime2](7) NOT NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FuelPumps]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FuelPumps](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[FuelStationId] [int] NOT NULL,
	[PumpNumber] [nvarchar](50) NOT NULL,
	[FuelTypeId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsOperational] [bit] NOT NULL,
	[LastMaintenanceDate] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FuelStations]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FuelStations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Address] [nvarchar](500) NOT NULL,
	[City] [nvarchar](100) NULL,
	[Latitude] [decimal](10, 8) NULL,
	[Longitude] [decimal](11, 8) NULL,
	[Phone] [nvarchar](50) NULL,
	[Email] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL,
	[IsOpen] [bit] NOT NULL,
	[IsTankerOffloading] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FuelStock]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FuelStock](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[FuelStationId] [int] NOT NULL,
	[FuelTypeId] [int] NOT NULL,
	[CurrentQuantity] [decimal](18, 2) NOT NULL,
	[Capacity] [decimal](18, 2) NOT NULL,
	[Unit] [nvarchar](20) NOT NULL,
	[LastUpdated] [datetime2](7) NOT NULL,
	[IsLowStock] [bit] NOT NULL,
	[LowStockThreshold] [decimal](18, 2) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FuelTransactions]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FuelTransactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[TransactionNumber] [nvarchar](100) NOT NULL,
	[FuelStationId] [int] NOT NULL,
	[FuelPumpId] [int] NOT NULL,
	[FuelTypeId] [int] NOT NULL,
	[PetroCardId] [int] NULL,
	[UserId] [nvarchar](450) NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[Currency] [nvarchar](10) NOT NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[TransactionStatus] [nvarchar](50) NOT NULL,
	[TransactionDate] [datetime2](7) NOT NULL,
	[StartedAt] [datetime2](7) NULL,
	[CompletedAt] [datetime2](7) NULL,
	[Notes] [nvarchar](1000) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FuelTypes]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FuelTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[Unit] [nvarchar](20) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organizations]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organizations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Address] [nvarchar](500) NULL,
	[Phone] [nvarchar](50) NULL,
	[Email] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PetroCards]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PetroCards](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[CardNumber] [nvarchar](50) NOT NULL,
	[RFIDTag] [nvarchar](100) NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[Currency] [nvarchar](10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsBlocked] [bit] NOT NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[PinHash] [nvarchar](255) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[LastUsedAt] [datetime2](7) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QueueInformation]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QueueInformation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[FuelStationId] [int] NOT NULL,
	[EstimatedQueueLength] [int] NOT NULL,
	[EstimatedWaitTimeMinutes] [int] NULL,
	[ActivePumps] [int] NOT NULL,
	[RecordedAt] [datetime2](7) NOT NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleClaims]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_RoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StationStatusHistory]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StationStatusHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[FuelStationId] [int] NOT NULL,
	[StatusType] [nvarchar](50) NOT NULL,
	[PreviousStatus] [nvarchar](50) NULL,
	[NewStatus] [nvarchar](50) NOT NULL,
	[Reason] [nvarchar](500) NULL,
	[ExpectedReopenTime] [datetime2](7) NULL,
	[StatusChangedAt] [datetime2](7) NOT NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockMovements]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockMovements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[FuelStationId] [int] NOT NULL,
	[FuelTypeId] [int] NOT NULL,
	[MovementType] [nvarchar](50) NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[Unit] [nvarchar](20) NOT NULL,
	[StockBefore] [decimal](18, 2) NOT NULL,
	[StockAfter] [decimal](18, 2) NOT NULL,
	[ReferenceNumber] [nvarchar](100) NULL,
	[DeliveryNoteNumber] [nvarchar](100) NULL,
	[TankerRegistration] [nvarchar](50) NULL,
	[DriverName] [nvarchar](255) NULL,
	[MovementDate] [datetime2](7) NOT NULL,
	[Notes] [nvarchar](1000) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemConfigurations]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfigurations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[organisation_id] [int] NOT NULL,
	[ConfigurationKey] [nvarchar](255) NOT NULL,
	[ConfigurationValue] [nvarchar](max) NULL,
	[ValueType] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[creator_id] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserClaims]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTokens]    Script Date: 1/26/2026 11:35:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260115190638_Identity', N'8.0.23')
GO
SET IDENTITY_INSERT [dbo].[Organizations] ON 
GO
INSERT [dbo].[Organizations] ([Id], [Name], [Code], [Address], [Phone], [Email], [IsActive], [CreatedAt], [UpdatedAt], [creator_id]) VALUES (1, N'Pindah Private Limited', N'Pindah Private Limited', N'Phase 4 Damofalls Ruwa', N'0714856897', N'admin@pindah.org', 1, CAST(N'2026-01-15T21:06:10.2124923' AS DateTime2), NULL, N'472c129b-bc18-4ece-a4ba-d31c7b2b6cf0')
GO
SET IDENTITY_INSERT [dbo].[Organizations] OFF
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'0980936e-0563-4477-8b21-2c37af05e9c2', N'OrganizationAdmin', N'ORGANIZATIONADMIN', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'2d4770c7-d966-4017-9fac-05a80c2dcec6', N'SuperAdmin', N'SUPERADMIN', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'376ff520-032c-4a05-969e-7c1f1062fc3f', N'FuelManager', N'FUELMANAGER', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'42ae5e27-c9e8-4b72-8531-2d4cb104d58d', N'Customer', N'CUSTOMER', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'4389abcc-0554-40d4-9eb6-b4ec497448e4', N'Maintenance', N'MAINTENANCE', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'5732d4c1-3692-44af-a503-edef09ff36b7', N'CustomerService', N'CUSTOMERSERVICE', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'6320661d-1ec7-4125-8841-4ef47c14d927', N'User', N'USER', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'6451c9e9-39cc-4bfb-9e03-f92e0d5bd4f3', N'CardManager', N'CARDMANAGER', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'7adfa5c1-f2c0-446c-a725-5c9eafcb5ddc', N'SystemAdmin', N'SYSTEMADMIN', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'831aa6e4-34ec-4315-acb5-0f3e41589ad8', N'CardOperator', N'CARDOPERATOR', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'9f09af1c-389a-4cfe-a1aa-30803e15411d', N'StationManager', N'STATIONMANAGER', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'c16b32f9-f38b-4c71-8d95-bf84be2c6427', N'Auditor', N'AUDITOR', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'd0f77d4e-ba8d-48f7-a42f-057456940df3', N'FuelOperator', N'FUELOPERATOR', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'dd7f8e88-6f2c-4825-b0f5-2d4bb0c5023a', N'OrganizationManager', N'ORGANIZATIONMANAGER', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'df81a7c5-f3f1-439b-869b-662e1256a954', N'ReportViewer', N'REPORTVIEWER', NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'ec0702ab-8d8c-4cb9-86b5-de4766196c5a', N'StationOperator', N'STATIONOPERATOR', NULL)
GO
INSERT [dbo].[UserRoles] ([UserId], [RoleId]) VALUES (N'472c129b-bc18-4ece-a4ba-d31c7b2b6cf0', N'0980936e-0563-4477-8b21-2c37af05e9c2')
GO
INSERT [dbo].[UserRoles] ([UserId], [RoleId]) VALUES (N'd8079141-2322-454c-910c-e27b26c1712f', N'42ae5e27-c9e8-4b72-8531-2d4cb104d58d')
GO
INSERT [dbo].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'472c129b-bc18-4ece-a4ba-d31c7b2b6cf0', N'admin@pindah.org', N'ADMIN@PINDAH.ORG', N'admin@pindah.org', N'ADMIN@PINDAH.ORG', 1, N'AQAAAAIAAYagAAAAEKHQb5BgdQvY5Ql78eANWr2xNTXxnh40EQQz+jTmhXNyd9PsTKv6S+/qE1e7+Q85Lg==', N'BUV3TAYYIC47OSNTA264R5TMD32Y2MS3', N'3f023ff1-95d8-41dd-929d-083c25896211', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'd8079141-2322-454c-910c-e27b26c1712f', N'markmubaiwa@gmail.com', N'MARKMUBAIWA@GMAIL.COM', N'markmubaiwa@gmail.com', N'MARKMUBAIWA@GMAIL.COM', 1, N'AQAAAAIAAYagAAAAEEgJhgDzEU6HhG3BtCqhdst4IXTJ3Jvfg7y6Wj4hmNhhY7mBBy0KPYz6hXsyhvoJMg==', N'KNUAHPEJL2OTY2YB3BUAEYO6NPTZOOWL', N'ea170a77-9d0a-4a96-9d1c-25adc75f6095', NULL, 0, 0, NULL, 1, 0)
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditLogs_Action]    Script Date: 1/26/2026 11:35:45 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_Action] ON [dbo].[AuditLogs]
(
	[Action] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuditLogs_CreatedAt]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_CreatedAt] ON [dbo].[AuditLogs]
(
	[CreatedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditLogs_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_creator_id] ON [dbo].[AuditLogs]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuditLogs_EntityId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityId] ON [dbo].[AuditLogs]
(
	[EntityId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditLogs_EntityType]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityType] ON [dbo].[AuditLogs]
(
	[EntityType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuditLogs_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_organisation_id] ON [dbo].[AuditLogs]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditLogs_UserId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Blockcha__D5ABDAFF2DA25C72]    Script Date: 1/26/2026 11:35:46 AM ******/
ALTER TABLE [dbo].[BlockchainTransactions] ADD UNIQUE NONCLUSTERED 
(
	[BlockchainHash] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_BlockchainTransactions_BlockchainHash]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BlockchainTransactions_BlockchainHash] ON [dbo].[BlockchainTransactions]
(
	[BlockchainHash] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BlockchainTransactions_CreatedAt]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BlockchainTransactions_CreatedAt] ON [dbo].[BlockchainTransactions]
(
	[CreatedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_BlockchainTransactions_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BlockchainTransactions_creator_id] ON [dbo].[BlockchainTransactions]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BlockchainTransactions_FuelTransactionId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BlockchainTransactions_FuelTransactionId] ON [dbo].[BlockchainTransactions]
(
	[FuelTransactionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BlockchainTransactions_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BlockchainTransactions_organisation_id] ON [dbo].[BlockchainTransactions]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_BlockchainTransactions_Status]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BlockchainTransactions_Status] ON [dbo].[BlockchainTransactions]
(
	[Status] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CardTransactions_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CardTransactions_creator_id] ON [dbo].[CardTransactions]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CardTransactions_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CardTransactions_organisation_id] ON [dbo].[CardTransactions]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CardTransactions_PetroCardId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CardTransactions_PetroCardId] ON [dbo].[CardTransactions]
(
	[PetroCardId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CardTransactions_TransactionDate]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CardTransactions_TransactionDate] ON [dbo].[CardTransactions]
(
	[TransactionDate] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CardTransactions_TransactionType]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CardTransactions_TransactionType] ON [dbo].[CardTransactions]
(
	[TransactionType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FuelPumps_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelPumps_creator_id] ON [dbo].[FuelPumps]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelPumps_FuelStationId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelPumps_FuelStationId] ON [dbo].[FuelPumps]
(
	[FuelStationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelPumps_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelPumps_organisation_id] ON [dbo].[FuelPumps]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FuelStations_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStations_creator_id] ON [dbo].[FuelStations]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelStations_IsActive]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStations_IsActive] ON [dbo].[FuelStations]
(
	[IsActive] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelStations_IsOpen]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStations_IsOpen] ON [dbo].[FuelStations]
(
	[IsOpen] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelStations_Location]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStations_Location] ON [dbo].[FuelStations]
(
	[Latitude] ASC,
	[Longitude] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelStations_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStations_organisation_id] ON [dbo].[FuelStations]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_FuelStock_Station_FuelType]    Script Date: 1/26/2026 11:35:46 AM ******/
ALTER TABLE [dbo].[FuelStock] ADD  CONSTRAINT [UQ_FuelStock_Station_FuelType] UNIQUE NONCLUSTERED 
(
	[FuelStationId] ASC,
	[FuelTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FuelStock_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStock_creator_id] ON [dbo].[FuelStock]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelStock_FuelStationId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStock_FuelStationId] ON [dbo].[FuelStock]
(
	[FuelStationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelStock_FuelTypeId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStock_FuelTypeId] ON [dbo].[FuelStock]
(
	[FuelTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelStock_LastUpdated]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStock_LastUpdated] ON [dbo].[FuelStock]
(
	[LastUpdated] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelStock_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelStock_organisation_id] ON [dbo].[FuelStock]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__FuelTran__E733A2BF3216A763]    Script Date: 1/26/2026 11:35:46 AM ******/
ALTER TABLE [dbo].[FuelTransactions] ADD UNIQUE NONCLUSTERED 
(
	[TransactionNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FuelTransactions_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_creator_id] ON [dbo].[FuelTransactions]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelTransactions_FuelPumpId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_FuelPumpId] ON [dbo].[FuelTransactions]
(
	[FuelPumpId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelTransactions_FuelStationId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_FuelStationId] ON [dbo].[FuelTransactions]
(
	[FuelStationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelTransactions_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_organisation_id] ON [dbo].[FuelTransactions]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelTransactions_PetroCardId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_PetroCardId] ON [dbo].[FuelTransactions]
(
	[PetroCardId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelTransactions_TransactionDate]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_TransactionDate] ON [dbo].[FuelTransactions]
(
	[TransactionDate] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FuelTransactions_TransactionNumber]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_TransactionNumber] ON [dbo].[FuelTransactions]
(
	[TransactionNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FuelTransactions_TransactionStatus]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_TransactionStatus] ON [dbo].[FuelTransactions]
(
	[TransactionStatus] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FuelTransactions_UserId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTransactions_UserId] ON [dbo].[FuelTransactions]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FuelTypes_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTypes_creator_id] ON [dbo].[FuelTypes]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FuelTypes_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_FuelTypes_organisation_id] ON [dbo].[FuelTypes]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Organizations_Code]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_Organizations_Code] ON [dbo].[Organizations]
(
	[Code] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Organizations_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_Organizations_creator_id] ON [dbo].[Organizations]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__PetroCar__411E34E6E9D8F135]    Script Date: 1/26/2026 11:35:46 AM ******/
ALTER TABLE [dbo].[PetroCards] ADD UNIQUE NONCLUSTERED 
(
	[RFIDTag] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__PetroCar__A4E9FFE9DD1223D2]    Script Date: 1/26/2026 11:35:46 AM ******/
ALTER TABLE [dbo].[PetroCards] ADD UNIQUE NONCLUSTERED 
(
	[CardNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PetroCards_CardNumber]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_PetroCards_CardNumber] ON [dbo].[PetroCards]
(
	[CardNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PetroCards_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_PetroCards_creator_id] ON [dbo].[PetroCards]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PetroCards_IsActive]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_PetroCards_IsActive] ON [dbo].[PetroCards]
(
	[IsActive] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PetroCards_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_PetroCards_organisation_id] ON [dbo].[PetroCards]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PetroCards_RFIDTag]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_PetroCards_RFIDTag] ON [dbo].[PetroCards]
(
	[RFIDTag] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PetroCards_UserId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_PetroCards_UserId] ON [dbo].[PetroCards]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_QueueInformation_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_QueueInformation_creator_id] ON [dbo].[QueueInformation]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QueueInformation_FuelStationId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_QueueInformation_FuelStationId] ON [dbo].[QueueInformation]
(
	[FuelStationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QueueInformation_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_QueueInformation_organisation_id] ON [dbo].[QueueInformation]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QueueInformation_RecordedAt]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_QueueInformation_RecordedAt] ON [dbo].[QueueInformation]
(
	[RecordedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RoleClaims_RoleId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_RoleClaims_RoleId] ON [dbo].[RoleClaims]
(
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[Roles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StationStatusHistory_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StationStatusHistory_creator_id] ON [dbo].[StationStatusHistory]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StationStatusHistory_FuelStationId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StationStatusHistory_FuelStationId] ON [dbo].[StationStatusHistory]
(
	[FuelStationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StationStatusHistory_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StationStatusHistory_organisation_id] ON [dbo].[StationStatusHistory]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StationStatusHistory_StatusChangedAt]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StationStatusHistory_StatusChangedAt] ON [dbo].[StationStatusHistory]
(
	[StatusChangedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StationStatusHistory_StatusType]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StationStatusHistory_StatusType] ON [dbo].[StationStatusHistory]
(
	[StatusType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockMovements_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_creator_id] ON [dbo].[StockMovements]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_FuelStationId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_FuelStationId] ON [dbo].[StockMovements]
(
	[FuelStationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_FuelTypeId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_FuelTypeId] ON [dbo].[StockMovements]
(
	[FuelTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_MovementDate]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_MovementDate] ON [dbo].[StockMovements]
(
	[MovementDate] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockMovements_MovementType]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_MovementType] ON [dbo].[StockMovements]
(
	[MovementType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_organisation_id] ON [dbo].[StockMovements]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_SystemConfigurations_Org_Key]    Script Date: 1/26/2026 11:35:46 AM ******/
ALTER TABLE [dbo].[SystemConfigurations] ADD  CONSTRAINT [UQ_SystemConfigurations_Org_Key] UNIQUE NONCLUSTERED 
(
	[organisation_id] ASC,
	[ConfigurationKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SystemConfigurations_ConfigurationKey]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_SystemConfigurations_ConfigurationKey] ON [dbo].[SystemConfigurations]
(
	[ConfigurationKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SystemConfigurations_creator_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_SystemConfigurations_creator_id] ON [dbo].[SystemConfigurations]
(
	[creator_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SystemConfigurations_organisation_id]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_SystemConfigurations_organisation_id] ON [dbo].[SystemConfigurations]
(
	[organisation_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserClaims_UserId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserClaims_UserId] ON [dbo].[UserClaims]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserLogins_UserId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserLogins_UserId] ON [dbo].[UserLogins]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserRoles_RoleId]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles]
(
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[Users]
(
	[NormalizedEmail] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 1/26/2026 11:35:46 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[Users]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuditLogs] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[BlockchainTransactions] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[BlockchainTransactions] ADD  DEFAULT ((0)) FOR [ConfirmationCount]
GO
ALTER TABLE [dbo].[BlockchainTransactions] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CardTransactions] ADD  DEFAULT ('USD') FOR [Currency]
GO
ALTER TABLE [dbo].[CardTransactions] ADD  DEFAULT (getutcdate()) FOR [TransactionDate]
GO
ALTER TABLE [dbo].[FuelPumps] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[FuelPumps] ADD  DEFAULT ((1)) FOR [IsOperational]
GO
ALTER TABLE [dbo].[FuelPumps] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[FuelStations] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[FuelStations] ADD  DEFAULT ((1)) FOR [IsOpen]
GO
ALTER TABLE [dbo].[FuelStations] ADD  DEFAULT ((0)) FOR [IsTankerOffloading]
GO
ALTER TABLE [dbo].[FuelStations] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[FuelStock] ADD  DEFAULT ((0)) FOR [CurrentQuantity]
GO
ALTER TABLE [dbo].[FuelStock] ADD  DEFAULT ('Litre') FOR [Unit]
GO
ALTER TABLE [dbo].[FuelStock] ADD  DEFAULT (getutcdate()) FOR [LastUpdated]
GO
ALTER TABLE [dbo].[FuelStock] ADD  DEFAULT ((0)) FOR [IsLowStock]
GO
ALTER TABLE [dbo].[FuelStock] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[FuelTransactions] ADD  DEFAULT ('USD') FOR [Currency]
GO
ALTER TABLE [dbo].[FuelTransactions] ADD  DEFAULT ('Completed') FOR [TransactionStatus]
GO
ALTER TABLE [dbo].[FuelTransactions] ADD  DEFAULT (getutcdate()) FOR [TransactionDate]
GO
ALTER TABLE [dbo].[FuelTypes] ADD  DEFAULT ('Litre') FOR [Unit]
GO
ALTER TABLE [dbo].[FuelTypes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[FuelTypes] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Organizations] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Organizations] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PetroCards] ADD  DEFAULT ((0)) FOR [Balance]
GO
ALTER TABLE [dbo].[PetroCards] ADD  DEFAULT ('USD') FOR [Currency]
GO
ALTER TABLE [dbo].[PetroCards] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PetroCards] ADD  DEFAULT ((0)) FOR [IsBlocked]
GO
ALTER TABLE [dbo].[PetroCards] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[QueueInformation] ADD  DEFAULT ((0)) FOR [EstimatedQueueLength]
GO
ALTER TABLE [dbo].[QueueInformation] ADD  DEFAULT ((0)) FOR [ActivePumps]
GO
ALTER TABLE [dbo].[QueueInformation] ADD  DEFAULT (getutcdate()) FOR [RecordedAt]
GO
ALTER TABLE [dbo].[StationStatusHistory] ADD  DEFAULT (getutcdate()) FOR [StatusChangedAt]
GO
ALTER TABLE [dbo].[StockMovements] ADD  DEFAULT ('Litre') FOR [Unit]
GO
ALTER TABLE [dbo].[StockMovements] ADD  DEFAULT (getutcdate()) FOR [MovementDate]
GO
ALTER TABLE [dbo].[SystemConfigurations] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SystemConfigurations] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_Creator] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[AuditLogs] CHECK CONSTRAINT [FK_AuditLogs_Creator]
GO
ALTER TABLE [dbo].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AuditLogs] CHECK CONSTRAINT [FK_AuditLogs_Organizations]
GO
ALTER TABLE [dbo].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[AuditLogs] CHECK CONSTRAINT [FK_AuditLogs_Users]
GO
ALTER TABLE [dbo].[BlockchainTransactions]  WITH CHECK ADD  CONSTRAINT [FK_BlockchainTransactions_FuelTransactions] FOREIGN KEY([FuelTransactionId])
REFERENCES [dbo].[FuelTransactions] ([Id])
GO
ALTER TABLE [dbo].[BlockchainTransactions] CHECK CONSTRAINT [FK_BlockchainTransactions_FuelTransactions]
GO
ALTER TABLE [dbo].[BlockchainTransactions]  WITH CHECK ADD  CONSTRAINT [FK_BlockchainTransactions_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlockchainTransactions] CHECK CONSTRAINT [FK_BlockchainTransactions_Organizations]
GO
ALTER TABLE [dbo].[BlockchainTransactions]  WITH CHECK ADD  CONSTRAINT [FK_BlockchainTransactions_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[BlockchainTransactions] CHECK CONSTRAINT [FK_BlockchainTransactions_Users]
GO
ALTER TABLE [dbo].[CardTransactions]  WITH CHECK ADD  CONSTRAINT [FK_CardTransactions_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CardTransactions] CHECK CONSTRAINT [FK_CardTransactions_Organizations]
GO
ALTER TABLE [dbo].[CardTransactions]  WITH CHECK ADD  CONSTRAINT [FK_CardTransactions_PetroCards] FOREIGN KEY([PetroCardId])
REFERENCES [dbo].[PetroCards] ([Id])
GO
ALTER TABLE [dbo].[CardTransactions] CHECK CONSTRAINT [FK_CardTransactions_PetroCards]
GO
ALTER TABLE [dbo].[CardTransactions]  WITH CHECK ADD  CONSTRAINT [FK_CardTransactions_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[CardTransactions] CHECK CONSTRAINT [FK_CardTransactions_Users]
GO
ALTER TABLE [dbo].[FuelPumps]  WITH CHECK ADD  CONSTRAINT [FK_FuelPumps_FuelStations] FOREIGN KEY([FuelStationId])
REFERENCES [dbo].[FuelStations] ([Id])
GO
ALTER TABLE [dbo].[FuelPumps] CHECK CONSTRAINT [FK_FuelPumps_FuelStations]
GO
ALTER TABLE [dbo].[FuelPumps]  WITH CHECK ADD  CONSTRAINT [FK_FuelPumps_FuelTypes] FOREIGN KEY([FuelTypeId])
REFERENCES [dbo].[FuelTypes] ([Id])
GO
ALTER TABLE [dbo].[FuelPumps] CHECK CONSTRAINT [FK_FuelPumps_FuelTypes]
GO
ALTER TABLE [dbo].[FuelPumps]  WITH CHECK ADD  CONSTRAINT [FK_FuelPumps_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FuelPumps] CHECK CONSTRAINT [FK_FuelPumps_Organizations]
GO
ALTER TABLE [dbo].[FuelPumps]  WITH CHECK ADD  CONSTRAINT [FK_FuelPumps_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[FuelPumps] CHECK CONSTRAINT [FK_FuelPumps_Users]
GO
ALTER TABLE [dbo].[FuelStations]  WITH CHECK ADD  CONSTRAINT [FK_FuelStations_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FuelStations] CHECK CONSTRAINT [FK_FuelStations_Organizations]
GO
ALTER TABLE [dbo].[FuelStations]  WITH CHECK ADD  CONSTRAINT [FK_FuelStations_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[FuelStations] CHECK CONSTRAINT [FK_FuelStations_Users]
GO
ALTER TABLE [dbo].[FuelStock]  WITH CHECK ADD  CONSTRAINT [FK_FuelStock_FuelStations] FOREIGN KEY([FuelStationId])
REFERENCES [dbo].[FuelStations] ([Id])
GO
ALTER TABLE [dbo].[FuelStock] CHECK CONSTRAINT [FK_FuelStock_FuelStations]
GO
ALTER TABLE [dbo].[FuelStock]  WITH CHECK ADD  CONSTRAINT [FK_FuelStock_FuelTypes] FOREIGN KEY([FuelTypeId])
REFERENCES [dbo].[FuelTypes] ([Id])
GO
ALTER TABLE [dbo].[FuelStock] CHECK CONSTRAINT [FK_FuelStock_FuelTypes]
GO
ALTER TABLE [dbo].[FuelStock]  WITH CHECK ADD  CONSTRAINT [FK_FuelStock_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FuelStock] CHECK CONSTRAINT [FK_FuelStock_Organizations]
GO
ALTER TABLE [dbo].[FuelStock]  WITH CHECK ADD  CONSTRAINT [FK_FuelStock_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[FuelStock] CHECK CONSTRAINT [FK_FuelStock_Users]
GO
ALTER TABLE [dbo].[FuelTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FuelTransactions_Creator] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[FuelTransactions] CHECK CONSTRAINT [FK_FuelTransactions_Creator]
GO
ALTER TABLE [dbo].[FuelTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FuelTransactions_FuelPumps] FOREIGN KEY([FuelPumpId])
REFERENCES [dbo].[FuelPumps] ([Id])
GO
ALTER TABLE [dbo].[FuelTransactions] CHECK CONSTRAINT [FK_FuelTransactions_FuelPumps]
GO
ALTER TABLE [dbo].[FuelTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FuelTransactions_FuelStations] FOREIGN KEY([FuelStationId])
REFERENCES [dbo].[FuelStations] ([Id])
GO
ALTER TABLE [dbo].[FuelTransactions] CHECK CONSTRAINT [FK_FuelTransactions_FuelStations]
GO
ALTER TABLE [dbo].[FuelTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FuelTransactions_FuelTypes] FOREIGN KEY([FuelTypeId])
REFERENCES [dbo].[FuelTypes] ([Id])
GO
ALTER TABLE [dbo].[FuelTransactions] CHECK CONSTRAINT [FK_FuelTransactions_FuelTypes]
GO
ALTER TABLE [dbo].[FuelTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FuelTransactions_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FuelTransactions] CHECK CONSTRAINT [FK_FuelTransactions_Organizations]
GO
ALTER TABLE [dbo].[FuelTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FuelTransactions_PetroCards] FOREIGN KEY([PetroCardId])
REFERENCES [dbo].[PetroCards] ([Id])
GO
ALTER TABLE [dbo].[FuelTransactions] CHECK CONSTRAINT [FK_FuelTransactions_PetroCards]
GO
ALTER TABLE [dbo].[FuelTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FuelTransactions_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[FuelTransactions] CHECK CONSTRAINT [FK_FuelTransactions_Users]
GO
ALTER TABLE [dbo].[FuelTypes]  WITH CHECK ADD  CONSTRAINT [FK_FuelTypes_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FuelTypes] CHECK CONSTRAINT [FK_FuelTypes_Organizations]
GO
ALTER TABLE [dbo].[FuelTypes]  WITH CHECK ADD  CONSTRAINT [FK_FuelTypes_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[FuelTypes] CHECK CONSTRAINT [FK_FuelTypes_Users]
GO
ALTER TABLE [dbo].[Organizations]  WITH CHECK ADD  CONSTRAINT [FK_Organizations_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Organizations] CHECK CONSTRAINT [FK_Organizations_Users]
GO
ALTER TABLE [dbo].[PetroCards]  WITH CHECK ADD  CONSTRAINT [FK_PetroCards_Creator] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[PetroCards] CHECK CONSTRAINT [FK_PetroCards_Creator]
GO
ALTER TABLE [dbo].[PetroCards]  WITH CHECK ADD  CONSTRAINT [FK_PetroCards_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PetroCards] CHECK CONSTRAINT [FK_PetroCards_Organizations]
GO
ALTER TABLE [dbo].[PetroCards]  WITH CHECK ADD  CONSTRAINT [FK_PetroCards_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PetroCards] CHECK CONSTRAINT [FK_PetroCards_Users]
GO
ALTER TABLE [dbo].[QueueInformation]  WITH CHECK ADD  CONSTRAINT [FK_QueueInformation_FuelStations] FOREIGN KEY([FuelStationId])
REFERENCES [dbo].[FuelStations] ([Id])
GO
ALTER TABLE [dbo].[QueueInformation] CHECK CONSTRAINT [FK_QueueInformation_FuelStations]
GO
ALTER TABLE [dbo].[QueueInformation]  WITH CHECK ADD  CONSTRAINT [FK_QueueInformation_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QueueInformation] CHECK CONSTRAINT [FK_QueueInformation_Organizations]
GO
ALTER TABLE [dbo].[QueueInformation]  WITH CHECK ADD  CONSTRAINT [FK_QueueInformation_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[QueueInformation] CHECK CONSTRAINT [FK_QueueInformation_Users]
GO
ALTER TABLE [dbo].[RoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoleClaims] CHECK CONSTRAINT [FK_RoleClaims_Roles_RoleId]
GO
ALTER TABLE [dbo].[StationStatusHistory]  WITH CHECK ADD  CONSTRAINT [FK_StationStatusHistory_FuelStations] FOREIGN KEY([FuelStationId])
REFERENCES [dbo].[FuelStations] ([Id])
GO
ALTER TABLE [dbo].[StationStatusHistory] CHECK CONSTRAINT [FK_StationStatusHistory_FuelStations]
GO
ALTER TABLE [dbo].[StationStatusHistory]  WITH CHECK ADD  CONSTRAINT [FK_StationStatusHistory_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StationStatusHistory] CHECK CONSTRAINT [FK_StationStatusHistory_Organizations]
GO
ALTER TABLE [dbo].[StationStatusHistory]  WITH CHECK ADD  CONSTRAINT [FK_StationStatusHistory_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[StationStatusHistory] CHECK CONSTRAINT [FK_StationStatusHistory_Users]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_FuelStations] FOREIGN KEY([FuelStationId])
REFERENCES [dbo].[FuelStations] ([Id])
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_FuelStations]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_FuelTypes] FOREIGN KEY([FuelTypeId])
REFERENCES [dbo].[FuelTypes] ([Id])
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_FuelTypes]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_Organizations]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_Users]
GO
ALTER TABLE [dbo].[SystemConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_SystemConfigurations_Organizations] FOREIGN KEY([organisation_id])
REFERENCES [dbo].[Organizations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SystemConfigurations] CHECK CONSTRAINT [FK_SystemConfigurations_Organizations]
GO
ALTER TABLE [dbo].[SystemConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_SystemConfigurations_Users] FOREIGN KEY([creator_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[SystemConfigurations] CHECK CONSTRAINT [FK_SystemConfigurations_Users]
GO
ALTER TABLE [dbo].[UserClaims]  WITH CHECK ADD  CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserClaims] CHECK CONSTRAINT [FK_UserClaims_Users_UserId]
GO
ALTER TABLE [dbo].[UserLogins]  WITH CHECK ADD  CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserLogins] CHECK CONSTRAINT [FK_UserLogins_Users_UserId]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Roles_RoleId]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Users_UserId]
GO
ALTER TABLE [dbo].[UserTokens]  WITH CHECK ADD  CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserTokens] CHECK CONSTRAINT [FK_UserTokens_Users_UserId]
GO
ALTER DATABASE [FuelManagementSoftware] SET  READ_WRITE 
GO
