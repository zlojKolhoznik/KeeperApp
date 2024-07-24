using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeeperApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlteredLoginModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerUsernameHash",
                table: "Logins",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerUsernameHash",
                table: "Logins");
        }
    }
}
