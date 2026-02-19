const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5072';

let tokenProvider = () => '';
let onUnauthorized = null;

export const setTokenProvider = (provider) => {
  tokenProvider = provider;
};

export const setUnauthorizedHandler = (handler) => {
  onUnauthorized = handler;
};

const request = async (path, options = {}) => {
  const headers = new Headers(options.headers || {});
  const token = tokenProvider ? tokenProvider() : '';
  if (token) {
    headers.set('Authorization', `Bearer ${token}`);
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers
  });

  if (response.status === 401 && onUnauthorized) {
    onUnauthorized();
  }

  return response;
};

export const requestJson = async (path, options = {}) => {
  const headers = new Headers(options.headers || {});
  headers.set('Accept', 'application/json');

  if (options.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  const response = await request(path, {
    ...options,
    headers
  });

  const contentType = response.headers.get('content-type') || '';
  let data = null;
  if (contentType.includes('application/json') || contentType.includes('+json')) {
    data = await response.json();
  }

  return { response, data };
};

export const getJson = (path) => requestJson(path, { method: 'GET' });

export const postJson = (path, body) =>
  requestJson(path, {
    method: 'POST',
    body: JSON.stringify(body)
  });
