using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFreeSizeAndReorderSizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add "Free Size" if it doesn't exist
            migrationBuilder.Sql(@"
                INSERT INTO ""Sizes"" (""Name"", ""SortOrder"")
                SELECT 'Free Size', 0
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""Sizes"" WHERE ""Name"" = 'Free Size'
                );
            ");

            // Update SortOrder for existing sizes to make room for Free Size
            migrationBuilder.Sql(@"
                UPDATE ""Sizes"" 
                SET ""SortOrder"" = 
                    CASE 
                        WHEN ""Name"" = 'Free Size' THEN 0
                        WHEN ""Name"" = 'S' THEN 1
                        WHEN ""Name"" = 'M' THEN 2
                        WHEN ""Name"" = 'L' THEN 3
                        WHEN ""Name"" = 'XL' THEN 4
                        WHEN ""Name"" = 'XXL' THEN 5
                        ELSE ""SortOrder"" + 1
                    END;
            ");

            // Update the PostgreSQL sequence to avoid conflicts
            migrationBuilder.Sql(@"
                SELECT setval(pg_get_serial_sequence('""Sizes""', 'Id'), (SELECT COALESCE(MAX(""Id""), 0) FROM ""Sizes""));
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove "Free Size" and restore original SortOrder
            migrationBuilder.Sql(@"
                DELETE FROM ""Sizes"" WHERE ""Name"" = 'Free Size';
            ");

            // Restore original SortOrder for remaining sizes
            migrationBuilder.Sql(@"
                UPDATE ""Sizes"" 
                SET ""SortOrder"" = 
                    CASE 
                        WHEN ""Name"" = 'S' THEN 0
                        WHEN ""Name"" = 'M' THEN 1
                        WHEN ""Name"" = 'L' THEN 2
                        WHEN ""Name"" = 'XL' THEN 3
                        WHEN ""Name"" = 'XXL' THEN 4
                        ELSE ""SortOrder""
                    END
                WHERE ""Name"" IN ('S', 'M', 'L', 'XL', 'XXL');
            ");
        }
    }
}
