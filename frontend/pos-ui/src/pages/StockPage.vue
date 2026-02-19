<template>
  <div class="stock-page">
    <v-card class="pos-card pa-4 mb-4">
      <div class="d-flex align-center gap-3">
        <div>
          <div class="text-h6">Stock</div>
          <div class="text-caption text-medium-emphasis">Saldos, movimientos y alertas</div>
        </div>
      </div>
    </v-card>

    <v-tabs v-model="tab" color="primary" class="mb-3">
      <v-tab value="saldos">Saldos</v-tab>
      <v-tab value="movimientos">Movimientos</v-tab>
      <v-tab value="alertas">Alertas</v-tab>
    </v-tabs>

    <v-window v-model="tab">
      <v-window-item value="saldos">
        <v-card class="pos-card pa-4">
          <v-row dense class="align-center">
            <v-col cols="12" md="3">
              <v-autocomplete
                v-model="saldoProducto"
                :items="saldoProductosLookup"
                :loading="saldoProductosLoading"
                item-title="label"
                return-object
                clearable
                label="Producto"
                variant="outlined"
                density="comfortable"
                hide-details
                @update:search="searchSaldoProductos"
                @update:model-value="onSaldoProductoChanged"
              />
            </v-col>
            <v-col cols="12" md="5">
              <v-autocomplete
                v-model="saldoProveedor"
                :items="proveedoresLookup"
                item-title="label"
                return-object
                clearable
                label="Proveedor"
                variant="outlined"
                density="comfortable"
                hide-details
                :loading="proveedorLoading"
                @update:search="searchProveedores"
              />
            </v-col>
            <v-col cols="12" md="2" class="d-none d-md-flex"></v-col>
            <v-col cols="12" md="2" class="d-flex align-center justify-md-end">
              <v-btn
                color="primary"
                variant="tonal"
                class="text-none w-100"
                :loading="saldosLoading"
                @click="loadSaldos"
              >
                Buscar
              </v-btn>
            </v-col>
          </v-row>

          <v-divider class="my-4" />

          <div class="text-subtitle-2">Ajuste de stock</div>
          <div class="text-caption text-medium-emphasis">
            Modific\u00e1 la cantidad y deja registrado el motivo.
          </div>

          <v-row dense class="mt-2">
            <v-col cols="12" md="4">
              <v-autocomplete
                v-model="ajusteProducto"
                :items="ajusteItems"
                :loading="ajusteLoading"
                item-title="label"
                return-object
                label="Producto"
                variant="outlined"
                density="comfortable"
                clearable
                :error-messages="ajusteErrors.producto"
                @update:search="searchAjusteProductos"
              />
            </v-col>
            <v-col cols="12" md="2">
              <v-text-field
                v-model="ajusteCantidadActual"
                label="Actual"
                variant="outlined"
                density="comfortable"
                readonly
              />
            </v-col>
            <v-col cols="12" md="2">
              <v-text-field
                v-model="ajusteCantidadNueva"
                label="Nuevo"
                type="number"
                min="0"
                step="0.01"
                variant="outlined"
                density="comfortable"
                :error-messages="ajusteErrors.cantidad"
              />
            </v-col>
            <v-col cols="12" md="4">
              <v-text-field
                v-model="ajusteMotivo"
                label="Motivo"
                variant="outlined"
                density="comfortable"
                :error-messages="ajusteErrors.motivo"
              />
            </v-col>
          </v-row>

          <div class="d-flex justify-end mt-2">
            <v-btn
              color="primary"
              class="text-none"
              :loading="ajusteSaving"
              @click="aplicarAjuste"
            >
              Guardar ajuste
            </v-btn>
          </div>

          <v-data-table
            class="mt-3"
            :headers="saldoHeaders"
            :items="saldos"
            item-key="productoId"
            density="compact"
            height="520"
          >
            <template v-slot:[`item.proveedor`]="{ item }">
              {{ item.proveedor || 'SIN PROVEEDOR' }}
            </template>
          </v-data-table>
        </v-card>
      </v-window-item>

      <v-window-item value="movimientos">
        <v-card class="pos-card pa-4">
          <div class="d-flex flex-wrap align-center gap-3">
            <v-text-field
              v-model="movFilters.productoId"
              label="Producto Id"
              variant="outlined"
              density="comfortable"
              hide-details
              style="min-width: 240px"
            />
            <v-text-field
              v-model="movFilters.ventaNumero"
              label="N\u00b0 Venta"
              variant="outlined"
              density="comfortable"
              hide-details
              style="max-width: 160px"
            />
            <v-select
              v-model="movFilters.facturada"
              :items="facturacionOptions"
              item-title="label"
              item-value="value"
              label="Facturacion"
              variant="outlined"
              density="comfortable"
              hide-details
              style="max-width: 220px"
            />
            <v-text-field
              v-model="movFilters.desde"
              label="Desde"
              type="date"
              variant="outlined"
              density="comfortable"
              hide-details
            />
            <v-text-field
              v-model="movFilters.hasta"
              label="Hasta"
              type="date"
              variant="outlined"
              density="comfortable"
              hide-details
            />
            <v-btn
              color="primary"
              variant="tonal"
              class="text-none"
              :loading="movLoading"
              @click="loadMovimientos"
            >
              Buscar
            </v-btn>
          </div>

          <v-expansion-panels class="mt-3" variant="accordion">
            <v-expansion-panel v-for="mov in movimientos" :key="mov.id">
              <v-expansion-panel-title>
                <div class="d-flex align-center justify-space-between w-100">
                  <div>
                    <div class="text-subtitle-2">{{ mov.tipo }}</div>
                    <div class="text-caption text-medium-emphasis">{{ mov.motivo }}</div>
                    <div
                      v-if="mov.tipo === 'AJUSTE' && mov.motivo"
                      class="text-caption text-medium-emphasis"
                    >
                      Motivo ajuste: {{ mov.motivo }}
                    </div>
                    <div v-if="mov.ventaNumero" class="text-caption text-medium-emphasis">
                      Venta N\u00b0 {{ mov.ventaNumero }}
                    </div>
                    <div
                      v-if="mov.ventaNumero && mov.ventaFacturada !== null && mov.ventaFacturada !== undefined"
                      class="text-caption text-medium-emphasis"
                    >
                      {{ mov.ventaFacturada ? 'Facturada' : 'No facturada' }}
                    </div>
                  </div>
                  <div class="d-flex align-center gap-2">
                    <v-btn
                      v-if="mov.ventaNumero"
                      size="small"
                      variant="tonal"
                      color="primary"
                      class="text-none"
                      @click.stop="printVentaTicket(mov.ventaNumero)"
                    >
                      Imprimir ticket
                    </v-btn>
                    <div class="text-caption text-medium-emphasis">{{ formatDate(mov.fecha) }}</div>
                  </div>
                </div>
              </v-expansion-panel-title>
              <v-expansion-panel-text>
                <v-list density="compact">
                  <v-list-item v-for="item in mov.items" :key="item.id">
                    <v-list-item-title>{{ item.nombre }} ({{ item.sku }})</v-list-item-title>
                    <v-list-item-subtitle>
                      Cantidad: {{ item.cantidad }} - {{ item.esIngreso ? 'Ingreso' : 'Egreso' }}
                    </v-list-item-subtitle>
                  </v-list-item>
                </v-list>
              </v-expansion-panel-text>
            </v-expansion-panel>
          </v-expansion-panels>

          <div v-if="!movimientos.length" class="text-caption text-medium-emphasis mt-3">
            Sin movimientos.
          </div>
        </v-card>
      </v-window-item>

      <v-window-item value="alertas">
        <v-card class="pos-card pa-4">
          <div class="d-flex align-center gap-3">
            <v-autocomplete
              v-model="alertaProveedor"
              :items="proveedoresLookup"
              item-title="label"
              return-object
              clearable
              label="Proveedor"
              variant="outlined"
              density="comfortable"
              :loading="proveedorLoading"
              style="min-width: 260px"
              @update:search="searchProveedores"
            />
            <v-btn
              color="primary"
              variant="tonal"
              class="text-none"
              :loading="alertasLoading"
              @click="loadAlertas"
            >
              Actualizar
            </v-btn>
            <v-btn
              color="primary"
              class="text-none"
              :disabled="!alertas.length"
              @click="openRemito"
            >
              Generar remito
            </v-btn>
          </div>

          <v-list density="compact" class="mt-3">
            <v-list-item v-for="alerta in alertas" :key="alerta.productoId">
              <v-list-item-title>{{ alerta.nombre }}</v-list-item-title>
              <v-list-item-subtitle>
                Stock: {{ alerta.cantidadActual }} / Min: {{ alerta.stockMinimo }} / Deseado: {{ alerta.stockDeseado }}
                <div class="text-caption text-medium-emphasis">
                  Proveedor: {{ alerta.proveedor || 'SIN PROVEEDOR' }}
                </div>
              </v-list-item-subtitle>
              <template #append>
                <v-chip
                  size="small"
                  class="status-chip"
                  :color="alerta.nivel === 'CRITICO' ? 'error' : 'warning'"
                  variant="tonal"
                >
                  {{ alerta.nivel }}
                </v-chip>
              </template>
            </v-list-item>
          </v-list>

          <div v-if="!alertas.length" class="text-caption text-medium-emphasis mt-3">
            Sin alertas.
          </div>
        </v-card>
      </v-window-item>
    </v-window>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="top end" timeout="1700">
      <div class="d-flex align-center gap-2">
        <v-icon>{{ snackbar.icon }}</v-icon>
        <span>{{ snackbar.text }}</span>
      </div>
    </v-snackbar>

    <v-dialog v-model="remitoDialog" width="720">
      <v-card>
        <v-card-title>Remito de compra</v-card-title>
        <v-card-text>
          <div class="text-caption text-medium-emphasis mb-2">
            Ajusta cantidades o elimina productos antes de generar el PDF.
          </div>
          <v-list density="compact">
            <v-list-item v-for="item in remitoItems" :key="item.productoId">
              <v-list-item-title>
                {{ item.nombre }} ({{ item.sku }})
              </v-list-item-title>
              <v-list-item-subtitle>
                Actual: {{ item.cantidadActual }} / Deseado: {{ item.stockDeseado }}
                <div class="text-caption text-medium-emphasis">
                  Proveedor: {{ item.proveedor || 'SIN PROVEEDOR' }}
                </div>
              </v-list-item-subtitle>
              <template #append>
                <div class="d-flex align-center gap-2">
                  <v-text-field
                    v-model.number="item.cantidad"
                    type="number"
                    min="0"
                    step="0.01"
                    density="compact"
                    variant="outlined"
                    hide-details
                    style="max-width: 120px"
                  />
                  <v-btn
                    icon="mdi-delete"
                    variant="text"
                    color="error"
                    @click="removeRemitoItem(item.productoId)"
                  />
                </div>
              </template>
            </v-list-item>
          </v-list>
          <div v-if="!remitoItems.length" class="text-caption text-medium-emphasis mt-2">
            Sin productos para remito.
          </div>
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" @click="remitoDialog = false">Cancelar</v-btn>
          <v-btn
            color="primary"
            class="text-none"
            :loading="remitoLoading"
            :disabled="!remitoItems.length"
            @click="generarRemito"
          >
            Generar PDF
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref, watch } from 'vue';
import { useAuthStore } from '../stores/auth';
import { getJson, postJson } from '../services/apiClient';

