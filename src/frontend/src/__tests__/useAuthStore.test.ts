import { describe, it, expect, beforeEach } from 'vitest';
import { useAuthStore } from '@/stores/useAuthStore';

describe('useAuthStore', () => {
  beforeEach(() => {
    localStorage.clear();
    useAuthStore.setState({ user: null, token: null, isAuthenticated: false });
  });

  it('has initial state as not authenticated', () => {
    const { user, token, isAuthenticated } = useAuthStore.getState();
    expect(user).toBeNull();
    expect(token).toBeNull();
    expect(isAuthenticated).toBe(false);
  });

  it('sets auth with user and token', () => {
    const user = {
      id: '1',
      email: 'test@test.com',
      nome: 'Test User',
      papel: 'Administrador',
      companyId: 'company-1',
    };
    const token = 'test-jwt-token';

    useAuthStore.getState().setAuth(user, token);

    const state = useAuthStore.getState();
    expect(state.user).toEqual(user);
    expect(state.token).toBe(token);
    expect(state.isAuthenticated).toBe(true);
  });

  it('clears auth on logout', () => {
    const user = {
      id: '1',
      email: 'test@test.com',
      nome: 'Test User',
      papel: 'Administrador',
      companyId: 'company-1',
    };

    useAuthStore.getState().setAuth(user, 'token');
    expect(useAuthStore.getState().isAuthenticated).toBe(true);

    useAuthStore.getState().logout();
    const state = useAuthStore.getState();
    expect(state.user).toBeNull();
    expect(state.token).toBeNull();
    expect(state.isAuthenticated).toBe(false);
  });

  it('persists auth to localStorage', () => {
    const user = {
      id: '1',
      email: 'test@test.com',
      nome: 'Test User',
      papel: 'Administrador',
      companyId: 'company-1',
    };

    useAuthStore.getState().setAuth(user, 'token');

    const stored = localStorage.getItem('severina-auth');
    expect(stored).toBeTruthy();
    const parsed = JSON.parse(stored!);
    expect(parsed.state.isAuthenticated).toBe(true);
    expect(parsed.state.user.email).toBe('test@test.com');
  });
});
