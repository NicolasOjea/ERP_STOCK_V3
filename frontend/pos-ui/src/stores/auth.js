import { defineStore } from 'pinia';
import { postJson } from '../services/apiClient';

const STORAGE_KEY = 'pos-auth';

const normalizeArray = (value) => {
  if (!value) return [];
  if (Array.isArray(value)) return value;
  if (typeof value === 'string') return [value];
  return [];
};

const parseJwt = (token) => {
  try {
    const parts = token.split('.');
    if (parts.length < 2) return {};
    const payload = parts[1];
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
    const binary = atob(base64);
    const json = decodeURIComponent(
      binary
        .split('')
        .map((char) => `%${char.charCodeAt(0).toString(16).padStart(2, '0')}`)
        .join('')
    );
    return JSON.parse(json);
  } catch {
    return {};
  }
};

const readExpiry = (token, fallback) => {
  if (fallback) return fallback;
  const payload = parseJwt(token);
  if (payload && payload.exp) {
    const ms = Number(payload.exp) * 1000;
    if (!Number.isNaN(ms)) {
      return new Date(ms).toISOString();
    }
  }
  return '';
};

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: '',
    expiresAt: '',
    tenantId: '',
    sucursalId: '',
    userId: '',
    roles: [],
    permissions: []
  }),
  getters: {
    isAuthenticated: (state) => {
      if (!state.token) return false;
      if (!state.expiresAt) return true;
      return new Date(state.expiresAt).getTime() > Date.now();
    }
  },
  actions: {
    hasPermission(permission) {
      if (!permission) return false;
      const normalized = permission.startsWith('PERM_')
        ? permission.slice(5)
        : permission;
      return this.permissions.includes(normalized) || this.permissions.includes(permission);
    },
    loadFromStorage() {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return;
      try {
        const data = JSON.parse(raw);
        this.token = data.token || '';
        this.expiresAt = data.expiresAt || '';
        this.tenantId = data.tenantId || '';
        this.sucursalId = data.sucursalId || '';
        this.userId = data.userId || '';
        this.roles = Array.isArray(data.roles) ? data.roles : [];
        this.permissions = Array.isArray(data.permissions) ? data.permissions : [];
        if (this.expiresAt && new Date(this.expiresAt).getTime() <= Date.now()) {
          this.clearSession();
        }
      } catch {
        this.clearSession();
      }
    },
    saveToStorage() {
      const payload = {
        token: this.token,
        expiresAt: this.expiresAt,
        tenantId: this.tenantId,
        sucursalId: this.sucursalId,
        userId: this.userId,
        roles: this.roles,
        permissions: this.permissions
      };
      localStorage.setItem(STORAGE_KEY, JSON.stringify(payload));
    },
    clearSession() {
      this.token = '';
      this.expiresAt = '';
      this.tenantId = '';
      this.sucursalId = '';
      this.userId = '';
      this.roles = [];
      this.permissions = [];
      localStorage.removeItem(STORAGE_KEY);
    },
    setSession(token, expiresAt) {
      this.token = token;
      this.expiresAt = readExpiry(token, expiresAt);

      const payload = parseJwt(token);
      this.tenantId = payload.tenant_id || '';
      this.sucursalId = payload.sucursal_id || '';
      this.userId = payload.user_id || '';
      this.roles = normalizeArray(payload.roles);
      this.permissions = normalizeArray(payload.permissions);

      this.saveToStorage();
    },
    async login({ username, password, tenantId, sucursalId }) {
      const request = {
        username: (username || '').trim(),
        password: password || '',
        tenantId: tenantId || null,
        sucursalId: sucursalId || null
      };

      const { response, data } = await postJson('/api/v1/auth/login', request);

      if (!response.ok) {
        if (response.status === 401) {
          throw new Error('Credenciales invalidas.');
        }
        const message = data?.detail || data?.title || 'Error de login.';
        throw new Error(message);
      }

      if (!data || !data.token) {
        throw new Error('Respuesta de login invalida.');
      }

      this.setSession(data.token, data.expiresAt || '');
    },
    logout() {
      this.clearSession();
    }
  }
});
