using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Panier_service.Migrations
{
    /// <inheritdoc />
    public partial class New : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Panier",
                columns: table => new
                {
                    Id_client = table.Column<int>(type: "int", nullable: false),
                    Id_produit = table.Column<int>(type: "int", nullable: false),
                    Qunatite = table.Column<int>(type: "int", nullable: false),
                    Prix = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panier", x => new { x.Id_client, x.Id_produit });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Panier");
        }
    }
}
