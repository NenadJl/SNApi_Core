using Microsoft.EntityFrameworkCore.Migrations;

namespace SN_App.Repo.Migrations
{
    public partial class FixedTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "Values");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Test",
                table: "Values",
                nullable: true);
        }
    }
}
