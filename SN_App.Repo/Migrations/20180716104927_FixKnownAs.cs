using Microsoft.EntityFrameworkCore.Migrations;

namespace SN_App.Repo.Migrations
{
    public partial class FixKnownAs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KnownAS",
                table: "Users",
                newName: "KnownAs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KnownAs",
                table: "Users",
                newName: "KnownAS");
        }
    }
}
