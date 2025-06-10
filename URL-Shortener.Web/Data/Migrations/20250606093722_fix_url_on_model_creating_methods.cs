using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URL_Shortener.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class fix_url_on_model_creating_methods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortenedURL",
                table: "URLs",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                computedColumnSql: "dbo.Base62Encode([Id])",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "HashedTargetURL",
                table: "URLs",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                computedColumnSql: "CONVERT(varchar(64), HASHBYTES('SHA2_256',[TargetURL]), 2)",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortenedURL",
                table: "URLs",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComputedColumnSql: "dbo.Base62Encode([Id])");

            migrationBuilder.AlterColumn<string>(
                name: "HashedTargetURL",
                table: "URLs",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComputedColumnSql: "CONVERT(varchar(64), HASHBYTES('SHA2_256',[TargetURL]), 2)");
        }
    }
}
