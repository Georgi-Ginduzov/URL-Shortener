using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URL_Shortener.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_base62_convertion_function : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            CREATE OR ALTER FUNCTION dbo.Base62Encode (@n BIGINT)
            RETURNS VARCHAR(11)
            WITH SCHEMABINDING
            AS BEGIN
                DECLARE @abc CHAR(62) =
                    '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
                        @s  VARCHAR(11) = '';
                IF (@n = 0) RETURN '0';
                WHILE @n > 0
                BEGIN
                    SET @s = SUBSTRING(@abc, (@n % 62)+1, 1) + @s;
                    SET @n = @n / 62;
                END
                RETURN @s;
            END
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.Base62Encode");
        }
    }
}
