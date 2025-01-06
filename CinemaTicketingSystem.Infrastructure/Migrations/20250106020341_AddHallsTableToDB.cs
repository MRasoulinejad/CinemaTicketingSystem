using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaTicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHallsTableToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Theatres_TheatreId",
                table: "Seats");

            migrationBuilder.RenameColumn(
                name: "TheatreId",
                table: "Seats",
                newName: "HallId");

            migrationBuilder.RenameIndex(
                name: "IX_Seats_TheatreId",
                table: "Seats",
                newName: "IX_Seats_HallId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Theatres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsReserved",
                table: "Seats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Row",
                table: "Seats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Halls",
                columns: table => new
                {
                    HallId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HallName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TheatreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halls", x => x.HallId);
                    table.ForeignKey(
                        name: "FK_Halls_Theatres_TheatreId",
                        column: x => x.TheatreId,
                        principalTable: "Theatres",
                        principalColumn: "TheatreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Halls_TheatreId",
                table: "Halls",
                column: "TheatreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Halls_HallId",
                table: "Seats",
                column: "HallId",
                principalTable: "Halls",
                principalColumn: "HallId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Halls_HallId",
                table: "Seats");

            migrationBuilder.DropTable(
                name: "Halls");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Theatres");

            migrationBuilder.DropColumn(
                name: "IsReserved",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "Row",
                table: "Seats");

            migrationBuilder.RenameColumn(
                name: "HallId",
                table: "Seats",
                newName: "TheatreId");

            migrationBuilder.RenameIndex(
                name: "IX_Seats_HallId",
                table: "Seats",
                newName: "IX_Seats_TheatreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Theatres_TheatreId",
                table: "Seats",
                column: "TheatreId",
                principalTable: "Theatres",
                principalColumn: "TheatreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
