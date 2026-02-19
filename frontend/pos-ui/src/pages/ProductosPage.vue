<template>
  <div class="productos-page">
    <v-tabs v-model="tab" color="primary" class="mb-4">
      <v-tab value="productos">Productos</v-tab>
      <v-tab value="proveedores">Proveedores</v-tab>
      <v-tab value="listas-precio">Lista de precios</v-tab>
    </v-tabs>

    <v-window v-model="tab">
      <v-window-item value="productos">
        <v-card class="pos-card pa-4 mb-4">
          <div class="d-flex flex-wrap align-center gap-3">
            <div>
              <div class="text-h6">Productos</div>
              <div class="text-caption text-medium-emphasis">ABM + stock</div>
            </div>
            <v-spacer />
            <v-btn color="primary" class="text-none" @click="resetForm">Nuevo</v-btn>
            <v-btn
              color="primary"
              variant="tonal"
              class="text-none"
              :loading="barcodeLoading"
              :disabled="products.length === 0"
              @click="printBarcodes"
            >
              IMPRIMIR CODIGOS DE BARRA
            </v-btn>
          </div>

          <div class="mt-4 d-flex flex-wrap align-center gap-3">
            <v-text-field
              v-model="search"
              label="Buscar (nombre o SKU)"
              variant="outlined"
              density="comfortable"
              hide-details
              style="min-width: 260px"
              @keyup.enter="loadProducts"
            />
            <v-text-field
              v-model="scanInput"
              label="Escanear SKU"
              variant="outlined"
              density="comfortable"
              hide-details
              style="min-width: 220px"
              @keyup.enter="handleScan"
            />
            <v-btn-toggle v-model="activoFilter" density="comfortable" mandatory class="ml-2">
              <v-btn value="all" class="text-none">Todos</v-btn>
              <v-btn value="true" class="text-none">Activos</v-btn>
              <v-btn value="false" class="text-none">Inactivos</v-btn>
            </v-btn-toggle>
            <v-btn
              color="primary"
              variant="tonal"
              class="text-none"
              :loading="loading"
              @click="loadProducts"
            >
              Buscar
            </v-btn>
          </div>
        </v-card>

        <v-row dense>
          <v-col cols="12" md="7">
            <v-card class="pos-card pa-4">
              <div class="text-h6">Listado</div>
              <div class="text-caption text-medium-emphasis">Selecciona un producto para editar</div>
              <v-data-table
                :headers="headers"
                :items="products"
                item-key="id"
                class="mt-3"
                density="compact"
                height="520"
                @click:row="selectProduct"
              >
                <template v-slot:[`item.precioBase`]="{ item }">
                  {{ formatMoney(item.precioBase) }}
                </template>
                <template v-slot:[`item.precioVenta`]="{ item }">
                  {{ formatMoney(item.precioVenta) }}
                </template>
                <template v-slot:[`item.isActive`]="{ item }">
                  <v-chip size="small" :color="item.isActive ? 'success' : 'error'" variant="tonal">
                    {{ item.isActive ? 'Activo' : 'Inactivo' }}
                  </v-chip>
                </template>
              </v-data-table>
            </v-card>
          </v-col>

          <v-col cols="12" md="5">
            <v-card class="pos-card pa-4">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-h6">{{ form.id ? 'Editar producto' : 'Nuevo producto' }}</div>
                  <div class="text-caption text-medium-emphasis">
                    {{ form.id ? shortId(form.id) : 'Sin seleccionar' }}
                  </div>
                </div>
                <v-btn
                  color="primary"
                  variant="tonal"
                  class="text-none"
                  :loading="saving"
                  @click="saveProduct"
                >
                  Guardar
                </v-btn>
              </div>

              <v-form class="mt-3">
                <v-text-field
                  v-model="form.name"
                  label="Nombre"
                  variant="outlined"
                  density="comfortable"
                  :error-messages="errors.name"
                  @blur="validateField('name')"
                  required
                />
                <v-text-field
                  v-model="form.sku"
                  label="SKU"
                  variant="outlined"
                  density="comfortable"
                  :error-messages="errors.sku"
                  @blur="validateField('sku')"
                  required
                />
                <v-autocomplete
                  v-model="form.proveedorId"
                  :items="proveedoresLookup"
                  :loading="proveedorLoading"
                  label="Proveedor"
                  item-title="name"
                  item-value="id"
                  variant="outlined"
                  density="comfortable"
                  clearable
                  :error-messages="errors.proveedorId"
                  @update:search="searchProveedores"
                  @blur="validateField('proveedorId')"
                  required
                />
                <v-text-field
                  v-model="form.precioBase"
                  label="Precio base"
                  variant="outlined"
                  density="comfortable"
                  :error-messages="errors.precioBase"
                  @blur="validateField('precioBase')"
                />
                <v-select
                  v-model="form.pricingMode"
                  :items="pricingModeItems"
                  label="Modo precio"
                  variant="outlined"
                  density="comfortable"
                  item-title="title"
                  item-value="value"
                />
                <v-autocomplete
                  v-if="form.pricingMode === 'CATEGORIA'"
                  v-model="form.categoriaId"
                  :items="categoriasLookupDisplay"
                  :loading="categoriaLoading"
                  label="Categoria"
                  item-title="displayTitle"
                  item-value="id"
                  variant="outlined"
                  density="comfortable"
                  clearable
                  no-data-text="No hay categorias. Crea una en 'Lista de precios'."
                  @update:search="searchCategorias"
                  @focus="ensureCategoriasLookup"
                />
                <div
                  v-if="form.pricingMode === 'CATEGORIA'"
                  class="text-caption text-medium-emphasis mb-2"
                >
                  Categoria seleccionada: {{ categoriaSeleccionadaLabel }}
                </div>
                <v-text-field
                  v-if="form.pricingMode === 'FIJO_PCT'"
                  v-model="form.margenPct"
                  label="% Ganancia fija"
                  variant="outlined"
                  density="comfortable"
                  type="number"
                  min="0"
                  step="0.01"
                  :error-messages="errors.margenPct"
                  @blur="validateField('margenPct')"
                />
                <div v-if="form.pricingMode === 'CATEGORIA'" class="text-caption text-medium-emphasis mb-2">
                  Margen categoria: {{ Number(form.margenCategoriaPct || 0).toFixed(2) }}%
                </div>
                <v-text-field
                  v-if="form.pricingMode === 'MANUAL'"
                  v-model="form.precioVentaManual"
                  label="Precio venta manual"
                  variant="outlined"
                  density="comfortable"
                  type="number"
                  min="0"
                  step="0.01"
                  :error-messages="errors.precioVentaManual"
                  @blur="validateField('precioVentaManual')"
                />
                <div v-else class="text-caption text-medium-emphasis">
                  Precio venta calculado: {{ formatMoney(precioVentaCalculado) }}
                </div>
                <v-text-field
                  v-model="stockConfig.stockMinimo"
                  label="Stock minimo"
                  type="number"
                  min="0"
                  step="0.01"
                  variant="outlined"
                  density="comfortable"
                  :error-messages="errors.stockMinimo"
                  @blur="validateField('stockMinimo')"
                />
                <v-text-field
                  v-model="stockConfig.stockDeseado"
                  label="Stock deseado"
                  type="number"
                  min="0"
                  step="0.01"
                  variant="outlined"
                  density="comfortable"
                  :error-messages="errors.stockDeseado"
                  @blur="validateField('stockDeseado')"
                />
                <v-text-field
                  v-model="stockConfig.toleranciaPct"
                  label="Tolerancia (%)"
                  type="number"
                  min="0"
                  step="0.01"
                  variant="outlined"
                  density="comfortable"
                  suffix="%"
                  :error-messages="errors.toleranciaPct"
                  @blur="validateField('toleranciaPct')"
                />
                <v-text-field
                  v-model="form.stockInicial"
                  label="Stock inicial"
                  type="number"
                  min="0"
                  step="0.01"
                  variant="outlined"
                  density="comfortable"
                  hint="Solo aplica al crear el producto"
                  persistent-hint
                  :disabled="!!form.id"
                  :error-messages="errors.stockInicial"
                  @blur="validateField('stockInicial')"
                />
                <v-switch
                  :model-value="form.isActive"
                  label="Activo"
                  color="primary"
                  inset
                  @update:model-value="toggleActive"
                />
              </v-form>

              <div class="d-flex justify-end mt-4">
                <v-btn
                  color="primary"
                  variant="tonal"
                  class="text-none"
                  :loading="saving"
                  @click="saveFromBottom"
                >
                  Guardar
                </v-btn>
              </div>
            </v-card>
          </v-col>
        </v-row>
      </v-window-item>

      <v-window-item value="proveedores">
        <v-card class="pos-card pa-4 mb-4">
          <div class="d-flex flex-wrap align-center gap-3">
            <div>
              <div class="text-h6">Proveedores</div>
              <div class="text-caption text-medium-emphasis">Alta y gestion</div>
            </div>
            <v-spacer />
            <v-btn color="primary" class="text-none" @click="resetProveedorForm">Nuevo</v-btn>
          </div>

          <div class="mt-4 d-flex flex-wrap align-center gap-3">
            <v-text-field
              v-model="proveedorSearch"
              label="Buscar proveedor"
              variant="outlined"
              density="comfortable"
              hide-details
              style="min-width: 260px"
              @keyup.enter="loadProveedores"
            />
            <v-btn-toggle v-model="proveedorActivoFilter" density="comfortable" mandatory class="ml-2">
              <v-btn value="all" class="text-none">Todos</v-btn>
              <v-btn value="true" class="text-none">Activos</v-btn>
              <v-btn value="false" class="text-none">Inactivos</v-btn>
            </v-btn-toggle>
            <v-btn
              color="primary"
              variant="tonal"
              class="text-none"
              :loading="proveedorLoadingTable"
              @click="loadProveedores"
            >
              Buscar
            </v-btn>
          </div>
        </v-card>

        <v-row dense>
          <v-col cols="12" md="7">
            <v-card class="pos-card pa-4">
              <div class="text-h6">Listado</div>
              <div class="text-caption text-medium-emphasis">Selecciona para editar</div>
              <v-data-table
                :headers="proveedorHeaders"
                :items="proveedoresTable"
                item-key="id"
                class="mt-3"
                density="compact"
                height="520"
                @click:row="selectProveedor"
              >
                <template v-slot:[`item.isActive`]="{ item }">
                  <v-chip size="small" :color="item.isActive ? 'success' : 'error'" variant="tonal">
                    {{ item.isActive ? 'Activo' : 'Inactivo' }}
                  </v-chip>
                </template>
              </v-data-table>
            </v-card>
          </v-col>

          <v-col cols="12" md="5">
            <v-card class="pos-card pa-4">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-h6">{{ proveedorForm.id ? 'Editar proveedor' : 'Nuevo proveedor' }}</div>
                  <div class="text-caption text-medium-emphasis">
                    {{ proveedorForm.id ? shortId(proveedorForm.id) : 'Sin seleccionar' }}
                  </div>
                </div>
                <v-btn
                  color="primary"
                  variant="tonal"
                  class="text-none"
                  :loading="proveedorSaving"
                  @click="saveProveedor"
                >
                  Guardar
                </v-btn>
              </div>

              <v-form class="mt-3">
                <v-text-field
                  v-model="proveedorForm.name"
                  label="Nombre"
                  variant="outlined"
                  density="comfortable"
                  :error-messages="proveedorErrors.name"
                  @blur="validateProveedorField('name')"
                  required
                />
                <v-text-field
                  v-model="proveedorForm.telefono"
                  label="Telefono"
                  variant="outlined"
                  density="comfortable"
                  :error-messages="proveedorErrors.telefono"
                  @blur="validateProveedorField('telefono')"
                  required
                />
                <v-text-field
                  v-model="proveedorForm.cuit"
                  label="CUIT"
                  variant="outlined"
                  density="comfortable"
                />
                <v-text-field
                  v-model="proveedorForm.direccion"
                  label="Direccion"
                  variant="outlined"
                  density="comfortable"
                />
                <v-switch
                  :model-value="proveedorForm.isActive"
                  label="Activo"
                  color="primary"
                  inset
                  @update:model-value="(value) => (proveedorForm.isActive = value)"
                />
              </v-form>
            </v-card>
          </v-col>
        </v-row>
      </v-window-item>

      <v-window-item value="listas-precio">
        <v-card class="pos-card pa-4 mb-4">
          <div class="d-flex align-center gap-3">
            <div>
              <div class="text-h6">Lista de precios</div>
              <div class="text-caption text-medium-emphasis">Margen por categoria</div>
            </div>
            <v-spacer />
            <v-btn color="primary" class="text-none" @click="resetCategoriaForm">Nueva categoria</v-btn>
          </div>

          <div class="mt-4 d-flex flex-wrap align-center gap-3">
            <v-text-field
              v-model="categoriaSearch"
              label="Buscar categoria"
              variant="outlined"
              density="comfortable"
              hide-details
              style="min-width: 280px"
              @keyup.enter="loadCategorias"
            />
            <v-btn-toggle v-model="categoriaActivoFilter" density="comfortable" mandatory>
              <v-btn value="all" class="text-none">Todos</v-btn>
              <v-btn value="true" class="text-none">Activos</v-btn>
              <v-btn value="false" class="text-none">Inactivos</v-btn>
            </v-btn-toggle>
            <v-btn color="primary" variant="tonal" class="text-none" :loading="categoriaLoadingTable" @click="loadCategorias">
              Buscar
            </v-btn>
          </div>
        </v-card>

        <v-row dense>
          <v-col cols="12" md="7">
            <v-card class="pos-card pa-4">
              <div class="text-h6">Categorias</div>
              <v-data-table
                :headers="categoriaHeaders"
                :items="categoriasTable"
                item-key="id"
                class="mt-3"
                density="compact"
                height="520"
                @click:row="selectCategoria"
              >
                <template v-slot:[`item.margenGananciaPct`]="{ item }">
                  {{ Number(item.margenGananciaPct || 0).toFixed(2) }}%
                </template>
                <template v-slot:[`item.isActive`]="{ item }">
                  <v-chip size="small" :color="item.isActive ? 'success' : 'error'" variant="tonal">
                    {{ item.isActive ? 'Activo' : 'Inactivo' }}
                  </v-chip>
                </template>
              </v-data-table>
            </v-card>
          </v-col>
          <v-col cols="12" md="5">
            <v-card class="pos-card pa-4">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-h6">{{ categoriaForm.id ? 'Editar categoria' : 'Nueva categoria' }}</div>
                  <div class="text-caption text-medium-emphasis">
                    {{ categoriaForm.id ? shortId(categoriaForm.id) : 'Sin seleccionar' }}
                  </div>
                </div>
                <v-btn color="primary" variant="tonal" class="text-none" :loading="categoriaSaving" @click="saveCategoria">
                  Guardar
                </v-btn>
              </div>

              <v-form class="mt-3">
                <v-text-field
                  v-model="categoriaForm.name"
                  label="Nombre"
                  variant="outlined"
                  density="comfortable"
                  :error-messages="categoriaErrors.name"
                  @blur="validateCategoriaField('name')"
                  required
                />
                <v-text-field
                  v-model="categoriaForm.margenGananciaPct"
                  label="% Ganancia"
                  variant="outlined"
                  density="comfortable"
                  type="number"
                  min="0"
                  step="0.01"
                  :error-messages="categoriaErrors.margenGananciaPct"
                  @blur="validateCategoriaField('margenGananciaPct')"
                  required
                />
                <v-checkbox
                  v-model="categoriaForm.aplicarAProductos"
                  label="Aplicar nuevo porcentaje a productos de la categoria"
                  color="primary"
                  hide-details
                />
                <v-switch
                  :model-value="categoriaForm.isActive"
                  label="Activo"
                  color="primary"
                  inset
                  @update:model-value="(value) => (categoriaForm.isActive = value)"
                />
              </v-form>
            </v-card>
          </v-col>
        </v-row>
      </v-window-item>
    </v-window>

    <v-dialog v-model="dialogDesactivar" width="420">
      <v-card>
        <v-card-title>Desactivar producto</v-card-title>
        <v-card-text>
          Esta accion desactiva el producto y lo oculta de operaciones. Continuar?
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" @click="cancelDeactivate">Cancelar</v-btn>
          <v-btn color="error" @click="confirmDeactivate">Desactivar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="top end" timeout="1700">
      <div class="d-flex align-center gap-2">
        <v-icon>{{ snackbar.icon }}</v-icon>
        <span>{{ snackbar.text }}</span>
      </div>
    </v-snackbar>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { getJson, postJson, requestJson } from '../services/apiClient';

