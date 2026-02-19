using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Pos.Infrastructure.Persistence;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[DbContext(typeof(PosDbContext))]
[Migration("20260210190000_EnsureStockDeseadoColumn")]
public partial class EnsureStockDeseadoColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "ALTER TABLE producto_stock_config ADD COLUMN IF NOT EXISTS stockdeseado numeric(18,4) NOT NULL DEFAULT 0;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "ALTER TABLE producto_stock_config DROP COLUMN IF EXISTS stockdeseado;");
    }
}
