using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedAndModifiedPropertiesInModelsAsWellAsFunctionalities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QRCodeUrl",
                table: "Tickets",
                newName: "QRCodeImageUrl");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "QRCodeData",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SaleAmountInPercentages",
                table: "PromoCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeData",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SaleAmountInPercentages",
                table: "PromoCodes");

            migrationBuilder.RenameColumn(
                name: "QRCodeImageUrl",
                table: "Tickets",
                newName: "QRCodeUrl");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
