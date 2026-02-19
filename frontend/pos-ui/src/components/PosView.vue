<template>
  <v-card class="pos-card pa-4">
    <div class="d-flex align-center justify-space-between">
      <div>
        <div class="text-h6">Venta activa</div>
        <div class="text-caption text-medium-emphasis">Estado: BORRADOR</div>
      </div>
      <v-chip color="secondary" variant="tonal" class="status-chip">BORRADOR</v-chip>
    </div>

    <div class="mt-4 d-flex align-center gap-3">
      <v-text-field
        ref="scanInputRef"
        v-model="scanInput"
        class="scan-input"
        :class="scanFlashClass"
        label="Escanear SKU"
        variant="outlined"
        density="comfortable"
        prepend-inner-icon="mdi-barcode-scan"
        @keyup.enter="handleScan"
        hide-details
      />
      <v-btn color="primary" size="large" class="text-none" @click="handleScan">
        Agregar
      </v-btn>
      <v-btn color="secondary" variant="tonal" size="large" class="text-none" @click="nuevaVenta">
        Nueva venta
      </v-btn>
    </div>

    <div class="hotkeys mt-3">
      <span class="hotkey">F2 Nueva venta</span>
      <span class="hotkey">F4 Confirmar</span>
      <span class="hotkey">F7 Anular</span>
      <span class="hotkey">Esc Volver al scan</span>
    </div>

    <div class="mt-4">
      <div class="d-flex align-center gap-2 mb-2">
        <v-text-field
          v-model="tableSearch"
          label="Busqueda rapida"
          variant="outlined"
          density="compact"
          prepend-inner-icon="mdi-magnify"
          hide-details
        />
        <v-chip color="primary" variant="tonal">{{ items.length }} items</v-chip>
      </div>

      <v-data-table-virtual
        :headers="headers"
        :items="items"
        :search="tableSearch"
        item-key="id"
        height="360"
        class="elevation-0"
      >
        <template v-slot:[`item.subtotal`]="{ item }">
          <strong>{{ formatMoney(item.subtotal) }}</strong>
        </template>
      </v-data-table-virtual>
    </div>

    <div class="mt-4 d-flex align-center justify-space-between flex-wrap">
      <div class="text-caption text-medium-emphasis">
        Total items: {{ totalItems }}
      </div>
      <div class="d-flex align-center gap-3">
        <div class="text-right">
          <div class="text-caption text-medium-emphasis">Total neto</div>
          <div class="text-h5">{{ formatMoney(totalNeto) }}</div>
        </div>
        <v-btn
          color="primary"
          size="large"
          class="text-none"
          :loading="isConfirming"
          @click="openConfirmDialog"
        >
          Confirmar venta
        </v-btn>
        <v-btn
          color="error"
          variant="tonal"
          size="large"
          class="text-none"
          @click="dialogAnular = true"
        >
          Anular venta
        </v-btn>
      </div>
    </div>

    <v-dialog v-model="dialogConfirmar" width="480">
      <v-card>
        <v-card-title>Confirmar venta</v-card-title>
        <v-card-text>
          Se confirmara la venta y se descontara stock. Continuar?
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" @click="dialogConfirmar = false">Cancelar</v-btn>
          <v-btn color="primary" :loading="isConfirming" @click="confirmarVenta">Confirmar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogAnular" width="480">
      <v-card>
        <v-card-title>Anular venta</v-card-title>
        <v-card-text>
          Esta accion es irreversible. Queres anular la venta?
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" @click="dialogAnular = false">Cancelar</v-btn>
          <v-btn color="error" @click="anularVenta">Anular</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="top end" timeout="1400">
      <div class="d-flex align-center gap-2">
        <v-icon>{{ snackbar.icon }}</v-icon>
        <span>{{ snackbar.text }}</span>
      </div>
    </v-snackbar>
  </v-card>
</template>

<script setup>
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue';

const scanInputRef = ref(null);
const scanInput = ref('');
const tableSearch = ref('');
const isConfirming = ref(false);
const dialogConfirmar = ref(false);
const dialogAnular = ref(false);
const scanFlash = ref('');

const snackbar = ref({
  show: false,
  text: '',
  color: 'success',
  icon: 'mdi-check-circle'
});

const catalog = ref([
  { id: 'p1', nombre: 'Yerba 1kg', sku: 'YER-1', precio: 1200, stock: 5 },
  { id: 'p2', nombre: 'Azucar 1kg', sku: 'AZU-1', precio: 890, stock: 3 },
  { id: 'p3', nombre: 'Cafe 250g', sku: 'CAF-250', precio: 2100, stock: 2 }
]);

