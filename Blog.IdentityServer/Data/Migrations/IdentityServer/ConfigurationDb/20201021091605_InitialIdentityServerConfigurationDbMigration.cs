using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.IdentityServer.Data.Migrations.IdentityServer.ConfigurationDb
{
    public partial class InitialIdentityServerConfigurationDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ids_ApiResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    AllowedAccessTokenSigningAlgorithms = table.Column<string>(maxLength: 100, nullable: true),
                    ShowInDiscoveryDocument = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    LastAccessed = table.Column<DateTime>(nullable: true),
                    NonEditable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ApiResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ApiScopes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    Emphasize = table.Column<bool>(nullable: false),
                    ShowInDiscoveryDocument = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ApiScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ids_Clients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enabled = table.Column<bool>(nullable: false),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    ProtocolType = table.Column<string>(maxLength: 200, nullable: false),
                    RequireClientSecret = table.Column<bool>(nullable: false),
                    ClientName = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    ClientUri = table.Column<string>(maxLength: 2000, nullable: true),
                    LogoUri = table.Column<string>(maxLength: 2000, nullable: true),
                    RequireConsent = table.Column<bool>(nullable: false),
                    AllowRememberConsent = table.Column<bool>(nullable: false),
                    AlwaysIncludeUserClaimsInIdToken = table.Column<bool>(nullable: false),
                    RequirePkce = table.Column<bool>(nullable: false),
                    AllowPlainTextPkce = table.Column<bool>(nullable: false),
                    RequireRequestObject = table.Column<bool>(nullable: false),
                    AllowAccessTokensViaBrowser = table.Column<bool>(nullable: false),
                    FrontChannelLogoutUri = table.Column<string>(maxLength: 2000, nullable: true),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(nullable: false),
                    BackChannelLogoutUri = table.Column<string>(maxLength: 2000, nullable: true),
                    BackChannelLogoutSessionRequired = table.Column<bool>(nullable: false),
                    AllowOfflineAccess = table.Column<bool>(nullable: false),
                    IdentityTokenLifetime = table.Column<int>(nullable: false),
                    AllowedIdentityTokenSigningAlgorithms = table.Column<string>(maxLength: 100, nullable: true),
                    AccessTokenLifetime = table.Column<int>(nullable: false),
                    AuthorizationCodeLifetime = table.Column<int>(nullable: false),
                    ConsentLifetime = table.Column<int>(nullable: true),
                    AbsoluteRefreshTokenLifetime = table.Column<int>(nullable: false),
                    SlidingRefreshTokenLifetime = table.Column<int>(nullable: false),
                    RefreshTokenUsage = table.Column<int>(nullable: false),
                    UpdateAccessTokenClaimsOnRefresh = table.Column<bool>(nullable: false),
                    RefreshTokenExpiration = table.Column<int>(nullable: false),
                    AccessTokenType = table.Column<int>(nullable: false),
                    EnableLocalLogin = table.Column<bool>(nullable: false),
                    IncludeJwtId = table.Column<bool>(nullable: false),
                    AlwaysSendClientClaims = table.Column<bool>(nullable: false),
                    ClientClaimsPrefix = table.Column<string>(maxLength: 200, nullable: true),
                    PairWiseSubjectSalt = table.Column<string>(maxLength: 200, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    LastAccessed = table.Column<DateTime>(nullable: true),
                    UserSsoLifetime = table.Column<int>(nullable: true),
                    UserCodeType = table.Column<string>(maxLength: 100, nullable: true),
                    DeviceCodeLifetime = table.Column<int>(nullable: false),
                    NonEditable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ids_IdentityResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    Emphasize = table.Column<bool>(nullable: false),
                    ShowInDiscoveryDocument = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    NonEditable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_IdentityResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ApiResourceClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(maxLength: 200, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ApiResourceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ApiResourceClaims_Ids_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "Ids_ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ApiResourceProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ApiResourceProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ApiResourceProperties_Ids_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "Ids_ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ApiResourceScopes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(maxLength: 200, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ApiResourceScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ApiResourceScopes_Ids_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "Ids_ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ApiResourceSecrets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Value = table.Column<string>(maxLength: 4000, nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ApiResourceSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ApiResourceSecrets_Ids_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "Ids_ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ApiScopeClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(maxLength: 200, nullable: false),
                    ScopeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ApiScopeClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ApiScopeClaims_Ids_ApiScopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Ids_ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ApiScopeProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ScopeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ApiScopeProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ApiScopeProperties_Ids_ApiScopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Ids_ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 250, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientClaims_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientCorsOrigins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin = table.Column<string>(maxLength: 150, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientCorsOrigins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientCorsOrigins_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientGrantTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrantType = table.Column<string>(maxLength: 250, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientGrantTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientGrantTypes_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientIdPRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Provider = table.Column<string>(maxLength: 200, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientIdPRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientIdPRestrictions_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientPostLogoutRedirectUris",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostLogoutRedirectUri = table.Column<string>(maxLength: 2000, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientPostLogoutRedirectUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientPostLogoutRedirectUris_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientProperties_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientRedirectUris",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RedirectUri = table.Column<string>(maxLength: 2000, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientRedirectUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientRedirectUris_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientScopes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(maxLength: 200, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientScopes_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_ClientSecrets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Value = table.Column<string>(maxLength: 4000, nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_ClientSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_ClientSecrets_Ids_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Ids_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_IdentityResourceClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(maxLength: 200, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_IdentityResourceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_IdentityResourceClaims_Ids_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "Ids_IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ids_IdentityResourceProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ids_IdentityResourceProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ids_IdentityResourceProperties_Ids_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "Ids_IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ApiResourceClaims_ApiResourceId",
                table: "Ids_ApiResourceClaims",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ApiResourceProperties_ApiResourceId",
                table: "Ids_ApiResourceProperties",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ApiResources_Name",
                table: "Ids_ApiResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ApiResourceScopes_ApiResourceId",
                table: "Ids_ApiResourceScopes",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ApiResourceSecrets_ApiResourceId",
                table: "Ids_ApiResourceSecrets",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ApiScopeClaims_ScopeId",
                table: "Ids_ApiScopeClaims",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ApiScopeProperties_ScopeId",
                table: "Ids_ApiScopeProperties",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ApiScopes_Name",
                table: "Ids_ApiScopes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientClaims_ClientId",
                table: "Ids_ClientClaims",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientCorsOrigins_ClientId",
                table: "Ids_ClientCorsOrigins",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientGrantTypes_ClientId",
                table: "Ids_ClientGrantTypes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientIdPRestrictions_ClientId",
                table: "Ids_ClientIdPRestrictions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientPostLogoutRedirectUris_ClientId",
                table: "Ids_ClientPostLogoutRedirectUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientProperties_ClientId",
                table: "Ids_ClientProperties",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientRedirectUris_ClientId",
                table: "Ids_ClientRedirectUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_Clients_ClientId",
                table: "Ids_Clients",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientScopes_ClientId",
                table: "Ids_ClientScopes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_ClientSecrets_ClientId",
                table: "Ids_ClientSecrets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_IdentityResourceClaims_IdentityResourceId",
                table: "Ids_IdentityResourceClaims",
                column: "IdentityResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_IdentityResourceProperties_IdentityResourceId",
                table: "Ids_IdentityResourceProperties",
                column: "IdentityResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ids_IdentityResources_Name",
                table: "Ids_IdentityResources",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ids_ApiResourceClaims");

            migrationBuilder.DropTable(
                name: "Ids_ApiResourceProperties");

            migrationBuilder.DropTable(
                name: "Ids_ApiResourceScopes");

            migrationBuilder.DropTable(
                name: "Ids_ApiResourceSecrets");

            migrationBuilder.DropTable(
                name: "Ids_ApiScopeClaims");

            migrationBuilder.DropTable(
                name: "Ids_ApiScopeProperties");

            migrationBuilder.DropTable(
                name: "Ids_ClientClaims");

            migrationBuilder.DropTable(
                name: "Ids_ClientCorsOrigins");

            migrationBuilder.DropTable(
                name: "Ids_ClientGrantTypes");

            migrationBuilder.DropTable(
                name: "Ids_ClientIdPRestrictions");

            migrationBuilder.DropTable(
                name: "Ids_ClientPostLogoutRedirectUris");

            migrationBuilder.DropTable(
                name: "Ids_ClientProperties");

            migrationBuilder.DropTable(
                name: "Ids_ClientRedirectUris");

            migrationBuilder.DropTable(
                name: "Ids_ClientScopes");

            migrationBuilder.DropTable(
                name: "Ids_ClientSecrets");

            migrationBuilder.DropTable(
                name: "Ids_IdentityResourceClaims");

            migrationBuilder.DropTable(
                name: "Ids_IdentityResourceProperties");

            migrationBuilder.DropTable(
                name: "Ids_ApiResources");

            migrationBuilder.DropTable(
                name: "Ids_ApiScopes");

            migrationBuilder.DropTable(
                name: "Ids_Clients");

            migrationBuilder.DropTable(
                name: "Ids_IdentityResources");
        }
    }
}
