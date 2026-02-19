import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import LoginPage from '../pages/LoginPage.vue';
import MainLayout from '../layouts/MainLayout.vue';
import PosPage from '../pages/PosPage.vue';
import ProductosPage from '../pages/ProductosPage.vue';
import StockPage from '../pages/StockPage.vue';
import ReportesPage from '../pages/ReportesPage.vue';
import CajaPage from '../pages/CajaPage.vue';
import RemitosPage from '../pages/RemitosPage.vue';
import PreRecepcionPage from '../pages/PreRecepcionPage.vue';
import RecepcionPage from '../pages/RecepcionPage.vue';
import EmpresaDatosPage from '../pages/EmpresaDatosPage.vue';

const routes = [
  {
    path: '/login',
    name: 'login',
    component: LoginPage,
    meta: { public: true }
  },
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        redirect: '/pos'
      },
      {
        path: 'pos',
        name: 'pos',
        component: PosPage,
        meta: { permission: 'PERM_VENTA_CREAR' }
      },
      {
        path: 'productos',
        name: 'productos',
        component: ProductosPage,
        meta: { permission: 'PERM_PRODUCTO_VER' }
      },
      {
        path: 'remitos',
        name: 'remitos',
        component: RemitosPage,
        meta: { permission: 'PERM_COMPRAS_REGISTRAR' }
      },
      {
        path: 'remitos/pre-recepcion/:id',
        name: 'pre-recepcion',
        component: PreRecepcionPage,
        meta: { permission: 'PERM_COMPRAS_REGISTRAR' }
      },
      {
        path: 'recepciones/:id',
        name: 'recepcion',
        component: RecepcionPage,
        meta: { permission: 'PERM_COMPRAS_REGISTRAR' }
      },
      {
        path: 'stock',
        name: 'stock',
        component: StockPage,
        meta: { permission: 'PERM_STOCK_AJUSTAR' }
      },
      {
        path: 'reportes',
        name: 'reportes',
        component: ReportesPage,
        meta: { permission: 'PERM_REPORTES_VER' }
      },
      {
        path: 'caja',
        name: 'caja',
        component: CajaPage,
        meta: { permission: 'PERM_CAJA_MOVIMIENTO' }
      },
      {
        path: 'empresa',
        name: 'empresa',
        component: EmpresaDatosPage,
        meta: { permission: 'PERM_PRODUCTO_VER' }
      }
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/pos'
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to) => {
  const auth = useAuthStore();
  if (to.meta.public) {
    if (auth.isAuthenticated && to.path === '/login') {
      return '/pos';
    }
    return true;
  }

  if (!auth.isAuthenticated) {
    return { path: '/login', query: { redirect: to.fullPath } };
  }

  return true;
});

export default router;
