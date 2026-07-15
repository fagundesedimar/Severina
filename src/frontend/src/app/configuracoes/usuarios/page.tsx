'use client';

import { useState, useEffect } from 'react';
import { useAuthStore } from '@/stores/useAuthStore';
import api from '@/services/api';

interface User {
  id: string;
  nome: string;
  email: string;
  papel: string;
  status: string;
  createdAt: string;
}

function useCompanyUsers(companyId: string | undefined, shouldLoad: boolean) {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!shouldLoad || !companyId) return;

    let cancelled = false;

    api.get(`/api/v1/users/company/${companyId}`)
      .then((response) => {
        if (!cancelled) setUsers(response.data);
      })
      .catch(() => {
        if (!cancelled) setError('Erro ao carregar usuários');
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => { cancelled = true; };
  }, [companyId, shouldLoad]);

  return { users, loading, error, setUsers, setError };
}

export default function UsuariosPage() {
  const { user } = useAuthStore();
  const [showInviteModal, setShowInviteModal] = useState(false);
  const [inviteEmail, setInviteEmail] = useState('');
  const [inviteRole, setInviteRole] = useState('Operacional');
  const [inviteLoading, setInviteLoading] = useState(false);

  const isAdmin = user?.papel === 'Administrador';
  const { users, loading, error, setUsers, setError } = useCompanyUsers(user?.companyId, isAdmin);

  const handleInvite = async (e: React.FormEvent) => {
    e.preventDefault();
    setInviteLoading(true);
    setError('');

    try {
      await api.post('/api/v1/invites', {
        email: inviteEmail,
        papel: inviteRole === 'Administrador' ? 0 : 1,
      });
      setShowInviteModal(false);
      setInviteEmail('');
      setInviteRole('Operacional');
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
      setUsers(users.filter((u) => u.id !== userId));
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Erro ao desativar usuário');
    }
  };

  const handleRoleChange = async (userId: string, newRole: string) => {
    try {
      await api.put(`/api/v1/users/company/${user?.companyId}/users/${userId}/role`, {
        papel: newRole === 'Administrador' ? 0 : 1,
      });
      setUsers(users.map((u) => u.id === userId ? { ...u, papel: newRole } : u));
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Erro ao alterar papel');
    }
  };

  if (!isAdmin) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-surface">
        <p className="text-on-surface">Acesso restrito a administradores.</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-surface p-6">
      <div className="max-w-4xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-on-surface">Gerenciar Usuários</h1>
          <button
            onClick={() => setShowInviteModal(true)}
            className="py-2 px-4 bg-primary text-white rounded-md hover:bg-primary/90 transition-colors"
          >
            Convidar Usuário
          </button>
        </div>

        {error && (
          <p className="text-red-500 text-sm mb-4" role="alert" aria-live="polite">{error}</p>
        )}

        {loading ? (
          <p className="text-on-surface">Carregando...</p>
        ) : (
          <div className="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
            <table className="w-full" role="grid">
              <thead className="bg-gray-50 dark:bg-gray-700">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-on-surface/60 uppercase tracking-wider">Nome</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-on-surface/60 uppercase tracking-wider">Email</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-on-surface/60 uppercase tracking-wider">Papel</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-on-surface/60 uppercase tracking-wider">Status</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-on-surface/60 uppercase tracking-wider">Ações</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200 dark:divide-gray-600">
                {users.map((u) => (
                  <tr key={u.id}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-on-surface">{u.nome}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-on-surface">{u.email}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <select
                        value={u.papel}
                        onChange={(e) => handleRoleChange(u.id, e.target.value)}
                        className="border rounded px-2 py-1 text-sm"
                        disabled={u.id === user?.id}
                        aria-label={`Papel de ${u.nome}`}
                      >
                        <option value="Administrador">Administrador</option>
                        <option value="Operacional">Operacional</option>
                      </select>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <span className={`px-2 py-1 rounded-full text-xs ${
                        u.status === 'Ativo' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                      }`}>
                        {u.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      {u.id !== user?.id && (
                        <button
                          onClick={() => handleDeactivate(u.id)}
                          className="text-red-500 hover:text-red-700"
                          aria-label={`Desativar ${u.nome}`}
                        >
                          Desativar
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {showInviteModal && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50" role="dialog" aria-modal="true">
            <div className="bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md">
              <h2 className="text-xl font-bold mb-4 text-on-surface">Convidar Usuário</h2>

              <form onSubmit={handleInvite} className="space-y-4">
                <div>
                  <label htmlFor="invite-email" className="block text-sm font-medium mb-1 text-on-surface">
                    Email do convite
                  </label>
                  <input
                    type="email"
                    id="invite-email"
                    value={inviteEmail}
                    onChange={(e) => setInviteEmail(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
                    required
                  />
                </div>

                <div>
                  <label htmlFor="invite-role" className="block text-sm font-medium mb-1 text-on-surface">
                    Papel
                  </label>
                  <select
                    id="invite-role"
                    value={inviteRole}
                    onChange={(e) => setInviteRole(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
                  >
                    <option value="Operacional">Operacional</option>
                    <option value="Administrador">Administrador</option>
                  </select>
                </div>

                <div className="flex gap-2 justify-end">
                  <button
                    type="button"
                    onClick={() => setShowInviteModal(false)}
                    className="py-2 px-4 border border-gray-300 rounded-md hover:bg-gray-100 transition-colors"
                  >
                    Cancelar
                  </button>
                  <button
                    type="submit"
                    disabled={inviteLoading}
                    className="py-2 px-4 bg-primary text-white rounded-md hover:bg-primary/90 disabled:opacity-50 transition-colors"
                  >
                    {inviteLoading ? 'Enviando...' : 'Enviar Convite'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
