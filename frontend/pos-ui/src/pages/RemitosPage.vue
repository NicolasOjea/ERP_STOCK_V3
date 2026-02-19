<template>
  <div class="remitos-page">
    <v-card class="pos-card pa-4">
      <div class="d-flex align-center gap-3">
        <div>
          <div class="text-h6">Ingreso manual de remito</div>
          <div class="text-caption text-medium-emphasis">
            El proveedor es opcional y funciona como filtro
          </div>
        </div>
      </div>

      <v-alert v-if="error" type="error" variant="tonal" class="mt-3" density="compact">
        {{ error }}
      </v-alert>

      <v-row dense class="mt-2">
        <v-col cols="12" md="6">
          <v-autocomplete
            v-model="selectedProveedorId"
            :items="proveedores"
            item-title="name"
            item-value="id"
            label="Proveedor"
            variant="outlined"
            density="comfortable"
            clearable
            :loading="loadingProveedores"
            @update:model-value="onProveedorChanged"
          />
        </v-col>
      </v-row>

      <v-divider class="my-2" />

      <v-row dense>
        <v-col cols="12" md="4">
          <v-text-field
            v-model="scanSku"
            label="Escanear SKU"
            variant="outlined"
            density="comfortable"
            clearable
            @keyup.enter="scanAndSelect"
          />
        </v-col>
        <v-col cols="12" md="4">
          <v-autocomplete
            v-model="selectedProductoId"
            v-model:search="productSearch"
            :items="productoOptions"
            item-title="label"
            item-value="id"
            label="Producto (nombre o SKU)"
            variant="outlined"
            density="comfortable"
            clearable
            :loading="loadingProductos"
            :no-data-text="'Sin resultados'"
            @update:search="onProductSearch"
          />
        </v-col>
        <v-col cols="12" md="2">
          <v-text-field
            v-model="addCantidad"
            label="Cantidad"
            type="number"
            min="0.01"
            step="0.01"
            variant="outlined"
            density="comfortable"
          />
        </v-col>
        <v-col cols="12" md="2" class="d-flex align-center">
          <v-btn
            color="primary"
            class="text-none w-100"
            :disabled="!selectedProductoId || addingItem"
            :loading="addingItem"
            @click="addItem"
          >
            Agregar
          </v-btn>
        </v-col>
      </v-row>

      <v-divider class="my-3" />

      <v-data-table
        :headers="headers"
        :items="items"
        item-key="productoId"
        density="compact"
        height="360"
      >
        <template v-slot:[`item.cantidad`]="{ item }">
          <v-text-field
            :model-value="item.cantidad"
            type="number"
            min="0.01"
            step="0.01"
            variant="outlined"
            density="compact"
            hide-details
            @update:model-value="(v) => updateCantidad(item.productoId, v)"
          />
        </template>
        <template v-slot:[`item.actions`]="{ item }">
          <v-btn icon="mdi-delete-outline" size="small" variant="text" color="error" @click="removeItem(item.productoId)" />
        </template>
      </v-data-table>

      <div class="d-flex justify-space-between align-center mt-4">
        <div class="text-caption text-medium-emphasis">
          Productos: {{ items.length }}
        </div>
        <div class="d-flex gap-2">
          <v-btn variant="text" class="text-none" :disabled="saving" @click="clearForm">Limpiar</v-btn>
          <v-btn
            color="primary"
            class="text-none"
            :loading="saving"
            :disabled="!canSave"
            @click="confirmarIngreso"
          >
            Confirmar ingreso
          </v-btn>
        </div>
      </div>
    </v-card>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="top end" timeout="1700">
      <div class="d-flex align-center gap-2">
        <v-icon>{{ snackbar.icon }}</v-icon>
        <span>{{ snackbar.text }}</span>
      </div>
    </v-snackbar>

    <v-dialog v-model="createDialog" width="560">
      <v-card>
        <v-card-title>Crear producto</v-card-title>
        <v-card-text>
          <div class="text-caption text-medium-emphasis mb-3">
            SKU no encontrado. Puedes crearlo ahora y seguir con el ingreso.
          </div>
          <v-text-field
            v-model="createForm.sku"
            label="SKU"
            variant="outlined"
            density="comfortable"
          />
          <v-text-field
            v-model="createForm.name"
            label="Nombre"
            variant="outlined"
            density="comfortable"
          />
          <v-autocomplete
            v-model="createForm.proveedorId"
            :items="proveedores"
            item-title="name"
            item-value="id"
            label="Proveedor"
            variant="outlined"
            density="comfortable"
          />
          <v-text-field
            v-model="createForm.precioBase"
            label="Precio base"
            variant="outlined"
            density="comfortable"
            type="number"
            min="0"
            step="0.01"
          />
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" @click="createDialog = false">Cancelar</v-btn>
          <v-btn color="primary" class="text-none" :loading="creatingProduct" @click="createProductFromIngreso">
            Crear producto
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue';
import { getJson, postJson } from '../services/apiClient';

