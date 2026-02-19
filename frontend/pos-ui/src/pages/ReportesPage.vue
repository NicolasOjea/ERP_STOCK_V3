<template>
  <div class="reportes-page">
    <v-card v-if="!canView" class="pos-card pa-4">
      <div class="text-h6">Reportes</div>
      <div class="text-caption text-medium-emphasis">No tenes permisos para ver reportes.</div>
    </v-card>

    <template v-else>
      <v-card class="pos-card pa-4 mb-4">
        <div class="d-flex flex-wrap align-center gap-3">
          <div>
            <div class="text-h6">Dashboard de reportes</div>
            <div class="text-caption text-medium-emphasis">Rangos y KPIs</div>
          </div>
          <v-spacer />
          <v-text-field
            v-model="filters.desde"
            label="Desde"
            type="date"
            variant="outlined"
            density="comfortable"
            hide-details
            style="max-width: 180px"
          />
          <v-text-field
            v-model="filters.hasta"
            label="Hasta"
            type="date"
            variant="outlined"
            density="comfortable"
            hide-details
            style="max-width: 180px"
          />
          <v-btn
            color="primary"
            class="text-none"
            :loading="loading"
            @click="loadReports"
          >
            Actualizar
          </v-btn>
        </div>
      </v-card>

      <v-row dense class="mb-4">
        <v-col cols="12" md="3">
          <v-card class="pos-card pa-4">
            <div class="text-caption text-medium-emphasis">Ventas</div>
            <div v-if="loading" class="mt-2">
              <v-skeleton-loader type="text" />
            </div>
            <div v-else class="text-h5">{{ formatMoney(resumen.totalIngresos) }}</div>
          </v-card>
        </v-col>
        <v-col cols="12" md="3">
          <v-card class="pos-card pa-4">
            <div class="text-caption text-medium-emphasis">Egresos</div>
            <div v-if="loading" class="mt-2">
              <v-skeleton-loader type="text" />
            </div>
            <div v-else class="text-h5">{{ formatMoney(resumen.totalEgresos) }}</div>
          </v-card>
        </v-col>
        <v-col cols="12" md="3">
          <v-card class="pos-card pa-4">
            <div class="text-caption text-medium-emphasis">Total facturado</div>
            <div v-if="loading" class="mt-2">
              <v-skeleton-loader type="text" />
            </div>
            <div v-else class="text-h5">{{ formatMoney(resumen.totalFacturado) }}</div>
          </v-card>
        </v-col>
        <v-col cols="12" md="3">
          <v-card class="pos-card pa-4">
            <div class="text-caption text-medium-emphasis">Total cotizado</div>
            <div v-if="loading" class="mt-2">
              <v-skeleton-loader type="text" />
            </div>
            <div v-else class="text-h5">{{ formatMoney(resumen.totalNoFacturado) }}</div>
          </v-card>
        </v-col>
      </v-row>

      <v-row dense class="mb-4">
        <v-col cols="12" md="6">
          <v-card class="pos-card pa-4">
            <div class="d-flex align-center justify-space-between">
              <div>
                <div class="text-h6">Ventas por dia</div>
                <div class="text-caption text-medium-emphasis">Linea</div>
              </div>
            </div>
            <div v-if="loading" class="mt-3">
              <v-skeleton-loader type="image" height="180" />
            </div>
            <div ref="ventasWrapperRef" v-else class="chart-wrapper mt-3">
              <svg viewBox="0 0 100 40" preserveAspectRatio="none" class="chart-svg">
                <polyline
                  :points="ventasLine"
                  fill="none"
                  stroke="#0f766e"
                  stroke-width="1.2"
                />
                <circle
                  v-for="(point, idx) in ventasPoints"
                  :key="`dot-${idx}`"
                  :cx="point.x"
                  :cy="point.y"
                  r="1.8"
                  fill="#0ea5a4"
                />
                <rect
                  v-for="(area, idx) in ventasHitAreas"
                  :key="`ventas-hit-${idx}`"
                  :x="area.x"
                  y="0"
                  :width="area.width"
                  height="35"
                  fill="transparent"
                  @mouseenter="showVentasTooltip($event, idx)"
                  @mousemove="moveVentasTooltip($event)"
                  @mouseleave="hideVentasTooltip"
                />
              </svg>
              <div
                v-if="ventasTooltipState.show"
                class="chart-tooltip"
                :style="{ left: `${ventasTooltipState.x}px`, top: `${ventasTooltipState.y}px` }"
              >
                {{ ventasTooltipState.text }}
              </div>
              <div class="chart-labels">
                <span v-for="label in ventasLabels" :key="label">{{ label }}</span>
              </div>
            </div>
          </v-card>
        </v-col>
        <v-col cols="12" md="6">
          <v-card class="pos-card pa-4">
            <div class="d-flex align-center justify-space-between">
              <div>
                <div class="text-h6">Medios de pago</div>
                <div class="text-caption text-medium-emphasis">Barras</div>
              </div>
            </div>
            <div v-if="loading" class="mt-3">
              <v-skeleton-loader type="image" height="180" />
            </div>
            <div ref="mediosWrapperRef" v-else class="chart-wrapper mt-3">
              <svg viewBox="0 0 100 40" preserveAspectRatio="none" class="chart-svg">
                <rect
                  v-for="(hit, idx) in mediosHitAreas"
                  :key="`medios-hit-${idx}`"
                  :x="hit.x"
                  y="0"
                  :width="hit.width"
                  height="35"
                  fill="transparent"
                  @mouseenter="showMediosTooltip($event, idx)"
                  @mousemove="moveMediosTooltip($event)"
                  @mouseleave="hideMediosTooltip"
                />
                <rect
                  v-for="(bar, idx) in mediosBars"
                  :key="idx"
                  :x="bar.x"
                  :y="bar.y"
                  :width="bar.width"
                  :height="bar.height"
                  fill="#ea580c"
                  rx="1"
                />
              </svg>
              <div
                v-if="mediosTooltipState.show"
                class="chart-tooltip"
                :style="{ left: `${mediosTooltipState.x}px`, top: `${mediosTooltipState.y}px` }"
              >
                {{ mediosTooltipState.text }}
              </div>
              <div class="chart-labels">
                <span v-for="label in mediosLabels" :key="label">{{ label }}</span>
              </div>
            </div>
          </v-card>
        </v-col>
      </v-row>

      <v-row dense>
        <v-col cols="12" md="6">
          <v-card class="pos-card pa-4">
            <div class="d-flex align-center justify-space-between">
              <div>
                <div class="text-h6">Top productos</div>
                <div class="text-caption text-medium-emphasis">Top 10 mas vendidos</div>
              </div>
              <v-btn variant="tonal" color="primary" class="text-none" @click="exportCsv('top-productos')">
                Exportar CSV
              </v-btn>
            </div>
            <div v-if="loading" class="mt-3">
              <v-skeleton-loader type="table" />
            </div>
            <v-data-table
              v-else
              class="mt-3"
              :headers="topHeaders"
              :items="topProductos"
              density="compact"
              item-key="productoId"
            />
          </v-card>
        </v-col>

        <v-col cols="12" md="6">
          <v-card class="pos-card pa-4">
            <div class="d-flex align-center justify-space-between">
              <div>
                <div class="text-h6">Inmovilizado</div>
                <div class="text-caption text-medium-emphasis">Sin ventas en el rango</div>
              </div>
              <v-btn variant="tonal" color="primary" class="text-none" @click="exportCsv('inmovilizado')">
                Exportar CSV
              </v-btn>
            </div>
            <div v-if="loading" class="mt-3">
              <v-skeleton-loader type="table" />
            </div>
            <v-data-table
              v-else
              class="mt-3"
              :headers="inmovilizadoHeaders"
              :items="inmovilizado"
              density="compact"
              item-key="productoId"
            />
          </v-card>
        </v-col>
      </v-row>
    </template>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="top end" timeout="1800">
      <div class="d-flex align-center gap-2">
        <v-icon>{{ snackbar.icon }}</v-icon>
        <span>{{ snackbar.text }}</span>
      </div>
    </v-snackbar>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { useAuthStore } from '../stores/auth';
