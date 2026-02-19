import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import { createVuetify } from 'vuetify';
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';
import '@mdi/font/css/materialdesignicons.css';
import 'vuetify/styles';
import './styles/main.css';
import router from './router';
import { useAuthStore } from './stores/auth';
import { setTokenProvider, setUnauthorizedHandler } from './services/apiClient';
import { registerSW } from 'virtual:pwa-register';

const vuetify = createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'posTheme',
    themes: {
      posTheme: {
        dark: false,
        colors: {
          primary: '#0f766e',
          secondary: '#ea580c',
          accent: '#0ea5a4',
          background: '#f5f3ef',
          surface: '#ffffff',
          info: '#0ea5a4',
          success: '#16a34a',
          warning: '#f59e0b',
          error: '#dc2626'
        }
      }
    }
  }
});

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);
app.use(vuetify);

const auth = useAuthStore(pinia);
auth.loadFromStorage();

setTokenProvider(() => auth.token);
setUnauthorizedHandler(() => {
  auth.logout();
  if (router.currentRoute.value.path !== '/login') {
    router.replace('/login');
  }
});

if (import.meta.env.PROD) {
  registerSW({ immediate: true });
}

app.mount('#app');
