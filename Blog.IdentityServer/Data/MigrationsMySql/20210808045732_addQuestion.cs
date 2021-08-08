using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.IdentityServer.Data.MigrationsMySql
{
    public partial class addQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstQuestion",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondQuestion",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstQuestion",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SecondQuestion",
                table: "AspNetUsers");
        }
    }
}
