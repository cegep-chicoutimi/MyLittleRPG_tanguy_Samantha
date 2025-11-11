using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG.Migrations
{
    /// <inheritdoc />
    public partial class renameXY2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PositionY",
                table: "Personnages",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "PositionX",
                table: "Personnages",
                newName: "X");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Y",
                table: "Personnages",
                newName: "PositionY");

            migrationBuilder.RenameColumn(
                name: "X",
                table: "Personnages",
                newName: "PositionX");
        }
    }
}
