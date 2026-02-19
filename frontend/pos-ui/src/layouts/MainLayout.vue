<template>
  <v-layout>
    <v-navigation-drawer
      v-model="drawer"
      :permanent="mdAndUp"
      width="260"
      class="pos-drawer"
    >
      <div class="d-flex flex-column h-100">
        <div class="pa-4">
          <div class="d-flex align-center gap-2">
            <v-icon size="28" color="primary">mdi-qrcode-scan</v-icon>
            <div>
              <div class="text-subtitle-1">POS</div>
              <div class="text-caption text-medium-emphasis">Modo rapido</div>
            </div>
          </div>
        </div>
        <v-divider />
        <v-list density="comfortable" nav>
          <v-list-item
            v-for="item in visibleItems"
            :key="item.to"
            :to="item.to"
            :prepend-icon="item.icon"
            :title="item.title"
            rounded
          />
        </v-list>
        <v-spacer />
        <v-divider />
        <div class="pa-2">
          <v-list density="comfortable" nav>
            <v-list-item
              v-if="auth.hasPermission('PERM_PRODUCTO_VER')"
              to="/empresa"
              prepend-icon="mdi-domain"
              title="Datos de mi empresa"
              rounded
            />
          </v-list>
        </div>
        <v-divider />
        <div class="pa-4 text-caption text-medium-emphasis">
          <div>Tenant: {{ shortId(auth.tenantId) }}</div>
          <div>Sucursal: {{ shortId(auth.sucursalId) }}</div>
        </div>
      </div>
    </v-navigation-drawer>

    <v-app-bar flat color="transparent" class="pos-app-bar">
      <v-btn
        icon="mdi-menu"
        class="d-md-none"
        @click="drawer = !drawer"
        aria-label="Toggle menu"
      />
      <div class="d-flex align-center gap-2">
        <div class="text-h6">{{ currentTitle }}</div>
        <v-chip color="primary" variant="tonal" size="small">Online</v-chip>
      </div>
      <v-spacer />
      <v-btn variant="tonal" color="primary" class="text-none" @click="logout">
        Logout
      </v-btn>
    </v-app-bar>

    <v-main>
      <v-container class="app-shell">
        <router-view />
      </v-container>
    </v-main>
  </v-layout>
</template>

<script setup>
import { computed, ref } from 'vue';
import { useDisplay } from 'vuetify';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';

const auth = useAuthStore();
const router = useRouter();
const route = useRoute();
const drawer = ref(true);
const { mdAndUp } = useDisplay();

const menuItems = [
  {
    title: 'POS',
    icon: 'mdi-cash-register',
    to: '/pos',
    permission: 'PERM_VENTA_CREAR'
  },
  {
    title: 'Productos',
    icon: 'mdi-package-variant',
    to: '/productos',
    path: '/productos',
    permission: 'PERM_PRODUCTO_VER'
  },
  {
    title: 'Ingreso',
    icon: 'mdi-file-document-outline',
    to: '/remitos',
    permission: 'PERM_COMPRAS_REGISTRAR'
  },
  {
    title: 'Stock',
    icon: 'mdi-warehouse',
    to: '/stock',
    permission: 'PERM_STOCK_AJUSTAR'
  },
  {
    title: 'Reportes',
    icon: 'mdi-chart-areaspline',
    to: '/reportes',
    permission: 'PERM_REPORTES_VER'
  },
  {
    title: 'Caja',
    icon: 'mdi-cash-multiple',
    to: '/caja',
    permission: 'PERM_CAJA_MOVIMIENTO',
    roles: ['ENCARGADO', 'ADMIN']
  }
];

const hasAnyRole = (roles) => roles.some((role) => auth.roles.includes(role));

const visibleItems = computed(() =>
  menuItems.filter((item) => {
    const hasPerm = item.permission ? auth.hasPermission(item.permission) : false;
    const hasRole = item.roles ? hasAnyRole(item.roles) : false;
    if (!item.permission && !item.roles) return true;
    return hasPerm || hasRole;
  })
);

const currentTitle = computed(() => {
  if (route.path.startsWith('/productos') && route.query.tab === 'proveedores') {
    return 'Proveedores';
  }
  if (route.path.startsWith('/empresa')) {
    return 'Datos de mi empresa';
  }

  const match = menuItems.find((item) => {
    const path = item.path || item.to;
    return typeof path === 'string' && route.path.startsWith(path);
  });
  return match ? match.title : 'POS';
});

const shortId = (value) => {
  if (!value) return 'n/a';
  return value.length > 8 ? value.slice(0, 8) : value;
};

const logout = () => {
  auth.logout();
  router.replace('/login');
};
</script>

<style scoped>
.pos-drawer {
  border-right: 1px solid rgba(15, 23, 42, 0.08);
}

.pos-app-bar {
  padding: 8px 16px;
  position: sticky;
  top: 0;
  z-index: 20;
  backdrop-filter: blur(6px);
  background: rgba(245, 245, 242, 0.92) !important;
}

.gap-2 {
  gap: 8px;
}
</style>

