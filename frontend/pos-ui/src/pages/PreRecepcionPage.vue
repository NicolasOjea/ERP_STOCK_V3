<template>
  <div class="pre-recepcion-page">
    <v-card class="pos-card pa-4 mb-4">
      <div class="d-flex flex-wrap align-center gap-3">
        <div>
          <div class="text-h6">Pre-recepcion</div>
          <div class="text-caption text-medium-emphasis">Documento {{ shortId(preRecepcion?.documentoCompraId) }}</div>
        </div>
        <v-spacer />
        <v-btn
          color="primary"
          class="text-none"
          :disabled="missingCount > 0 || confirmLoading"
          :loading="confirmLoading"
          @click="dialogConfirm = true"
        >
          Confirmar recepcion
        </v-btn>
      </div>
    </v-card>

    <v-alert
      v-if="missingCount > 0"
      type="warning"
      variant="tonal"
      density="compact"
      class="mb-3"
    >
      Hay {{ missingCount }} items sin producto asignado. No se puede confirmar.
    </v-alert>

    <v-card class="pos-card pa-4">
      <v-data-table
        :headers="headers"
        :items="items"
        item-key="id"
        density="compact"
        height="520"
      >
        <template v-slot:[`item.producto`]="{ item }">
          <div v-if="getRow(item).estado === 'OK'">
            <div>{{ getRow(item).producto }}</div>
            <div class="text-caption text-medium-emphasis">{{ getRow(item).productoSku }}</div>
          </div>
          <v-autocomplete
            v-else
            v-model="productSelections[getRow(item).id]"
            :items="productOptions[getRow(item).id] || []"
            item-title="title"
            item-value="value"
            label="Buscar producto"
            density="compact"
            variant="outlined"
            :loading="searching[getRow(item).id]"
            @update:search="(query) => searchProducts(getRow(item).id, query)"
            @update:model-value="(value) => selectProduct(getRow(item), value)"
          />
        </template>
        <template v-slot:[`item.cantidad`]="{ item }">
          <v-text-field
            v-model.number="cantidadEdits[getRow(item).id]"
            type="number"
            min="0"
            step="0.01"
            density="compact"
            variant="outlined"
            hide-details
            style="max-width: 120px"
            :disabled="saving[getRow(item).id]"
            @blur="updateCantidad(getRow(item))"
            @keyup.enter="updateCantidad(getRow(item))"
          />
        </template>
        <template v-slot:[`item.estado`]="{ item }">
          <v-chip
            size="small"
            :color="estadoColor(getRow(item).estado)"
            variant="tonal"
          >
            {{ getRow(item).estado }}
          </v-chip>
        </template>
      </v-data-table>
    </v-card>

    <v-dialog v-model="dialogConfirm" width="480">
      <v-card>
        <v-card-title>Confirmar recepcion</v-card-title>
        <v-card-text>
          Esta accion impacta stock. Continuar?
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" @click="dialogConfirm = false">Cancelar</v-btn>
          <v-btn color="primary" :loading="confirmLoading" :disabled="missingCount > 0" @click="confirmar">
            Confirmar
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="top end" timeout="2500">
      <div class="d-flex align-center gap-2">
        <v-icon>{{ snackbar.icon }}</v-icon>
        <span>{{ snackbar.text }}</span>
        <v-btn
          v-if="snackbar.action"
          variant="text"
          color="white"
          class="text-none"
          @click="snackbar.action()"
        >
          Ver recepcion
        </v-btn>
      </div>
    </v-snackbar>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getJson, postJson, requestJson } from '../services/apiClient';

const route = useRoute();
const router = useRouter();

const preRecepcion = ref(null);
const items = ref([]);
const loading = ref(false);
const confirmLoading = ref(false);
const dialogConfirm = ref(false);

const productOptions = reactive({});
const productSelections = reactive({});
const searching = reactive({});
const searchTimers = reactive({});
const saving = reactive({});
const cantidadEdits = reactive({});

const snackbar = ref({
  show: false,
  text: '',
  color: 'success',
  icon: 'mdi-check-circle',
  action: null
});

const headers = [
  { title: 'SKU', value: 'codigo' },
  { title: 'Descripcion', value: 'descripcion' },
  { title: 'Producto', value: 'producto' },
  { title: 'Cantidad', value: 'cantidad', align: 'end' },
  { title: 'Estado', value: 'estado', align: 'center' }
];

