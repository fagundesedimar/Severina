'use client';

interface StatusFilterProps {
  value: string;
  onChange: (value: string) => void;
}

export function StatusFilter({ value, onChange }: StatusFilterProps) {
  return (
    <div className="flex gap-2">
      <button
        onClick={() => onChange('')}
        className={`px-4 py-2 rounded-full text-sm font-medium transition-colors ${
          value === ''
            ? 'bg-primary text-on-primary'
            : 'border border-outline-variant text-on-surface/60 hover:bg-surface-container-low'
        }`}
      >
        Todos
      </button>
      <button
        onClick={() => onChange('Ativo')}
        className={`px-4 py-2 rounded-full text-sm font-medium transition-colors ${
          value === 'Ativo'
            ? 'bg-emerald-600 text-white'
            : 'border border-outline-variant text-on-surface/60 hover:bg-surface-container-low'
        }`}
      >
        Ativos
      </button>
      <button
        onClick={() => onChange('Inativo')}
        className={`px-4 py-2 rounded-full text-sm font-medium transition-colors ${
          value === 'Inativo'
            ? 'bg-red-600 text-white'
            : 'border border-outline-variant text-on-surface/60 hover:bg-surface-container-low'
        }`}
      >
        Inativos
      </button>
    </div>
  );
}
