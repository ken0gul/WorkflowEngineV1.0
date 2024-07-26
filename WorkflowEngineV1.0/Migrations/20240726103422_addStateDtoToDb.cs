using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowEngineV1._0.Migrations
{
    /// <inheritdoc />
    public partial class addStateDtoToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StateDTO",
                table: "TaskItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateDTO",
                table: "TaskItems");
        }
    }
}