import { getJson } from '../services/apiClient';

const auth = useAuthStore();
const canView = computed(() => auth.hasPermission('PERM_REPORTES_VER'));

const today = new Date();
const sevenDaysAgo = new Date();
sevenDaysAgo.setDate(today.getDate() - 7);

const filters = reactive({
  desde: sevenDaysAgo.toISOString().slice(0, 10),
  hasta: today.toISOString().slice(0, 10)
});

const loading = ref(false);
const ventasChart = ref(null);
const mediosChart = ref(null);
const resumen = ref({
  totalIngresos: 0,
  totalEgresos: 0,
  totalFacturado: 0,
  totalNoFacturado: 0
});
const topProductos = ref([]);
const inmovilizado = ref([]);

const snackbar = ref({
  show: false,
  text: '',
  color: 'success',
  icon: 'mdi-check-circle'
});

const topHeaders = [
  { title: 'Producto', value: 'nombre' },
  { title: 'SKU', value: 'sku' },
  { title: 'Cantidad', value: 'cantidad', align: 'end' },
  { title: 'Total', value: 'total', align: 'end' }
];

const inmovilizadoHeaders = [
  { title: 'Producto', value: 'nombre' },
  { title: 'SKU', value: 'sku' },
  { title: 'Stock', value: 'stockActual', align: 'end' },
  { title: 'Ultimo mov', value: 'ultimoMovimiento' },
  { title: 'Dias', value: 'diasSinMovimiento', align: 'end' }
];

