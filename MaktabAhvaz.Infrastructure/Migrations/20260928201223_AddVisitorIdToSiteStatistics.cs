using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaktabAhvaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitorIdToSiteStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "SiteStatistics",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "SiteStatistics");
        }
    }
}
