using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AggregationService.Migrations
{
    /// <inheritdoc />
    public partial class AddDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregatedResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PULocationID = table.Column<int>(type: "int", nullable: false),
                    Quarter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TripCount = table.Column<int>(type: "int", nullable: false),
                    TotalFareAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AverageTripDistance = table.Column<double>(type: "float", nullable: false),
                    AggregationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatedResults");
        }
    }
}
