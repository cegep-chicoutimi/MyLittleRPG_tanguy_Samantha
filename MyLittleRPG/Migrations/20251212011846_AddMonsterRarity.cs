using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG.Migrations
{
    /// <inheritdoc />
    public partial class AddMonsterRarity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rarity",
                table: "Monsters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rarity",
                table: "Monsters");
        }
    }
}
