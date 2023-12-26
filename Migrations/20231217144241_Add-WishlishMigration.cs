using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_project_core.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlishMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.id);
                    table.ForeignKey(
                        name: "FK_Wishlish_User",
                        column: x => x.idUser,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishlishDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idWishlish = table.Column<int>(type: "int", nullable: false),
                    _idProduct = table.Column<string>(type: "nvarchar(256)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlishDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_WishlishDetails_Cart",
                        column: x => x.idWishlish,
                        principalTable: "Wishlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WishlishDetails_Product",
                        column: x => x._idProduct,
                        principalTable: "Products",
                        principalColumn: "_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WishlishDetails__idProduct",
                table: "WishlishDetails",
                column: "_idProduct");

            migrationBuilder.CreateIndex(
                name: "IX_WishlishDetails_idWishlish",
                table: "WishlishDetails",
                column: "idWishlish");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_idUser",
                table: "Wishlists",
                column: "idUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WishlishDetails");

            migrationBuilder.DropTable(
                name: "Wishlists");
        }
    }
}
