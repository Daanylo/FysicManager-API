using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FysicManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueryString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    ResponseTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    ResponseSize = table.Column<int>(type: "int", nullable: true),
                    DataSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLogs");
        }
    }
}
