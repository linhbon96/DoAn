using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    public partial class UpdateShowtimeSchema : Migration
    {
       protected override void Up(MigrationBuilder migrationBuilder)
{
    // Đổi tên StartTime thành TempDate
    migrationBuilder.RenameColumn(
        name: "StartTime",
        table: "ShowTimes",
        newName: "TempDate");

    // Thêm cột ShowDate và ShowHour
    migrationBuilder.AddColumn<DateTime>(
        name: "ShowDate",
        table: "ShowTimes",
        nullable: true);

    migrationBuilder.AddColumn<TimeSpan>(
        name: "ShowHour",
        table: "ShowTimes",
        type: "interval",
        nullable: false,
        defaultValue: new TimeSpan(0, 0, 0));

    // Chuyển dữ liệu từ TempDate sang ShowDate và ShowHour
    migrationBuilder.Sql(
        @"
        UPDATE ""ShowTimes""
        SET ""ShowDate"" = ""TempDate""::date,
            ""ShowHour"" = ""TempDate""::time
        ");

    // Xóa cột TempDate
    migrationBuilder.DropColumn(
        name: "TempDate",
        table: "ShowTimes");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    // Khôi phục lại cột TempDate
    migrationBuilder.AddColumn<DateTime>(
        name: "TempDate",
        table: "ShowTimes",
        nullable: false,
        defaultValue: DateTime.Now);

    // Sao chép dữ liệu từ ShowDate và ShowHour quay lại TempDate
    migrationBuilder.Sql(
        @"
        UPDATE ""ShowTimes""
        SET ""TempDate"" = ""ShowDate"" + ""ShowHour""::interval
        ");

    // Xóa cột ShowDate và ShowHour
    migrationBuilder.DropColumn(
        name: "ShowDate",
        table: "ShowTimes");

    migrationBuilder.DropColumn(
        name: "ShowHour",
        table: "ShowTimes");

    // Đổi tên TempDate trở lại thành StartTime
    migrationBuilder.RenameColumn(
        name: "TempDate",
        table: "ShowTimes",
        newName: "StartTime");
}

        }
}
