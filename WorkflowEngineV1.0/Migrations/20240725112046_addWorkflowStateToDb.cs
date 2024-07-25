using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowEngineV1._0.Migrations
{
    /// <inheritdoc />
    public partial class addWorkflowStateToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Workflows",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Workflows");
        }
    }
}