const tab = ref('productos');
const route = useRoute();
const router = useRouter();
const auth = useAuthStore();

const products = ref([]);
const loading = ref(false);
const saving = ref(false);
const barcodeLoading = ref(false);

const search = ref('');
const scanInput = ref('');
const activoFilter = ref('all');

const proveedoresLookup = ref([]);
const proveedorLoading = ref(false);
const categoriaLoading = ref(false);
const categoriasLookup = ref([]);

const form = reactive({
  id: '',
  name: '',
  sku: '',
  proveedorId: '',
  categoriaId: '',
  precioBase: '',
  pricingMode: 'FIJO_PCT',
  margenPct: '30',
  margenCategoriaPct: '0',
  precioVentaManual: '',
  stockInicial: '',
  isActive: true
});

const stockConfig = reactive({
  stockMinimo: '',
  stockDeseado: '',
  toleranciaPct: '25'
});

const errors = reactive({
  name: '',
  sku: '',
  proveedorId: '',
  precioBase: '',
  margenPct: '',
  precioVentaManual: '',
  stockInicial: '',
  stockMinimo: '',
  stockDeseado: '',
  toleranciaPct: ''
});


const dialogDesactivar = ref(false);
const pendingActive = ref(true);

const snackbar = ref({
  show: false,
  text: '',
  color: 'success',
  icon: 'mdi-check-circle'
});

