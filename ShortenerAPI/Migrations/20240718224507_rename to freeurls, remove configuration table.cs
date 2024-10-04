﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShortenerAPI.Migrations
{
    /// <inheritdoc />
    public partial class renametofreeurlsremoveconfigurationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Urls",
                table: "Urls");

            migrationBuilder.RenameTable(
                name: "Urls",
                newName: "FreeUrls");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FreeUrls",
                table: "FreeUrls",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FreeUrls",
                table: "FreeUrls");

            migrationBuilder.RenameTable(
                name: "FreeUrls",
                newName: "Urls");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Urls",
                table: "Urls",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });
        }
    }
}
