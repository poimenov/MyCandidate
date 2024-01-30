using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCandidate.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddVacancyResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VacancyResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    VacancyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourceTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyResources_ResourceTypes_ResourceTypeId",
                        column: x => x.ResourceTypeId,
                        principalTable: "ResourceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyResources_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacancyResources_ResourceTypeId",
                table: "VacancyResources",
                column: "ResourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyResources_VacancyId",
                table: "VacancyResources",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacancyResources");
        }
    }
}
