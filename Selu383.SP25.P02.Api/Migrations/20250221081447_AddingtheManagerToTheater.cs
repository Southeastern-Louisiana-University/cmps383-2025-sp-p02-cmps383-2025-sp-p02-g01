using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Selu383.SP25.P02.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingtheManagerToTheater : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Theaters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Theaters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Theaters_ManagerId",
                table: "Theaters",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Theaters_AspNetUsers_ManagerId",
                table: "Theaters",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Theaters_AspNetUsers_ManagerId",
                table: "Theaters");

            migrationBuilder.DropIndex(
                name: "IX_Theaters_ManagerId",
                table: "Theaters");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Theaters");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Theaters",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