const headers = [
  { title: 'Nombre', value: 'name' },
  { title: 'SKU', value: 'sku' },
  { title: 'Categoria', value: 'categoria' },
  { title: 'Proveedor', value: 'proveedor' },
  { title: 'Precio base', value: 'precioBase' },
  { title: 'Precio venta', value: 'precioVenta' },
  { title: 'Estado', value: 'isActive' }
];

const pricingModeItems = [
  { title: '% fijo', value: 'FIJO_PCT' },
  { title: 'Por categoria', value: 'CATEGORIA' },
  { title: 'Manual', value: 'MANUAL' }
];

const proveedorHeaders = [
  { title: 'Nombre', value: 'name' },
  { title: 'Telefono', value: 'telefono' },
  { title: 'CUIT', value: 'cuit' },
  { title: 'Direccion', value: 'direccion' },
  { title: 'Estado', value: 'isActive' }
];

const categoriaHeaders = [
  { title: 'Nombre', value: 'name' },
  { title: '% Ganancia', value: 'margenGananciaPct' },
  { title: 'Estado', value: 'isActive' }
];

const proveedoresTable = ref([]);
const proveedorSearch = ref('');
const proveedorActivoFilter = ref('all');
const proveedorLoadingTable = ref(false);
const proveedorSaving = ref(false);

