using Microsoft.Data.Entity.Migrations;

namespace Accursed.Migrations
{
    public partial class FileNormalisation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalisedName",
                table: "File",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "NormalisedName", table: "File");
        }
    }
}