const items = ref([]);

const headers = [
  { title: 'Producto', value: 'nombre' },
  { title: 'SKU', value: 'sku' },
  { title: 'Cant', value: 'cantidad', align: 'end' },
  { title: 'Precio', value: 'precio', align: 'end' },
  { title: 'Subtotal', value: 'subtotal', align: 'end' }
];

const totalNeto = computed(() =>
  items.value.reduce((acc, item) => acc + item.subtotal, 0)
);

const totalItems = computed(() =>
  items.value.reduce((acc, item) => acc + item.cantidad, 0)
);

const scanFlashClass = computed(() => {
  if (scanFlash.value === 'success') return 'scan-flash-success';
  if (scanFlash.value === 'error') return 'scan-flash-error';
  return '';
});

const formatMoney = (value) =>
  new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 0 }).format(value);

const focusScan = () => {
  nextTick(() => {
    const el = scanInputRef.value?.$el?.querySelector('input');
    if (el) el.focus();
  });
};

const flash = (type, text) => {
  snackbar.value = {
    show: true,
    text,
    color: type === 'success' ? 'success' : 'error',
    icon: type === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'
  };
  scanFlash.value = type;
  setTimeout(() => {
    scanFlash.value = '';
  }, 250);
};

const handleScan = () => {
  const code = scanInput.value.trim();
  if (!code) return;

  const product = catalog.value.find(
    (item) => item.sku === code
  );

  if (!product) {
    flash('error', `SKU no encontrado: ${code}`);
    scanInput.value = '';
    focusScan();
    return;
  }

  const existing = items.value.find((item) => item.productoId === product.id);
  if (existing) {
    existing.cantidad += 1;
    existing.subtotal = existing.cantidad * existing.precio;
  } else {
    items.value.push({
      id: `${product.id}-${Date.now()}`,
      productoId: product.id,
      nombre: product.nombre,
      sku: product.sku,
      cantidad: 1,
      precio: product.precio,
      subtotal: product.precio
    });
  }

  flash('success', `Scan OK: ${product.nombre}`);
  scanInput.value = '';
  focusScan();
};

const nuevaVenta = () => {
  items.value = [];
  tableSearch.value = '';
  flash('success', 'Venta nueva');
  focusScan();
};

const openConfirmDialog = () => {
  if (!items.value.length) {
    flash('error', 'No hay items para confirmar');
    return;
  }
  dialogConfirmar.value = true;
};

const confirmarVenta = () => {
  dialogConfirmar.value = false;
  isConfirming.value = true;

  setTimeout(() => {
    const falta = items.value.reduce((acc, item) => {
      const stock = catalog.value.find((c) => c.id === item.productoId)?.stock ?? 0;
      const diff = item.cantidad - stock;
      return diff > 0 ? acc + diff : acc;
    }, 0);

    if (falta > 0) {
      flash('error', `Stock insuficiente: faltan ${falta}`);
      isConfirming.value = false;
      focusScan();
      return;
    }

    flash('success', 'Venta confirmada');
    isConfirming.value = false;
    focusScan();
  }, 600);
};

const anularVenta = () => {
  dialogAnular.value = false;
  items.value = [];
  flash('error', 'Venta anulada');
  focusScan();
};

const onKeydown = (event) => {
  const activeTag = document.activeElement?.tagName || '';
  const isInput = activeTag === 'INPUT' || activeTag === 'TEXTAREA';
  const isShortcutKey = event.ctrlKey || event.metaKey || event.altKey;
  if (!isInput && !isShortcutKey && event.key.length === 1) {
    focusScan();
  }

  if (event.key === 'F2') {
    event.preventDefault();
    nuevaVenta();
  }
  if (event.key === 'F4') {
    event.preventDefault();
    openConfirmDialog();
  }
  if (event.key === 'F7') {
    event.preventDefault();
    dialogAnular.value = true;
  }
  if (event.key === 'Escape') {
    event.preventDefault();
    focusScan();
  }
};

watch(dialogConfirmar, (open) => {
  if (!open) {
    focusScan();
  }
});

watch(dialogAnular, (open) => {
  if (!open) {
    focusScan();
  }
});

onMounted(() => {
  focusScan();
  window.addEventListener('keydown', onKeydown);
});

onBeforeUnmount(() => {
  window.removeEventListener('keydown', onKeydown);
});
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}

.gap-3 {
  gap: 12px;
}
</style>