const auth = useAuthStore();

const tab = ref('saldos');

const saldos = ref([]);
const saldosLoading = ref(false);
const saldoProducto = ref(null);
const saldoProductosLookup = ref([]);
const saldoProductosLoading = ref(false);
const saldoProveedor = ref(null);

const ajusteItems = ref([]);
const ajusteLoading = ref(false);
const ajusteProducto = ref(null);
const ajusteCantidadActual = ref(0);
const ajusteCantidadNueva = ref('');
const ajusteMotivo = ref('');
const ajusteSaving = ref(false);
const ajusteErrors = reactive({
  producto: '',
  cantidad: '',
  motivo: ''
});

const movimientos = ref([]);
const movLoading = ref(false);
const movFilters = reactive({
  productoId: '',
  ventaNumero: '',
  facturada: '',
  desde: '',
  hasta: ''
});
const facturacionOptions = [
  { label: 'Todas', value: '' },
  { label: 'Facturadas', value: 'true' },
  { label: 'No facturadas', value: 'false' }
];

const alertas = ref([]);
const alertasLoading = ref(false);
const alertaProveedor = ref(null);
const proveedoresLookup = ref([]);
const proveedorLoading = ref(false);

const remitoDialog = ref(false);
const remitoItems = ref([]);
const remitoLoading = ref(false);
const printingVentaTicket = ref(false);

