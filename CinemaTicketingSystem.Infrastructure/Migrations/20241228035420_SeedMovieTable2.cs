using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaTicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedMovieTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Movies",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "Description", "Duration", "Genre", "Poster", "ReleaseDate", "Title" },
                values: new object[] { 1, "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.", 136, "Action, Sci-Fi", "https://upload.wikimedia.org/wikipedia/en/c/c1/The_Matrix_Poster.jpg", new DateOnly(1999, 3, 1), "The Matrix" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Movies");
        }
    }
}
