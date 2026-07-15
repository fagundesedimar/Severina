'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import type { Client, Interaction } from '@/types/client';
import { clientApi } from '@/services/clientApi';

export default function ClienteDetailPage() {
  const params = useParams();
  const router = useRouter();
  const clientId = params.id as string;

  const [client, setClient] = useState<Client | null>(null);
  const [interactions, setInteractions] = useState<Interaction[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [editForm, setEditForm] = useState({ nome: '', email: '', telefone: '', empresa: '' });
  const [newTag, setNewTag] = useState('');
  const [newNote, setNewNote] = useState('');
  const [newInteraction, setNewInteraction] = useState({ type: 'Message', content: '' });

  useEffect(() => {
    async function load() {
      try {
        const [clientData, interactionsData] = await Promise.all([
          clientApi.getById(clientId),
          clientApi.listInteractions(clientId),
        ]);
        setClient(clientData);
        setInteractions(interactionsData.items);
        setEditForm({
          nome: clientData.nome,
          email: clientData.email || '',
          telefone: clientData.telefone || '',
          empresa: clientData.empresa || '',
        });
      } catch {
        router.push('/clientes');
      } finally {
        setIsLoading(false);
      }
    }
    load();
  }, [clientId, router]);

  const handleSave = async () => {
    if (!client) return;
    try {
      await clientApi.update(client.id, editForm);
      setClient({ ...client, ...editForm });
      setIsEditing(false);
    } catch {
      alert('Erro ao salvar');
    }
  };

  const handleAddTag = async () => {
    if (!client || !newTag.trim()) return;
    try {
      await clientApi.addTag(client.id, newTag.trim());
      setClient({ ...client, tags: [...client.tags, newTag.trim()] });
      setNewTag('');
    } catch {
      alert('Erro ao adicionar tag');
    }
  };

  const handleRemoveTag = async (tag: string) => {
    if (!client) return;
    try {
      await clientApi.removeTag(client.id, tag);
      setClient({ ...client, tags: client.tags.filter((t) => t !== tag) });
    } catch {
      alert('Erro ao remover tag');
    }
  };

  const handleAddNote = async () => {
    if (!client || !newNote.trim()) return;
    try {
      await clientApi.addNote(client.id, newNote.trim());
      setNewNote('');
      const updated = await clientApi.getById(client.id);
      setClient(updated);
    } catch {
      alert('Erro ao adicionar nota');
    }
  };

  const handleAddInteraction = async () => {
    if (!client || !newInteraction.content.trim()) return;
    try {
      await clientApi.createInteraction(client.id, newInteraction);
      setNewInteraction({ type: 'Message', content: '' });
      const result = await clientApi.listInteractions(client.id);
      setInteractions(result.items);
    } catch {
      alert('Erro ao registrar interação');
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-surface p-6">
        <div className="animate-pulse space-y-4">
          <div className="h-8 w-48 bg-gray-200 dark:bg-gray-700 rounded" />
          <div className="h-64 bg-gray-200 dark:bg-gray-700 rounded-xl" />
        </div>
      </div>
    );
  }

  if (!client) return null;

  const initials = client.nome
    .split(' ')
    .map((n) => n[0])
    .slice(0, 2)
    .join('')
    .toUpperCase();

  return (
    <div className="min-h-screen bg-surface">
      <header className="px-4 md:px-6 pt-4 md:pt-6 mb-6">
        <button
          onClick={() => router.back()}
          className="flex items-center gap-2 text-on-surface/60 hover:text-on-surface transition-colors mb-4"
        >
          <span className="material-symbols-outlined">arrow_back</span>
          <span className="text-sm">Voltar</span>
        </button>
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 rounded-full bg-primary-container dark:bg-primary-fixed-dim flex items-center justify-center text-on-primary-container dark:text-on-primary-fixed font-bold text-xl">
            {initials}
          </div>
          <div>
            {isEditing ? (
              <input
                value={editForm.nome}
                onChange={(e) => setEditForm({ ...editForm, nome: e.target.value })}
                className="text-2xl font-bold bg-transparent border-b-2 border-primary outline-none"
              />
            ) : (
              <h1 className="text-2xl font-bold text-on-surface">{client.nome}</h1>
            )}
            {client.empresa && !isEditing && (
              <p className="text-sm text-on-surface/60">{client.empresa}</p>
            )}
          </div>
        </div>
      </header>

      <main className="px-4 md:px-6 max-w-[1000px] mx-auto space-y-6">
        {/* Client Info */}
        <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-on-surface">Dados do Cliente</h2>
            <div className="flex gap-2">
              {isEditing ? (
                <>
                  <button
                    onClick={handleSave}
                    className="px-4 py-2 bg-primary text-on-primary rounded-lg text-sm font-medium"
                  >
                    Salvar
                  </button>
                  <button
                    onClick={() => setIsEditing(false)}
                    className="px-4 py-2 border border-outline-variant rounded-lg text-sm text-on-surface/60"
                  >
                    Cancelar
                  </button>
                </>
              ) : (
                <button
                  onClick={() => setIsEditing(true)}
                  className="px-4 py-2 border border-outline-variant rounded-lg text-sm text-on-surface/60 hover:bg-surface-container-low"
                >
                  Editar
                </button>
              )}
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-medium text-on-surface/50 mb-1">Email</label>
              {isEditing ? (
                <input
                  value={editForm.email}
                  onChange={(e) => setEditForm({ ...editForm, email: e.target.value })}
                  className="w-full px-3 py-2 border border-outline-variant rounded-lg text-sm"
                />
              ) : (
                <p className="text-sm text-on-surface">{client.email || '—'}</p>
              )}
            </div>
            <div>
              <label className="block text-xs font-medium text-on-surface/50 mb-1">Telefone</label>
              {isEditing ? (
                <input
                  value={editForm.telefone}
                  onChange={(e) => setEditForm({ ...editForm, telefone: e.target.value })}
                  className="w-full px-3 py-2 border border-outline-variant rounded-lg text-sm"
                />
              ) : (
                <p className="text-sm text-on-surface">{client.telefone || '—'}</p>
              )}
            </div>
            <div>
              <label className="block text-xs font-medium text-on-surface/50 mb-1">Empresa</label>
              {isEditing ? (
                <input
                  value={editForm.empresa}
                  onChange={(e) => setEditForm({ ...editForm, empresa: e.target.value })}
                  className="w-full px-3 py-2 border border-outline-variant rounded-lg text-sm"
                />
              ) : (
                <p className="text-sm text-on-surface">{client.empresa || '—'}</p>
              )}
            </div>
            <div>
              <label className="block text-xs font-medium text-on-surface/50 mb-1">Status</label>
              <span
                className={`inline-block px-3 py-1 rounded-full text-xs font-medium ${
                  client.status === 'Ativo'
                    ? 'bg-emerald-50 text-emerald-600'
                    : 'bg-red-50 text-red-600'
                }`}
              >
                {client.status}
              </span>
            </div>
          </div>
        </div>

        {/* Tags */}
        <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
          <h2 className="text-lg font-semibold text-on-surface mb-4">Tags</h2>
          <div className="flex flex-wrap gap-2 mb-4">
            {client.tags.map((tag) => (
              <span
                key={tag}
                className="inline-flex items-center gap-1 px-3 py-1 bg-primary/10 text-primary rounded-full text-xs font-medium"
              >
                {tag}
                <button
                  onClick={() => handleRemoveTag(tag)}
                  className="hover:text-red-500 transition-colors"
                >
                  <span className="material-symbols-outlined text-sm">close</span>
                </button>
              </span>
            ))}
          </div>
          <div className="flex gap-2">
            <input
              value={newTag}
              onChange={(e) => setNewTag(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleAddTag()}
              placeholder="Nova tag..."
              className="flex-1 px-3 py-2 border border-outline-variant rounded-lg text-sm"
            />
            <button
              onClick={handleAddTag}
              className="px-4 py-2 bg-primary text-on-primary rounded-lg text-sm font-medium"
            >
              Adicionar
            </button>
          </div>
        </div>

        {/* Notes */}
        <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
          <h2 className="text-lg font-semibold text-on-surface mb-4">Notas</h2>
          <div className="space-y-3 mb-4">
            {client.notes.length === 0 ? (
              <p className="text-sm text-on-surface/50 italic">Nenhuma nota ainda.</p>
            ) : (
              client.notes.map((note) => (
                <div key={note.id} className="p-3 bg-surface-container-low rounded-lg">
                  <p className="text-sm text-on-surface">{note.content}</p>
                  <p className="text-xs text-on-surface/40 mt-1">
                    {new Date(note.createdAt).toLocaleDateString('pt-BR')}
                  </p>
                </div>
              ))
            )}
          </div>
          <div className="flex gap-2">
            <input
              value={newNote}
              onChange={(e) => setNewNote(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleAddNote()}
              placeholder="Nova nota..."
              className="flex-1 px-3 py-2 border border-outline-variant rounded-lg text-sm"
            />
            <button
              onClick={handleAddNote}
              className="px-4 py-2 bg-primary text-on-primary rounded-lg text-sm font-medium"
            >
              Adicionar
            </button>
          </div>
        </div>

        {/* Interactions Timeline */}
        <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
          <h2 className="text-lg font-semibold text-on-surface mb-4">Histórico de Interações</h2>

          <div className="flex gap-2 mb-6">
            <select
              value={newInteraction.type}
              onChange={(e) => setNewInteraction({ ...newInteraction, type: e.target.value })}
              className="px-3 py-2 border border-outline-variant rounded-lg text-sm"
            >
              <option value="Message">Mensagem</option>
              <option value="Call">Chamada</option>
              <option value="Email">Email</option>
              <option value="Note">Nota</option>
              <option value="Appointment">Compromisso</option>
            </select>
            <input
              value={newInteraction.content}
              onChange={(e) => setNewInteraction({ ...newInteraction, content: e.target.value })}
              onKeyDown={(e) => e.key === 'Enter' && handleAddInteraction()}
              placeholder="Descreva a interação..."
              className="flex-1 px-3 py-2 border border-outline-variant rounded-lg text-sm"
            />
            <button
              onClick={handleAddInteraction}
              className="px-4 py-2 bg-primary text-on-primary rounded-lg text-sm font-medium"
            >
              Registrar
            </button>
          </div>

          {interactions.length === 0 ? (
            <p className="text-sm text-on-surface/50 italic">Nenhuma interação registrada.</p>
          ) : (
            <div className="relative border-l-2 border-primary ml-2 pl-6 space-y-6">
              {interactions.map((interaction) => (
                <div key={interaction.id} className="relative">
                  <span className="absolute -left-[31px] top-0 w-4 h-4 rounded-full bg-primary border-4 border-surface dark:border-inverse-surface" />
                  <div className="flex items-center gap-2 mb-1">
                    <span className="text-xs font-semibold text-primary">
                      {new Date(interaction.createdAt).toLocaleDateString('pt-BR')}
                    </span>
                    <span className="px-2 py-0.5 bg-surface-container-high rounded text-xs text-on-surface/60">
                      {interaction.type}
                    </span>
                  </div>
                  <p className="text-sm text-on-surface/80">{interaction.content}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  );
}
