using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaTicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewColToTheatre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Theatres");

            migrationBuilder.AddColumn<string>(
                name: "TheatreImage",
                table: "Theatres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TheatreImage",
                table: "Theatres");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Theatres",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
