<template>
  <div class="empresa-page">
    <v-card class="pos-card pa-4">
      <div class="d-flex align-center justify-space-between">
        <div>
          <div class="text-h6">Datos de mi empresa</div>
          <div class="text-caption text-medium-emphasis">Datos del negocio que usa esta aplicacion</div>
        </div>
        <v-btn color="primary" class="text-none" :loading="saving" @click="save">
          Guardar
        </v-btn>
      </div>

      <v-form class="mt-4">
        <v-row dense>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="form.razonSocial"
              label="Razon social"
              variant="outlined"
              density="comfortable"
              :error-messages="errors.razonSocial"
              @blur="validate('razonSocial')"
              required
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field v-model="form.cuit" label="CUIT" variant="outlined" density="comfortable" />
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field v-model="form.telefono" label="Telefono" variant="outlined" density="comfortable" />
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field v-model="form.email" label="Email" variant="outlined" density="comfortable" />
          </v-col>
          <v-col cols="12">
            <v-text-field v-model="form.direccion" label="Direccion" variant="outlined" density="comfortable" />
          </v-col>
          <v-col cols="12">
            <v-text-field v-model="form.web" label="Web" variant="outlined" density="comfortable" />
          </v-col>
          <v-col cols="12">
            <v-textarea
              v-model="form.observaciones"
              label="Otros datos"
              variant="outlined"
              density="comfortable"
              rows="3"
            />
          </v-col>
        </v-row>
      </v-form>
    </v-card>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="top end" timeout="1600">
      <div class="d-flex align-center gap-2">
        <v-icon>{{ snackbar.icon }}</v-icon>
        <span>{{ snackbar.text }}</span>
      </div>
    </v-snackbar>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue';
import { getJson, requestJson } from '../services/apiClient';

const saving = ref(false);
const form = reactive({
  razonSocial: '',
  cuit: '',
  telefono: '',
  direccion: '',
  email: '',
  web: '',
  observaciones: ''
});

const errors = reactive({
  razonSocial: ''
});

const snackbar = ref({
  show: false,
  text: '',
  color: 'success',
  icon: 'mdi-check-circle'
});

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
    if (firstKey && data.errors[firstKey]?.length) return `${firstKey}: ${data.errors[firstKey][0]}`;
  }
  return data.detail || data.title || 'Error inesperado.';
};

const validate = (field) => {
  if (field === 'razonSocial') {
    errors.razonSocial = form.razonSocial.trim() ? '' : 'La razon social es obligatoria.';
  }
};

const load = async () => {
  try {
    const { response, data } = await getJson('/api/v1/empresa/datos');
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    form.razonSocial = data?.razonSocial || '';
    form.cuit = data?.cuit || '';
    form.telefono = data?.telefono || '';
    form.direccion = data?.direccion || '';
    form.email = data?.email || '';
    form.web = data?.web || '';
    form.observaciones = data?.observaciones || '';
  } catch (err) {
    flash('error', err?.message || 'No se pudieron cargar los datos.');
  }
};

const save = async () => {
  if (saving.value) return;
  validate('razonSocial');
  if (errors.razonSocial) return;

  saving.value = true;
  try {
    const payload = {
      razonSocial: form.razonSocial.trim(),
      cuit: form.cuit.trim() || null,
      telefono: form.telefono.trim() || null,
      direccion: form.direccion.trim() || null,
      email: form.email.trim() || null,
      web: form.web.trim() || null,
      observaciones: form.observaciones.trim() || null
    };

    const { response, data } = await requestJson('/api/v1/empresa/datos', {
      method: 'PUT',
      body: JSON.stringify(payload)
    });
    if (!response.ok) {
      throw new Error(extractProblemMessage(data));
    }

    flash('success', 'Datos guardados');
  } catch (err) {
    flash('error', err?.message || 'No se pudieron guardar los datos.');
  } finally {
    saving.value = false;
  }
};

onMounted(() => {
  load();
});
</script>

<style scoped>
.empresa-page {
  animation: fade-in 0.3s ease;
}
</style>
