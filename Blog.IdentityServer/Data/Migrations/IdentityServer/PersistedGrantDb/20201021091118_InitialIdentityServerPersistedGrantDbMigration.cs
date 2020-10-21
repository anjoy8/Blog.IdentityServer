using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.IdentityServer.Data.Migrations.IdentityServer.PersistedGrantDb
{
    public partial class InitialIdentityServerPersistedGrantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ids_DeviceCodes",
                columns: table => new
                {
                    UserCode = table.Column<string>(maxLength: 200, nullable: false),
                    DeviceCode = table.Column<string>(maxLength: 200, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    SessionId = table.Column<string>(maxLength: 100, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_DeviceCodes", x => x.UserCode);
                });

            migrationBuilder.CreateTable(
                name: "Ids_PersistedGrants",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    SessionId = table.Column<string>(maxLength: 100, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    ConsumedTime = table.Column<DateTime>(nullable: true),
                    Data = table.Column<string>(maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_PersistedGrants", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ids_DeviceCodes_DeviceCode",
                table: "Ids_DeviceCodes",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ids_DeviceCodes_Expiration",
                table: "Ids_DeviceCodes",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_PersistedGrants_Expiration",
                table: "Ids_PersistedGrants",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_PersistedGrants_SubjectId_ClientId_Type",
                table: "Ids_PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Ids_PersistedGrants_SubjectId_SessionId_Type",
                table: "Ids_PersistedGrants",
                columns: new[] { "SubjectId", "SessionId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ids_DeviceCodes");

            migrationBuilder.DropTable(
                name: "Ids_PersistedGrants");
        }
    }
}
