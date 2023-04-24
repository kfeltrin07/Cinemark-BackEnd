using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Filmovi_projekt.Migrations.Comments
{
    /// <inheritdoc />
    public partial class CommentsCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id_comment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_user = table.Column<int>(type: "int", nullable: false),
                    id_film = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "varchar(255)", nullable: false),
                    change_date = table.Column<string>(type: "varchar(255)", nullable: false),
                    insert_date = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.id_comment);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
