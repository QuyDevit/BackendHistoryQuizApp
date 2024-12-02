using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoryQuizApp.Migrations
{
    /// <inheritdoc />
    public partial class update_db_021224_L3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Test",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Test_GradeId",
                table: "Test",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Test_Grade_GradeId",
                table: "Test",
                column: "GradeId",
                principalTable: "Grade",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Test_Grade_GradeId",
                table: "Test");

            migrationBuilder.DropIndex(
                name: "IX_Test_GradeId",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Test");
        }
    }
}
