using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatToTicketInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemOrders",
                table: "ItemOrders");

            migrationBuilder.DropIndex(
                name: "IX_ItemOrders_OrderId",
                table: "ItemOrders");

            migrationBuilder.DropColumn(
                name: "ItemOrderId",
                table: "ItemOrders");

            migrationBuilder.AddColumn<int>(
                name: "SeatId",
                table: "TicketInfos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemOrders",
                table: "ItemOrders",
                columns: new[] { "OrderId", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_MovieId",
                table: "Tickets",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TheaterId",
                table: "Tickets",
                column: "TheaterId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfos_SeatId",
                table: "TicketInfos",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfos_Seats_SeatId",
                table: "TicketInfos",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Movies_MovieId",
                table: "Tickets",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "MovieId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Theaters_TheaterId",
                table: "Tickets",
                column: "TheaterId",
                principalTable: "Theaters",
                principalColumn: "TheaterId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfos_Seats_SeatId",
                table: "TicketInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Movies_MovieId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Theaters_TheaterId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_MovieId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TheaterId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfos_SeatId",
                table: "TicketInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemOrders",
                table: "ItemOrders");

            migrationBuilder.DropColumn(
                name: "SeatId",
                table: "TicketInfos");

            migrationBuilder.AddColumn<int>(
                name: "ItemOrderId",
                table: "ItemOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemOrders",
                table: "ItemOrders",
                column: "ItemOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrders_OrderId",
                table: "ItemOrders",
                column: "OrderId");
        }
    }
}
