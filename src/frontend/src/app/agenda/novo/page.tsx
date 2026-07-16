'use client';

import { Suspense, useState, useEffect } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { useAuthStore } from '@/stores/useAuthStore';
import { appointmentApi } from '@/services/appointmentApi';
import { clientApi } from '@/services/clientApi';
import { TipoCompromisso, TipoCompromissoLabels } from '@/types/appointment';
import type { Client } from '@/types/client';

function NovoCompromissoForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { isAuthenticated } = useAuthStore();

  const [titulo, setTitulo] = useState('');
  const [descricao, setDescricao] = useState('');
  const [dataInicio, setDataInicio] = useState('');
  const [horaInicio, setHoraInicio] = useState('09:00');
  const [dataFim, setDataFim] = useState('');
  const [horaFim, setHoraFim] = useState('10:00');
  const [tipo, setTipo] = useState<TipoCompromisso>(TipoCompromisso.Reuniao);
  const [clientId, setClientId] = useState('');
  const [clientes, setClientes] = useState<Client[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});

  useEffect(() => {
    if (!isAuthenticated) {
      router.push('/login');
      return;
    }

    const today = searchParams.get('data') || new Date().toISOString().split('T')[0];
    setDataInicio(today);
    setDataFim(today);

    clientApi.list({ pageSize: 100 }).then((res) => setClientes(res.items)).catch(() => {});
  }, [isAuthenticated, router, searchParams]);

  if (!isAuthenticated) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setFieldErrors({});

    try {
      await appointmentApi.create({
        titulo,
        descricao: descricao || undefined,
        dataHoraInicio: new Date(`${dataInicio}T${horaInicio}:00`).toISOString(),
        dataHoraFim: new Date(`${dataFim}T${horaFim}:00`).toISOString(),
        tipo,
        clientId: clientId || undefined,
      });
      router.push('/agenda');
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string; errors?: Record<string, string[]> } } };
      const data = axiosErr.response?.data;
      if (data?.errors && Object.keys(data.errors).length > 0) {
        setFieldErrors(data.errors);
      }
      setError(data?.message || 'Erro ao criar compromisso');
    } finally {
      setLoading(false);
    }
  };

  const inputClass = (field: string) =>
    `w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 ${
      fieldErrors[field]
        ? 'border-red-500 focus:ring-red-500'
        : 'border-gray-300 focus:ring-primary'
    }`;

  return (
    <div className="min-h-screen bg-surface">
      <header className="bg-white dark:bg-gray-800 shadow sticky top-0 z-50">
        <div className="max-w-3xl mx-auto px-4 py-4 flex items-center gap-4">
          <button
            onClick={() => router.back()}
            className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
            aria-label="Voltar"
          >
            <span className="material-symbols-outlined">arrow_back</span>
          </button>
          <h1 className="text-xl font-bold text-on-surface">Novo Compromisso</h1>
        </div>
      </header>

      <main className="max-w-3xl mx-auto px-4 py-6">
        <form onSubmit={handleSubmit} className="bg-white dark:bg-gray-800 rounded-lg shadow p-6 space-y-5">
          <div>
            <label htmlFor="titulo" className="block text-sm font-medium mb-1 text-on-surface">
              Titulo *
            </label>
            <input
              type="text"
              id="titulo"
              value={titulo}
              onChange={(e) => { setTitulo(e.target.value); setFieldErrors((prev) => { const n = { ...prev }; delete n.titulo; return n; }); }}
              placeholder="Ex: Reuniao de acompanhamento"
              className={inputClass('titulo')}
              required
            />
            {fieldErrors['titulo'] && <p className="text-red-500 text-xs mt-1">{fieldErrors['titulo'][0]}</p>}
          </div>

          <div>
            <label htmlFor="tipo" className="block text-sm font-medium mb-1 text-on-surface">
              Tipo *
            </label>
            <select
              id="tipo"
              value={tipo}
              onChange={(e) => setTipo(Number(e.target.value) as TipoCompromisso)}
              className={inputClass('tipo')}
            >
              {Object.entries(TipoCompromissoLabels).map(([value, label]) => (
                <option key={value} value={value}>{label}</option>
              ))}
            </select>
            {fieldErrors['tipo'] && <p className="text-red-500 text-xs mt-1">{fieldErrors['tipo'][0]}</p>}
          </div>

          <div>
            <label htmlFor="cliente" className="block text-sm font-medium mb-1 text-on-surface">
              Cliente
            </label>
            <select
              id="cliente"
              value={clientId}
              onChange={(e) => setClientId(e.target.value)}
              className={inputClass('clientId')}
            >
              <option value="">Nenhum</option>
              {clientes.map((c) => (
                <option key={c.id} value={c.id}>{c.nome}</option>
              ))}
            </select>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label htmlFor="dataInicio" className="block text-sm font-medium mb-1 text-on-surface">
                Data Inicio *
              </label>
              <input
                type="date"
                id="dataInicio"
                value={dataInicio}
                onChange={(e) => setDataInicio(e.target.value)}
                className={inputClass('dataHoraInicio')}
                required
              />
              {fieldErrors['dataHoraInicio'] && <p className="text-red-500 text-xs mt-1">{fieldErrors['dataHoraInicio'][0]}</p>}
            </div>
            <div>
              <label htmlFor="horaInicio" className="block text-sm font-medium mb-1 text-on-surface">
                Hora Inicio *
              </label>
              <input
                type="time"
                id="horaInicio"
                value={horaInicio}
                onChange={(e) => setHoraInicio(e.target.value)}
                className={inputClass('dataHoraInicio')}
                required
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label htmlFor="dataFim" className="block text-sm font-medium mb-1 text-on-surface">
                Data Fim *
              </label>
              <input
                type="date"
                id="dataFim"
                value={dataFim}
                onChange={(e) => setDataFim(e.target.value)}
                className={inputClass('dataHoraFim')}
                required
              />
              {fieldErrors['dataHoraFim'] && <p className="text-red-500 text-xs mt-1">{fieldErrors['dataHoraFim'][0]}</p>}
            </div>
            <div>
              <label htmlFor="horaFim" className="block text-sm font-medium mb-1 text-on-surface">
                Hora Fim *
              </label>
              <input
                type="time"
                id="horaFim"
                value={horaFim}
                onChange={(e) => setHoraFim(e.target.value)}
                className={inputClass('dataHoraFim')}
                required
              />
            </div>
          </div>

          <div>
            <label htmlFor="descricao" className="block text-sm font-medium mb-1 text-on-surface">
              Descricao
            </label>
            <textarea
              id="descricao"
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              rows={3}
              placeholder="Detalhes do compromisso..."
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary resize-none"
            />
          </div>

          {error && (
            <p className="text-red-500 text-sm" role="alert">{error}</p>
          )}

          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={() => router.back()}
              className="flex-1 py-2 px-4 border border-gray-300 rounded-md text-on-surface hover:bg-gray-50 transition-colors"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading}
              className="flex-1 py-2 px-4 bg-primary text-on-primary rounded-md hover:bg-primary/90 disabled:opacity-50 transition-colors"
            >
              {loading ? 'Salvando...' : 'Salvar'}
            </button>
          </div>
        </form>
      </main>
    </div>
  );
}

export default function NovoCompromissoPage() {
  return (
    <Suspense fallback={<div className="min-h-screen bg-surface flex items-center justify-center"><p className="text-on-surface/60">Carregando...</p></div>}>
      <NovoCompromissoForm />
    </Suspense>
  );
}