const snackbar = ref({
  show: false,
  text: '',
  color: 'success',
  icon: 'mdi-check-circle'
});

const saldoHeaders = [
  { title: 'Producto', value: 'nombre' },
  { title: 'Proveedor', value: 'proveedor' },
  { title: 'SKU', value: 'sku' },
  { title: 'Cantidad', value: 'cantidadActual', align: 'end' }
];

const flash = (type, text) => {
  snackbar.value = {
    show: true,
    text,
    color: type === 'success' ? 'success' : 'error',
    icon: type === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'
  };
};

const extractProblemMessage = (data) => {
  if (!data) return 'Error inesperado.';
  if (data.errors) {
    const firstKey = Object.keys(data.errors)[0];
    if (firstKey && data.errors[firstKey]?.length) {
      return `${firstKey}: ${data.errors[firstKey][0]}`;
    }
  }
  return data.detail || data.title || 'Error inesperado.';
};

const mapSaldoItems = (items) =>
  (items || [])
    .slice()
    .sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '', 'es'))
    .map((item) => ({
      ...item,
      label: `${item.nombre} (${item.sku})`
    }));

const mapSaldoLookup = (items) =>
  (items || [])
    .slice()
    .sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '', 'es'))
    .map((item) => ({
      ...item,
      label: `${item.nombre} (${item.sku})`
    }));

