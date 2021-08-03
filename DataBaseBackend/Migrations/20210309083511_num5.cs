using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseBackend.Migrations
{
    public partial class num5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "mablagh",
                table: "BasketItem",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mablagh",
                table: "BasketItem");
        }
    }
}
