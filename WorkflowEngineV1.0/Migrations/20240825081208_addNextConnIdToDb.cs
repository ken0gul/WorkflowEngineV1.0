using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowEngineV1._0.Migrations
{
    /// <inheritdoc />
    public partial class addNextConnIdToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NextConnId",
                table: "Connections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextConnId",
                table: "Connections");
        }
    }
}