const mapProveedorItems = (items) =>
  (items || [])
    .slice()
    .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'))
    .map((item) => ({
      ...item,
      label: `${item.name} (${item.telefono || 'sin tel'})`
    }));

const formatDate = (value) => {
  if (!value) return '-';
  try {
    return new Date(value).toLocaleString('es-AR');
  } catch {
    return value;
  }
};

const formatMoney = (value) =>
  new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
    minimumFractionDigits: 0
  }).format(Number(value || 0));

const formatDateTime = (value) => {
  if (!value) return '-';
  try {
    return new Date(value).toLocaleString('es-AR');
  } catch {
    return value;
  }
};

const printVentaTicket = async (ventaNumero) => {
  if (!ventaNumero || printingVentaTicket.value) return;
  printingVentaTicket.value = true;
  try {
    const { response, data } = await getJson(`/api/v1/ventas/numero/${ventaNumero}/ticket`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    const ventaData = data?.venta;
    const pagosData = data?.pagos || [];
    if (!ventaData) {
      throw new Error('No se encontro el detalle de la venta.');
    }

    const win = window.open('', '_blank', 'width=380,height=600');
    if (!win) return;

    const itemsRows = (ventaData.items || [])
      .map((item) => {
        const subtotal = item.subtotal ?? item.cantidad * item.precioUnitario;
        return `
          <tr>
            <td>${item.nombre}</td>
            <td style="text-align:right">${item.cantidad}</td>
            <td style="text-align:right">${formatMoney(item.precioUnitario)}</td>
            <td style="text-align:right">${formatMoney(subtotal)}</td>
          </tr>`;
      })
      .join('');

    const pagosRows = (pagosData || [])
      .map(
        (pago) =>
          `<tr><td>${pago.medioPago}</td><td style="text-align:right">${formatMoney(pago.monto)}</td></tr>`
      )
      .join('');

    win.document.write(`
      <html>
        <head>
          <title>Ticket venta</title>
          <style>
            body { font-family: Arial, sans-serif; font-size: 12px; padding: 10px; }
            h1 { margin: 0 0 6px 0; font-size: 14px; }
            table { width: 100%; border-collapse: collapse; margin-top: 8px; }
            th, td { border-bottom: 1px dashed #ccc; padding: 4px 0; }
            th { text-align: left; font-weight: 600; }
          </style>
        </head>
        <body>
          <h1>Ticket de venta</h1>
          <div>Venta N°: ${ventaData.numero ?? '-'}</div>
          <div>Fecha: ${formatDateTime(ventaData.createdAt)}</div>
          <table>
            <thead>
              <tr>
                <th>Producto</th>
                <th style="text-align:right">Cant</th>
                <th style="text-align:right">Precio</th>
                <th style="text-align:right">Subtotal</th>
              </tr>
            </thead>
            <tbody>
              ${itemsRows}
            </tbody>
          </table>
          <div style="margin-top:8px"><strong>Total: ${formatMoney(ventaData.totalNeto)}</strong></div>
          <div style="margin-top:8px">Pagos</div>
          <table>
            <tbody>
              ${pagosRows || '<tr><td>Sin detalle</td><td></td></tr>'}
            </tbody>
          </table>
        </body>
      </html>
    `);
    win.document.close();
    win.focus();
    win.print();
    setTimeout(() => win.close(), 300);
  } catch (err) {
    flash('error', err?.message || 'No se pudo imprimir el ticket.');
  } finally {
    printingVentaTicket.value = false;
  }
};

const loadSaldos = async () => {
  saldosLoading.value = true;
  try {
    const params = new URLSearchParams();
    const productSearch = saldoProducto.value?.sku || saldoProducto.value?.nombre || '';
    if (productSearch.trim()) params.set('search', productSearch.trim());
    if (saldoProveedor.value?.id) params.set('proveedorId', saldoProveedor.value.id);
    const { response, data } = await getJson(`/api/v1/stock/saldos?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    saldos.value = (data || [])
      .slice()
      .sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '', 'es'));
    ajusteItems.value = mapSaldoItems(saldos.value);
    saldoProductosLookup.value = mapSaldoLookup(saldos.value);
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar saldos.');
  } finally {
    saldosLoading.value = false;
  }
};

const searchSaldoProductos = async (term) => {
  saldoProductosLoading.value = true;
  try {
    const params = new URLSearchParams();
    if (term && term.trim()) params.set('search', term.trim());
    if (saldoProveedor.value?.id) params.set('proveedorId', saldoProveedor.value.id);
    const { response, data } = await getJson(`/api/v1/stock/saldos?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    saldoProductosLookup.value = mapSaldoLookup(data || []);
  } catch (err) {
    flash('error', err?.message || 'No se pudieron buscar productos.');
  } finally {
    saldoProductosLoading.value = false;
  }
};

const onSaldoProductoChanged = async () => {
  await loadSaldos();
};

const searchAjusteProductos = async (term) => {
  ajusteLoading.value = true;
  try {
    const params = new URLSearchParams();
    if (term && term.trim()) params.set('search', term.trim());
    if (saldoProveedor.value?.id) params.set('proveedorId', saldoProveedor.value.id);
    const { response, data } = await getJson(`/api/v1/stock/saldos?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    ajusteItems.value = mapSaldoItems(data || []);
  } catch (err) {
    flash('error', err?.message || 'No se pudieron buscar productos.');
  } finally {
    ajusteLoading.value = false;
  }
};

const resetAjusteErrors = () => {
  ajusteErrors.producto = '';
  ajusteErrors.cantidad = '';
  ajusteErrors.motivo = '';
};

const validateAjuste = () => {
  resetAjusteErrors();
  if (!ajusteProducto.value) {
    ajusteErrors.producto = 'Selecciona un producto.';
  }

  if (ajusteCantidadNueva.value === '' || Number.isNaN(Number(ajusteCantidadNueva.value))) {
    ajusteErrors.cantidad = 'Cantidad invalida.';
  } else if (Number(ajusteCantidadNueva.value) < 0) {
    ajusteErrors.cantidad = 'Cantidad no puede ser negativa.';
  }

  if (!ajusteMotivo.value.trim()) {
    ajusteErrors.motivo = 'El motivo es obligatorio.';
  }

  return !ajusteErrors.producto && !ajusteErrors.cantidad && !ajusteErrors.motivo;
};

const aplicarAjuste = async () => {
  if (ajusteSaving.value) return;
  if (!validateAjuste()) return;

  const actual = Number(ajusteCantidadActual.value || 0);
  const nuevo = Number(ajusteCantidadNueva.value);
  const delta = nuevo - actual;

  if (delta === 0) {
    flash('error', 'La cantidad no cambio.');
    return;
  }

  ajusteSaving.value = true;
  try {
    const payload = {
      tipo: 'AJUSTE',
      motivo: ajusteMotivo.value.trim(),
      items: [
        {
          productoId: ajusteProducto.value.productoId,
          cantidad: Math.abs(delta),
          esIngreso: delta > 0
        }
      ]
    };

    const { response, data } = await postJson('/api/v1/stock/ajustes', payload);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    flash('success', 'Stock ajustado');
    ajusteMotivo.value = '';
    await loadSaldos();
    if (tab.value === 'movimientos') {
      await loadMovimientos();
    }
  } catch (err) {
    flash('error', err?.message || 'No se pudo ajustar el stock.');
  } finally {
    ajusteSaving.value = false;
  }
};

const loadMovimientos = async () => {
  movLoading.value = true;
  try {
    const params = new URLSearchParams();
    if (movFilters.productoId.trim()) params.set('productoId', movFilters.productoId.trim());
    if (movFilters.ventaNumero && movFilters.ventaNumero.toString().trim()) {
      params.set('ventaNumero', movFilters.ventaNumero.toString().trim());
    }
    if (movFilters.facturada === 'true' || movFilters.facturada === 'false') {
      params.set('facturada', movFilters.facturada);
    }
    if (movFilters.desde) params.set('desde', movFilters.desde);
    if (movFilters.hasta) params.set('hasta', movFilters.hasta);

    const { response, data } = await getJson(`/api/v1/stock/movimientos?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    movimientos.value = (data || []).map((mov) => ({
      ...mov,
      items: (mov.items || [])
        .slice()
        .sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '', 'es'))
    }));
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar movimientos.');
  } finally {
    movLoading.value = false;
  }
};

const loadAlertas = async () => {
  alertasLoading.value = true;
  try {
    const params = new URLSearchParams();
    if (alertaProveedor.value?.id) {
      params.set('proveedorId', alertaProveedor.value.id);
    }
    const url = params.toString() ? `/api/v1/stock/alertas?${params.toString()}` : '/api/v1/stock/alertas';
    const { response, data } = await getJson(url);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    alertas.value = (data || [])
      .slice()
      .sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '', 'es'));
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar alertas.');
  } finally {
    alertasLoading.value = false;
  }
};

