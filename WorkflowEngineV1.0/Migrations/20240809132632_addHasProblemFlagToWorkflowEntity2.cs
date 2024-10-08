﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowEngineV1._0.Migrations
{
    /// <inheritdoc />
    public partial class addHasProblemFlagToWorkflowEntity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "hasProblem",
                table: "Workflows",
                newName: "HasProblem");

            migrationBuilder.AddColumn<string>(
                name: "ProblemTaskId",
                table: "Workflows",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProblemTaskId",
                table: "Workflows");

            migrationBuilder.RenameColumn(
                name: "HasProblem",
                table: "Workflows",
                newName: "hasProblem");
        }
    }
}
