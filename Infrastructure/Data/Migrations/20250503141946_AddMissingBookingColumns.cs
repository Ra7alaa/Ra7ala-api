using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingBookingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TicketCode",
                table: "Tickets",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EndStationId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTickets",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "StartStationId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketCode",
                table: "Tickets",
                column: "TicketCode");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EndStationId",
                table: "Bookings",
                column: "EndStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StartStationId",
                table: "Bookings",
                column: "StartStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Stations_EndStationId",
                table: "Bookings",
                column: "EndStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Stations_StartStationId",
                table: "Bookings",
                column: "StartStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Stations_EndStationId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Stations_StartStationId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketCode",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_EndStationId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_StartStationId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TicketCode",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EndStationId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "NumberOfTickets",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StartStationId",
                table: "Bookings");
        }
    }
}
