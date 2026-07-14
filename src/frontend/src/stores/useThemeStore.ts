'use client';

import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import api from '@/services/api';

type Theme = 'light' | 'dark' | 'system';

interface ThemeState {
  theme: Theme;
  resolvedTheme: 'light' | 'dark';
  setTheme: (theme: Theme) => void;
}

const syncThemeToApi = async (theme: Theme) => {
  try {
    const authStore = JSON.parse(localStorage.getItem('severina-auth') || '{}');
    if (authStore?.state?.token) {
      await api.put('/api/v1/users/preferences', { key: 'theme', value: theme });
    }
  } catch {
    // Silently fail - theme is still saved in localStorage
  }
};

const loadThemeFromApi = async (): Promise<Theme | null> => {
  try {
    const authStore = JSON.parse(localStorage.getItem('severina-auth') || '{}');
    if (authStore?.state?.token) {
      const response = await api.get('/api/v1/users/preferences/theme');
      return response.data.value as Theme;
    }
  } catch {
    // Silently fail - use localStorage theme
  }
  return null;
};

export const useThemeStore = create<ThemeState>()(
  persist(
    (set) => ({
      theme: 'system',
      resolvedTheme: 'light',
      setTheme: (theme: Theme) => {
        const resolvedTheme = theme === 'system'
          ? (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light')
          : theme;
        
        set({ theme, resolvedTheme });
        
        document.documentElement.setAttribute('data-theme', resolvedTheme);
        syncThemeToApi(theme);
      },
    }),
    {
      name: 'severina-theme',
      onRehydrateStorage: () => (state) => {
        if (state) {
          const resolvedTheme = state.theme === 'system'
            ? (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light')
            : state.theme;
          
          state.resolvedTheme = resolvedTheme;
          document.documentElement.setAttribute('data-theme', resolvedTheme);

          loadThemeFromApi().then((apiTheme) => {
            if (apiTheme && apiTheme !== state.theme) {
              state.setTheme(apiTheme);
            }
          });
        }
      },
    }
  )
);
