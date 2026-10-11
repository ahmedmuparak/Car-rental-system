using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCarBranchRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Branches_BranchId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Branches_BranchId1",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_BranchId1",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "BranchId1",
                table: "Cars");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Branches_BranchId",
                table: "Cars",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Branches_BranchId",
                table: "Cars");

            migrationBuilder.AddColumn<int>(
                name: "BranchId1",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_BranchId1",
                table: "Cars",
                column: "BranchId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Branches_BranchId",
                table: "Cars",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Branches_BranchId1",
                table: "Cars",
                column: "BranchId1",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
