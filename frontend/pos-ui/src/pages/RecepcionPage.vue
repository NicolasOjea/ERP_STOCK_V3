<template>
  <div class="recepcion-page">
    <v-card class="pos-card pa-4">
      <div class="d-flex align-center gap-3">
        <div>
          <div class="text-h6">Recepcion</div>
          <div class="text-caption text-medium-emphasis">Id {{ shortId(recepcion?.id || route.params.id) }}</div>
        </div>
        <v-spacer />
        <v-btn variant="tonal" color="primary" class="text-none" @click="goBack">
          Volver
        </v-btn>
      </div>

      <div v-if="!recepcion" class="text-caption text-medium-emphasis mt-3">
        No hay datos locales. Volve a pre-recepcion para ver el detalle.
      </div>

      <div v-else class="mt-3">
        <div class="text-caption text-medium-emphasis">Creada: {{ formatDate(recepcion.createdAt) }}</div>
        <v-data-table
          class="mt-3"
          :headers="headers"
          :items="recepcion.items || []"
          density="compact"
          item-key="id"
        />
      </div>
    </v-card>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

const recepcion = ref(null);

const headers = [
  { title: 'Producto', value: 'producto' },
  { title: 'SKU', value: 'sku' },
  { title: 'SKU', value: 'codigo' },
  { title: 'Cantidad', value: 'cantidad', align: 'end' }
];

const shortId = (value) => {
  if (!value) return 'n/a';
  return value.length > 8 ? value.slice(0, 8) : value;
};

const formatDate = (value) => {
  if (!value) return '-';
  try {
    return new Date(value).toLocaleString('es-AR');
  } catch {
    return value;
  }
};

const goBack = () => {
  router.back();
};

onMounted(() => {
  const raw = localStorage.getItem('pos-recepcion-last');
  if (!raw) return;
  try {
    const data = JSON.parse(raw);
    if (data?.id === route.params.id) {
      recepcion.value = data;
    }
  } catch {
    recepcion.value = null;
  }
});
</script>

<style scoped>
.recepcion-page {
  animation: fade-in 0.3s ease;
}
</style>