const shouldIncludeInRemito = (alerta) => {
  const minimo = Number(alerta.stockMinimo ?? 0);
  const tolerancia = Number(alerta.toleranciaPct ?? 0);
  const limite = minimo * (1 + tolerancia / 100);
  return Number(alerta.cantidadActual ?? 0) <= limite;
};

const openRemito = async () => {
  if (alertasLoading.value) return;
  await loadAlertas();
  const filtradas = alertas.value.filter(shouldIncludeInRemito);
  remitoItems.value = filtradas.map((alerta) => ({
    productoId: alerta.productoId,
    nombre: alerta.nombre,
    sku: alerta.sku,
    proveedor: alerta.proveedor,
    cantidadActual: alerta.cantidadActual,
    stockDeseado: alerta.stockDeseado ?? 0,
    cantidad: (() => {
      const deseado = Number(alerta.stockDeseado ?? 0);
      const minimo = Number(alerta.stockMinimo ?? 0);
      const objetivo = deseado > 0 ? deseado : minimo;
      const sugerido = alerta.sugerido ?? Math.max(objetivo - alerta.cantidadActual, 0);
      return sugerido;
    })()
  }));
  remitoDialog.value = true;
};

const removeRemitoItem = (productoId) => {
  remitoItems.value = remitoItems.value.filter((item) => item.productoId !== productoId);
};

