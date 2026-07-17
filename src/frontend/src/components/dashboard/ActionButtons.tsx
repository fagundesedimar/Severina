'use client';

import { useRouter } from 'next/navigation';

interface ActionButton {
  label: string;
  icon: string;
  href: string;
}

const actions: ActionButton[] = [
  { label: 'Novo Atendimento', icon: 'chat', href: '/atendimento/novo' },
  { label: 'Novo Cliente', icon: 'person_add', href: '/clientes/novo' },
  { label: 'Nova Cobrança', icon: 'receipt', href: '/financeiro/cobrancas' },
];

export function ActionButtons() {
  const router = useRouter();

  return (
    <div className="flex flex-wrap gap-2">
      {actions.map((action) => (
        <button
          key={action.href}
          onClick={() => router.push(action.href)}
          className="flex items-center gap-1.5 px-3 py-1.5 bg-primary text-on-primary rounded-lg hover:bg-primary/90 active:scale-95 transition-all text-xs font-medium"
        >
          <span className="material-symbols-outlined text-sm">{action.icon}</span>
          {action.label}
        </button>
      ))}
    </div>
  );
}
