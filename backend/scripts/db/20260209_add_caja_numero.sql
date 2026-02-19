ALTER TABLE cajas ADD COLUMN IF NOT EXISTS "Numero" varchar(50);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_cajas_TenantId_SucursalId_Numero"
ON cajas ("TenantId", "SucursalId", "Numero")
WHERE "Numero" IS NOT NULL;
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion")
VALUES ('20260209170000_AddCajaNumero','8.0.6')
ON CONFLICT DO NOTHING;
