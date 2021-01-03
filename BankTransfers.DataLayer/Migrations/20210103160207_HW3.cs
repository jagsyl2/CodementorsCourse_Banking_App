using Microsoft.EntityFrameworkCore.Migrations;

namespace BankTransfers.DataLayer.Migrations
{
    public partial class HW3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypOfTransfer",
                table: "Transfers",
                newName: "TypeOfTransfer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeOfTransfer",
                table: "Transfers",
                newName: "TypOfTransfer");
        }
    }
}