const proveedorForm = reactive({
  id: '',
  name: '',
  telefono: '',
  cuit: '',
  direccion: '',
  isActive: true
});

const proveedorErrors = reactive({
  name: '',
  telefono: ''
});

const categoriasTable = ref([]);
const categoriaSearch = ref('');
const categoriaActivoFilter = ref('all');
const categoriaLoadingTable = ref(false);
const categoriaSaving = ref(false);

const categoriaForm = reactive({
  id: '',
  name: '',
  margenGananciaPct: '30',
  aplicarAProductos: true,
  isActive: true
});

const categoriaErrors = reactive({
  name: '',
  margenGananciaPct: ''
});

const formatMoney = (value) =>
  new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 0 }).format(value || 0);

const categoriasLookupDisplay = computed(() =>
  (categoriasLookup.value || []).map((item) => ({
    ...item,
    displayTitle: `${item.name || 'Sin nombre'} (${Number(item.margenGananciaPct || 0).toFixed(2)}%)`
  }))
);

const categoriaSeleccionadaLabel = computed(() => {
  if (!form.categoriaId) return 'Sin categoria';
  const categoria = (categoriasLookup.value || []).find((item) => item.id === form.categoriaId);
  if (!categoria) return 'Sin categoria';
  return `${categoria.name || 'Sin nombre'} (${Number(categoria.margenGananciaPct || 0).toFixed(2)}%)`;
});