const formatMoney = (value) =>
  new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 0 }).format(value || 0);

const getVentasTooltip = (idx) => {
  const label = ventasLabels.value[idx] || '-';
  const value = ventasSeries.value[idx] || 0;
  return `${label}: ${formatMoney(value)}`;
};

const getMediosTooltip = (idx) => {
  const label = mediosLabels.value[idx] || '-';
  const value = mediosSeries.value[idx] || 0;
  return `${label}: ${formatMoney(value)}`;
};

const ventasTooltipState = ref({ show: false, text: '', x: 0, y: 0 });
const mediosTooltipState = ref({ show: false, text: '', x: 0, y: 0 });
const ventasWrapperRef = ref(null);
const mediosWrapperRef = ref(null);

const tooltipPosition = (event, wrapperRef) => {
  const rect = wrapperRef?.value?.getBoundingClientRect?.();
  if (!rect) return { x: 12, y: 12 };
  const tooltipWidth = 220;
  const x = Math.max(8, Math.min(event.clientX - rect.left + 12, rect.width - tooltipWidth));
  const y = Math.max(8, Math.min(event.clientY - rect.top - 10, rect.height - 28));
  return { x, y };
};

const showVentasTooltip = (event, idx) => {
  const pos = tooltipPosition(event, ventasWrapperRef);
  ventasTooltipState.value = {
    show: true,
    text: getVentasTooltip(idx),
    x: pos.x,
    y: pos.y
  };
};

const moveVentasTooltip = (event) => {
  if (!ventasTooltipState.value.show) return;
  const pos = tooltipPosition(event, ventasWrapperRef);
  ventasTooltipState.value = {
    ...ventasTooltipState.value,
    x: pos.x,
    y: pos.y
  };
};

const hideVentasTooltip = () => {
  ventasTooltipState.value.show = false;
};

const showMediosTooltip = (event, idx) => {
  const pos = tooltipPosition(event, mediosWrapperRef);
  mediosTooltipState.value = {
    show: true,
    text: getMediosTooltip(idx),
    x: pos.x,
    y: pos.y
  };
};

const moveMediosTooltip = (event) => {
  if (!mediosTooltipState.value.show) return;
  const pos = tooltipPosition(event, mediosWrapperRef);
  mediosTooltipState.value = {
    ...mediosTooltipState.value,
    x: pos.x,
    y: pos.y
  };
};

