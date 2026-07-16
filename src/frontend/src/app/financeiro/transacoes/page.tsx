'use client';

import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { financialApi } from '@/services/financialApi';
import { ExportModal } from '@/components/financial/ExportModal';
import { useAuthStore } from '@/stores/useAuthStore';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import { AppShell } from '@/components/layout/AppShell';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

export default function TransacoesPage() {
  const { user } = useAuthStore();
  const router = useRouter();
  const queryClient = useQueryClient();
  const [tipo, setTipo] = useState('');
  const [categoria, setCategoria] = useState('');
  const [page, setPage] = useState(1);
  const [showCreate, setShowCreate] = useState(false);
  const [showExport, setShowExport] = useState(false);
  const [form, setForm] = useState({ tipo: 'Receita', valor: '', data: '', categoria: 'Servicos', descricao: '', clientId: '' });

  useEffect(() => {
    if (!user) router.push('/login');
  }, [user, router]);

  const { data, isLoading } = useQuery({
    queryKey: ['transactions', { tipo, categoria, page }],
    queryFn: () => financialApi.listTransactions({ tipo: tipo || undefined, categoria: categoria || undefined, page, pageSize: 15 }),
    enabled: !!user,
  });

  const createMutation = useMutation({
    mutationFn: (formData: typeof form) =>
      financialApi.createTransaction({
        ...formData,
        valor: parseFloat(formData.valor),
        clientId: formData.clientId || undefined,
        descricao: formData.descricao || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] });
      queryClient.invalidateQueries({ queryKey: ['financial-dashboard'] });
      setShowCreate(false);
      setForm({ tipo: 'Receita', valor: '', data: '', categoria: 'Servicos', descricao: '', clientId: '' });
    },
  });

  const formatCurrency = (v: number) =>
    new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v);

  const tipoColors: Record<string, string> = { Receita: 'text-emerald-600', Despesa: 'text-red-600' };
  const statusColors: Record<string, string> = {
    Pending: 'bg-amber-100 text-amber-800',
    Approved: 'bg-emerald-100 text-emerald-800',
    Rejected: 'bg-red-100 text-red-800',
  };

  if (!user) return null;

  return (
    <AppShell
      title="Transações"
      actions={
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={() => setShowExport(true)}>
            Exportar
          </Button>
          <Button size="sm" onClick={() => setShowCreate(!showCreate)}>
            {showCreate ? 'Cancelar' : '+ Nova Transação'}
          </Button>
        </div>
      }
    >
      <div className="max-w-[1440px] mx-auto">
        {showCreate && (
          <form
            onSubmit={(e) => { e.preventDefault(); createMutation.mutate(form); }}
            className="bg-card border border-border rounded-lg p-6 mb-6 space-y-4"
          >
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <select value={form.tipo} onChange={(e) => setForm({ ...form, tipo: e.target.value })} className="border border-border rounded-lg px-3 py-2 text-sm bg-background text-foreground">
                <option value="Receita">Receita</option>
                <option value="Despesa">Despesa</option>
              </select>
              <Input type="number" step="0.01" placeholder="Valor" required value={form.valor} onChange={(e) => setForm({ ...form, valor: e.target.value })} />
              <Input type="date" required value={form.data} onChange={(e) => setForm({ ...form, data: e.target.value })} />
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <select value={form.categoria} onChange={(e) => setForm({ ...form, categoria: e.target.value })} className="border border-border rounded-lg px-3 py-2 text-sm bg-background text-foreground">
                <option value="Servicos">Serviços</option>
                <option value="Materiais">Materiais</option>
                <option value="Frente">Frente de Trabalho</option>
                <option value="Impostos">Impostos</option>
                <option value="Outros">Outros</option>
              </select>
              <Input type="text" placeholder="Descrição (opcional)" value={form.descricao} onChange={(e) => setForm({ ...form, descricao: e.target.value })} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="ghost" onClick={() => setShowCreate(false)}>Cancelar</Button>
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending ? 'Salvando...' : 'Salvar'}
              </Button>
            </div>
          </form>
        )}

        <div className="flex gap-3 mb-4">
          <select value={tipo} onChange={(e) => { setTipo(e.target.value); setPage(1); }} className="border border-border rounded-lg px-3 py-2 text-sm bg-background text-foreground">
            <option value="">Todos os tipos</option>
            <option value="Receita">Receita</option>
            <option value="Despesa">Despesa</option>
          </select>
          <select value={categoria} onChange={(e) => { setCategoria(e.target.value); setPage(1); }} className="border border-border rounded-lg px-3 py-2 text-sm bg-background text-foreground">
            <option value="">Todas as categorias</option>
            <option value="Servicos">Serviços</option>
            <option value="Materiais">Materiais</option>
            <option value="Frente">Frente de Trabalho</option>
            <option value="Impostos">Impostos</option>
            <option value="Outros">Outros</option>
          </select>
        </div>

        {isLoading ? (
          <p className="text-muted-foreground">Carregando...</p>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-border">
                    <th className="text-left py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Data</th>
                    <th className="text-left py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Tipo</th>
                    <th className="text-left py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Descrição</th>
                    <th className="text-left py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Categoria</th>
                    <th className="text-right py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Valor</th>
                    <th className="text-center py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Status</th>
                  </tr>
                </thead>
                <tbody>
                  {data?.items.map((t) => (
                    <tr key={t.id} className="border-b border-border hover:bg-muted transition-colors">
                      <td className="py-3 px-4 text-sm text-foreground">{new Date(t.data).toLocaleDateString('pt-BR')}</td>
                      <td className="py-3 px-4 text-sm font-semibold text-foreground">{t.tipo}</td>
                      <td className="py-3 px-4 text-sm text-muted-foreground">{t.descricao || '-'}</td>
                      <td className="py-3 px-4 text-sm text-muted-foreground">{t.categoria}</td>
                      <td className={`py-3 px-4 text-right text-sm font-mono font-semibold ${tipoColors[t.tipo]}`}>
                        {t.tipo === 'Receita' ? '+' : '-'}{formatCurrency(t.valor)}
                      </td>
                      <td className="py-3 px-4 text-center">
                        <span className={`text-xs font-medium px-2 py-1 rounded-full ${statusColors[t.status]}`}>
                          {t.status === 'Pending' ? 'Pendente' : t.status === 'Approved' ? 'Aprovado' : 'Rejeitado'}
                        </span>
                      </td>
                    </tr>
                  ))}
                  {(!data?.items || data.items.length === 0) && (
                    <tr><td colSpan={6} className="py-8 text-center text-muted-foreground text-sm">Nenhuma transação encontrada</td></tr>
                  )}
                </tbody>
              </table>
            </div>
            {data && data.totalCount > 15 && (
              <div className="flex justify-center gap-2 mt-4">
                <Button variant="outline" size="sm" onClick={() => setPage(Math.max(1, page - 1))} disabled={page === 1}>Anterior</Button>
                <span className="px-3 py-1 text-sm text-muted-foreground">Página {page} de {Math.ceil(data.totalCount / 15)}</span>
                <Button variant="outline" size="sm" onClick={() => setPage(page + 1)} disabled={page >= Math.ceil(data.totalCount / 15)}>Próxima</Button>
              </div>
            )}
          </>
        )}
      </div>
      {showExport && <ExportModal type="transactions" onClose={() => setShowExport(false)} />}
    </AppShell>
  );
}
