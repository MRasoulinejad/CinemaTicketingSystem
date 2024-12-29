using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaTicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TarilUrlAddedToMovies2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrailerUrl",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1,
                column: "TrailerUrl",
                value: "https://www.youtube.com/embed/m8e-FF8MsqU");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrailerUrl",
                table: "Movies");
        }
    }
}
