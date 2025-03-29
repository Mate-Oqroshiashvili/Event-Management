using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedPurchaseOnRelatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseId",
                table: "Participants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_PurchaseId",
                table: "Participants",
                column: "PurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Purchases_PurchaseId",
                table: "Participants",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Purchases_PurchaseId",
                table: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_Participants_PurchaseId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "PurchaseId",
                table: "Participants");
        }
    }
}