const precioVentaCalculado = computed(() => {
  const base = Number(form.precioBase);
  if (Number.isNaN(base) || base <= 0) return 0;

  if (form.pricingMode === 'MANUAL') {
    const manual = Number(form.precioVentaManual);
    return Number.isNaN(manual) || manual < 0 ? 0 : manual;
  }

  const margen = form.pricingMode === 'CATEGORIA'
    ? Number(form.margenCategoriaPct || 0)
    : Number(form.margenPct);
  if (Number.isNaN(margen) || margen < 0) return base;
  return base * (1 + margen / 100);
});

const shortId = (value) => {
  if (!value) return 'n/a';
  return value.length > 8 ? value.slice(0, 8) : value;
};

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

const clearErrors = () => {
  Object.keys(errors).forEach((key) => {
    errors[key] = '';
  });
};

const validateField = (field) => {
  if (field === 'name') {
    errors.name = form.name.trim() ? '' : 'El nombre es obligatorio.';
  }
  if (field === 'sku') {
    errors.sku = form.sku.trim() ? '' : 'El SKU es obligatorio.';
  }
  if (field === 'proveedorId') {
    errors.proveedorId = form.proveedorId ? '' : 'El proveedor es obligatorio.';
  }
  if (field === 'precioBase') {
    if (form.precioBase === '') {
      errors.precioBase = '';
    } else if (Number(form.precioBase) < 0 || Number.isNaN(Number(form.precioBase))) {
      errors.precioBase = 'Precio base invalido.';
    } else {
      errors.precioBase = '';
    }
  }
  if (field === 'margenPct') {
    if (form.pricingMode !== 'FIJO_PCT') {
      errors.margenPct = '';
    } else if (form.margenPct === '') {
      errors.margenPct = '';
    } else if (Number(form.margenPct) < 0 || Number.isNaN(Number(form.margenPct))) {
      errors.margenPct = 'Margen invalido.';
    } else {
      errors.margenPct = '';
    }
  }
  if (field === 'precioVentaManual') {
    if (form.pricingMode !== 'MANUAL') {
      errors.precioVentaManual = '';
    } else if (form.precioVentaManual === '') {
      errors.precioVentaManual = 'Precio venta obligatorio.';
    } else if (Number(form.precioVentaManual) < 0 || Number.isNaN(Number(form.precioVentaManual))) {
      errors.precioVentaManual = 'Precio venta invalido.';
    } else {
      errors.precioVentaManual = '';
    }
  }
  if (field === 'stockMinimo') {
    if (stockConfig.stockMinimo === '') {
      errors.stockMinimo = '';
    } else if (Number(stockConfig.stockMinimo) < 0 || Number.isNaN(Number(stockConfig.stockMinimo))) {
      errors.stockMinimo = 'Stock minimo invalido.';
    } else {
      errors.stockMinimo = '';
    }
  }
  if (field === 'stockDeseado') {
    if (stockConfig.stockDeseado === '') {
      errors.stockDeseado = '';
    } else if (Number(stockConfig.stockDeseado) < 0 || Number.isNaN(Number(stockConfig.stockDeseado))) {
      errors.stockDeseado = 'Stock deseado invalido.';
    } else {
      errors.stockDeseado = '';
    }
  }
  if (field === 'stockInicial') {
    if (form.stockInicial === '') {
      errors.stockInicial = '';
    } else if (Number(form.stockInicial) < 0 || Number.isNaN(Number(form.stockInicial))) {
      errors.stockInicial = 'Stock inicial invalido.';
    } else {
      errors.stockInicial = '';
    }
  }
  if (field === 'toleranciaPct') {
    if (stockConfig.toleranciaPct === '') {
      errors.toleranciaPct = '';
    } else if (Number(stockConfig.toleranciaPct) < 0 || Number.isNaN(Number(stockConfig.toleranciaPct))) {
      errors.toleranciaPct = 'Tolerancia invalida.';
    } else {
      errors.toleranciaPct = '';
    }
  }
};

const validateForm = () => {
  validateField('name');
  validateField('sku');
  validateField('proveedorId');
  validateField('precioBase');
  validateField('margenPct');
  validateField('precioVentaManual');
  validateField('stockInicial');
  validateField('stockMinimo');
  validateField('stockDeseado');
  validateField('toleranciaPct');
  return (
    !errors.name &&
    !errors.sku &&
    !errors.proveedorId &&
    !errors.precioBase &&
    !errors.margenPct &&
    !errors.precioVentaManual &&
    !errors.stockInicial &&
    !errors.stockMinimo &&
    !errors.stockDeseado &&
    !errors.toleranciaPct
  );
};

