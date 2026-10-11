using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteListModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteList_AspNetUsers_UserId",
                table: "FavoriteList");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteList_Cars_CarId",
                table: "FavoriteList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteList",
                table: "FavoriteList");

            migrationBuilder.RenameTable(
                name: "FavoriteList",
                newName: "FavoriteLists");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteList_UserId",
                table: "FavoriteLists",
                newName: "IX_FavoriteLists_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteList_CarId",
                table: "FavoriteLists",
                newName: "IX_FavoriteLists_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteLists",
                table: "FavoriteLists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteLists_AspNetUsers_UserId",
                table: "FavoriteLists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteLists_Cars_CarId",
                table: "FavoriteLists",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteLists_AspNetUsers_UserId",
                table: "FavoriteLists");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteLists_Cars_CarId",
                table: "FavoriteLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteLists",
                table: "FavoriteLists");

            migrationBuilder.RenameTable(
                name: "FavoriteLists",
                newName: "FavoriteList");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteLists_UserId",
                table: "FavoriteList",
                newName: "IX_FavoriteList_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteLists_CarId",
                table: "FavoriteList",
                newName: "IX_FavoriteList_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteList",
                table: "FavoriteList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteList_AspNetUsers_UserId",
                table: "FavoriteList",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteList_Cars_CarId",
                table: "FavoriteList",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
