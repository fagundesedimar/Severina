'use client';

import { useState, useCallback } from 'react';
import Link from 'next/link';
import { ClientList } from '@/components/clients/ClientList';
import { SearchBar } from '@/components/clients/SearchBar';
import { StatusFilter } from '@/components/clients/StatusFilter';
import { AppShell } from '@/components/layout/AppShell';
import { Button } from '@/components/ui/button';

export default function ClientesPage() {
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  const handleSearch = useCallback((value: string) => {
    setSearch(value);
  }, []);

  return (
    <AppShell
      title="Clientes"
      actions={
        <Link href="/clientes/importar">
          <Button size="sm">
            <span className="material-symbols-outlined text-lg">upload</span>
            Importar
          </Button>
        </Link>
      }
    >
      <div className="max-w-[1200px] mx-auto space-y-6">
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <SearchBar onSearch={handleSearch} />
          </div>
          <StatusFilter value={statusFilter} onChange={setStatusFilter} />
        </div>

        <ClientList search={search} statusFilter={statusFilter} />
      </div>

      <Link
        href="/clientes/novo"
        className="fixed bottom-24 right-4 lg:bottom-8 lg:right-8 w-14 h-14 bg-primary text-primary-foreground rounded-full shadow-lg flex items-center justify-center transition-transform hover:scale-105 active:scale-95 z-40"
      >
        <span className="material-symbols-outlined text-[32px]">add</span>
      </Link>
    </AppShell>
  );
}
