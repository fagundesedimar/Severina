'use client';

import { useState, useEffect } from 'react';
import { useAuthStore } from '@/stores/useAuthStore';
import api from '@/services/api';
import { AppShell } from '@/components/layout/AppShell';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

interface User {
  id: string;
  nome: string;
  email: string;
  papel: string;
  status: string;
  createdAt: string;
}

interface Invite {
  code: string;
  email: string;
  papel: string;
  expiresAt: string;
  createdAt: string;
}

function useCompanyUsers(companyId: string | undefined, shouldLoad: boolean) {
  const [users, setUsers] = useState<User[]>([]);
  const [invites, setInvites] = useState<Invite[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchData = async () => {
    if (!shouldLoad || !companyId) return;
    setLoading(true);
    setError('');
    try {
      const [usersRes, invitesRes] = await Promise.all([
        api.get(`/api/v1/users/company/${companyId}`),
        api.get('/api/v1/invites').catch(() => ({ data: [] })),
      ]);
      setUsers(usersRes.data);
      setInvites(invitesRes.data);
    } catch {
      setError('Erro ao carregar usuários');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    let cancelled = false;
    fetchData().then(() => {});
    return () => { cancelled = true; };
  }, [companyId, shouldLoad]);

  return { users, invites, loading, error, setUsers, setInvites, setError, refresh: fetchData };
}

export default function UsuariosPage() {
  const { user } = useAuthStore();
  const [showInviteModal, setShowInviteModal] = useState(false);
  const [inviteEmail, setInviteEmail] = useState('');
  const [inviteRole, setInviteRole] = useState('Operacional');
  const [inviteLoading, setInviteLoading] = useState(false);

  const isAdmin = user?.papel === 'Administrador';
  const { users, invites, loading, error, setUsers, setInvites, setError, refresh } = useCompanyUsers(user?.companyId, isAdmin);

  const handleInvite = async (e: React.FormEvent) => {
    e.preventDefault();
    setInviteLoading(true);
    setError('');

    try {
      await api.post('/api/v1/invites', {
        email: inviteEmail,
        papel: inviteRole,
      });
      setShowInviteModal(false);
      setInviteEmail('');
      setInviteRole('Operacional');
      refresh();
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Erro ao enviar convite');
    } finally {
      setInviteLoading(false);
    }
  };

  const handleDeactivate = async (userId: string) => {
    if (!confirm('Tem certeza que deseja desativar este usuário?')) return;

    try {
      await api.delete(`/api/v1/users/company/${user?.companyId}/users/${userId}`);
      setUsers(users.map((u) => u.id === userId ? { ...u, status: 'Inativo' } : u));
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Erro ao desativar usuário');
    }
  };

  const handleActivate = async (userId: string) => {
    try {
      await api.put(`/api/v1/users/company/${user?.companyId}/users/${userId}/activate`);
      setUsers(users.map((u) => u.id === userId ? { ...u, status: 'Ativo' } : u));
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Erro ao ativar usuário');
    }
  };

  const handleRoleChange = async (userId: string, newRole: string) => {
    try {
      await api.put(`/api/v1/users/company/${user?.companyId}/users/${userId}/role`, {
        papel: newRole,
      });
      setUsers(users.map((u) => u.id === userId ? { ...u, papel: newRole } : u));
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Erro ao alterar papel');
    }
  };

  const handleRevokeInvite = async (code: string) => {
    if (!confirm('Tem certeza que deseja revogar este convite?')) return;

    try {
      await api.delete(`/api/v1/invites/${code}`);
      setInvites(invites.filter((i) => i.code !== code));
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Erro ao revogar convite');
    }
  };

  if (!isAdmin) {
    return (
      <AppShell title="Configurações">
        <div className="flex items-center justify-center min-h-[50vh]">
          <p className="text-foreground">Acesso restrito a administradores.</p>
        </div>
      </AppShell>
    );
  }

  return (
    <AppShell
      title="Gerenciar Usuários"
      actions={
        <Button size="sm" onClick={() => setShowInviteModal(true)}>
          Convidar Usuário
        </Button>
      }
    >
      <div className="max-w-4xl mx-auto">
        {error && (
          <p className="text-destructive text-sm mb-4" role="alert" aria-live="polite">{error}</p>
        )}

        {loading ? (
          <p className="text-muted-foreground">Carregando...</p>
        ) : (
          <div className="bg-card border border-border rounded-lg overflow-hidden">
            <table className="w-full" role="grid">
              <thead className="bg-muted">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Nome</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Email</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Papel</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Status</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Ações</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {users.map((u) => (
                  <tr key={u.id}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">{u.nome}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">{u.email}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <select
                        value={u.papel}
                        onChange={(e) => handleRoleChange(u.id, e.target.value)}
                        className="border border-border rounded px-2 py-1 text-sm bg-background text-foreground"
                        disabled={u.id === user?.id}
                        aria-label={`Papel de ${u.nome}`}
                      >
                        <option value="administrador">Administrador</option>
                        <option value="operacional">Operacional</option>
                      </select>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <span className={`px-2 py-1 rounded-full text-xs ${
                        u.status === 'Ativo' || u.status === 'ativo' ? 'bg-success/10 text-success' : 'bg-destructive/10 text-destructive'
                      }`}>
                        {u.status === 'Ativo' || u.status === 'ativo' ? 'Ativo' : 'Inativo'}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      {u.id !== user?.id && (
                        u.status === 'Ativo' || u.status === 'ativo' ? (
                          <button
                            onClick={() => handleDeactivate(u.id)}
                            className="text-destructive hover:text-destructive-hover"
                            aria-label={`Desativar ${u.nome}`}
                          >
                            Desativar
                          </button>
                        ) : (
                          <button
                            onClick={() => handleActivate(u.id)}
                            className="text-success hover:opacity-80"
                            aria-label={`Ativar ${u.nome}`}
                          >
                            Ativar
                          </button>
                        )
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {invites.length > 0 && (
          <div className="mt-6">
            <h3 className="text-sm font-semibold text-foreground mb-3">Convites Pendentes</h3>
            <div className="bg-card border border-border rounded-lg overflow-hidden">
              <table className="w-full">
                <thead className="bg-muted">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Email</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Papel</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Expira em</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">Ações</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {invites.map((invite, idx) => (
                    <tr key={invite.code || `${invite.email}-${idx}`}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">{invite.email}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">{invite.papel === 'administrador' ? 'Administrador' : 'Operacional'}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-muted-foreground">
                        {new Date(invite.expiresAt).toLocaleDateString('pt-BR')}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <button
                          onClick={() => handleRevokeInvite(invite.code)}
                          className="text-destructive hover:text-destructive-hover"
                        >
                          Revogar
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {showInviteModal && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50" role="dialog" aria-modal="true">
            <div className="bg-card border border-border rounded-lg p-6 w-full max-w-md">
              <h2 className="text-xl font-bold mb-4 text-foreground">Convidar Usuário</h2>

              <form onSubmit={handleInvite} className="space-y-4">
                <div>
                  <label htmlFor="invite-email" className="block text-sm font-medium mb-1 text-foreground">
                    Email do convite
                  </label>
                  <Input
                    type="email"
                    id="invite-email"
                    value={inviteEmail}
                    onChange={(e) => setInviteEmail(e.target.value)}
                    required
                  />
                </div>

                <div>
                  <label htmlFor="invite-role" className="block text-sm font-medium mb-1 text-foreground">
                    Papel
                  </label>
                  <select
                    id="invite-role"
                    value={inviteRole}
                    onChange={(e) => setInviteRole(e.target.value)}
                    className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:border-primary bg-background text-foreground"
                  >
                    <option value="operacional">Operacional</option>
                    <option value="administrador">Administrador</option>
                  </select>
                </div>

                <div className="flex gap-2 justify-end">
                  <Button type="button" variant="outline" onClick={() => setShowInviteModal(false)}>
                    Cancelar
                  </Button>
                  <Button type="submit" disabled={inviteLoading}>
                    {inviteLoading ? 'Enviando...' : 'Enviar Convite'}
                  </Button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </AppShell>
  );
}
