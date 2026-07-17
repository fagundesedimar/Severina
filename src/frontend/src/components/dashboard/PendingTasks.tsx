'use client';

import { useRouter } from 'next/navigation';
import type { PendingTaskDto } from '@/types/dashboard';
import { TaskPriority } from '@/types/dashboard';

function getPriorityStyle(priority: TaskPriority): string {
  switch (priority) {
    case TaskPriority.Overdue:
      return 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-400 border-red-200 dark:border-red-800';
    case TaskPriority.Pending:
      return 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-700 dark:text-yellow-400 border-yellow-200 dark:border-yellow-800';
    case TaskPriority.Upcoming:
      return 'bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-400 border-blue-200 dark:border-blue-800';
    default:
      return 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 border-gray-200 dark:border-gray-600';
  }
}

function getPriorityLabel(priority: TaskPriority): string {
  switch (priority) {
    case TaskPriority.Overdue: return 'Atrasado';
    case TaskPriority.Pending: return 'Pendente';
    case TaskPriority.Upcoming: return 'Próximo';
    default: return '';
  }
}

function formatDueDate(dueDate?: string): string {
  if (!dueDate) return '';
  const date = new Date(dueDate);
  const now = new Date();
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  const dueDay = new Date(date.getFullYear(), date.getMonth(), date.getDate());

  if (dueDay.getTime() === today.getTime()) {
    return `Hoje ${date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}`;
  }

  const tomorrow = new Date(today);
  tomorrow.setDate(tomorrow.getDate() + 1);
  if (dueDay.getTime() === tomorrow.getTime()) {
    return `Amanhã ${date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}`;
  }

  return date.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' }) +
    ' ' + date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
}

export function PendingTasks({ tasks, totalCount }: { tasks: PendingTaskDto[]; totalCount: number }) {
  const router = useRouter();

  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl p-3 h-full flex flex-col">
      <div className="flex items-center justify-between mb-1">
        <h3 className="text-xs font-semibold text-on-surface flex items-center gap-2">
          <span className="material-symbols-outlined text-primary text-sm">task_alt</span>
          Tarefas Pendentes
        </h3>
        {totalCount > 0 && (
          <span className="inline-flex items-center justify-center w-5 h-5 text-[10px] font-bold text-white bg-red-500 rounded-full">
            {totalCount > 99 ? '99+' : totalCount}
          </span>
        )}
      </div>
      {tasks.length === 0 ? (
        <p className="text-xs text-gray-500 dark:text-gray-400 text-center py-4">
          Nenhuma tarefa pendente
        </p>
      ) : (
        <div className="space-y-1 flex-1">
          {tasks.slice(0, 8).map((task) => (
            <div
              key={task.id}
              onClick={() => task.sourceUrl && router.push(task.sourceUrl)}
              className={`flex items-center gap-2 p-1.5 rounded-lg border transition-colors ${
                task.sourceUrl
                  ? 'hover:bg-gray-50 dark:hover:bg-gray-700/50 cursor-pointer'
                  : ''
              } ${getPriorityStyle(task.priority)}`}
            >
              <span className="material-symbols-outlined text-[10px] flex-shrink-0">
                {task.type === 'appointment' ? 'event' :
                 task.type === 'invoice' ? 'receipt' :
                 task.type === 'message' ? 'chat' : 'task'}
              </span>
              <div className="flex-1 min-w-0">
                <p className="text-[11px] font-medium truncate">{task.title}</p>
                {task.dueDate && (
                  <p className="text-[9px] opacity-75">{formatDueDate(task.dueDate)}</p>
                )}
              </div>
              <span className="text-[8px] font-bold uppercase tracking-tighter opacity-75">
                {getPriorityLabel(task.priority)}
              </span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