const generarRemito = async () => {
  if (remitoLoading.value) return;
  const items = remitoItems.value
    .filter((item) => Number(item.cantidad) > 0)
    .map((item) => ({ productoId: item.productoId, cantidad: Number(item.cantidad) }));

  if (!items.length) {
    flash('error', 'No hay cantidades para generar.');
    return;
  }

  remitoLoading.value = true;
  try {
    const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5072';
    const proveedorId = alertaProveedor.value?.id || null;
    const response = await fetch(`${baseUrl}/api/v1/stock/alertas/remito`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(auth.token ? { Authorization: `Bearer ${auth.token}` } : {})
      },
      body: JSON.stringify({ items, proveedorId })
    });

    if (!response.ok) {
      let message = 'No se pudo generar el remito.';
      try {
        const data = await response.json();
        message = extractProblemMessage(data);
      } catch {
        // ignore
      }
      throw new Error(message);
    }

    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'remito-alertas.pdf';
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
    flash('success', 'Remito generado');
    remitoDialog.value = false;
  } catch (err) {
    flash('error', err?.message || 'No se pudo generar el remito.');
  } finally {
    remitoLoading.value = false;
  }
};

const searchProveedores = async (term) => {
  proveedorLoading.value = true;
  try {
    const params = new URLSearchParams();
    if (term && term.trim()) params.set('search', term.trim());
    params.set('activo', 'true');
    const { response, data } = await getJson(`/api/v1/proveedores?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    proveedoresLookup.value = mapProveedorItems(data || []);
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar proveedores.');
  } finally {
    proveedorLoading.value = false;
  }
};

watch(tab, (value) => {
  if (value === 'saldos') {
    loadSaldos();
  }
  if (value === 'movimientos') {
    loadMovimientos();
  }
  if (value === 'alertas') {
    loadAlertas();
  }
});

watch(alertaProveedor, () => {
  if (tab.value === 'alertas') {
    loadAlertas();
  }
});

watch(saldoProveedor, () => {
  saldoProducto.value = null;
  searchSaldoProductos('');
  if (tab.value === 'saldos') {
    loadSaldos();
  }
});

watch(ajusteProducto, (value) => {
  if (!value) {
    ajusteCantidadActual.value = 0;
    ajusteCantidadNueva.value = '';
    return;
  }

  ajusteCantidadActual.value = value.cantidadActual ?? 0;
  ajusteCantidadNueva.value = (value.cantidadActual ?? 0).toString();
});

onMounted(() => {
  searchProveedores('');
  searchSaldoProductos('');
  loadSaldos();
});
</script>

<style scoped>
.stock-page {
  animation: fade-in 0.3s ease;
}

</style>