const loadProducts = async () => {
  loading.value = true;
  try {
    const params = new URLSearchParams();
    if (search.value.trim()) params.set('search', search.value.trim());
    if (activoFilter.value !== 'all') params.set('activo', activoFilter.value);

    const { response, data } = await getJson(`/api/v1/productos?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    products.value = (data || [])
      .slice()
      .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'));
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar los productos.');
  } finally {
    loading.value = false;
  }
};

const handleScan = () => {
  const code = scanInput.value.trim();
  if (!code) return;
  search.value = code;
  loadProducts();
  scanInput.value = '';
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
    proveedoresLookup.value = (data || [])
      .slice()
      .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'));
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar proveedores.');
  } finally {
    proveedorLoading.value = false;
  }
};

const applyCategoriaMargin = () => {
  const categoria = categoriasLookup.value.find((item) => item.id === form.categoriaId);
  form.margenCategoriaPct = categoria ? Number(categoria.margenGananciaPct || 0).toString() : '0';
};

const searchCategorias = async (term) => {
  categoriaLoading.value = true;
  try {
    const params = new URLSearchParams();
    if (term && term.trim()) params.set('search', term.trim());
    params.set('activo', 'true');
    const { response, data } = await getJson(`/api/v1/categorias-precio?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    categoriasLookup.value = (data || [])
      .slice()
      .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'));
    applyCategoriaMargin();
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar categorias.');
  } finally {
    categoriaLoading.value = false;
  }
};

const ensureCategoriasLookup = async () => {
  if (categoriasLookup.value.length > 0 || categoriaLoading.value) return;
  await searchCategorias('');
};

const selectProduct = async (event, row) => {
  const product = row?.item?.raw ?? row?.item ?? row;
  if (!product?.id) return;
  try {
    const { response, data } = await getJson(`/api/v1/productos/${product.id}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    form.id = data.id;
    form.name = data.name || '';
    form.sku = data.sku || '';
    form.proveedorId = data.proveedorId || '';
    form.categoriaId = data.categoriaId || '';
    form.isActive = data.isActive ?? true;
    const base = Number(data.precioBase ?? 0);
    const venta = Number(data.precioVenta ?? base);
    const margen = base > 0 ? ((venta / base) - 1) * 100 : 0;
    form.precioBase = base ? base.toString() : '';
    form.pricingMode = data.pricingMode || 'FIJO_PCT';
    form.margenPct = Number.isFinite(margen) ? margen.toFixed(2) : '0';
    form.margenCategoriaPct = data.margenGananciaPct != null
      ? Number(data.margenGananciaPct).toString()
      : form.margenCategoriaPct;
    form.precioVentaManual = venta ? venta.toString() : '';
    form.stockInicial = '';

    const configResp = await getJson(`/api/v1/productos/${product.id}/stock-config`);
    if (configResp.response.ok) {
      stockConfig.stockMinimo = configResp.data.stockMinimo?.toString?.() ?? '0';
      stockConfig.stockDeseado = configResp.data.stockDeseado?.toString?.() ?? '0';
      stockConfig.toleranciaPct = configResp.data.toleranciaPct?.toString?.() ?? '25';
    } else {
      stockConfig.stockMinimo = '0';
      stockConfig.stockDeseado = '0';
      stockConfig.toleranciaPct = '25';
    }

    clearErrors();
    applyCategoriaMargin();
  } catch (err) {
    flash('error', err?.message || 'No se pudo cargar el producto.');
  }
};

const resetForm = () => {
  form.id = '';
  form.name = '';
  form.sku = '';
  form.proveedorId = '';
  form.categoriaId = '';
  form.precioBase = '';
  form.pricingMode = 'FIJO_PCT';
  form.margenPct = '30';
  form.margenCategoriaPct = '0';
  form.precioVentaManual = '';
  form.stockInicial = '';
  form.isActive = true;
  stockConfig.stockMinimo = '0';
  stockConfig.stockDeseado = '0';
  stockConfig.toleranciaPct = '25';
  clearErrors();
};

const saveStockConfig = async (productId) => {
  const payload = {
    stockMinimo: stockConfig.stockMinimo === '' ? 0 : Number(stockConfig.stockMinimo),
    stockDeseado: stockConfig.stockDeseado === '' ? 0 : Number(stockConfig.stockDeseado),
    toleranciaPct: stockConfig.toleranciaPct === '' ? 25 : Number(stockConfig.toleranciaPct)
  };

  const { response, data } = await requestJson(`/api/v1/productos/${productId}/stock-config`, {
    method: 'PATCH',
    body: JSON.stringify(payload)
  });
  if (!response.ok) {
    throw new Error(extractProblemMessage(data));
  }
};

const applyStockInicial = async (productId) => {
  if (form.stockInicial === '') return;
  const qty = Number(form.stockInicial);
  if (Number.isNaN(qty) || qty <= 0) return;

  const payload = {
    tipo: 'AJUSTE',
    motivo: 'Stock inicial',
    items: [
      {
        productoId: productId,
        cantidad: qty,
        esIngreso: true
      }
    ]
  };

  const { response, data } = await postJson('/api/v1/stock/ajustes', payload);
  if (!response.ok) {
    throw new Error(extractProblemMessage(data));
  }
  form.stockInicial = '';
};

const saveProduct = async () => {
  if (saving.value) return;
  if (!validateForm()) return;

  saving.value = true;
  try {
    const precioBase = form.precioBase === '' ? null : Number(form.precioBase);
    const precioVenta = form.pricingMode === 'MANUAL'
      ? (form.precioVentaManual === '' ? null : Number(form.precioVentaManual))
      : (precioBase == null ? null : Number(precioVentaCalculado.value));
    const margenGananciaPct = form.pricingMode === 'FIJO_PCT'
      ? (form.margenPct === '' ? null : Number(form.margenPct))
      : null;

    if (!form.id) {
      const payload = {
        name: form.name.trim(),
        sku: form.sku.trim(),
        proveedorId: form.proveedorId,
        categoriaId: form.categoriaId || null,
        isActive: form.isActive,
        precioBase,
        precioVenta,
        pricingMode: form.pricingMode,
        margenGananciaPct
      };

      const { response, data } = await postJson('/api/v1/productos', payload);
      if (!response.ok) {
        throw new Error(extractProblemMessage(data));
      }
      form.id = data.id;
      await saveStockConfig(form.id);
      await applyStockInicial(form.id);
      flash('success', 'Producto creado');
      await loadProducts();
      return;
    }

    const payload = {
      name: form.name.trim() || null,
      sku: form.sku.trim() || null,
      proveedorId: form.proveedorId,
      categoriaId: form.categoriaId || null,
      isActive: form.isActive,
      precioBase,
      precioVenta,
      pricingMode: form.pricingMode,
      margenGananciaPct
    };

    const { response, data } = await requestJson(`/api/v1/productos/${form.id}`, {
      method: 'PATCH',
      body: JSON.stringify(payload)
    });
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    await saveStockConfig(form.id);
    flash('success', 'Producto actualizado');
    await loadProducts();
  } catch (err) {
    flash('error', err?.message || 'No se pudo guardar el producto.');
  } finally {
    saving.value = false;
  }
};

const saveFromBottom = async () => {
  window.scrollTo({ top: 0, behavior: 'smooth' });
  await saveProduct();
};

const printBarcodes = async () => {
  if (barcodeLoading.value) return;

  const productoIds = products.value
    .map((p) => p.id)
    .filter((id) => typeof id === 'string' && id.length > 0);

  if (!productoIds.length) {
    flash('error', 'No hay productos para imprimir.');
    return;
  }

  barcodeLoading.value = true;
  try {
    const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5072';
    const response = await fetch(`${baseUrl}/api/v1/etiquetas/codigos-barra/pdf`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(auth.token ? { Authorization: `Bearer ${auth.token}` } : {})
      },
      body: JSON.stringify({ productoIds })
    });

    if (!response.ok) {
      let message = 'No se pudo generar el PDF.';
      try {
        const data = await response.json();
        message = extractProblemMessage(data);
      } catch {
        // ignore parse errors and keep generic message
      }

      throw new Error(message);
    }

    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'codigos-barra-productos.pdf';
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
    flash('success', 'PDF generado');
  } catch (err) {
    flash('error', err?.message || 'No se pudo generar el PDF.');
  } finally {
    barcodeLoading.value = false;
  }
};

const toggleActive = (value) => {
  if (form.isActive && !value) {
    pendingActive.value = false;
    dialogDesactivar.value = true;
    return;
  }
  form.isActive = value;
};

const cancelDeactivate = () => {
  dialogDesactivar.value = false;
  form.isActive = true;
};

const confirmDeactivate = () => {
  dialogDesactivar.value = false;
  form.isActive = pendingActive.value;
};

const validateProveedorField = (field) => {
  if (field === 'name') {
    proveedorErrors.name = proveedorForm.name.trim() ? '' : 'El nombre es obligatorio.';
  }
  if (field === 'telefono') {
    proveedorErrors.telefono = proveedorForm.telefono.trim() ? '' : 'El telefono es obligatorio.';
  }
};

const resetProveedorForm = () => {
  proveedorForm.id = '';
  proveedorForm.name = '';
  proveedorForm.telefono = '';
  proveedorForm.cuit = '';
  proveedorForm.direccion = '';
  proveedorForm.isActive = true;
  proveedorErrors.name = '';
  proveedorErrors.telefono = '';
};

const loadProveedores = async () => {
  proveedorLoadingTable.value = true;
  try {
    const params = new URLSearchParams();
    if (proveedorSearch.value.trim()) params.set('search', proveedorSearch.value.trim());
    if (proveedorActivoFilter.value !== 'all') params.set('activo', proveedorActivoFilter.value);
    const { response, data } = await getJson(`/api/v1/proveedores?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    proveedoresTable.value = (data || [])
      .slice()
      .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'));
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar proveedores.');
  } finally {
    proveedorLoadingTable.value = false;
  }
};

const selectProveedor = (event, row) => {
  const proveedor = row?.item?.raw ?? row?.item ?? row;
  if (!proveedor?.id) return;
  proveedorForm.id = proveedor.id;
  proveedorForm.name = proveedor.name || '';
  proveedorForm.telefono = proveedor.telefono || '';
  proveedorForm.cuit = proveedor.cuit || '';
  proveedorForm.direccion = proveedor.direccion || '';
  proveedorForm.isActive = proveedor.isActive ?? true;
  proveedorErrors.name = '';
  proveedorErrors.telefono = '';
};

const saveProveedor = async () => {
  if (proveedorSaving.value) return;
  validateProveedorField('name');
  validateProveedorField('telefono');
  if (proveedorErrors.name || proveedorErrors.telefono) return;

  proveedorSaving.value = true;
  try {
    const payload = {
      name: proveedorForm.name.trim(),
      telefono: proveedorForm.telefono.trim(),
      cuit: proveedorForm.cuit.trim() || null,
      direccion: proveedorForm.direccion.trim() || null,
      isActive: proveedorForm.isActive
    };

    if (!proveedorForm.id) {
      const { response, data } = await postJson('/api/v1/proveedores', payload);
      if (!response.ok) {
        throw new Error(extractProblemMessage(data));
      }
      flash('success', 'Proveedor creado');
      resetProveedorForm();
    } else {
      const { response, data } = await requestJson(`/api/v1/proveedores/${proveedorForm.id}`, {
        method: 'PATCH',
        body: JSON.stringify(payload)
      });
      if (!response.ok) {
        throw new Error(extractProblemMessage(data));
      }
      flash('success', 'Proveedor actualizado');
    }

    await loadProveedores();
    await searchProveedores('');
  } catch (err) {
    flash('error', err?.message || 'No se pudo guardar el proveedor.');
  } finally {
    proveedorSaving.value = false;
  }
};

const validateCategoriaField = (field) => {
  if (field === 'name') {
    categoriaErrors.name = categoriaForm.name.trim() ? '' : 'El nombre es obligatorio.';
  }
  if (field === 'margenGananciaPct') {
    const value = Number(categoriaForm.margenGananciaPct);
    categoriaErrors.margenGananciaPct = Number.isNaN(value) || value < 0
      ? 'Margen invalido.'
      : '';
  }
};

const resetCategoriaForm = () => {
  categoriaForm.id = '';
  categoriaForm.name = '';
  categoriaForm.margenGananciaPct = '30';
  categoriaForm.aplicarAProductos = true;
  categoriaForm.isActive = true;
  categoriaErrors.name = '';
  categoriaErrors.margenGananciaPct = '';
};

const loadCategorias = async () => {
  categoriaLoadingTable.value = true;
  try {
    const params = new URLSearchParams();
    if (categoriaSearch.value.trim()) params.set('search', categoriaSearch.value.trim());
    if (categoriaActivoFilter.value !== 'all') params.set('activo', categoriaActivoFilter.value);

    const { response, data } = await getJson(`/api/v1/categorias-precio?${params.toString()}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    categoriasTable.value = (data || [])
      .slice()
      .sort((a, b) => (a.name || '').localeCompare(b.name || '', 'es'));
    categoriasLookup.value = categoriasTable.value
      .filter((item) => item.isActive)
      .map((item) => ({ ...item }));
    applyCategoriaMargin();
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar categorias.');
  } finally {
    categoriaLoadingTable.value = false;
  }
};

const selectCategoria = (event, row) => {
  const categoria = row?.item?.raw ?? row?.item ?? row;
  if (!categoria?.id) return;
  categoriaForm.id = categoria.id;
  categoriaForm.name = categoria.name || '';
  categoriaForm.margenGananciaPct = Number(categoria.margenGananciaPct || 0).toString();
  categoriaForm.aplicarAProductos = true;
  categoriaForm.isActive = categoria.isActive ?? true;
  categoriaErrors.name = '';
  categoriaErrors.margenGananciaPct = '';
};

const saveCategoria = async () => {
  if (categoriaSaving.value) return;
  validateCategoriaField('name');
  validateCategoriaField('margenGananciaPct');
  if (categoriaErrors.name || categoriaErrors.margenGananciaPct) return;

  categoriaSaving.value = true;
  try {
    const payload = {
      name: categoriaForm.name.trim(),
      margenGananciaPct: Number(categoriaForm.margenGananciaPct),
      isActive: categoriaForm.isActive,
      aplicarAProductos: categoriaForm.aplicarAProductos
    };

    if (!categoriaForm.id) {
      const { response, data } = await postJson('/api/v1/categorias-precio', payload);
      if (!response.ok) throw new Error(extractProblemMessage(data));
      flash('success', 'Categoria creada');
    } else {
      const { response, data } = await requestJson(`/api/v1/categorias-precio/${categoriaForm.id}`, {
        method: 'PATCH',
        body: JSON.stringify(payload)
      });
      if (!response.ok) throw new Error(extractProblemMessage(data));
      flash('success', 'Categoria actualizada');
    }

    await loadCategorias();
    await loadProducts();
    resetCategoriaForm();
  } catch (err) {
    flash('error', err?.message || 'No se pudo guardar la categoria.');
  } finally {
    categoriaSaving.value = false;
  }
};

const syncTabFromRoute = (value) => {
  if (value === 'proveedores') {
    tab.value = 'proveedores';
    return;
  }
  if (value === 'listas-precio') {
    tab.value = 'listas-precio';
    return;
  }
  tab.value = 'productos';
};

watch(
  () => tab.value,
  (value) => {
    const nextQuery = { ...route.query, tab: value };
    router.replace({ path: '/productos', query: nextQuery });
  }
);

watch(
  () => route.query.tab,
  (value) => {
    syncTabFromRoute(value);
  }
);

watch(
  () => form.pricingMode,
  (value) => {
    if (value === 'MANUAL' && !form.precioVentaManual) {
      form.precioVentaManual = precioVentaCalculado.value.toFixed(2);
    }
    if (value === 'CATEGORIA') {
      applyCategoriaMargin();
      ensureCategoriasLookup();
    }
    validateField('margenPct');
    validateField('precioVentaManual');
  }
);

watch(
  () => form.categoriaId,
  () => {
    applyCategoriaMargin();
  }
);

onMounted(() => {
  syncTabFromRoute(route.query.tab);
  loadProducts();
  loadProveedores();
  loadCategorias();
  searchProveedores('');
  searchCategorias('');
});
</script>

<style scoped>
.productos-page {
  animation: fade-in 0.3s ease;
}

</style>

