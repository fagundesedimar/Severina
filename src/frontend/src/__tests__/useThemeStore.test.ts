import { describe, it, expect, beforeEach } from 'vitest';
import { useThemeStore } from '@/stores/useThemeStore';

describe('useThemeStore', () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-theme');
    useThemeStore.setState({ theme: 'system', resolvedTheme: 'light' });
  });

  it('has default theme as system', () => {
    const { theme } = useThemeStore.getState();
    expect(theme).toBe('system');
  });

  it('sets theme to dark', () => {
    useThemeStore.getState().setTheme('dark');
    const { theme, resolvedTheme } = useThemeStore.getState();
    expect(theme).toBe('dark');
    expect(resolvedTheme).toBe('dark');
    expect(document.documentElement.getAttribute('data-theme')).toBe('dark');
  });

  it('sets theme to light', () => {
    useThemeStore.getState().setTheme('light');
    const { theme, resolvedTheme } = useThemeStore.getState();
    expect(theme).toBe('light');
    expect(resolvedTheme).toBe('light');
    expect(document.documentElement.getAttribute('data-theme')).toBe('light');
  });

  it('sets theme to system', () => {
    useThemeStore.getState().setTheme('system');
    const { theme } = useThemeStore.getState();
    expect(theme).toBe('system');
  });

  it('persists theme to localStorage', () => {
    useThemeStore.getState().setTheme('dark');
    const stored = localStorage.getItem('severina-theme');
    expect(stored).toBeTruthy();
    const parsed = JSON.parse(stored!);
    expect(parsed.state.theme).toBe('dark');
  });
});
