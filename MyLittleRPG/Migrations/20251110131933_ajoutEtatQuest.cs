using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG.Migrations
{
    /// <inheritdoc />
    public partial class ajoutEtatQuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Niveau",
                table: "QuetesNiveauAtteint",
                newName: "NiveauPerso");

            migrationBuilder.AddColumn<int>(
                name: "DistanceX",
                table: "QuetesVisiterTuile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DistanceY",
                table: "QuetesVisiterTuile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NbMonstresVaincu",
                table: "QuetesVaincreMonstres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NiveauAAtteindre",
                table: "QuetesNiveauAtteint",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceX",
                table: "QuetesVisiterTuile");

            migrationBuilder.DropColumn(
                name: "DistanceY",
                table: "QuetesVisiterTuile");

            migrationBuilder.DropColumn(
                name: "NbMonstresVaincu",
                table: "QuetesVaincreMonstres");

            migrationBuilder.DropColumn(
                name: "NiveauAAtteindre",
                table: "QuetesNiveauAtteint");

            migrationBuilder.RenameColumn(
                name: "NiveauPerso",
                table: "QuetesNiveauAtteint",
                newName: "Niveau");
        }
    }
}
