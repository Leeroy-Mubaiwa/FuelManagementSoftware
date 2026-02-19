using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelManagementSoftware.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RoleId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Organiza__3214EC0701075EE5", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Changes = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditLog__3214EC0766CE7448", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Creator",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLogs_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FuelStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10, 8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11, 8)", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsOpen = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsTankerOffloading = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FuelStat__3214EC07AA22693D", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelStations_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelStations_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FuelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "Litre"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FuelType__3214EC0785D74B7C", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelTypes_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelTypes_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PetroCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    CardNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RFIDTag = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, defaultValue: "USD"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsBlocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PinHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PetroCar__3214EC0781ADAEA4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetroCards_Creator",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PetroCards_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetroCards_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    ConfigurationKey = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ConfigurationValue = table.Column<string>(type: "TEXT", nullable: true),
                    ValueType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SystemCo__3214EC07E66E31AD", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemConfigurations_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemConfigurations_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QueueInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelStationId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedQueueLength = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedWaitTimeMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    ActivePumps = table.Column<int>(type: "INTEGER", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__QueueInf__3214EC07222CE06D", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueInformation_FuelStations",
                        column: x => x.FuelStationId,
                        principalTable: "FuelStations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QueueInformation_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueueInformation_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StationStatusHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelStationId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PreviousStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ExpectedReopenTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StatusChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StationS__3214EC07C6F93A9A", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationStatusHistory_FuelStations",
                        column: x => x.FuelStationId,
                        principalTable: "FuelStations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StationStatusHistory_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StationStatusHistory_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FuelPumps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelStationId = table.Column<int>(type: "INTEGER", nullable: false),
                    PumpNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FuelTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsOperational = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FuelPump__3214EC0766109EC3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelPumps_FuelStations",
                        column: x => x.FuelStationId,
                        principalTable: "FuelStations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelPumps_FuelTypes",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelPumps_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelPumps_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FuelStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelStationId = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Capacity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "Litre"),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    IsLowStock = table.Column<bool>(type: "INTEGER", nullable: false),
                    LowStockThreshold = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FuelStoc__3214EC07B8E88608", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelStock_FuelStations",
                        column: x => x.FuelStationId,
                        principalTable: "FuelStations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelStock_FuelTypes",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelStock_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelStock_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelStationId = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    MovementType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "Litre"),
                    StockBefore = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StockAfter = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeliveryNoteNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TankerRegistration = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DriverName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MovementDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StockMov__3214EC077571867C", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_FuelStations",
                        column: x => x.FuelStationId,
                        principalTable: "FuelStations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_FuelTypes",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockMovements_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CardTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    PetroCardId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, defaultValue: "USD"),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CardTran__3214EC07015D32F2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardTransactions_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardTransactions_PetroCards",
                        column: x => x.PetroCardId,
                        principalTable: "PetroCards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CardTransactions_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FuelTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FuelStationId = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelPumpId = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PetroCardId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, defaultValue: "USD"),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TransactionStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Completed"),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FuelTran__3214EC0768AD478D", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelTransactions_Creator",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelTransactions_FuelPumps",
                        column: x => x.FuelPumpId,
                        principalTable: "FuelPumps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelTransactions_FuelStations",
                        column: x => x.FuelStationId,
                        principalTable: "FuelStations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelTransactions_FuelTypes",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelTransactions_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelTransactions_PetroCards",
                        column: x => x.PetroCardId,
                        principalTable: "PetroCards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelTransactions_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlockchainTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelTransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    BlockchainHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PreviousHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    BlockNumber = table.Column<long>(type: "INTEGER", nullable: true),
                    TransactionIndex = table.Column<int>(type: "INTEGER", nullable: true),
                    BlockchainNetwork = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SmartContractAddress = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    GasUsed = table.Column<decimal>(type: "decimal(18, 8)", nullable: true),
                    GasPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ConfirmationCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    ConfirmedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Blockcha__3214EC0703A4D565", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockchainTransactions_FuelTransactions",
                        column: x => x.FuelTransactionId,
                        principalTable: "FuelTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlockchainTransactions_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockchainTransactions_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_creator_id",
                table: "AuditLogs",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_organisation_id",
                table: "AuditLogs",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_BlockchainHash",
                table: "BlockchainTransactions",
                column: "BlockchainHash");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_CreatedAt",
                table: "BlockchainTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_creator_id",
                table: "BlockchainTransactions",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_FuelTransactionId",
                table: "BlockchainTransactions",
                column: "FuelTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_organisation_id",
                table: "BlockchainTransactions",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_Status",
                table: "BlockchainTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UQ__Blockcha__D5ABDAFF2DA25C72",
                table: "BlockchainTransactions",
                column: "BlockchainHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_creator_id",
                table: "CardTransactions",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_organisation_id",
                table: "CardTransactions",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_PetroCardId",
                table: "CardTransactions",
                column: "PetroCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_TransactionDate",
                table: "CardTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_TransactionType",
                table: "CardTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPumps_creator_id",
                table: "FuelPumps",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPumps_FuelStationId",
                table: "FuelPumps",
                column: "FuelStationId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPumps_FuelTypeId",
                table: "FuelPumps",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPumps_organisation_id",
                table: "FuelPumps",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStations_creator_id",
                table: "FuelStations",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStations_IsActive",
                table: "FuelStations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStations_IsOpen",
                table: "FuelStations",
                column: "IsOpen");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStations_Location",
                table: "FuelStations",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_FuelStations_organisation_id",
                table: "FuelStations",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStock_creator_id",
                table: "FuelStock",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStock_FuelStationId",
                table: "FuelStock",
                column: "FuelStationId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStock_FuelTypeId",
                table: "FuelStock",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStock_LastUpdated",
                table: "FuelStock",
                column: "LastUpdated");

            migrationBuilder.CreateIndex(
                name: "IX_FuelStock_organisation_id",
                table: "FuelStock",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "UQ_FuelStock_Station_FuelType",
                table: "FuelStock",
                columns: new[] { "FuelStationId", "FuelTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_creator_id",
                table: "FuelTransactions",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_FuelPumpId",
                table: "FuelTransactions",
                column: "FuelPumpId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_FuelStationId",
                table: "FuelTransactions",
                column: "FuelStationId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_FuelTypeId",
                table: "FuelTransactions",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_organisation_id",
                table: "FuelTransactions",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_PetroCardId",
                table: "FuelTransactions",
                column: "PetroCardId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_TransactionDate",
                table: "FuelTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_TransactionNumber",
                table: "FuelTransactions",
                column: "TransactionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_TransactionStatus",
                table: "FuelTransactions",
                column: "TransactionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_UserId",
                table: "FuelTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__FuelTran__E733A2BF3216A763",
                table: "FuelTransactions",
                column: "TransactionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelTypes_creator_id",
                table: "FuelTypes",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTypes_organisation_id",
                table: "FuelTypes",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Code",
                table: "Organizations",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_creator_id",
                table: "Organizations",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_PetroCards_CardNumber",
                table: "PetroCards",
                column: "CardNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PetroCards_creator_id",
                table: "PetroCards",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_PetroCards_IsActive",
                table: "PetroCards",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PetroCards_organisation_id",
                table: "PetroCards",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_PetroCards_RFIDTag",
                table: "PetroCards",
                column: "RFIDTag");

            migrationBuilder.CreateIndex(
                name: "IX_PetroCards_UserId",
                table: "PetroCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__PetroCar__411E34E6E9D8F135",
                table: "PetroCards",
                column: "RFIDTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__PetroCar__A4E9FFE9DD1223D2",
                table: "PetroCards",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QueueInformation_creator_id",
                table: "QueueInformation",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_QueueInformation_FuelStationId",
                table: "QueueInformation",
                column: "FuelStationId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueInformation_organisation_id",
                table: "QueueInformation",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_QueueInformation_RecordedAt",
                table: "QueueInformation",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "([NormalizedName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_StationStatusHistory_creator_id",
                table: "StationStatusHistory",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_StationStatusHistory_FuelStationId",
                table: "StationStatusHistory",
                column: "FuelStationId");

            migrationBuilder.CreateIndex(
                name: "IX_StationStatusHistory_organisation_id",
                table: "StationStatusHistory",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_StationStatusHistory_StatusChangedAt",
                table: "StationStatusHistory",
                column: "StatusChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StationStatusHistory_StatusType",
                table: "StationStatusHistory",
                column: "StatusType");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_creator_id",
                table: "StockMovements",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_FuelStationId",
                table: "StockMovements",
                column: "FuelStationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_FuelTypeId",
                table: "StockMovements",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_MovementDate",
                table: "StockMovements",
                column: "MovementDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_MovementType",
                table: "StockMovements",
                column: "MovementType");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_organisation_id",
                table: "StockMovements",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigurations_ConfigurationKey",
                table: "SystemConfigurations",
                column: "ConfigurationKey");

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigurations_creator_id",
                table: "SystemConfigurations",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigurations_organisation_id",
                table: "SystemConfigurations",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "UQ_SystemConfigurations_Org_Key",
                table: "SystemConfigurations",
                columns: new[] { "organisation_id", "ConfigurationKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "([NormalizedUserName] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BlockchainTransactions");

            migrationBuilder.DropTable(
                name: "CardTransactions");

            migrationBuilder.DropTable(
                name: "FuelStock");

            migrationBuilder.DropTable(
                name: "QueueInformation");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "StationStatusHistory");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "SystemConfigurations");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "FuelTransactions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "FuelPumps");

            migrationBuilder.DropTable(
                name: "PetroCards");

            migrationBuilder.DropTable(
                name: "FuelStations");

            migrationBuilder.DropTable(
                name: "FuelTypes");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
