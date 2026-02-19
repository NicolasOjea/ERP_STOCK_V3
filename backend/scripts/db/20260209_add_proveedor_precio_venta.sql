ALTER TABLE proveedores ADD COLUMN IF NOT EXISTS "Telefono" varchar(50) NOT NULL DEFAULT '';
ALTER TABLE proveedores ADD COLUMN IF NOT EXISTS "Cuit" varchar(20);
ALTER TABLE proveedores ADD COLUMN IF NOT EXISTS "Direccion" varchar(250);
ALTER TABLE productos ADD COLUMN IF NOT EXISTS "PrecioVenta" numeric(18,4) NOT NULL DEFAULT 0;
UPDATE productos SET "PrecioVenta" = "PrecioBase" WHERE "PrecioVenta" = 0;
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion")
VALUES ('20260209150000_AddProveedorFieldsAndPrecioVenta','8.0.6')
ON CONFLICT DO NOTHING;
