using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_project_core.Migrations
{
    /// <inheritdoc />
    public partial class Products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    _id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(555)", maxLength: 555, nullable: true),
                    brand = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    category = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    gender = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    weight = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rating = table.Column<decimal>(type: "decimal(3,1)", precision: 3, scale: 1, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    newPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    trending = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x._id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
