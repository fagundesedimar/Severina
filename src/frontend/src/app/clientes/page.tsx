'use client';

import { useState, useCallback } from 'react';
import Link from 'next/link';
import { ClientList } from '@/components/clients/ClientList';
import { SearchBar } from '@/components/clients/SearchBar';
import { StatusFilter } from '@/components/clients/StatusFilter';

export default function ClientesPage() {
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  const handleSearch = useCallback((value: string) => {
    setSearch(value);
  }, []);

  return (
    <div className="min-h-screen bg-surface">
      <header className="mb-6 px-4 md:px-6 pt-4 md:pt-6">
        <div className="flex items-center justify-between mb-4">
          <div>
            <h1 className="text-2xl md:text-3xl font-bold text-on-surface">Clientes</h1>
            <p className="text-sm text-on-surface/60 mt-1">Gerencie sua base de clientes</p>
          </div>
          <Link
            href="/clientes/importar"
            className="flex items-center gap-2 px-4 py-2 bg-primary text-on-primary rounded-full text-sm font-medium hover:bg-primary/90 transition-colors"
          >
            <span className="material-symbols-outlined text-lg">upload</span>
            Importar
          </Link>
        </div>
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <SearchBar onSearch={handleSearch} />
          </div>
          <StatusFilter value={statusFilter} onChange={setStatusFilter} />
        </div>
      </header>

      <main className="px-4 md:px-6 max-w-[1200px] mx-auto">
        <ClientList search={search} statusFilter={statusFilter} />
      </main>

      <Link
        href="/clientes/novo"
        className="fixed bottom-24 right-4 md:right-6 w-14 h-14 bg-primary text-on-primary rounded-full shadow-lg flex items-center justify-center transition-transform hover:scale-105 active:scale-95 z-40"
      >
        <span className="material-symbols-outlined text-[32px]">add</span>
      </Link>
    </div>
  );
}
