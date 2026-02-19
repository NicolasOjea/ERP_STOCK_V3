-- Seed completo (demo) para entorno local
-- Recomendado: ejecutar despues de reset.sql + init.sql

BEGIN;

-- =========================
-- 0) Constantes
-- =========================
-- Tenant/Sucursal
-- Tenant demo
-- 1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01
-- Sucursal central
-- 3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1

-- =========================
-- 1) Seguridad base
-- =========================
INSERT INTO tenants ("Id","Name","IsActive","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01','Demo',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO sucursales ("Id","Name","Code","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','Central','CENTRAL','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('4b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a2','Sucursal 2','SUC2','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO usuarios ("Id","Username","PasswordHash","IsActive","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b','admin','sha256:8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('6ca96f7d-6a24-4f9f-a7f9-6f8c3c11c101','cajero1','sha256:8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('6ca96f7d-6a24-4f9f-a7f9-6f8c3c11c102','encargado1','sha256:8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO roles ("Id","Name","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01','CAJERO','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02','ENCARGADO','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03','ADMIN','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO permisos ("Id","Code","Description","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001','VENTA_CREAR','Crear venta','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002','VENTA_CONFIRMAR','Confirmar venta','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a003','VENTA_ANULAR','Anular venta','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a004','DEVOLUCION_REGISTRAR','Registrar devolucion','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a005','CAJA_ABRIR','Abrir caja','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a006','CAJA_CERRAR','Cerrar caja','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a007','STOCK_AJUSTAR','Ajustar stock','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a008','PRODUCTO_VER','Ver productos','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a009','PRODUCTO_EDITAR','Editar productos','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a010','USUARIO_ADMIN','Administrar usuarios','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a011','REPORTES_VER','Ver reportes','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a012','CLIENTE_GESTIONAR','Gestionar clientes','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a013','PROVEEDOR_GESTIONAR','Gestionar proveedores','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a014','COMPRAS_REGISTRAR','Registrar compras','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a015','CONFIGURACION_VER','Ver configuracion','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a016','CAJA_MOVIMIENTO','Movimientos de caja','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO usuario_roles ("Id","UserId","RoleId","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('f19b3d0a-92f1-4b7d-9d3f-05b0f8f1a101','c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b','e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f19b3d0a-92f1-4b7d-9d3f-05b0f8f1a102','6ca96f7d-6a24-4f9f-a7f9-6f8c3c11c101','5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f19b3d0a-92f1-4b7d-9d3f-05b0f8f1a103','6ca96f7d-6a24-4f9f-a7f9-6f8c3c11c102','9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO rol_permisos ("Id","RoleId","PermissionId","TenantId","CreatedAt","UpdatedAt","DeletedAt")
SELECT gen_random_uuid(), 'e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03', p."Id", '1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01', NOW(), NOW(), NULL
FROM permisos p
WHERE p."TenantId" = '1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01';

INSERT INTO rol_permisos ("Id","RoleId","PermissionId","TenantId","CreatedAt","UpdatedAt","DeletedAt")
SELECT gen_random_uuid(), '5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01', p."Id", '1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01', NOW(), NOW(), NULL
FROM permisos p
WHERE p."Code" IN ('VENTA_CREAR','VENTA_CONFIRMAR','CAJA_ABRIR','CAJA_CERRAR','CAJA_MOVIMIENTO','PRODUCTO_VER');

INSERT INTO rol_permisos ("Id","RoleId","PermissionId","TenantId","CreatedAt","UpdatedAt","DeletedAt")
SELECT gen_random_uuid(), '9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02', p."Id", '1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01', NOW(), NOW(), NULL
FROM permisos p
WHERE p."Code" IN ('VENTA_CREAR','VENTA_CONFIRMAR','VENTA_ANULAR','STOCK_AJUSTAR','PRODUCTO_VER','PRODUCTO_EDITAR','PROVEEDOR_GESTIONAR','COMPRAS_REGISTRAR','REPORTES_VER','CAJA_MOVIMIENTO');

-- =========================
-- 2) Datos negocio
-- =========================
INSERT INTO empresa_datos ("Id","RazonSocial","Cuit","Telefono","Direccion","Email","Web","Observaciones","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('70b37b20-6c40-43db-850b-bcf8b52a8101','Mi Empresa Demo SRL','30-12345678-9','11-5555-0000','Av. Siempre Viva 123','contacto@miempresa.demo','https://miempresa.demo','Empresa demo para pruebas del sistema POS','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO categorias ("Id","Name","MargenGananciaPct","IsActive","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('80111111-1111-1111-1111-111111111111','Bebidas',35,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('80222222-2222-2222-2222-222222222222','Almacen',28,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('80333333-3333-3333-3333-333333333333','Limpieza',40,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('80444444-4444-4444-4444-444444444444','Perfumeria',45,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO marcas ("Id","Name","IsActive","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('81111111-1111-1111-1111-111111111111','Coca Cola',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('82222222-2222-2222-2222-222222222222','Arcor',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('83333333-3333-3333-3333-333333333333','Unilever',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO proveedores ("Id","Name","Telefono","Cuit","Direccion","IsActive","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('91111111-1111-1111-1111-111111111111','Mantecaa','11-4444-1000','30-11111111-1','Bouchard 100',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('92222222-2222-2222-2222-222222222222','Arcor Distribucion','11-4444-2000','30-22222222-2','Rivadavia 2500',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('93333333-3333-3333-3333-333333333333','Branca Mayorista','11-4444-3000','30-33333333-3','Sarmiento 980',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('94444444-4444-4444-4444-444444444444','Limpieza SRL','11-4444-4000','30-44444444-4','Warnes 455',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO productos ("Id","Name","Sku","CategoriaId","MarcaId","ProveedorId","PrecioBase","PrecioVenta","PricingMode","MargenGananciaPct","IsActive","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a0010000-0000-0000-0000-000000000001','Coca Cola 2.25L','7790895000011','80111111-1111-1111-1111-111111111111','81111111-1111-1111-1111-111111111111','91111111-1111-1111-1111-111111111111',1200,1560,0,30,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000002','Fernet Branca 750ml','7790290101010','80111111-1111-1111-1111-111111111111','83333333-3333-3333-3333-333333333333','93333333-3333-3333-3333-333333333333',7000,9800,2,NULL,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000003','Sprite 2.25L','7790895000028','80111111-1111-1111-1111-111111111111','81111111-1111-1111-1111-111111111111','91111111-1111-1111-1111-111111111111',1150,1495,1,NULL,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000004','Harina 000 x1kg','7791234000001','80222222-2222-2222-2222-222222222222',NULL,'92222222-2222-2222-2222-222222222222',700,900,1,NULL,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000005','Yerba 1kg','7791234560001','80222222-2222-2222-2222-222222222222',NULL,'92222222-2222-2222-2222-222222222222',2600,3500,0,34.62,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000006','Azucar 1kg','7791234560002','80222222-2222-2222-2222-222222222222',NULL,'92222222-2222-2222-2222-222222222222',900,1200,0,33.33,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000007','Lavandina 1L','7794000000001','80333333-3333-3333-3333-333333333333','83333333-3333-3333-3333-333333333333','94444444-4444-4444-4444-444444444444',950,1450,1,NULL,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000008','Detergente 750ml','7794000000002','80333333-3333-3333-3333-333333333333','83333333-3333-3333-3333-333333333333','94444444-4444-4444-4444-444444444444',1200,1800,1,NULL,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000009','Jabon en polvo 800g','7794000000003','80333333-3333-3333-3333-333333333333','83333333-3333-3333-3333-333333333333','94444444-4444-4444-4444-444444444444',2200,3200,2,NULL,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a0010000-0000-0000-0000-000000000010','Desodorante','7791293020297','80444444-4444-4444-4444-444444444444','83333333-3333-3333-3333-333333333333','94444444-4444-4444-4444-444444444444',3500,4550,2,NULL,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO producto_proveedor ("Id","ProductoId","ProveedorId","EsPrincipal","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('b0010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000001','91111111-1111-1111-1111-111111111111',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000002','a0010000-0000-0000-0000-000000000002','93333333-3333-3333-3333-333333333333',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000003','a0010000-0000-0000-0000-000000000003','91111111-1111-1111-1111-111111111111',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000004','a0010000-0000-0000-0000-000000000004','92222222-2222-2222-2222-222222222222',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000005','a0010000-0000-0000-0000-000000000005','92222222-2222-2222-2222-222222222222',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000006','a0010000-0000-0000-0000-000000000006','92222222-2222-2222-2222-222222222222',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000007','a0010000-0000-0000-0000-000000000007','94444444-4444-4444-4444-444444444444',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000008','a0010000-0000-0000-0000-000000000008','94444444-4444-4444-4444-444444444444',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000009','a0010000-0000-0000-0000-000000000009','94444444-4444-4444-4444-444444444444',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('b0010000-0000-0000-0000-000000000010','a0010000-0000-0000-0000-000000000010','94444444-4444-4444-4444-444444444444',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO producto_codigos ("Id","ProductoId","Codigo","TenantId","CreatedAt","UpdatedAt","DeletedAt")
SELECT gen_random_uuid(), p."Id", p."Sku", p."TenantId", NOW(), NOW(), NULL
FROM productos p
WHERE p."TenantId" = '1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01';

INSERT INTO producto_stock_config ("Id","ProductoId","SucursalId","StockMinimo","stockdeseado","ToleranciaPct","TenantId","CreatedAt","UpdatedAt","DeletedAt")
SELECT gen_random_uuid(), p."Id", '3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1', 10, 30, 20, p."TenantId", NOW(), NOW(), NULL
FROM productos p
WHERE p."TenantId" = '1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01';

INSERT INTO stock_saldos ("Id","ProductoId","SucursalId","CantidadActual","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('c0010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',120,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000002','a0010000-0000-0000-0000-000000000002','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',20,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000003','a0010000-0000-0000-0000-000000000003','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',110,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000004','a0010000-0000-0000-0000-000000000004','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',180,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000005','a0010000-0000-0000-0000-000000000005','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',90,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000006','a0010000-0000-0000-0000-000000000006','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',95,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000007','a0010000-0000-0000-0000-000000000007','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',75,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000008','a0010000-0000-0000-0000-000000000008','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',60,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000009','a0010000-0000-0000-0000-000000000009','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',40,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('c0010000-0000-0000-0000-000000000010','a0010000-0000-0000-0000-000000000010','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',30,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO listas_precio ("Id","Nombre","IsActive","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('d0010000-0000-0000-0000-000000000001','PUBLICO',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('d0010000-0000-0000-0000-000000000002','MAYORISTA',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO lista_precio_items ("Id","ListaPrecioId","ProductoId","Precio","TenantId","CreatedAt","UpdatedAt","DeletedAt")
SELECT gen_random_uuid(),'d0010000-0000-0000-0000-000000000001',p."Id",p."PrecioVenta",p."TenantId",NOW(),NOW(),NULL
FROM productos p
WHERE p."TenantId" = '1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01';

INSERT INTO lista_precio_items ("Id","ListaPrecioId","ProductoId","Precio","TenantId","CreatedAt","UpdatedAt","DeletedAt")
SELECT gen_random_uuid(),'d0010000-0000-0000-0000-000000000002',p."Id",ROUND((p."PrecioVenta" * 0.88)::numeric,2),p."TenantId",NOW(),NOW(),NULL
FROM productos p
WHERE p."TenantId" = '1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01';

-- =========================
-- 3) Caja
-- =========================
INSERT INTO cajas ("Id","SucursalId","Name","Numero","IsActive","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('e0010000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','Caja 1','1',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('e0010000-0000-0000-0000-000000000002','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','Caja 2','2',true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO caja_sesiones ("Id","CajaId","SucursalId","turno","MontoInicial","MontoCierre","DiferenciaTotal","MotivoDiferencia","ArqueoJson","AperturaAt","CierreAt","Estado","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('e1010000-0000-0000-0000-000000000001','e0010000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','MANANA',15000,16850,0,'', '{"EFECTIVO":12000,"TARJETA":3000,"TRANSFERENCIA":1000,"APLICATIVO":850}'::jsonb, NOW() - INTERVAL '1 day', NOW() - INTERVAL '1 day' + INTERVAL '8 hours',2,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('e1010000-0000-0000-0000-000000000002','e0010000-0000-0000-0000-000000000002','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','TARDE',10000,NULL,0,NULL,NULL, NOW() - INTERVAL '2 hours', NULL,1,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO caja_movimientos ("Id","CajaSesionId","Tipo","MedioPago","Monto","Motivo","Fecha","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('e2010000-0000-0000-0000-000000000001','e1010000-0000-0000-0000-000000000001',4,'EFECTIVO',5000,'Ingreso inicial',NOW() - INTERVAL '23 hours','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('e2010000-0000-0000-0000-000000000002','e1010000-0000-0000-0000-000000000001',2,'EFECTIVO',1200,'Compra insumos',NOW() - INTERVAL '22 hours','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('e2010000-0000-0000-0000-000000000003','e1010000-0000-0000-0000-000000000001',1,'EFECTIVO',800,'Retiro encargado',NOW() - INTERVAL '21 hours','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

-- =========================
-- 4) Ventas + stock + comprobantes
-- =========================
INSERT INTO ventas ("Id","SucursalId","UserId","numero","ListaPrecio","Estado","TotalNeto","TotalPagos","facturada","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('f0010000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b',1001,'PUBLICO',1,6110,6110,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW() - INTERVAL '3 days',NOW() - INTERVAL '3 days',NULL),
('f0010000-0000-0000-0000-000000000002','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','6ca96f7d-6a24-4f9f-a7f9-6f8c3c11c101',1002,'PUBLICO',1,15600,15600,false,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW() - INTERVAL '2 days',NOW() - INTERVAL '2 days',NULL),
('f0010000-0000-0000-0000-000000000003','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','6ca96f7d-6a24-4f9f-a7f9-6f8c3c11c101',1003,'MAYORISTA',1,11500,11500,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW() - INTERVAL '1 day',NOW() - INTERVAL '1 day',NULL);

SELECT setval('venta_numero_seq', 1003, true);

INSERT INTO venta_items ("Id","VentaId","ProductoId","Codigo","Cantidad","PrecioUnitario","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('f1010000-0000-0000-0000-000000000001','f0010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000001','7790895000011',2,1560,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f1010000-0000-0000-0000-000000000002','f0010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000010','7791293020297',1,4550,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f1010000-0000-0000-0000-000000000003','f0010000-0000-0000-0000-000000000002','a0010000-0000-0000-0000-000000000002','7790290101010',1,9800,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f1010000-0000-0000-0000-000000000004','f0010000-0000-0000-0000-000000000002','a0010000-0000-0000-0000-000000000001','7790895000011',2,2900,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f1010000-0000-0000-0000-000000000005','f0010000-0000-0000-0000-000000000003','a0010000-0000-0000-0000-000000000005','7791234560001',2,3000,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f1010000-0000-0000-0000-000000000006','f0010000-0000-0000-0000-000000000003','a0010000-0000-0000-0000-000000000006','7791234560002',5,1100,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO venta_pagos ("Id","VentaId","MedioPago","Monto","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('f2010000-0000-0000-0000-000000000001','f0010000-0000-0000-0000-000000000001','EFECTIVO',6110,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f2010000-0000-0000-0000-000000000002','f0010000-0000-0000-0000-000000000002','TARJETA',15600,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f2010000-0000-0000-0000-000000000003','f0010000-0000-0000-0000-000000000003','TRANSFERENCIA',8000,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f2010000-0000-0000-0000-000000000004','f0010000-0000-0000-0000-000000000003','APLICATIVO',3500,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO stock_movimientos ("Id","SucursalId","Tipo","Motivo","Fecha","VentaNumero","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('f3010000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',3,'Salida por venta 1001',NOW() - INTERVAL '3 days',1001,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f3010000-0000-0000-0000-000000000002','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',3,'Salida por venta 1002',NOW() - INTERVAL '2 days',1002,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f3010000-0000-0000-0000-000000000003','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',3,'Salida por venta 1003',NOW() - INTERVAL '1 day',1003,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f3010000-0000-0000-0000-000000000004','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1',5,'Ingreso compra proveedor',NOW() - INTERVAL '20 hours',NULL,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO stock_movimiento_items ("Id","MovimientoId","ProductoId","Cantidad","EsIngreso","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('f4010000-0000-0000-0000-000000000001','f3010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000001',2,false,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f4010000-0000-0000-0000-000000000002','f3010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000010',1,false,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f4010000-0000-0000-0000-000000000003','f3010000-0000-0000-0000-000000000002','a0010000-0000-0000-0000-000000000002',1,false,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f4010000-0000-0000-0000-000000000004','f3010000-0000-0000-0000-000000000002','a0010000-0000-0000-0000-000000000001',2,false,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f4010000-0000-0000-0000-000000000005','f3010000-0000-0000-0000-000000000003','a0010000-0000-0000-0000-000000000005',2,false,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f4010000-0000-0000-0000-000000000006','f3010000-0000-0000-0000-000000000003','a0010000-0000-0000-0000-000000000006',5,false,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f4010000-0000-0000-0000-000000000007','f3010000-0000-0000-0000-000000000004','a0010000-0000-0000-0000-000000000005',20,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f4010000-0000-0000-0000-000000000008','f3010000-0000-0000-0000-000000000004','a0010000-0000-0000-0000-000000000006',30,true,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO comprobantes ("Id","SucursalId","VentaId","UserId","Estado","Total","Numero","FiscalProvider","FiscalPayload","EmitidoAt","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('f5010000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','f0010000-0000-0000-0000-000000000001','c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b',1,6110,'A-0001-00001001','AFIP-TEST','{}',NOW() - INTERVAL '3 days','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('f5010000-0000-0000-0000-000000000003','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','f0010000-0000-0000-0000-000000000003','6ca96f7d-6a24-4f9f-a7f9-6f8c3c11c101',1,11500,'A-0001-00001003','AFIP-TEST','{}',NOW() - INTERVAL '1 day','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

-- =========================
-- 5) Compras / Recepcion
-- =========================
INSERT INTO ordenes_compra ("Id","SucursalId","ProveedorId","Estado","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a9010000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','92222222-2222-2222-2222-222222222222',2,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW() - INTERVAL '2 days',NOW() - INTERVAL '1 day',NULL);

INSERT INTO orden_compra_items ("Id","OrdenCompraId","ProductoId","Cantidad","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a9020000-0000-0000-0000-000000000001','a9010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000005',20,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a9020000-0000-0000-0000-000000000002','a9010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000006',30,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO documentos_compra ("Id","SucursalId","ProveedorId","Numero","Fecha","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a9030000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','92222222-2222-2222-2222-222222222222','FC-00000045',CURRENT_DATE - 1,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO documento_compra_items ("Id","DocumentoCompraId","Codigo","Descripcion","Cantidad","CostoUnitario","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a9040000-0000-0000-0000-000000000001','a9030000-0000-0000-0000-000000000001','7791234560001','Yerba 1kg',20,2400,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a9040000-0000-0000-0000-000000000002','a9030000-0000-0000-0000-000000000001','7791234560002','Azucar 1kg',30,800,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO pre_recepciones ("Id","SucursalId","DocumentoCompraId","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a9050000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','a9030000-0000-0000-0000-000000000001','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO pre_recepcion_items ("Id","PreRecepcionId","DocumentoCompraItemId","Codigo","Descripcion","Cantidad","CostoUnitario","ProductoId","Estado","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a9060000-0000-0000-0000-000000000001','a9050000-0000-0000-0000-000000000001','a9040000-0000-0000-0000-000000000001','7791234560001','Yerba 1kg',20,2400,'a0010000-0000-0000-0000-000000000005',1,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a9060000-0000-0000-0000-000000000002','a9050000-0000-0000-0000-000000000001','a9040000-0000-0000-0000-000000000002','7791234560002','Azucar 1kg',30,800,'a0010000-0000-0000-0000-000000000006',1,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO recepciones ("Id","SucursalId","PreRecepcionId","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a9070000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','a9050000-0000-0000-0000-000000000001','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO recepcion_items ("Id","RecepcionId","PreRecepcionItemId","ProductoId","Codigo","Descripcion","Cantidad","CostoUnitario","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('a9080000-0000-0000-0000-000000000001','a9070000-0000-0000-0000-000000000001','a9060000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000005','7791234560001','Yerba 1kg',20,2400,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL),
('a9080000-0000-0000-0000-000000000002','a9070000-0000-0000-0000-000000000001','a9060000-0000-0000-0000-000000000002','a0010000-0000-0000-0000-000000000006','7791234560002','Azucar 1kg',30,800,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

-- =========================
-- 6) Devolucion / Nota de credito
-- =========================
INSERT INTO devoluciones ("Id","SucursalId","VentaId","UserId","Motivo","Total","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('aa010000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','f0010000-0000-0000-0000-000000000001','c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b','Producto en mal estado',1560,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW() - INTERVAL '2 days',NOW() - INTERVAL '2 days',NULL);

INSERT INTO devolucion_items ("Id","DevolucionId","ProductoId","Cantidad","PrecioUnitario","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('aa020000-0000-0000-0000-000000000001','aa010000-0000-0000-0000-000000000001','a0010000-0000-0000-0000-000000000001',1,1560,'1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

INSERT INTO nota_credito_interna ("Id","SucursalId","DevolucionId","Total","Motivo","TenantId","CreatedAt","UpdatedAt","DeletedAt")
VALUES
('aa030000-0000-0000-0000-000000000001','3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1','aa010000-0000-0000-0000-000000000001',1560,'Nota de credito por devolucion','1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01',NOW(),NOW(),NULL);

-- =========================
-- 7) Ajuste final de saldos por movimientos demo
-- =========================
UPDATE stock_saldos SET "CantidadActual" = "CantidadActual" - 4 WHERE "ProductoId" = 'a0010000-0000-0000-0000-000000000001' AND "SucursalId" = '3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1';
UPDATE stock_saldos SET "CantidadActual" = "CantidadActual" - 1 WHERE "ProductoId" = 'a0010000-0000-0000-0000-000000000002' AND "SucursalId" = '3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1';
UPDATE stock_saldos SET "CantidadActual" = "CantidadActual" - 1 WHERE "ProductoId" = 'a0010000-0000-0000-0000-000000000010' AND "SucursalId" = '3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1';
UPDATE stock_saldos SET "CantidadActual" = "CantidadActual" + 20 WHERE "ProductoId" = 'a0010000-0000-0000-0000-000000000005' AND "SucursalId" = '3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1';
UPDATE stock_saldos SET "CantidadActual" = "CantidadActual" + 25 WHERE "ProductoId" = 'a0010000-0000-0000-0000-000000000006' AND "SucursalId" = '3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1';

COMMIT;
