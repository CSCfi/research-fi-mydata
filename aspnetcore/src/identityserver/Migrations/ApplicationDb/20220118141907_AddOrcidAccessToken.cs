using Microsoft.EntityFrameworkCore.Migrations;

namespace identityserver.Migrations.ApplicationDb
{
    public partial class AddOrcidAccessToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrcidAccessToken",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrcidAccessToken",
                table: "AspNetUsers");
        }
    }
}
