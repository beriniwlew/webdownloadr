using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDownloadr.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddDownloadedPage : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "DownloadedPages",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "TEXT", nullable: false),
          WebPageId = table.Column<Guid>(type: "TEXT", nullable: false),
          Content = table.Column<string>(type: "TEXT", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_DownloadedPages", x => x.Id);
        });
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "DownloadedPages");
  }
}