const loadingProveedores = ref(false);
const loadingProductos = ref(false);
const addingItem = ref(false);
const saving = ref(false);

const error = ref('');
const proveedores = ref([]);
const productos = ref([]);

const selectedProveedorId = ref(null);
const selectedProductoId = ref(null);
const productSearch = ref('');
const scanSku = ref('');
const addCantidad = ref('1');
const items = ref([]);
const createDialog = ref(false);
const creatingProduct = ref(false);
const createForm = ref({
  sku: '',
  name: '',
  proveedorId: null,
  precioBase: '0'
});

const snackbar = ref({
  show: false,
  text: '',
  color: 'success',
  icon: 'mdi-check-circle'
});

const headers = [
  { title: 'Producto', value: 'name' },
  { title: 'SKU', value: 'sku' },
  { title: 'Cantidad', value: 'cantidad', width: 180 },
  { title: '', value: 'actions', sortable: false, align: 'end', width: 80 }
];

const productoOptions = computed(() =>
  productos.value
    .slice()
    .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'))
    .map((p) => ({
      ...p,
      label: `${p.name} (${p.sku})`
    }))
);

const canSave = computed(() => items.value.length > 0);

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

const loadProveedores = async () => {
  loadingProveedores.value = true;
  try {
    const { response, data } = await getJson('/api/v1/proveedores?activo=true');
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    proveedores.value = (data || [])
      .slice()
      .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'));
  } catch (err) {
    error.value = err?.message || 'No se pudieron cargar proveedores.';
  } finally {
    loadingProveedores.value = false;
  }
};

