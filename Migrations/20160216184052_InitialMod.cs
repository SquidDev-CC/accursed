using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Accursed.Migrations
{
    public partial class InitialMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    VersionRefresh = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mod", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "ModVersion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DownloadId = table.Column<uint>(nullable: false),
                    FancyName = table.Column<string>(nullable: true),
                    MCVersion = table.Column<string>(nullable: true),
                    ModId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModVersion_Mod_ModId",
                        column: x => x.ModId,
                        principalTable: "Mod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DownloadId = table.Column<uint>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    VersionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_ModVersion_VersionId",
                        column: x => x.VersionId,
                        principalTable: "ModVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("File");
            migrationBuilder.DropTable("ModVersion");
            migrationBuilder.DropTable("Mod");
        }
    }
}
