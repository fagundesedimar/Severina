'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import api from '@/services/api';

interface InviteData {
  email: string;
  expiresAt: string;
}

function useInviteValidation(code: string) {
  const [invite, setInvite] = useState<InviteData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [valid, setValid] = useState(false);

  useEffect(() => {
    let cancelled = false;

    api.get(`/api/v1/invites/${code}/validate`)
      .then((response) => {
        if (!cancelled) {
          setInvite(response.data);
          setValid(true);
        }
      })
      .catch((err: unknown) => {
        if (!cancelled) {
          const axiosErr = err as { response?: { status?: number } };
          if (axiosErr.response?.status === 410) {
            setError('Este convite expirado. Solicite um novo convite.');
          } else {
            setError('Convite não encontrado ou inválido.');
          }
          setValid(false);
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => { cancelled = true; };
  }, [code]);

  return { invite, loading, error, valid };
}

export default function ConvitePage() {
  const params = useParams();
  const router = useRouter();
  const code = params.code as string;

  const { invite, loading, error, valid } = useInviteValidation(code);

  const [nome, setNome] = useState('');
  const [senha, setSenha] = useState('');
  const [confirmSenha, setConfirmSenha] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (senha !== confirmSenha) {
      setSubmitError('As senhas não coincidem.');
      return;
    }

    if (senha.length < 8) {
      setSubmitError('A senha deve ter no mínimo 8 caracteres.');
      return;
    }

    setSubmitting(true);
    setSubmitError('');

    try {
      await api.post(`/api/v1/invites/${code}/accept`, {
        nome,
        senha,
      });
      router.push('/login?convite=aceito');
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setSubmitError(axiosErr.response?.data?.message || 'Erro ao aceitar convite');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-surface">
        <p className="text-on-surface">Validando convite...</p>
      </div>
    );
  }

  if (!valid) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-surface p-4">
        <div className="w-full max-w-md p-8 bg-white dark:bg-gray-800 rounded-lg shadow-lg text-center">
          <h1 className="text-2xl font-bold mb-4 text-on-surface">Convite Inválido</h1>
          <p className="text-on-surface/70 mb-6">{error}</p>
          <a
            href="/login"
            className="inline-block py-2 px-4 bg-primary text-white rounded-md hover:bg-primary/90 transition-colors"
          >
            Ir para o Login
          </a>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-surface p-4">
      <div className="w-full max-w-md p-8 bg-white dark:bg-gray-800 rounded-lg shadow-lg">
        <h1 className="text-2xl font-bold text-center mb-2 text-on-surface">
          Aceitar Convite
        </h1>
        <p className="text-center text-on-surface/60 mb-6">
          Convite para: <strong>{invite?.email}</strong>
        </p>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="nome" className="block text-sm font-medium mb-1 text-on-surface">
              Seu Nome
            </label>
            <input
              type="text"
              id="nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              required
              aria-describedby="nome-help"
            />
            <p id="nome-help" className="text-xs text-on-surface/60 mt-1">
              Nome completo como será exibido no sistema
            </p>
          </div>

          <div>
            <label htmlFor="senha" className="block text-sm font-medium mb-1 text-on-surface">
              Crie sua Senha
            </label>
            <input
              type="password"
              id="senha"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              required
              minLength={8}
              aria-describedby="senha-help"
            />
            <p id="senha-help" className="text-xs text-on-surface/60 mt-1">
              Mínimo de 8 caracteres
            </p>
          </div>

          <div>
            <label htmlFor="confirm-senha" className="block text-sm font-medium mb-1 text-on-surface">
              Confirme sua Senha
            </label>
            <input
              type="password"
              id="confirm-senha"
              value={confirmSenha}
              onChange={(e) => setConfirmSenha(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              required
            />
          </div>

          {(submitError || error) && (
            <p className="text-red-500 text-sm" role="alert" aria-live="polite">{submitError || error}</p>
          )}

          <button
            type="submit"
            disabled={submitting}
            className="w-full py-2 px-4 bg-primary text-white rounded-md hover:bg-primary/90 disabled:opacity-50 transition-colors"
          >
            {submitting ? 'Criando conta...' : 'Aceitar Convite'}
          </button>
        </form>
      </div>
    </div>
  );
}
