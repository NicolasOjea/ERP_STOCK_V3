<template>
  <v-card class="pos-card pa-4">
    <div class="d-flex align-center justify-space-between">
      <div>
        <div class="text-h6">Reportes rapidos</div>
        <div class="text-caption text-medium-emphasis">Resumen diario</div>
      </div>
      <v-chip color="primary" variant="tonal">Hoy</v-chip>
    </div>

    <v-row class="mt-3" dense>
      <v-col cols="6">
        <v-card variant="tonal" color="primary" class="pa-3">
          <div class="text-caption">Ventas</div>
          <div class="text-h6">$ 124.300</div>
        </v-card>
      </v-col>
      <v-col cols="6">
        <v-card variant="tonal" color="secondary" class="pa-3">
          <div class="text-caption">Ticket promedio</div>
          <div class="text-h6">$ 6.200</div>
        </v-card>
      </v-col>
    </v-row>

    <div class="mt-4">
      <div class="d-flex align-center gap-2 mb-2">
        <div class="text-subtitle-2">Top productos</div>
        <v-spacer />
        <v-text-field
          v-model="search"
          label="Buscar"
          density="compact"
          variant="outlined"
          hide-details
          style="max-width: 160px"
        />
      </div>
      <v-data-table
        :headers="headers"
        :items="items"
        :search="search"
        density="compact"
        class="elevation-0"
        item-key="sku"
      />
    </div>

    <div class="mt-3 d-flex gap-2">
      <v-btn color="primary" variant="tonal" class="text-none" :loading="isClosing" @click="confirmCerrarCaja">
        Cerrar caja
      </v-btn>
      <v-btn color="secondary" variant="tonal" class="text-none" :loading="isReceiving" @click="confirmRecepcion">
        Confirmar recepcion
      </v-btn>
    </div>

    <v-dialog v-model="dialogCerrarCaja" width="480">
      <v-card>
        <v-card-title>Cerrar caja</v-card-title>
        <v-card-text>
          Se va a cerrar la sesion y generar arqueo. Continuar?
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" @click="dialogCerrarCaja = false">Cancelar</v-btn>
          <v-btn color="primary" :loading="isClosing" @click="cerrarCaja">Cerrar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogRecepcion" width="480">
      <v-card>
        <v-card-title>Confirmar recepcion</v-card-title>
        <v-card-text>
          Esta accion impacta stock. Confirmar recepcion ahora?
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn variant="text" @click="dialogRecepcion = false">Cancelar</v-btn>
          <v-btn color="secondary" :loading="isReceiving" @click="confirmarRecepcion">Confirmar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script setup>
import { ref } from 'vue';

const search = ref('');
const dialogCerrarCaja = ref(false);
const dialogRecepcion = ref(false);
const isClosing = ref(false);
const isReceiving = ref(false);

const headers = [
  { title: 'Producto', value: 'nombre' },
  { title: 'SKU', value: 'sku' },
  { title: 'Total', value: 'total', align: 'end' }
];

const items = [
  { nombre: 'Yerba 1kg', sku: 'YER-1', total: '$ 32.400' },
  { nombre: 'Azucar 1kg', sku: 'AZU-1', total: '$ 18.600' },
  { nombre: 'Cafe 250g', sku: 'CAF-250', total: '$ 12.300' }
];

const confirmCerrarCaja = () => {
  dialogCerrarCaja.value = true;
};

const confirmRecepcion = () => {
  dialogRecepcion.value = true;
};

const cerrarCaja = () => {
  isClosing.value = true;
  setTimeout(() => {
    isClosing.value = false;
    dialogCerrarCaja.value = false;
  }, 700);
};

const confirmarRecepcion = () => {
  isReceiving.value = true;
  setTimeout(() => {
    isReceiving.value = false;
    dialogRecepcion.value = false;
  }, 700);
};
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>
