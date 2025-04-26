using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedPropertiesInUserForAcquiringPromoCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPromoClaimedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PromoCodeIsClaimable",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPromoClaimedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PromoCodeIsClaimable",
                table: "Users");
        }
    }
}
