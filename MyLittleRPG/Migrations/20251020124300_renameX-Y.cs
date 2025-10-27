using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG.Migrations
{
    /// <inheritdoc />
    public partial class renameXY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PositionY",
                table: "Tiles",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "PositionX",
                table: "Tiles",
                newName: "X");

            migrationBuilder.RenameColumn(
                name: "PositionY",
                table: "InstanceMonstres",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "PositionX",
                table: "InstanceMonstres",
                newName: "X");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Y",
                table: "Tiles",
                newName: "PositionY");

            migrationBuilder.RenameColumn(
                name: "X",
                table: "Tiles",
                newName: "PositionX");

            migrationBuilder.RenameColumn(
                name: "Y",
                table: "InstanceMonstres",
                newName: "PositionY");

            migrationBuilder.RenameColumn(
                name: "X",
                table: "InstanceMonstres",
                newName: "PositionX");
        }
    }
}
