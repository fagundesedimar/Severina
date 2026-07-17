'use client';

import { useRouter } from 'next/navigation';
import type { ActivityDto } from '@/types/dashboard';

function formatTimeAgo(timestamp: string): string {
  const now = new Date();
  const date = new Date(timestamp);
  const diffMs = now.getTime() - date.getTime();
  const diffMin = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMs / 3600000);
  const diffDays = Math.floor(diffMs / 86400000);

  if (diffMin < 1) return 'agora';
  if (diffMin < 60) return `${diffMin}m atrás`;
  if (diffHours < 24) return `${diffHours}h atrás`;
  return `${diffDays}d atrás`;
}

function getActivityIcon(type: string): string {
  switch (type) {
    case 'appointment': return 'event';
    case 'client': return 'person';
    case 'payment': return 'payments';
    case 'message': return 'chat';
    default: return 'info';
  }
}

function getActivityColor(type: string): string {
  switch (type) {
    case 'appointment': return 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400';
    case 'client': return 'bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400';
    case 'payment': return 'bg-purple-100 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400';
    case 'message': return 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-600 dark:text-yellow-400';
    default: return 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400';
  }
}

export function ActivityFeed({ activities }: { activities: ActivityDto[] }) {
  const router = useRouter();

  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl p-3 h-full flex flex-col">
      <div className="flex items-center justify-between mb-1">
        <h3 className="text-xs font-semibold text-on-surface flex items-center gap-2">
          <span className="material-symbols-outlined text-primary text-sm">history</span>
          Atividades Recentes
        </h3>
        {activities.length > 0 && (
          <span className="inline-flex items-center justify-center w-5 h-5 text-[10px] font-bold text-white bg-blue-500 rounded-full">
            {activities.length > 99 ? '99+' : activities.length}
          </span>
        )}
      </div>
      {activities.length === 0 ? (
        <p className="text-xs text-gray-500 dark:text-gray-400 text-center py-4">
          Nenhuma atividade recente
        </p>
      ) : (
        <div className="space-y-1 flex-1">
          {activities.slice(0, 8).map((activity) => (
            <div
              key={activity.id}
              onClick={() => activity.sourceUrl && router.push(activity.sourceUrl)}
              className={`flex items-center gap-2 p-1.5 rounded-lg transition-colors ${
                activity.sourceUrl
                  ? 'hover:bg-gray-50 dark:hover:bg-gray-700/50 cursor-pointer'
                  : ''
              }`}
            >
              <div className={`w-5 h-5 rounded-full flex items-center justify-center flex-shrink-0 ${getActivityColor(activity.type)}`}>
                <span className="material-symbols-outlined text-[9px]">{getActivityIcon(activity.type)}</span>
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-[11px] text-on-surface truncate">{activity.description}</p>
                <p className="text-[9px] text-gray-500 dark:text-gray-400">
                  {formatTimeAgo(activity.timestamp)}
                </p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
