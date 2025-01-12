using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaTicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HallIdAddedToShowTimeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HallId",
                table: "ShowTimes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HallId",
                table: "ShowTimes");
        }
    }
}
