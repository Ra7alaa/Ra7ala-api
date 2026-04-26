using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Passengers_PassengerId1",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Buses_BusId1",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_BusId1",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PassengerId1",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BusId1",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "PassengerId1",
                table: "Tickets");

            migrationBuilder.AddColumn<string>(
                name: "SuperAdminUserName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuperAdminUserName",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "BusId1",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassengerId1",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_BusId1",
                table: "Trips",
                column: "BusId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PassengerId1",
                table: "Tickets",
                column: "PassengerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Passengers_PassengerId1",
                table: "Tickets",
                column: "PassengerId1",
                principalTable: "Passengers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Buses_BusId1",
                table: "Trips",
                column: "BusId1",
                principalTable: "Buses",
                principalColumn: "Id");
        }
    }
}
