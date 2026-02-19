using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260212100000_AddVentaNumeroAndStockMovVentaNumero")]
public partial class AddVentaNumeroAndStockMovVentaNumero : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE SEQUENCE IF NOT EXISTS venta_numero_seq;");
        migrationBuilder.Sql("DO $$ BEGIN IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ventas' AND column_name='Numero') THEN EXECUTE 'ALTER TABLE ventas RENAME COLUMN \"Numero\" TO numero'; END IF; END $$;");
        migrationBuilder.Sql("ALTER TABLE ventas ADD COLUMN IF NOT EXISTS numero bigint;");
        migrationBuilder.Sql("ALTER TABLE ventas ALTER COLUMN numero SET DEFAULT nextval('venta_numero_seq');");
        migrationBuilder.Sql("UPDATE ventas SET numero = nextval('venta_numero_seq') WHERE numero IS NULL OR numero = 0;");
        migrationBuilder.Sql("CREATE UNIQUE INDEX IF NOT EXISTS \"IX_ventas_TenantId_Numero\" ON ventas (\"TenantId\", numero);");

        migrationBuilder.Sql("ALTER TABLE stock_movimientos ADD COLUMN IF NOT EXISTS \"VentaNumero\" bigint;");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_stock_movimientos_TenantId_VentaNumero\" ON stock_movimientos (\"TenantId\", \"VentaNumero\");");

        migrationBuilder.Sql("DO $$ BEGIN IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='producto_stock_config' AND column_name='StockDeseado') THEN EXECUTE 'ALTER TABLE producto_stock_config RENAME COLUMN \"StockDeseado\" TO stockdeseado'; END IF; END $$;");
        migrationBuilder.Sql("ALTER TABLE producto_stock_config ADD COLUMN IF NOT EXISTS stockdeseado numeric(18,4) NOT NULL DEFAULT 0;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_stock_movimientos_TenantId_VentaNumero\";");
        migrationBuilder.Sql("ALTER TABLE stock_movimientos DROP COLUMN IF EXISTS \"VentaNumero\";");

        migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_ventas_TenantId_Numero\";");
        migrationBuilder.Sql("ALTER TABLE ventas ALTER COLUMN numero DROP DEFAULT;");
        migrationBuilder.Sql("ALTER TABLE ventas DROP COLUMN IF EXISTS numero;");
        migrationBuilder.Sql("DROP SEQUENCE IF EXISTS venta_numero_seq;");
    }
}
