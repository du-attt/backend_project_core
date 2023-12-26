using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_project_core.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceDetailMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceDetail",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idInvoice = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    _idProduct = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetail", x => x.id);
                    table.ForeignKey(
                        name: "FK_InvoiceDetail_Invoice",
                        column: x => x.idInvoice,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Product",
                        column: x => x._idProduct,
                        principalTable: "Products",
                        principalColumn: "_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetail__idProduct",
                table: "InvoiceDetail",
                column: "_idProduct");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetail_idInvoice",
                table: "InvoiceDetail",
                column: "idInvoice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceDetail");
        }
    }
}
