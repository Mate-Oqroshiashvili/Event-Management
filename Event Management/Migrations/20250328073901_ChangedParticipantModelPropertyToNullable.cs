using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Management.Migrations
{
    /// <inheritdoc />
    public partial class ChangedParticipantModelPropertyToNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participants_EventId_UserId_TicketId_Id",
                table: "Participants");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "Participants",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_EventId_UserId_TicketId_Id",
                table: "Participants",
                columns: new[] { "EventId", "UserId", "TicketId", "Id" },
                unique: true,
                filter: "[TicketId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participants_EventId_UserId_TicketId_Id",
                table: "Participants");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "Participants",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_EventId_UserId_TicketId_Id",
                table: "Participants",
                columns: new[] { "EventId", "UserId", "TicketId", "Id" },
                unique: true);
        }
    }
}