const loadProductos = async (search = '') => {
  loadingProductos.value = true;
  try {
    const params = new URLSearchParams();
    params.set('activo', 'true');
    if (search.trim()) {
      params.set('search', search.trim());
    }

    const { response, data } = await getJson(`/api/v1/productos?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    const allProducts = (data || [])
      .slice()
      .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'));
    productos.value = selectedProveedorId.value
      ? allProducts.filter((p) => p.proveedorId === selectedProveedorId.value)
      : allProducts;
  } catch (err) {
    error.value = err?.message || 'No se pudieron cargar productos.';
  } finally {
    loadingProductos.value = false;
  }
};

const onProveedorChanged = async () => {
  selectedProductoId.value = null;
  await loadProductos('');
};

const onProductSearch = async (value) => {
  await loadProductos(value || '');
};

const addItem = async () => {
  if (!selectedProductoId.value) return;

  const qty = Number(addCantidad.value);
  if (Number.isNaN(qty) || qty <= 0) {
    flash('error', 'Cantidad invalida.');
    return;
  }

  addingItem.value = true;
  try {
    const producto = productos.value.find((p) => p.id === selectedProductoId.value);
    if (!producto) {
      flash('error', 'Producto no encontrado.');
      return;
    }

    const existing = items.value.find((i) => i.productoId === producto.id);
    if (existing) {
      existing.cantidad = Number(existing.cantidad) + qty;
    } else {
      items.value.push({
        productoId: producto.id,
        name: producto.name,
        sku: producto.sku,
        cantidad: qty
      });
    }

    selectedProductoId.value = null;
    addCantidad.value = '1';
  } finally {
    addingItem.value = false;
  }
};

const removeItem = (productId) => {
  items.value = items.value.filter((i) => i.productoId !== productId);
};

const updateCantidad = (productId, value) => {
  const qty = Number(value);
  const item = items.value.find((i) => i.productoId === productId);
  if (!item) return;

  if (Number.isNaN(qty) || qty <= 0) {
    item.cantidad = 0;
    return;
  }
  item.cantidad = qty;
};

const clearForm = () => {
  selectedProveedorId.value = null;
  selectedProductoId.value = null;
  productSearch.value = '';
  addCantidad.value = '1';
  items.value = [];
  error.value = '';
  loadProductos('');
};

const resetCreateForm = () => {
  createForm.value = {
    sku: '',
    name: '',
    proveedorId: selectedProveedorId.value || null,
    precioBase: '0'
  };
};

const scanAndSelect = async () => {
  const sku = (scanSku.value || '').trim();
  if (!sku) return;

  const params = new URLSearchParams();
  params.set('activo', 'true');
  params.set('search', sku);
  const { response, data } = await getJson(`/api/v1/productos?${params.toString()}`);
  if (!response.ok) {
    flash('error', extractProblemMessage(data));
    return;
  }

  const all = data || [];
  const exact = all.find((p) => (p.sku || '').trim().toLowerCase() === sku.toLowerCase());
  if (exact) {
    await loadProductos(sku);
    selectedProductoId.value = exact.id;
    scanSku.value = '';
    flash('success', `Producto encontrado: ${exact.name}`);
    return;
  }

  resetCreateForm();
  createForm.value.sku = sku;
  createForm.value.name = sku;
  createDialog.value = true;
};

const createProductFromIngreso = async () => {
  if (creatingProduct.value) return;

  const sku = (createForm.value.sku || '').trim();
  const name = (createForm.value.name || '').trim();
  const proveedorId = createForm.value.proveedorId;
  const precioBase = Number(createForm.value.precioBase || 0);

  if (!sku) {
    flash('error', 'SKU obligatorio.');
    return;
  }
  if (!name) {
    flash('error', 'Nombre obligatorio.');
    return;
  }
  if (!proveedorId) {
    flash('error', 'Proveedor obligatorio para crear producto.');
    return;
  }

  creatingProduct.value = true;
  try {
    const payload = {
      name,
      sku,
      proveedorId,
      isActive: true,
      precioBase: Number.isNaN(precioBase) ? 0 : precioBase,
      precioVenta: Number.isNaN(precioBase) ? 0 : precioBase
    };

    const { response, data } = await postJson('/api/v1/productos', payload);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    await loadProductos(sku);
    const created = productos.value.find((p) => p.id === data.id) || productos.value.find((p) => p.sku === sku);
    if (created) {
      selectedProductoId.value = created.id;
    }

    createDialog.value = false;
    scanSku.value = '';
    flash('success', 'Producto creado. Ya puedes agregarlo al ingreso.');
  } catch (err) {
    flash('error', err?.message || 'No se pudo crear el producto.');
  } finally {
    creatingProduct.value = false;
  }
};

const confirmarIngreso = async () => {
  if (saving.value || !canSave.value) return;

  const invalid = items.value.some((i) => Number(i.cantidad) <= 0);
  if (invalid) {
    flash('error', 'Todas las cantidades deben ser mayores a 0.');
    return;
  }

  const proveedor = proveedores.value.find((p) => p.id === selectedProveedorId.value);
  const motivo = proveedor
    ? `Ingreso manual remito - filtro proveedor ${proveedor.name}`
    : 'Ingreso manual remito - varios proveedores';

  saving.value = true;
  try {
    const payload = {
      tipo: 'AJUSTE',
      motivo,
      items: items.value.map((i) => ({
        productoId: i.productoId,
        cantidad: Number(i.cantidad),
        esIngreso: true
      }))
    };

    const { response, data } = await postJson('/api/v1/stock/ajustes', payload);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    flash('success', 'Ingreso de productos registrado.');
    clearForm();
    await loadProveedores();
  } catch (err) {
    flash('error', err?.message || 'No se pudo registrar el ingreso.');
  } finally {
    saving.value = false;
  }
};

onMounted(async () => {
  await loadProveedores();
  await loadProductos('');
  resetCreateForm();
});
</script>

<style scoped>
.remitos-page {
  animation: fade-in 0.3s ease;
}
</style>

