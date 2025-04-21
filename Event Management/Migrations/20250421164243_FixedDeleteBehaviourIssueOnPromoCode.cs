using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Management.Migrations
{
    /// <inheritdoc />
    public partial class FixedDeleteBehaviourIssueOnPromoCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPromoCodes_PromoCodes_PromoCodeId",
                table: "UsedPromoCodes");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPromoCodes_PromoCodes_PromoCodeId",
                table: "UsedPromoCodes",
                column: "PromoCodeId",
                principalTable: "PromoCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPromoCodes_PromoCodes_PromoCodeId",
                table: "UsedPromoCodes");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPromoCodes_PromoCodes_PromoCodeId",
                table: "UsedPromoCodes",
                column: "PromoCodeId",
                principalTable: "PromoCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
