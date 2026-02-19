-- Extra DDL required by model defaults/custom SQL migrations
CREATE SEQUENCE IF NOT EXISTS venta_numero_seq START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;
CREATE TABLE tenants (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_tenants" PRIMARY KEY ("Id")
);
CREATE TABLE categorias (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "MargenGananciaPct" numeric(6,2) NOT NULL DEFAULT 30.0,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_categorias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_categorias_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE empresa_datos (
    "Id" uuid NOT NULL,
    "RazonSocial" character varying(250) NOT NULL,
    "Cuit" character varying(32),
    "Telefono" character varying(64),
    "Direccion" character varying(300),
    "Email" character varying(160),
    "Web" character varying(180),
    "Observaciones" character varying(1000),
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_empresa_datos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_empresa_datos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE listas_precio (
    "Id" uuid NOT NULL,
    "Nombre" character varying(150) NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_listas_precio" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_listas_precio_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE marcas (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_marcas" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_marcas_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE permisos (
    "Id" uuid NOT NULL,
    "Code" character varying(120) NOT NULL,
    "Description" character varying(300) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_permisos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_permisos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE proveedores (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Telefono" character varying(50) NOT NULL,
    "Cuit" character varying(20),
    "Direccion" character varying(250),
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_proveedores" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_proveedores_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE roles (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_roles" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_roles_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE sucursales (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Code" character varying(50),
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_sucursales" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_sucursales_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE usuarios (
    "Id" uuid NOT NULL,
    "Username" character varying(100) NOT NULL,
    "PasswordHash" character varying(512) NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_usuarios" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_usuarios_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE productos (
    "Id" uuid NOT NULL,
    "Name" character varying(250) NOT NULL,
    "Sku" character varying(80) NOT NULL,
    "CategoriaId" uuid,
    "MarcaId" uuid,
    "ProveedorId" uuid,
    "PrecioBase" numeric(18,4) NOT NULL DEFAULT 1.0,
    "PrecioVenta" numeric(18,4) NOT NULL DEFAULT 1.0,
    "PricingMode" integer NOT NULL DEFAULT 0,
    "MargenGananciaPct" numeric(6,2),
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_productos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_productos_categorias_CategoriaId" FOREIGN KEY ("CategoriaId") REFERENCES categorias ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_productos_marcas_MarcaId" FOREIGN KEY ("MarcaId") REFERENCES marcas ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_productos_proveedores_ProveedorId" FOREIGN KEY ("ProveedorId") REFERENCES proveedores ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_productos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE rol_permisos (
    "Id" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    "PermissionId" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_rol_permisos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_rol_permisos_permisos_PermissionId" FOREIGN KEY ("PermissionId") REFERENCES permisos ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_rol_permisos_roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES roles ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_rol_permisos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE audit_logs (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "UserId" uuid,
    "Action" integer NOT NULL,
    "EntityName" character varying(150) NOT NULL,
    "EntityId" character varying(120) NOT NULL,
    "OccurredAt" timestamp with time zone NOT NULL,
    "BeforeJson" text,
    "AfterJson" text,
    "MetadataJson" text,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_audit_logs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_audit_logs_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_audit_logs_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE cajas (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Numero" character varying(50),
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_cajas" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_cajas_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_cajas_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE documentos_compra (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "ProveedorId" uuid,
    "Numero" character varying(120) NOT NULL,
    "Fecha" date NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_documentos_compra" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_documentos_compra_proveedores_ProveedorId" FOREIGN KEY ("ProveedorId") REFERENCES proveedores ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_documentos_compra_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_documentos_compra_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE ordenes_compra (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "ProveedorId" uuid,
    "Estado" integer NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_ordenes_compra" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ordenes_compra_proveedores_ProveedorId" FOREIGN KEY ("ProveedorId") REFERENCES proveedores ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ordenes_compra_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ordenes_compra_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE stock_movimientos (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "Tipo" integer NOT NULL,
    "Motivo" character varying(500) NOT NULL,
    "Fecha" timestamp with time zone NOT NULL,
    "VentaNumero" bigint,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_stock_movimientos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_stock_movimientos_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_stock_movimientos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE usuario_roles (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_usuario_roles" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_usuario_roles_roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES roles ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_usuario_roles_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_usuario_roles_usuarios_UserId" FOREIGN KEY ("UserId") REFERENCES usuarios ("Id") ON DELETE CASCADE
);
CREATE TABLE ventas (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "UserId" uuid,
    numero bigint NOT NULL DEFAULT (nextval('venta_numero_seq')),
    "ListaPrecio" character varying(120) NOT NULL,
    "Estado" integer NOT NULL,
    "TotalNeto" numeric(18,4) NOT NULL DEFAULT 0.0,
    "TotalPagos" numeric(18,4) NOT NULL DEFAULT 0.0,
    facturada boolean NOT NULL DEFAULT FALSE,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_ventas" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ventas_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ventas_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ventas_usuarios_UserId" FOREIGN KEY ("UserId") REFERENCES usuarios ("Id") ON DELETE RESTRICT
);
CREATE TABLE lista_precio_items (
    "Id" uuid NOT NULL,
    "ListaPrecioId" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "Precio" numeric(18,4) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_lista_precio_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_lista_precio_items_listas_precio_ListaPrecioId" FOREIGN KEY ("ListaPrecioId") REFERENCES listas_precio ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_lista_precio_items_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_lista_precio_items_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE producto_codigos (
    "Id" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "Codigo" character varying(120) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_producto_codigos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_producto_codigos_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_producto_codigos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE producto_proveedor (
    "Id" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "ProveedorId" uuid NOT NULL,
    "EsPrincipal" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_producto_proveedor" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_producto_proveedor_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_producto_proveedor_proveedores_ProveedorId" FOREIGN KEY ("ProveedorId") REFERENCES proveedores ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_producto_proveedor_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE producto_stock_config (
    "Id" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "StockMinimo" numeric(18,4) NOT NULL,
    stockdeseado numeric(18,4) NOT NULL DEFAULT 0.0,
    "ToleranciaPct" numeric(6,2) NOT NULL DEFAULT 25.0,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_producto_stock_config" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_producto_stock_config_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_producto_stock_config_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_producto_stock_config_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE stock_saldos (
    "Id" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "CantidadActual" numeric(18,4) NOT NULL DEFAULT 0.0,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_stock_saldos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_stock_saldos_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_stock_saldos_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_stock_saldos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE caja_sesiones (
    "Id" uuid NOT NULL,
    "CajaId" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    turno character varying(16) NOT NULL,
    "MontoInicial" numeric(18,4) NOT NULL,
    "MontoCierre" numeric(18,4),
    "DiferenciaTotal" numeric(18,4) NOT NULL DEFAULT 0.0,
    "MotivoDiferencia" character varying(500),
    "ArqueoJson" jsonb,
    "AperturaAt" timestamp with time zone NOT NULL,
    "CierreAt" timestamp with time zone,
    "Estado" integer NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_caja_sesiones" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_caja_sesiones_cajas_CajaId" FOREIGN KEY ("CajaId") REFERENCES cajas ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_caja_sesiones_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_caja_sesiones_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE documento_compra_items (
    "Id" uuid NOT NULL,
    "DocumentoCompraId" uuid NOT NULL,
    "Codigo" character varying(120) NOT NULL,
    "Descripcion" character varying(500) NOT NULL,
    "Cantidad" numeric(18,4) NOT NULL,
    "CostoUnitario" numeric(18,4),
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_documento_compra_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_documento_compra_items_documentos_compra_DocumentoCompraId" FOREIGN KEY ("DocumentoCompraId") REFERENCES documentos_compra ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_documento_compra_items_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE pre_recepciones (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "DocumentoCompraId" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_pre_recepciones" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_pre_recepciones_documentos_compra_DocumentoCompraId" FOREIGN KEY ("DocumentoCompraId") REFERENCES documentos_compra ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_pre_recepciones_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_pre_recepciones_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE orden_compra_items (
    "Id" uuid NOT NULL,
    "OrdenCompraId" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "Cantidad" numeric(18,4) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_orden_compra_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_orden_compra_items_ordenes_compra_OrdenCompraId" FOREIGN KEY ("OrdenCompraId") REFERENCES ordenes_compra ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_orden_compra_items_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_orden_compra_items_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE stock_movimiento_items (
    "Id" uuid NOT NULL,
    "MovimientoId" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "Cantidad" numeric(18,4) NOT NULL,
    "EsIngreso" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_stock_movimiento_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_stock_movimiento_items_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_stock_movimiento_items_stock_movimientos_MovimientoId" FOREIGN KEY ("MovimientoId") REFERENCES stock_movimientos ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_stock_movimiento_items_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE comprobantes (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "VentaId" uuid NOT NULL,
    "UserId" uuid,
    "Estado" integer NOT NULL,
    "Total" numeric(18,4) NOT NULL DEFAULT 0.0,
    "Numero" character varying(120),
    "FiscalProvider" character varying(50),
    "FiscalPayload" text,
    "EmitidoAt" timestamp with time zone,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_comprobantes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_comprobantes_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_comprobantes_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_comprobantes_usuarios_UserId" FOREIGN KEY ("UserId") REFERENCES usuarios ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_comprobantes_ventas_VentaId" FOREIGN KEY ("VentaId") REFERENCES ventas ("Id") ON DELETE RESTRICT
);
CREATE TABLE devoluciones (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "VentaId" uuid NOT NULL,
    "UserId" uuid,
    "Motivo" character varying(500) NOT NULL,
    "Total" numeric(18,4) NOT NULL DEFAULT 0.0,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_devoluciones" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_devoluciones_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_devoluciones_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_devoluciones_usuarios_UserId" FOREIGN KEY ("UserId") REFERENCES usuarios ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_devoluciones_ventas_VentaId" FOREIGN KEY ("VentaId") REFERENCES ventas ("Id") ON DELETE RESTRICT
);
CREATE TABLE venta_items (
    "Id" uuid NOT NULL,
    "VentaId" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "Codigo" character varying(120) NOT NULL,
    "Cantidad" numeric(18,4) NOT NULL,
    "PrecioUnitario" numeric(18,4) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_venta_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_venta_items_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_venta_items_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_venta_items_ventas_VentaId" FOREIGN KEY ("VentaId") REFERENCES ventas ("Id") ON DELETE RESTRICT
);
CREATE TABLE venta_pagos (
    "Id" uuid NOT NULL,
    "VentaId" uuid NOT NULL,
    "MedioPago" character varying(100) NOT NULL,
    "Monto" numeric(18,4) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_venta_pagos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_venta_pagos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_venta_pagos_ventas_VentaId" FOREIGN KEY ("VentaId") REFERENCES ventas ("Id") ON DELETE RESTRICT
);
CREATE TABLE caja_movimientos (
    "Id" uuid NOT NULL,
    "CajaSesionId" uuid NOT NULL,
    "Tipo" integer NOT NULL,
    "MedioPago" character varying(100) NOT NULL,
    "Monto" numeric(18,4) NOT NULL,
    "Motivo" character varying(500) NOT NULL,
    "Fecha" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_caja_movimientos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_caja_movimientos_caja_sesiones_CajaSesionId" FOREIGN KEY ("CajaSesionId") REFERENCES caja_sesiones ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_caja_movimientos_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE pre_recepcion_items (
    "Id" uuid NOT NULL,
    "PreRecepcionId" uuid NOT NULL,
    "DocumentoCompraItemId" uuid NOT NULL,
    "Codigo" character varying(120) NOT NULL,
    "Descripcion" character varying(500) NOT NULL,
    "Cantidad" numeric(18,4) NOT NULL,
    "CostoUnitario" numeric(18,4),
    "ProductoId" uuid,
    "Estado" integer NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_pre_recepcion_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_pre_recepcion_items_documento_compra_items_DocumentoCompraI~" FOREIGN KEY ("DocumentoCompraItemId") REFERENCES documento_compra_items ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_pre_recepcion_items_pre_recepciones_PreRecepcionId" FOREIGN KEY ("PreRecepcionId") REFERENCES pre_recepciones ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_pre_recepcion_items_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_pre_recepcion_items_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE recepciones (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "PreRecepcionId" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_recepciones" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_recepciones_pre_recepciones_PreRecepcionId" FOREIGN KEY ("PreRecepcionId") REFERENCES pre_recepciones ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_recepciones_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_recepciones_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE devolucion_items (
    "Id" uuid NOT NULL,
    "DevolucionId" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "Cantidad" numeric(18,4) NOT NULL,
    "PrecioUnitario" numeric(18,4) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_devolucion_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_devolucion_items_devoluciones_DevolucionId" FOREIGN KEY ("DevolucionId") REFERENCES devoluciones ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_devolucion_items_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT
);
CREATE TABLE nota_credito_interna (
    "Id" uuid NOT NULL,
    "SucursalId" uuid NOT NULL,
    "DevolucionId" uuid NOT NULL,
    "Total" numeric(18,4) NOT NULL DEFAULT 0.0,
    "Motivo" character varying(500) NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_nota_credito_interna" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_nota_credito_interna_devoluciones_DevolucionId" FOREIGN KEY ("DevolucionId") REFERENCES devoluciones ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_nota_credito_interna_sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES sucursales ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_nota_credito_interna_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE TABLE recepcion_items (
    "Id" uuid NOT NULL,
    "RecepcionId" uuid NOT NULL,
    "PreRecepcionItemId" uuid NOT NULL,
    "ProductoId" uuid NOT NULL,
    "Codigo" character varying(120) NOT NULL,
    "Descripcion" character varying(500) NOT NULL,
    "Cantidad" numeric(18,4) NOT NULL,
    "CostoUnitario" numeric(18,4),
    "TenantId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "DeletedAt" timestamp with time zone,
    CONSTRAINT "PK_recepcion_items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_recepcion_items_pre_recepcion_items_PreRecepcionItemId" FOREIGN KEY ("PreRecepcionItemId") REFERENCES pre_recepcion_items ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_recepcion_items_productos_ProductoId" FOREIGN KEY ("ProductoId") REFERENCES productos ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_recepcion_items_recepciones_RecepcionId" FOREIGN KEY ("RecepcionId") REFERENCES recepciones ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_recepcion_items_tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES tenants ("Id") ON DELETE RESTRICT
);
CREATE INDEX "IX_audit_logs_SucursalId" ON audit_logs ("SucursalId");
CREATE INDEX "IX_audit_logs_TenantId" ON audit_logs ("TenantId");
CREATE INDEX "IX_caja_movimientos_CajaSesionId" ON caja_movimientos ("CajaSesionId");
CREATE INDEX "IX_caja_movimientos_Fecha" ON caja_movimientos ("Fecha");
CREATE INDEX "IX_caja_movimientos_MedioPago" ON caja_movimientos ("MedioPago");
CREATE INDEX "IX_caja_movimientos_TenantId" ON caja_movimientos ("TenantId");
CREATE INDEX "IX_caja_sesiones_AperturaAt" ON caja_sesiones ("AperturaAt");
CREATE INDEX "IX_caja_sesiones_CajaId" ON caja_sesiones ("CajaId");
CREATE UNIQUE INDEX "IX_caja_sesiones_CajaId_Estado" ON caja_sesiones ("CajaId", "Estado") WHERE "Estado" = 1;
CREATE INDEX "IX_caja_sesiones_SucursalId" ON caja_sesiones ("SucursalId");
CREATE INDEX "IX_caja_sesiones_TenantId" ON caja_sesiones ("TenantId");
CREATE INDEX "IX_cajas_SucursalId" ON cajas ("SucursalId");
CREATE INDEX "IX_cajas_TenantId" ON cajas ("TenantId");
CREATE UNIQUE INDEX "IX_cajas_TenantId_SucursalId_Numero" ON cajas ("TenantId", "SucursalId", "Numero") WHERE "Numero" IS NOT NULL;
CREATE INDEX "IX_categorias_TenantId" ON categorias ("TenantId");
CREATE INDEX "IX_comprobantes_CreatedAt" ON comprobantes ("CreatedAt");
CREATE INDEX "IX_comprobantes_Estado" ON comprobantes ("Estado");
CREATE INDEX "IX_comprobantes_SucursalId" ON comprobantes ("SucursalId");
CREATE INDEX "IX_comprobantes_TenantId" ON comprobantes ("TenantId");
CREATE UNIQUE INDEX "IX_comprobantes_TenantId_VentaId" ON comprobantes ("TenantId", "VentaId");
CREATE INDEX "IX_comprobantes_UserId" ON comprobantes ("UserId");
CREATE INDEX "IX_comprobantes_VentaId" ON comprobantes ("VentaId");
CREATE INDEX "IX_devolucion_items_DevolucionId" ON devolucion_items ("DevolucionId");
CREATE UNIQUE INDEX "IX_devolucion_items_DevolucionId_ProductoId" ON devolucion_items ("DevolucionId", "ProductoId");
CREATE INDEX "IX_devolucion_items_ProductoId" ON devolucion_items ("ProductoId");
CREATE INDEX "IX_devolucion_items_TenantId" ON devolucion_items ("TenantId");
CREATE INDEX "IX_devoluciones_CreatedAt" ON devoluciones ("CreatedAt");
CREATE INDEX "IX_devoluciones_SucursalId" ON devoluciones ("SucursalId");
CREATE INDEX "IX_devoluciones_TenantId" ON devoluciones ("TenantId");
CREATE INDEX "IX_devoluciones_UserId" ON devoluciones ("UserId");
CREATE INDEX "IX_devoluciones_VentaId" ON devoluciones ("VentaId");
CREATE INDEX "IX_documento_compra_items_Codigo" ON documento_compra_items ("Codigo");
CREATE INDEX "IX_documento_compra_items_DocumentoCompraId" ON documento_compra_items ("DocumentoCompraId");
CREATE INDEX "IX_documento_compra_items_TenantId" ON documento_compra_items ("TenantId");
CREATE INDEX "IX_documentos_compra_Fecha" ON documentos_compra ("Fecha");
CREATE INDEX "IX_documentos_compra_Numero" ON documentos_compra ("Numero");
CREATE INDEX "IX_documentos_compra_ProveedorId" ON documentos_compra ("ProveedorId");
CREATE INDEX "IX_documentos_compra_SucursalId" ON documentos_compra ("SucursalId");
CREATE INDEX "IX_documentos_compra_TenantId" ON documentos_compra ("TenantId");
CREATE UNIQUE INDEX "IX_empresa_datos_TenantId" ON empresa_datos ("TenantId");
CREATE INDEX "IX_lista_precio_items_ListaPrecioId" ON lista_precio_items ("ListaPrecioId");
CREATE INDEX "IX_lista_precio_items_ProductoId" ON lista_precio_items ("ProductoId");
CREATE INDEX "IX_lista_precio_items_TenantId" ON lista_precio_items ("TenantId");
CREATE UNIQUE INDEX "IX_lista_precio_items_TenantId_ListaPrecioId_ProductoId" ON lista_precio_items ("TenantId", "ListaPrecioId", "ProductoId");
CREATE INDEX "IX_listas_precio_TenantId" ON listas_precio ("TenantId");
CREATE UNIQUE INDEX "IX_listas_precio_TenantId_Nombre" ON listas_precio ("TenantId", "Nombre");
CREATE INDEX "IX_marcas_TenantId" ON marcas ("TenantId");
CREATE UNIQUE INDEX "IX_nota_credito_interna_DevolucionId" ON nota_credito_interna ("DevolucionId");
CREATE INDEX "IX_nota_credito_interna_SucursalId" ON nota_credito_interna ("SucursalId");
CREATE INDEX "IX_nota_credito_interna_TenantId" ON nota_credito_interna ("TenantId");
CREATE INDEX "IX_orden_compra_items_OrdenCompraId" ON orden_compra_items ("OrdenCompraId");
CREATE UNIQUE INDEX "IX_orden_compra_items_OrdenCompraId_ProductoId" ON orden_compra_items ("OrdenCompraId", "ProductoId");
CREATE INDEX "IX_orden_compra_items_ProductoId" ON orden_compra_items ("ProductoId");
CREATE INDEX "IX_orden_compra_items_TenantId" ON orden_compra_items ("TenantId");
CREATE INDEX "IX_ordenes_compra_CreatedAt" ON ordenes_compra ("CreatedAt");
CREATE INDEX "IX_ordenes_compra_Estado" ON ordenes_compra ("Estado");
CREATE INDEX "IX_ordenes_compra_ProveedorId" ON ordenes_compra ("ProveedorId");
CREATE INDEX "IX_ordenes_compra_SucursalId" ON ordenes_compra ("SucursalId");
CREATE INDEX "IX_ordenes_compra_TenantId" ON ordenes_compra ("TenantId");
CREATE INDEX "IX_permisos_TenantId" ON permisos ("TenantId");
CREATE UNIQUE INDEX "IX_permisos_TenantId_Code" ON permisos ("TenantId", "Code");
CREATE INDEX "IX_pre_recepcion_items_DocumentoCompraItemId" ON pre_recepcion_items ("DocumentoCompraItemId");
CREATE INDEX "IX_pre_recepcion_items_PreRecepcionId" ON pre_recepcion_items ("PreRecepcionId");
CREATE UNIQUE INDEX "IX_pre_recepcion_items_PreRecepcionId_DocumentoCompraItemId" ON pre_recepcion_items ("PreRecepcionId", "DocumentoCompraItemId");
CREATE INDEX "IX_pre_recepcion_items_ProductoId" ON pre_recepcion_items ("ProductoId");
CREATE INDEX "IX_pre_recepcion_items_TenantId" ON pre_recepcion_items ("TenantId");
CREATE INDEX "IX_pre_recepciones_CreatedAt" ON pre_recepciones ("CreatedAt");
CREATE INDEX "IX_pre_recepciones_DocumentoCompraId" ON pre_recepciones ("DocumentoCompraId");
CREATE INDEX "IX_pre_recepciones_SucursalId" ON pre_recepciones ("SucursalId");
CREATE INDEX "IX_pre_recepciones_TenantId" ON pre_recepciones ("TenantId");
CREATE INDEX "IX_producto_codigos_ProductoId" ON producto_codigos ("ProductoId");
CREATE INDEX "IX_producto_codigos_TenantId" ON producto_codigos ("TenantId");
CREATE UNIQUE INDEX "IX_producto_codigos_TenantId_Codigo" ON producto_codigos ("TenantId", "Codigo");
CREATE INDEX "IX_producto_proveedor_ProductoId" ON producto_proveedor ("ProductoId");
CREATE INDEX "IX_producto_proveedor_ProveedorId" ON producto_proveedor ("ProveedorId");
CREATE INDEX "IX_producto_proveedor_TenantId" ON producto_proveedor ("TenantId");
CREATE UNIQUE INDEX "IX_producto_proveedor_TenantId_ProductoId" ON producto_proveedor ("TenantId", "ProductoId") WHERE "EsPrincipal" = true;
CREATE UNIQUE INDEX "IX_producto_proveedor_TenantId_ProductoId_ProveedorId" ON producto_proveedor ("TenantId", "ProductoId", "ProveedorId");
CREATE INDEX "IX_producto_stock_config_ProductoId" ON producto_stock_config ("ProductoId");
CREATE INDEX "IX_producto_stock_config_SucursalId" ON producto_stock_config ("SucursalId");
CREATE INDEX "IX_producto_stock_config_TenantId" ON producto_stock_config ("TenantId");
CREATE UNIQUE INDEX "IX_producto_stock_config_TenantId_ProductoId_SucursalId" ON producto_stock_config ("TenantId", "ProductoId", "SucursalId");
CREATE INDEX "IX_productos_CategoriaId" ON productos ("CategoriaId");
CREATE INDEX "IX_productos_MarcaId" ON productos ("MarcaId");
CREATE INDEX "IX_productos_ProveedorId" ON productos ("ProveedorId");
CREATE INDEX "IX_productos_TenantId" ON productos ("TenantId");
CREATE UNIQUE INDEX "IX_productos_TenantId_Sku" ON productos ("TenantId", "Sku");
CREATE INDEX "IX_proveedores_TenantId" ON proveedores ("TenantId");
CREATE INDEX "IX_recepcion_items_PreRecepcionItemId" ON recepcion_items ("PreRecepcionItemId");
CREATE INDEX "IX_recepcion_items_ProductoId" ON recepcion_items ("ProductoId");
CREATE INDEX "IX_recepcion_items_RecepcionId" ON recepcion_items ("RecepcionId");
CREATE UNIQUE INDEX "IX_recepcion_items_RecepcionId_PreRecepcionItemId" ON recepcion_items ("RecepcionId", "PreRecepcionItemId");
CREATE INDEX "IX_recepcion_items_TenantId" ON recepcion_items ("TenantId");
CREATE INDEX "IX_recepciones_CreatedAt" ON recepciones ("CreatedAt");
CREATE UNIQUE INDEX "IX_recepciones_PreRecepcionId" ON recepciones ("PreRecepcionId");
CREATE INDEX "IX_recepciones_SucursalId" ON recepciones ("SucursalId");
CREATE INDEX "IX_recepciones_TenantId" ON recepciones ("TenantId");
CREATE INDEX "IX_rol_permisos_PermissionId" ON rol_permisos ("PermissionId");
CREATE INDEX "IX_rol_permisos_RoleId" ON rol_permisos ("RoleId");
CREATE INDEX "IX_rol_permisos_TenantId" ON rol_permisos ("TenantId");
CREATE UNIQUE INDEX "IX_rol_permisos_TenantId_RoleId_PermissionId" ON rol_permisos ("TenantId", "RoleId", "PermissionId");
CREATE INDEX "IX_roles_TenantId" ON roles ("TenantId");
CREATE UNIQUE INDEX "IX_roles_TenantId_Name" ON roles ("TenantId", "Name");
CREATE INDEX "IX_stock_movimiento_items_MovimientoId" ON stock_movimiento_items ("MovimientoId");
CREATE INDEX "IX_stock_movimiento_items_ProductoId" ON stock_movimiento_items ("ProductoId");
CREATE INDEX "IX_stock_movimiento_items_TenantId" ON stock_movimiento_items ("TenantId");
CREATE INDEX "IX_stock_movimientos_Fecha" ON stock_movimientos ("Fecha");
CREATE INDEX "IX_stock_movimientos_SucursalId" ON stock_movimientos ("SucursalId");
CREATE INDEX "IX_stock_movimientos_TenantId" ON stock_movimientos ("TenantId");
CREATE INDEX "IX_stock_movimientos_TenantId_VentaNumero" ON stock_movimientos ("TenantId", "VentaNumero");
CREATE INDEX "IX_stock_saldos_ProductoId" ON stock_saldos ("ProductoId");
CREATE INDEX "IX_stock_saldos_SucursalId" ON stock_saldos ("SucursalId");
CREATE INDEX "IX_stock_saldos_TenantId" ON stock_saldos ("TenantId");
CREATE UNIQUE INDEX "IX_stock_saldos_TenantId_ProductoId_SucursalId" ON stock_saldos ("TenantId", "ProductoId", "SucursalId");
CREATE INDEX "IX_sucursales_TenantId" ON sucursales ("TenantId");
CREATE INDEX "IX_tenants_TenantId" ON tenants ("TenantId");
CREATE INDEX "IX_usuario_roles_RoleId" ON usuario_roles ("RoleId");
CREATE INDEX "IX_usuario_roles_TenantId" ON usuario_roles ("TenantId");
CREATE UNIQUE INDEX "IX_usuario_roles_TenantId_UserId_RoleId" ON usuario_roles ("TenantId", "UserId", "RoleId");
CREATE INDEX "IX_usuario_roles_UserId" ON usuario_roles ("UserId");
CREATE INDEX "IX_usuarios_TenantId" ON usuarios ("TenantId");
CREATE UNIQUE INDEX "IX_usuarios_TenantId_Username" ON usuarios ("TenantId", "Username");
CREATE INDEX "IX_venta_items_ProductoId" ON venta_items ("ProductoId");
CREATE INDEX "IX_venta_items_TenantId" ON venta_items ("TenantId");
CREATE INDEX "IX_venta_items_VentaId" ON venta_items ("VentaId");
CREATE UNIQUE INDEX "IX_venta_items_VentaId_ProductoId" ON venta_items ("VentaId", "ProductoId");
CREATE INDEX "IX_venta_pagos_MedioPago" ON venta_pagos ("MedioPago");
CREATE INDEX "IX_venta_pagos_TenantId" ON venta_pagos ("TenantId");
CREATE INDEX "IX_venta_pagos_VentaId" ON venta_pagos ("VentaId");
CREATE INDEX "IX_ventas_CreatedAt" ON ventas ("CreatedAt");
CREATE INDEX "IX_ventas_Estado" ON ventas ("Estado");
CREATE INDEX "IX_ventas_SucursalId" ON ventas ("SucursalId");
CREATE INDEX "IX_ventas_TenantId" ON ventas ("TenantId");
CREATE UNIQUE INDEX "IX_ventas_TenantId_numero" ON ventas ("TenantId", numero);
CREATE INDEX "IX_ventas_UserId" ON ventas ("UserId");