const hideMediosTooltip = () => {
  mediosTooltipState.value.show = false;
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

const ventasLabels = computed(() => ventasChart.value?.labels || []);
const ventasSeries = computed(() => ventasChart.value?.series?.[0]?.data || []);
const mediosLabels = computed(() => mediosChart.value?.labels || []);
const mediosSeries = computed(() => mediosChart.value?.series?.[0]?.data || []);

const buildLinePoints = (values) => {
  if (!values.length) return [];
  const min = Math.min(...values);
  const max = Math.max(...values);
  const range = max - min || 1;
  return values.map((value, index) => {
    const x = (index / Math.max(values.length - 1, 1)) * 100;
    const y = 35 - ((value - min) / range) * 25;
    return { x, y };
  });
};

const ventasPoints = computed(() => buildLinePoints(ventasSeries.value));
const ventasLine = computed(() => ventasPoints.value.map((point) => `${point.x},${point.y}`).join(' '));
const ventasHitAreas = computed(() => {
  const points = ventasPoints.value;
  if (!points.length) return [];
  const step = 100 / points.length;
  return points.map((p) => ({
    x: Math.max(0, p.x - step / 2),
    width: step
  }));
});

const mediosBars = computed(() => {
  const values = mediosSeries.value;
  if (!values.length) return [];
  const max = Math.max(...values) || 1;
  const barWidth = 100 / values.length - 4;
  return values.map((value, index) => {
    const height = (value / max) * 28;
    return {
      x: index * (barWidth + 4) + 2,
      y: 35 - height,
      width: barWidth,
      height
    };
  });
});

const mediosHitAreas = computed(() => {
  const len = mediosSeries.value.length;
  if (!len) return [];
  const step = 100 / len;
  return Array.from({ length: len }, (_, i) => ({
    x: i * step,
    width: step
  }));
});

const buildQuery = () => {
  const params = new URLSearchParams();
  if (filters.desde) {
    const desdeUtc = new Date(`${filters.desde}T00:00:00.000Z`).toISOString();
    params.set('desde', desdeUtc);
  }
  if (filters.hasta) {
    const hastaUtc = new Date(`${filters.hasta}T23:59:59.999Z`).toISOString();
    params.set('hasta', hastaUtc);
  }
  return params.toString();
};

const loadReports = async () => {
  if (!canView.value) return;
  loading.value = true;
  try {
    const query = buildQuery();

    const [resumenResp, ventasResp, mediosResp, topResp, inmResp] = await Promise.all([
      getJson(`/api/v1/reportes/resumen-ventas?${query}`),
      getJson(`/api/v1/reportes/ventas-por-dia?${query}`),
      getJson(`/api/v1/reportes/medios-pago?${query}`),
      getJson(`/api/v1/reportes/top-productos?${query}&top=10`),
      getJson(`/api/v1/reportes/stock-inmovilizado?${query}`)
    ]);

    if (!resumenResp.response.ok) throw new Error(extractProblemMessage(resumenResp.data));
    if (!ventasResp.response.ok) throw new Error(extractProblemMessage(ventasResp.data));
    if (!mediosResp.response.ok) throw new Error(extractProblemMessage(mediosResp.data));
    if (!topResp.response.ok) throw new Error(extractProblemMessage(topResp.data));
    if (!inmResp.response.ok) throw new Error(extractProblemMessage(inmResp.data));

    resumen.value = resumenResp.data || resumen.value;
    ventasChart.value = ventasResp.data;
    mediosChart.value = mediosResp.data;
    topProductos.value = topResp.data?.rows || [];
    inmovilizado.value = (inmResp.data?.rows || []).map((row) => ({
      ...row,
      ultimoMovimiento: row.ultimoMovimiento ? new Date(row.ultimoMovimiento).toLocaleDateString('es-AR') : '-'
    }));
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar los reportes.');
  } finally {
    loading.value = false;
  }
};

const toCsv = (rows, headers) => {
  const headerLine = headers.map((header) => `"${header.title}"`).join(',');
  const lines = rows.map((row) =>
    headers
      .map((header) => {
        const value = row[header.value] ?? '';
        return `"${String(value).replace(/"/g, '""')}"`;
      })
      .join(',')
  );

  return [headerLine, ...lines].join('\n');
};

const downloadFile = (content, filename) => {
  const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  link.click();
  URL.revokeObjectURL(url);
};

const exportCsv = (type) => {
  if (type === 'top-productos') {
    downloadFile(toCsv(topProductos.value, topHeaders), 'top-productos.csv');
  }
  if (type === 'inmovilizado') {
    downloadFile(toCsv(inmovilizado.value, inmovilizadoHeaders), 'stock-inmovilizado.csv');
  }
};

onMounted(() => {
  loadReports();
});
</script>

<style scoped>
.reportes-page {
  animation: fade-in 0.3s ease;
}

.chart-wrapper {
  border: 1px solid rgba(15, 23, 42, 0.08);
  border-radius: 12px;
  padding: 12px;
  background: #fff;
  position: relative;
}

.chart-svg {
  width: 100%;
  height: 180px;
}

.chart-labels {
  display: flex;
  justify-content: space-between;
  font-size: 0.75rem;
  color: #475569;
  margin-top: 6px;
}

.chart-tooltip {
  position: absolute;
  z-index: 2;
  background: rgba(15, 23, 42, 0.92);
  color: #fff;
  font-size: 0.75rem;
  padding: 6px 8px;
  border-radius: 6px;
  pointer-events: none;
  white-space: nowrap;
}
</style>
