using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Management.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedTheUniquenessOnParticipantEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participants_EventId_UserId",
                table: "Participants");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_EventId_UserId_TicketId",
                table: "Participants",
                columns: new[] { "EventId", "UserId", "TicketId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participants_EventId_UserId_TicketId",
                table: "Participants");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_EventId_UserId",
                table: "Participants",
                columns: new[] { "EventId", "UserId" },
                unique: true);
        }
    }
}
