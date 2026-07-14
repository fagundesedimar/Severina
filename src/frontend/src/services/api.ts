import axios from 'axios';

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000',
  withCredentials: true,
});

api.interceptors.request.use((config) => {
  const authStore = typeof window !== 'undefined'
    ? JSON.parse(localStorage.getItem('severina-auth') || '{}')
    : null;
  
  if (authStore?.state?.token) {
    config.headers.Authorization = `Bearer ${authStore.state.token}`;
  }
  
  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      const originalRequest = error.config;
      
      if (!originalRequest._retry) {
        originalRequest._retry = true;
        
        try {
          const response = await api.post('/api/v1/auth/refresh');
          const { accessToken } = response.data;
          
          const authStore = JSON.parse(localStorage.getItem('severina-auth') || '{}');
          if (authStore.state) {
            authStore.state.token = accessToken;
            localStorage.setItem('severina-auth', JSON.stringify(authStore));
          }
          
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          return api(originalRequest);
        } catch (refreshError) {
          localStorage.removeItem('severina-auth');
          window.location.href = '/login';
          return Promise.reject(refreshError);
        }
      }
    }
    
    return Promise.reject(error);
  }
);

export default api;
