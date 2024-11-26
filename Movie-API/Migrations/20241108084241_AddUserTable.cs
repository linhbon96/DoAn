using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowTimes_Theater_TheaterId",
                table: "ShowTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Theater_TheaterId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Theater",
                table: "Theater");

            migrationBuilder.RenameTable(
                name: "Theater",
                newName: "Theaters");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Items",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "ItemOrders",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Theaters",
                table: "Theaters",
                column: "TheaterId");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "varchar(255)", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", nullable: false),
                    Role = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ShowTimes_Theaters_TheaterId",
                table: "ShowTimes",
                column: "TheaterId",
                principalTable: "Theaters",
                principalColumn: "TheaterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Theaters_TheaterId",
                table: "Tickets",
                column: "TheaterId",
                principalTable: "Theaters",
                principalColumn: "TheaterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowTimes_Theaters_TheaterId",
                table: "ShowTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Theaters_TheaterId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Theaters",
                table: "Theaters");

            migrationBuilder.RenameTable(
                name: "Theaters",
                newName: "Theater");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Items",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "ItemOrders",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Theater",
                table: "Theater",
                column: "TheaterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowTimes_Theater_TheaterId",
                table: "ShowTimes",
                column: "TheaterId",
                principalTable: "Theater",
                principalColumn: "TheaterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Theater_TheaterId",
                table: "Tickets",
                column: "TheaterId",
                principalTable: "Theater",
                principalColumn: "TheaterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
