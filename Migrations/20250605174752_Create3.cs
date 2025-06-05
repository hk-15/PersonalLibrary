using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PersonalLibrary.Migrations
{
    /// <inheritdoc />
    public partial class Create3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Collections_CollectionID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Translators_TranslatorId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Translators");

            migrationBuilder.DropIndex(
                name: "IX_Books_TranslatorId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TranslatorId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "CollectionID",
                table: "Books",
                newName: "CollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_CollectionID",
                table: "Books",
                newName: "IX_Books_CollectionId");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Authors",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Translator",
                table: "Books",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Collections_CollectionId",
                table: "Books",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "CollectionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Collections_CollectionId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Translator",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "CollectionId",
                table: "Books",
                newName: "CollectionID");

            migrationBuilder.RenameIndex(
                name: "IX_Books_CollectionId",
                table: "Books",
                newName: "IX_Books_CollectionID");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Authors",
                newName: "LastName");

            migrationBuilder.AddColumn<int>(
                name: "TranslatorId",
                table: "Books",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Authors",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Translators",
                columns: table => new
                {
                    TranslatorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translators", x => x.TranslatorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_TranslatorId",
                table: "Books",
                column: "TranslatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Collections_CollectionID",
                table: "Books",
                column: "CollectionID",
                principalTable: "Collections",
                principalColumn: "CollectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Translators_TranslatorId",
                table: "Books",
                column: "TranslatorId",
                principalTable: "Translators",
                principalColumn: "TranslatorId");
        }
    }
}