const shortId = (value) => {
  if (!value) return 'n/a';
  return value.length > 8 ? value.slice(0, 8) : value;
};

const getRow = (row) => row?.raw ?? row;

const estadoColor = (estado) => {
  if (estado === 'OK') return 'success';
  if (estado === 'DUDA') return 'warning';
  return 'error';
};

const flash = (type, text, action = null) => {
  snackbar.value = {
    show: true,
    text,
    color: type === 'success' ? 'success' : 'error',
    icon: type === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle',
    action
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

const missingCount = computed(() =>
  items.value.filter((item) => item.estado !== 'OK').length
);

const loadPreRecepcion = async () => {
  loading.value = true;
  try {
    const { response, data } = await getJson(`/api/v1/pre-recepciones/${route.params.id}`);
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }
    preRecepcion.value = data;
    items.value = data.items || [];
    items.value.forEach((item) => {
      productSelections[item.id] = item.productoId || null;
      cantidadEdits[item.id] = item.cantidad;
    });
  } catch (err) {
    flash('error', err?.message || 'No se pudo cargar la pre-recepcion.');
  } finally {
    loading.value = false;
  }
};

const searchProducts = (itemId, query) => {
  if (!query || query.length < 2) {
    productOptions[itemId] = [];
    return;
  }

  if (searchTimers[itemId]) {
    clearTimeout(searchTimers[itemId]);
  }

  searchTimers[itemId] = setTimeout(async () => {
    searching[itemId] = true;
    try {
      const { response, data } = await getJson(`/api/v1/productos?search=${encodeURIComponent(query)}`);
      if (!response.ok) {
        throw new Error(extractProblemMessage(data));
      }
      productOptions[itemId] = (data || []).map((product) => ({
        title: `${product.name} (${product.sku})`,
        value: product.id,
        name: product.name,
        sku: product.sku
      }));
    } catch (err) {
      flash('error', err?.message || 'No se pudo buscar productos.');
    } finally {
      searching[itemId] = false;
    }
  }, 300);
};

const applyUpdateResponse = (data) => {
  preRecepcion.value = data;
  items.value = data.items || [];
  items.value.forEach((item) => {
    productSelections[item.id] = item.productoId || null;
    cantidadEdits[item.id] = item.cantidad;
  });
};

const updateItem = async (itemId, productoId, cantidad) => {
  saving[itemId] = true;
  try {
    const payload = {
      items: [
        {
          itemId,
          productoId: productoId ?? null,
          cantidad: cantidad ?? null
        }
      ]
    };

    const { response, data } = await requestJson(`/api/v1/pre-recepciones/${route.params.id}`, {
      method: 'PATCH',
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    applyUpdateResponse(data);
  } catch (err) {
    flash('error', err?.message || 'No se pudo actualizar el item.');
  } finally {
    saving[itemId] = false;
  }
};

const selectProduct = (item, value) => {
  if (!item) return;
  const productoId = value || null;
  const cantidad = cantidadEdits[item.id];
  updateItem(item.id, productoId, cantidad);
};

const updateCantidad = (item) => {
  if (!item) return;
  const cantidad = Number(cantidadEdits[item.id]);
  if (Number.isNaN(cantidad) || cantidad <= 0) {
    cantidadEdits[item.id] = item.cantidad;
    flash('error', 'Cantidad invalida');
    return;
  }
  if (cantidad === item.cantidad) return;
  updateItem(item.id, item.productoId, cantidad);
};

const confirmar = async () => {
  if (confirmLoading.value || missingCount.value > 0) return;
  confirmLoading.value = true;
  try {
    const { response, data } = await postJson(`/api/v1/pre-recepciones/${route.params.id}/confirmar`, {});
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    localStorage.setItem('pos-recepcion-last', JSON.stringify(data));
    dialogConfirm.value = false;
    flash('success', 'Recepcion confirmada', () => {
      router.push(`/recepciones/${data.id}`);
    });
  } catch (err) {
    flash('error', err?.message || 'No se pudo confirmar la recepcion.');
  } finally {
    confirmLoading.value = false;
  }
};

onMounted(() => {
  loadPreRecepcion();
});
</script>

<style scoped>
.pre-recepcion-page {
  animation: fade-in 0.3s ease;
}
</style>

