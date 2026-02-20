using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelManagementSoftware.Migrations
{
    [Migration("20260219120000_MakeFuelTypeGlobal")]
    public partial class MakeFuelTypeGlobal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = 0;");

            // SQLite: recreate FuelTypes without organisation_id
            migrationBuilder.CreateTable(
                name: "FuelTypes_new",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "Litre"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelTypes_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.Sql(@"
                INSERT INTO FuelTypes_new (Id, Name, Code, Description, UnitPrice, Unit, IsActive, CreatedAt, UpdatedAt, creator_id)
                SELECT Id, Name, Code, Description, UnitPrice, Unit, IsActive, CreatedAt, UpdatedAt, creator_id FROM FuelTypes;
            ");

            migrationBuilder.DropTable(name: "FuelTypes");
            migrationBuilder.Sql("ALTER TABLE FuelTypes_new RENAME TO FuelTypes;");
            migrationBuilder.CreateIndex(name: "IX_FuelTypes_creator_id", table: "FuelTypes", column: "creator_id");

            migrationBuilder.Sql("PRAGMA foreign_keys = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_FuelTypes_creator_id", table: "FuelTypes");
            migrationBuilder.RenameTable(name: "FuelTypes", newName: "FuelTypes_old");

            migrationBuilder.CreateTable(
                name: "FuelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "Litre"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'utc')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    creator_id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    organisation_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelTypes_Users",
                        column: x => x.creator_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelTypes_Organizations",
                        column: x => x.organisation_id,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                INSERT INTO FuelTypes (Id, Name, Code, Description, UnitPrice, Unit, IsActive, CreatedAt, UpdatedAt, creator_id, organisation_id)
                SELECT Id, Name, Code, Description, UnitPrice, Unit, IsActive, CreatedAt, UpdatedAt, COALESCE(creator_id, ''), 1 FROM FuelTypes_old;
            ");
            migrationBuilder.DropTable(name: "FuelTypes_old");
            migrationBuilder.CreateIndex(name: "IX_FuelTypes_creator_id", table: "FuelTypes", column: "creator_id");
            migrationBuilder.CreateIndex(name: "IX_FuelTypes_organisation_id", table: "FuelTypes", column: "organisation_id");
        }
    }
}
