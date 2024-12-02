using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoryQuizApp.Migrations
{
    /// <inheritdoc />
    public partial class update_db_021224 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Questions");
        }
    }
}
