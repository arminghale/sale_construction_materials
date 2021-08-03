using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseBackend.Migrations
{
    public partial class num6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "seen",
                table: "Product",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "seen",
                table: "Product");
        }
    }
}
