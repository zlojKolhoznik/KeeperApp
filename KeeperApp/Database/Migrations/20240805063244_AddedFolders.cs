using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeeperApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Logins",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "CardCredentials",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IconPath = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    OwnerUsernameHash = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_Folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logins_ParentId",
                table: "Logins",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CardCredentials_ParentId",
                table: "CardCredentials",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ParentId",
                table: "Folders",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardCredentials_Folders_ParentId",
                table: "CardCredentials",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logins_Folders_ParentId",
                table: "Logins",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardCredentials_Folders_ParentId",
                table: "CardCredentials");

            migrationBuilder.DropForeignKey(
                name: "FK_Logins_Folders_ParentId",
                table: "Logins");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Logins_ParentId",
                table: "Logins");

            migrationBuilder.DropIndex(
                name: "IX_CardCredentials_ParentId",
                table: "CardCredentials");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "CardCredentials");
        }
    }
}
