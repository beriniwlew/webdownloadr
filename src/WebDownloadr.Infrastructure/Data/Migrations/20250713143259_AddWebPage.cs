using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDownloadr.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddWebPage : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "WebPages",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "TEXT", nullable: false),
          Url = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
          Status = table.Column<int>(type: "INTEGER", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_WebPages", x => x.Id);
        });
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "WebPages");
  }
}
