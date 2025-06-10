using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URL_Shortener.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_core_entity_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "URLs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ShortenedURL = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    HashedTargetURL = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TargetURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnalitycsMode = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_URLs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_URLs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_URLs_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClickDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlId = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAdress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClickDetails_URLs_UrlId",
                        column: x => x.UrlId,
                        principalTable: "URLs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClickDetails_UrlId",
                table: "ClickDetails",
                column: "UrlId");

            migrationBuilder.CreateIndex(
                name: "IX_URLs_SessionId_HashedTargetURL",
                table: "URLs",
                columns: new[] { "SessionId", "HashedTargetURL" },
                unique: true,
                filter: "[UserId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_URLs_UserId_HashedTargetURL",
                table: "URLs",
                columns: new[] { "UserId", "HashedTargetURL" },
                unique: true,
                filter: "[SessionId] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClickDetails");

            migrationBuilder.DropTable(
                name: "URLs");

            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
