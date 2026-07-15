'use client';

import { useState, useEffect } from 'react';

interface SearchBarProps {
  onSearch: (value: string) => void;
}

export function SearchBar({ onSearch }: SearchBarProps) {
  const [value, setValue] = useState('');

  useEffect(() => {
    const timer = setTimeout(() => {
      onSearch(value);
    }, 300);

    return () => clearTimeout(timer);
  }, [value, onSearch]);

  return (
    <div className="relative group">
      <span className="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-on-surface/40">
        search
      </span>
      <input
        type="text"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        placeholder="Buscar clientes por nome ou telefone..."
        className="w-full h-12 pl-12 pr-4 bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl focus:ring-2 focus:ring-primary focus:border-primary transition-all text-sm outline-none"
      />
    </div>
  );
}
