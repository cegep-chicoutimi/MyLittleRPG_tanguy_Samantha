using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG.Migrations
{
    /// <inheritdoc />
    public partial class modifUser2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TempsConexion",
                table: "Utilisateurs",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "TempsConexion",
                table: "Utilisateurs",
                type: "time(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
